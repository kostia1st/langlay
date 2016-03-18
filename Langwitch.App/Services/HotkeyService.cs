using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Langwitch
{
    public class HotkeyService : IDisposable
    {
        private GlobalKeyboardHook Hooker { get; set; }
        private IConfigService ConfigService { get; set; }
        private IOverlayService OverlayService { get; set; }
        private IDictionary<string, IntPtr> CultureToLastUsedLayout
            = new Dictionary<string, IntPtr>();
        private bool IsStarted { get; set; }

        public HotkeyService(
            IConfigService configService, IOverlayService overlayService)
        {
            if (configService == null)
                throw new ArgumentNullException("configService");
            if (overlayService == null)
                throw new ArgumentNullException("overlayService");
            ConfigService = configService;
            OverlayService = overlayService;
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Hooker = new GlobalKeyboardHook(false);
                Hooker.HookedKeys.Add(ConfigService.LanguageSwitchKeys);

                Hooker.KeyDown = Hooker_KeyDown;
                Hooker.Hook();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                if (Hooker != null)
                    Hooker.Unhook();
            }
        }

        private void Hooker_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var currentLanguage = InputLanguageHelper.GetGlobalCurrentInputLanguage();
                if (currentLanguage != null)
                {
                    CultureToLastUsedLayout[currentLanguage.Culture.EnglishName] = currentLanguage.Handle;
                    var nextLanguageName = InputLanguageHelper.GetNextInputLanguageName(currentLanguage.Culture.EnglishName);
                    IntPtr layoutToSet;
                    if (CultureToLastUsedLayout.ContainsKey(nextLanguageName))
                        layoutToSet = CultureToLastUsedLayout[nextLanguageName];
                    else
                        layoutToSet = InputLanguageHelper.GetDefaultLayoutForLanguage(nextLanguageName);
                    InputLanguageHelper.SetCurrentLayout(layoutToSet);

                    var inputLayout = InputLanguageHelper.GetInputLanguageByHandle(layoutToSet);
                    if (inputLayout != null)
                        OverlayService.PushMessage(inputLayout.Culture.TwoLetterISOLanguageName + ": " + inputLayout.LayoutName);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Stop();

                disposedValue = true;
            }
        }

        ~HotkeyService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

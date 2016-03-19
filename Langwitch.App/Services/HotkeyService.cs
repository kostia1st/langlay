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
        private ILanguageService LanguageService { get; set; }
        private bool IsStarted { get; set; }

        public HotkeyService(
            IConfigService configService, 
            ILanguageService languageService)
        {
            if (configService == null)
                throw new ArgumentNullException("configService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");
            ConfigService = configService;
            LanguageService = languageService;
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
            e.Handled = LanguageService.SwitchLanguage();
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

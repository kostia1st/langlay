using System;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class HotkeyService : IDisposable, IHotkeyService
    {
        private GlobalKeyboardHook Hooker { get; set; }
        private IConfigService ConfigService { get; set; }
        private ILanguageService LanguageService { get; set; }
        private bool IsStarted { get; set; }
        private bool IsEnabled { get; set; }

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
            IsEnabled = true;
        }

        public bool GetIsEnabled()
        {
            return IsEnabled;
        }

        public void SetEnabledState(bool isEnabled)
        {
            if (IsEnabled != isEnabled)
                IsEnabled = isEnabled;
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Hooker = new GlobalKeyboardHook(false);
                if (ConfigService.DoSwitchLanguage && ConfigService.LanguageSwitchKeys != default(KeyCode))
                    Hooker.HookedKeys.Add((Keys) ConfigService.LanguageSwitchKeys);
                if (ConfigService.DoSwitchLayout && ConfigService.LayoutSwitchKeys != default(KeyCode))
                    Hooker.HookedKeys.Add((Keys) ConfigService.LayoutSwitchKeys);

                Hooker.KeyDown = Hooker_KeyDown;
                Hooker.KeyUp = Hooker_KeyUp;
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

        private const int InactivePeriod = 500;
        private DateTime? InactiveTill { get; set; }

        private void Hooker_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsEnabled && (InactiveTill == null || InactiveTill < DateTime.Now))
            {
                if (ConfigService.DoSwitchLanguage
                    && ConfigService.LanguageSwitchKeys != KeyCode.None
                    && ((KeyCode) e.KeyData & ConfigService.LanguageSwitchKeys) == ConfigService.LanguageSwitchKeys)
                {
                    if (ConfigService.DoSwitchLayout 
                        && (KeyCode) e.KeyData == ConfigService.LayoutSwitchKeys)
                        e.Handled = LanguageService.SwitchLanguageAndLayout();
                    else
                        e.Handled = LanguageService.SwitchLanguage(true);
                }
                else if (ConfigService.DoSwitchLayout
                    && ConfigService.LayoutSwitchKeys != KeyCode.None
                    && ((KeyCode) e.KeyData & ConfigService.LayoutSwitchKeys) == ConfigService.LayoutSwitchKeys)
                {
                    e.Handled = LanguageService.SwitchLayout(true);
                }
                if (e.Handled)
                {
                    // Here, we place a timeout on when the next KeyDown could be applied 
                    // without resetting it by KeyUp
                    InactiveTill = DateTime.Now.AddMilliseconds(InactivePeriod);
                }
            }
        }

        private void Hooker_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsEnabled)
            {
                // We're supposed to handle the key-up as well as the key-down
                // otherwise the target app will face a strange situation,
                // which is not guaranteed to work properly.
                if (ConfigService.DoSwitchLanguage
                    && (KeyCode) e.KeyCode == ConfigService.LanguageSwitchKeys)
                    e.Handled = true;
                else if (ConfigService.DoSwitchLayout
                    && (KeyCode) e.KeyCode == ConfigService.LayoutSwitchKeys)
                    e.Handled = true;
                if (e.Handled)
                    InactiveTill = null;
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

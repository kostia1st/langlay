using System;
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
                if (ConfigService.LanguageSwitchKeys != default(Keys))
                    Hooker.HookedKeys.Add(ConfigService.LanguageSwitchKeys);
                if (ConfigService.LayoutSwitchKeys != default(Keys))
                    Hooker.HookedKeys.Add(ConfigService.LayoutSwitchKeys);

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
            if (InactiveTill == null || InactiveTill < DateTime.Now)
            {
                if (ConfigService.LanguageSwitchKeys != Keys.None
                    && (e.KeyData & ConfigService.LanguageSwitchKeys) == ConfigService.LanguageSwitchKeys)
                {
                    if (e.KeyCode == ConfigService.LayoutSwitchKeys)
                        e.Handled = LanguageService.SwitchLanguageAndLayout();
                    else
                        e.Handled = LanguageService.SwitchLanguage(true);
                }
                else if (ConfigService.LayoutSwitchKeys != Keys.None
                    && (e.KeyData & ConfigService.LayoutSwitchKeys) == ConfigService.LayoutSwitchKeys)
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
            // We're supposed to handle the key-up as well as the key-down
            // otherwise the target app will face a strange situation,
            // which is not guaranteed to work properly.
            if (e.KeyCode == ConfigService.LanguageSwitchKeys)
                e.Handled = true;
            else if (e.KeyCode == ConfigService.LayoutSwitchKeys)
                e.Handled = true;
            if (e.Handled)
                InactiveTill = null;
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

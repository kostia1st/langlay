using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Product.Common;
using WindowsInput;

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

        public void SetEnabled(bool value)
        {
            if (IsEnabled != value)
            {
                if (!value)
                    IsEnabled = false;
                var savedKeyDown = this.SavedKeyDown;
                if (savedKeyDown != null)
                {
                    if (!value)
                    {
                        Trace.WriteLine("-- START Simulating full keystroke up");
                        Trace.Indent();
                        InputSimulator.SimulateKeyUp((VirtualKeyCode) savedKeyDown.NonModifiers);
                        InputSimulator.SimulateKeyUp((VirtualKeyCode) savedKeyDown.Modifiers);
                        Trace.Unindent();
                        Trace.WriteLine("-- END Simulating full keystroke up");
                        
                        // This line (or equivalent) is necessary to avoid 
                        // phantom KEY UP messages afterwards.
                        Application.DoEvents();
                    }
                    else
                    {
                        // This line (or equivalent) is necessary to avoid 
                        // phantom KEY UP messages afterwards.
                        Application.DoEvents();

                        Trace.WriteLine("-- START Simulating full keystroke down");
                        Trace.Indent();
                        InputSimulator.SimulateKeyDown((VirtualKeyCode) savedKeyDown.Modifiers);
                        InputSimulator.SimulateKeyDown((VirtualKeyCode) savedKeyDown.NonModifiers);
                        Trace.Unindent();
                        Trace.WriteLine("-- END Simulating full keystroke down");
                    }
                }
                if (value)
                    IsEnabled = true;

            }
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Hooker = new GlobalKeyboardHook(false);
                if (ConfigService.DoSwitchLanguage && (ConfigService.LanguageSwitchModifiers | ConfigService.LanguageSwitchNonModifiers) != default(KeyCode))
                    Hooker.HookedKeys.Add(new KeyStroke((Keys) ConfigService.LanguageSwitchNonModifiers, (Keys) ConfigService.LanguageSwitchModifiers));
                if (ConfigService.DoSwitchLayout && (ConfigService.LayoutSwitchModifiers | ConfigService.LayoutSwitchNonModifiers) != default(KeyCode))
                    Hooker.HookedKeys.Add(new KeyStroke((Keys) ConfigService.LayoutSwitchNonModifiers, (Keys) ConfigService.LayoutSwitchModifiers));

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

        private bool HandleSwitch(KeyboardSwitch keyboardSwitch)
        {
            return LanguageService.ConductSwitch(keyboardSwitch);
        }

        private KeyEventArgs2 SavedKeyDown;

        private void ResetKeyDown()
        {
            InactiveTill = null;
            SavedKeyDown = null;
        }

        private bool GetPrevKeyDownEffective()
        {
            var now = DateTime.Now;
            return InactiveTill != null && now < InactiveTill.Value;
        }

        private void SetKeyDownEffective()
        {
            // Here, we place a timeout on when the next KeyDown could be applied 
            // without resetting it by KeyUp
            InactiveTill = DateTime.Now.AddMilliseconds(InactivePeriod);
        }

        private void Hooker_KeyDown(object sender, KeyEventArgs2 e)
        {
            KeyboardSwitch? switchToApply = null;

            var triggeredLanguageSwitch = ConfigService.DoSwitchLanguage
                && ((KeyCode) e.NonModifiers & ConfigService.LanguageSwitchNonModifiers) == ConfigService.LanguageSwitchNonModifiers
                && ((KeyCode) e.Modifiers & ConfigService.LanguageSwitchModifiers) == ConfigService.LanguageSwitchModifiers;

            var triggeredLayoutSwitch = ConfigService.DoSwitchLayout
                && ((KeyCode) e.NonModifiers & ConfigService.LayoutSwitchNonModifiers) == ConfigService.LayoutSwitchNonModifiers
                && ((KeyCode) e.Modifiers & ConfigService.LayoutSwitchModifiers) == ConfigService.LayoutSwitchModifiers;

            if (triggeredLanguageSwitch)
            {
                if (triggeredLayoutSwitch)
                    switchToApply = KeyboardSwitch.LanguageAndLayout;
                else
                    switchToApply = KeyboardSwitch.LanguageRestoreLayout;
            }
            else if (triggeredLayoutSwitch)
            {
                switchToApply = KeyboardSwitch.Layout;
            }

            if (switchToApply != null)
            {
                if (!GetPrevKeyDownEffective())
                {
                    if (IsEnabled)
                    {
                        SavedKeyDown = e;
                        e.Handled = HandleSwitch(switchToApply.Value);
                        if (e.Handled)
                        {
                            SetKeyDownEffective();
                        }
                    }
                    else
                        e.Handled = true;
                }
                else
                    e.Handled = true;
            }
        }

        private void Hooker_KeyUp(object sender, KeyEventArgs2 e)
        {
            var isLanguageNonModifiersUp = ((KeyCode) e.NonModifiers & ConfigService.LanguageSwitchNonModifiers) == ConfigService.LanguageSwitchNonModifiers;
            var isLanguageModifiersUp = ((KeyCode) e.Modifiers & ConfigService.LanguageSwitchModifiers) == ConfigService.LanguageSwitchModifiers;
            
            var isLayoutNonModifiersUp = ((KeyCode) e.NonModifiers & ConfigService.LayoutSwitchNonModifiers) == ConfigService.LayoutSwitchNonModifiers;
            var isLayoutModifiersUp = ((KeyCode) e.Modifiers & ConfigService.LayoutSwitchModifiers) == ConfigService.LayoutSwitchModifiers;

            // We're supposed to handle the key-up as well as the key-down
            // otherwise the target app will face a strange situation,
            // which is not guaranteed to work properly.
            var triggeredLanguageSwitch = ConfigService.DoSwitchLanguage
                && (isLanguageNonModifiersUp && isLanguageModifiersUp 
                || isLanguageModifiersUp && GetPrevKeyDownEffective());

            var triggeredLayoutSwitch = ConfigService.DoSwitchLayout
                && (isLayoutNonModifiersUp && isLayoutModifiersUp
                || isLayoutModifiersUp && GetPrevKeyDownEffective());

            if (triggeredLanguageSwitch || triggeredLayoutSwitch)
            {
                // Since the sequence is fully owned by the app, we should never pass it thru.
                e.Handled = true;

                if (IsEnabled)
                    // Reset the previous key down only when allowed to.
                    ResetKeyDown();
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

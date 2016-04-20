using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;
using WindowsInput;
using WindowsInput.Native;

namespace Product
{
    public class HookedHotkeyService : IDisposable, IHotkeyService
    {
        private KeyboardHooker Hooker { get; set; }
        private IConfigService ConfigService { get; set; }
        private ILanguageService LanguageService { get; set; }
        private bool IsStarted { get; set; }
        private bool IsEnabled { get; set; }

        private KeyboardSimulator KeyboardSimulator { get; set; }

        public HookedHotkeyService(
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
            KeyboardSimulator = new KeyboardSimulator(new InputSimulator());
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
                        if (savedKeyDown.KeyStroke.NonModifiers != Keys.None)
                            KeyboardSimulator.KeyUp((VirtualKeyCode) savedKeyDown.KeyStroke.NonModifiers);
                        if (savedKeyDown.KeyStroke.Modifiers != Keys.None)
                            KeyboardSimulator.KeyUp((VirtualKeyCode) savedKeyDown.KeyStroke.Modifiers);
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
                        if (savedKeyDown.KeyStroke.Modifiers != Keys.None)
                            KeyboardSimulator.KeyDown((VirtualKeyCode) savedKeyDown.KeyStroke.Modifiers);
                        if (savedKeyDown.KeyStroke.NonModifiers != Keys.None)
                            KeyboardSimulator.KeyDown((VirtualKeyCode) savedKeyDown.KeyStroke.NonModifiers);
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
                Hooker = new KeyboardHooker(false);
                Hooker.KeyDown = Hooker_KeyDown;
                Hooker.KeyUp = Hooker_KeyUp;
                Hooker.SetHook();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                if (Hooker != null)
                {
                    Hooker.Dispose();
                    Hooker = null;
                }
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
                && ((ConfigService.LanguageSwitchNonModifiers | ConfigService.LanguageSwitchModifiers) != KeyCode.None)
                && (KeyCode) e.KeyStroke.NonModifiers == ConfigService.LanguageSwitchNonModifiers
                && (KeyCode) e.KeyStroke.Modifiers == ConfigService.LanguageSwitchModifiers;

            var triggeredLayoutSwitch = ConfigService.DoSwitchLayout
                && ((ConfigService.LayoutSwitchNonModifiers | ConfigService.LayoutSwitchModifiers) != KeyCode.None)
                && (KeyCode) e.KeyStroke.NonModifiers == ConfigService.LayoutSwitchNonModifiers
                && (KeyCode) e.KeyStroke.Modifiers == ConfigService.LayoutSwitchModifiers;

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
            var isLanguageNonModifiersUp = ((KeyCode) e.KeyStroke.NonModifiers & ConfigService.LanguageSwitchNonModifiers) == ConfigService.LanguageSwitchNonModifiers;
            var isLanguageModifiersUp = ((KeyCode) e.KeyStroke.Modifiers & ConfigService.LanguageSwitchModifiers) == ConfigService.LanguageSwitchModifiers;
            
            var isLayoutNonModifiersUp = ((KeyCode) e.KeyStroke.NonModifiers & ConfigService.LayoutSwitchNonModifiers) == ConfigService.LayoutSwitchNonModifiers;
            var isLayoutModifiersUp = ((KeyCode) e.KeyStroke.Modifiers & ConfigService.LayoutSwitchModifiers) == ConfigService.LayoutSwitchModifiers;

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

        ~HookedHotkeyService()
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

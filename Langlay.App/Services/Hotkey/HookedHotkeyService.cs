using System;
using System.Diagnostics;
using System.Linq;
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

        #region Enabled/Disabled

        private bool IsEnabled { get; set; }

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
                    var keysToSimulate = savedKeyDown.KeyStroke.KeysPressedBefore;
                    if (keysToSimulate.Count > 0)
                    {
                        if (!value)
                        {
                            Trace.WriteLine("-- START Simulating full keystroke up");
                            Trace.Indent();
                            foreach (var key in keysToSimulate)
                            {
                                KeyboardSimulator.KeyUp((VirtualKeyCode) key);
                            }
                            Trace.Unindent();
                            Trace.WriteLine("-- END Simulating full keystroke up");

                            // This line (or equivalent) is necessary to
                            // avoid phantom KEY UP messages afterwards.
                            Application.DoEvents();
                        }
                        else
                        {
                            // This line (or equivalent) is necessary to
                            // avoid phantom KEY UP messages afterwards.
                            Application.DoEvents();

                            Trace.WriteLine("-- START Simulating full keystroke down");
                            Trace.Indent();

                            foreach (var key in keysToSimulate.Reverse())
                            {
                                KeyboardSimulator.KeyDown((VirtualKeyCode) key);
                            }
                            Trace.Unindent();
                            Trace.WriteLine("-- END Simulating full keystroke down");
                        }
                    }
                }
                if (value)
                    IsEnabled = true;
            }
        }

        #endregion Enabled/Disabled

        #region Start/Stop

        private bool IsStarted { get; set; }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Hooker = new KeyboardHooker(false, HookProcedureWrapper);
                Hooker.KeyDown = Hooker_KeyDown;
                Hooker.KeyUp = Hooker_KeyUp;
                Hooker.IsEnabledHandler = GetIsEnabled;
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

        #endregion Start/Stop

        #region KeyDown saved

        private const int InactivePeriod = 500;
        private DateTime? InactiveTill { get; set; }
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
            // Here, we place a timeout on when the next KeyDown could be
            // applied without resetting it by KeyUp
            InactiveTill = DateTime.Now.AddMilliseconds(InactivePeriod);
        }

        #endregion KeyDown saved

        private void HandleSwitch(KeyboardSwitch keyboardSwitch)
        {
            LanguageService.ConductSwitch(keyboardSwitch);
        }

        private KeyboardSwitch? GetSwitchToApply(KeyEventArgs2 e)
        {
            var switchToApply = (KeyboardSwitch?) null;
            var keyCodes = e.KeyStroke.Keys.Select(x => (KeyCode) x).ToList();

            var triggeredLanguageSwitch = ConfigService.GetLanguageSwitchConfigured()
                && ConfigService.LanguageSwitchKeyArray.AreEqual(keyCodes);
            var triggeredLayoutSwitch = ConfigService.GetLayoutSwitchConfigured()
                && ConfigService.LayoutSwitchKeyArray.AreEqual(keyCodes);

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

            return switchToApply;
        }

        #region Hook handling

        private int? HookProcedureWrapper(Func<int?> func)
        {
            var result = (int?) null;
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
#if DEBUG
                MessageBox.Show(ex.ToString());
#endif
            }
            return result;
        }

        private void Hooker_KeyDown(object sender, KeyEventArgs2 e)
        {
            if (IsEnabled)
            {
                var switchToApply = GetSwitchToApply(e);
                if (switchToApply != null)
                {
                    e.Handled = true;
                    if (!GetPrevKeyDownEffective())
                    {
                        SavedKeyDown = e;
                        HandleSwitch(switchToApply.Value);
                        SetKeyDownEffective();
                    }
                }
                else if (ConfigService.DoDisableCapsLockToggle)
                {
                    if (e.KeyStroke.Keys.Contains(Keys.CapsLock)
                        && !KeyUtils.GetIsKeyToggled(Keys.CapsLock))
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void Hooker_KeyUp(object sender, KeyEventArgs2 e)
        {
            // The handling could be disabled only by intention - intention
            // to pass everything thru.
            if (IsEnabled)
            {
                // We're supposed to handle the key-up as well as the
                // key-down otherwise the target app will face a strange
                // situation, which is not guaranteed to work properly.
                var switchToApply = GetSwitchToApply(e);

                if (switchToApply != null)
                {
                    // Since the sequence is fully owned by the app, we
                    // should not pass it thru.
                    e.Handled = true;

                    var isKeyDownSaved = SavedKeyDown != null;
                    ResetKeyDown();

                    // This case for all the situations when the needed
                    // sequence was combined with something else when was
                    // down, but is being UP alone.
                    // TODO: review if we could to ignore the whole thing in
                    //       this case, or still switch
                    if (!isKeyDownSaved)
                    {
                        IsEnabled = false;

                        try
                        {
                            Trace.WriteLine("Pass the key-up thru");

                            KeyboardSimulator.KeyUp((VirtualKeyCode) e.KeyStroke.KeyTriggeredEvent);

                            // Here we disable the Caps by fake-pressing Shift
                            if ((KeyCode) e.KeyStroke.KeyTriggeredEvent == KeyCode.CapsLock
                                && SystemSettings.GetIsShiftToDisableCapsLock())
                            {
                                // This is a work around for this single case:
                                // - the hotkey is Caps Lock
                                // - the Shift is set up to turn Caps off
                                //   See issue #65 on GitHub for details.
                                Trace.WriteLine("-- START #65 case");
                                Trace.Indent();
                                KeyboardSimulator.KeyPress(VirtualKeyCode.LSHIFT);
                                Trace.Unindent();
                                Trace.WriteLine("-- END #65 case");

                                // -- end of workaround
                            }

                            HandleSwitch(switchToApply.Value);
                        }
                        finally
                        {
                            IsEnabled = true;
                        }
                    }
                }
            }
        }

        #endregion Hook handling

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

        #endregion IDisposable Support
    }
}
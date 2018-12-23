using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Product.Common;

namespace Product {
    public class AutoSwitchService: IAutoSwitchService, ILifecycled {
        private IEventService EventService { get; set; }

        #region Start/Stop
        public bool IsStarted { get; private set; }

        public void Start() {
            if (!IsStarted) {
                IsStarted = true;
                StartTimer();

                EventService = ServiceRegistry.Instance.Get<IEventService>();
                EventService.KeyboardInput += EventService_Input;
                EventService.MouseInput += EventService_Input;
            }
        }

        public void Stop() {
            if (IsStarted) {
                IsStarted = false;

                EventService.KeyboardInput -= EventService_Input;
                EventService.MouseInput -= EventService_Input;

                StopTimer();

                if (_lastInputElapsed.IsRunning)
                    _lastInputElapsed.Stop();
                LastFocusedWindowHandle = null;
            }
        }
        #endregion Start/Stop

        #region Timer

        private Timer AppTitleCheckTimer { get; set; }
        private const uint PeriodToCheckForLayoutSwitch = 1000;
        private Stopwatch _lastInputElapsed = new Stopwatch() { };

        private void EventService_Input() {
            _lastInputElapsed.Restart();
            if (GetIsTimerPaused())
                ResumeTimer();
        }

        private void StartTimer() {
            AppTitleCheckTimer = new Timer { Interval = 50 };
            AppTitleCheckTimer.Tick += AppTitleCheckTimer_Tick;
            AppTitleCheckTimer.Start();
        }

        private void StopTimer() {
            if (AppTitleCheckTimer != null) {
                AppTitleCheckTimer.Stop();
                AppTitleCheckTimer.Tick -= AppTitleCheckTimer_Tick;
                AppTitleCheckTimer.Dispose();
                AppTitleCheckTimer = null;
            }
        }

        private void PauseTimer() {
            if (AppTitleCheckTimer != null) {
                AppTitleCheckTimer.Stop();
                AppTitleCheckTimer_Tick(AppTitleCheckTimer, EventArgs.Empty);
            }
        }

        private void ResumeTimer() {
            if (AppTitleCheckTimer != null) {
                AppTitleCheckTimer.Start();
            }
        }

        private bool GetIsTimerPaused() {
            return AppTitleCheckTimer != null && !AppTitleCheckTimer.Enabled;
        }

        private IntPtr? LastFocusedWindowHandle { get; set; }

        private void DoOnTimer() {
            var configService = ServiceRegistry.Instance.Get<IConfigService>();
            if (configService.AppAttachmentArray.Count > 0) {
                var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
                var currentFocusedWindowHandle = Win32.GetForegroundWindow();
                if (currentFocusedWindowHandle != IntPtr.Zero) {
                    if (
                        LastFocusedWindowHandle != null
                        && LastFocusedWindowHandle != currentFocusedWindowHandle
                    ) {
                        var text = Win32.GetWindowText(currentFocusedWindowHandle);
#if TRACE
                        Trace.WriteLine("Active window title: " + text);
#endif
                        var attachment = configService.AppAttachmentArray.FirstOrDefault(x => text.Contains(x.AppMask));
                        if (attachment != null) {
                            var languageSetterService = ServiceRegistry.Instance.Get<ILanguageSetterService>();
                            var inputLayouts = languageService.GetInputLayouts();
                            var layout = inputLayouts.FirstOrDefault(x => x.Id == attachment.LayoutId);
                            if (layout != null) {
                                languageSetterService.SetCurrentLayout(layout.Handle);
                            }
                        }
                    }
                    LastFocusedWindowHandle = currentFocusedWindowHandle;
                }
            }
        }

        private void OnTimer() {
            if (_lastInputElapsed.IsRunning
                && _lastInputElapsed.ElapsedMilliseconds < PeriodToCheckForLayoutSwitch) {
                DoOnTimer();
            } else {
                _lastInputElapsed.Stop();
                if (!GetIsTimerPaused())
                    PauseTimer();
            }
        }

        private void AppTitleCheckTimer_Tick(object sender, System.EventArgs e) {
            // Use the condition to make sure we don't get any "old" timer
            // influencing our overlay.
            if (sender == this.AppTitleCheckTimer) {
                try {
                    OnTimer();
                } catch (Exception ex) {
#if TRACE
                    Trace.TraceError(ex.ToString());
#endif
                }
            }
        }

        #endregion Timer
    }
}

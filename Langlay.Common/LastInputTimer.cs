using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Product.Common {
    public class LastInputTimer {
        public Action OnTimer { get; set; }

        private const uint KeepRunningAfterInputFor = 1000;
        private Timer TimerInternal { get; set; }
        private Stopwatch LastInputElapsed { get; set; } = new Stopwatch() { };

        public void SignalInput() {
            LastInputElapsed.Restart();
            if (GetIsPaused())
                Resume();
        }

        public void Start() {
            TimerInternal = new Timer { Interval = 50 };
            TimerInternal.Tick += TimerInternal_Tick;
            TimerInternal.Start();
            LastInputElapsed.Restart();
        }

        public void Stop() {
            if (TimerInternal != null) {
                TimerInternal.Stop();
                TimerInternal.Tick -= TimerInternal_Tick;
                TimerInternal.Dispose();
                TimerInternal = null;
            }
            if (LastInputElapsed.IsRunning)
                LastInputElapsed.Stop();
        }

        public void Pause() {
            if (TimerInternal != null) {
                TimerInternal.Stop();
                TimerInternal_Tick(TimerInternal, EventArgs.Empty);
            }
        }

        public void Resume() {
            if (TimerInternal != null) {
                TimerInternal.Start();
            }
        }

        private bool GetIsPaused() {
            return TimerInternal != null && !TimerInternal.Enabled;
        }

        private void HandleTimer() {
            if (LastInputElapsed.IsRunning
                && LastInputElapsed.ElapsedMilliseconds < KeepRunningAfterInputFor) {
                OnTimer?.Invoke();
            } else {
                LastInputElapsed.Stop();
                if (!GetIsPaused())
                    Pause();
            }
        }

        private void TimerInternal_Tick(object sender, EventArgs e) {
            // Use the condition to make sure we don't get any "old" timer
            // influencing our overlay.
            if (sender == this.TimerInternal) {
                try {
                    HandleTimer();
                } catch (Exception ex) {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

    }
}

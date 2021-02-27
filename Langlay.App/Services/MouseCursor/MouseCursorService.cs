using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Product.Common;

namespace Product {

    public class MouseCursorService : IMouseCursorService, IDisposable, ILifecycled {
        private IConfigService ConfigService { get; set; }
        private MouseHooker Hooker { get; set; }
        private IntPtr LastFocusedWindowHandle { get; set; }

        private bool IsLastDownHandled;

        public MouseCursorService() {
        }

        #region Start/Stop

        public bool IsStarted { get; private set; }

        public void Start() {
            ConfigService = ServiceRegistry.Instance.Get<IConfigService>();
            if (!IsStarted) {
                IsStarted = true;
                Hooker = new MouseHooker(false, HookProcedureWrapper);
                Hooker.ButtonDown = Hooker_ButtonDown;
                Hooker.ButtonUp = Hooker_ButtonUp;
                Hooker.MouseMove = Hooker_MouseMove;
                Hooker.SetHook();
            }
        }

        public void Stop() {
            if (IsStarted) {
                IsStarted = false;
                if (Hooker != null) {
                    Hooker.Dispose();
                    Hooker = null;
                }
                IsLastDownHandled = false;
            }
        }

        #endregion Start/Stop

        private string GetLanguageName(InputLayout layout) {
            return ConfigService.DoShowLanguageNameInNative
                ? layout.LanguageNameThreeLetterNative.ToLower()
                : layout.LanguageNameThreeLetter.ToLower();
        }

        private bool GetDoShowTooltip() {
            var doShowTooltip = ConfigService.DoShowCursorTooltip_WhenFocusNotChanged;
            if (!doShowTooltip) {
                var currentFocusedWindowHandle = Win32.GetForegroundWindow();
                doShowTooltip = currentFocusedWindowHandle != LastFocusedWindowHandle;
                Debug.WriteLine($"Last focused handle was {LastFocusedWindowHandle}");
                Debug.WriteLine($"New focused handle is {currentFocusedWindowHandle}");
                LastFocusedWindowHandle = currentFocusedWindowHandle;
            }
            return doShowTooltip;
        }

        private void ShowTooltip(MouseEventArgs2 e) {
            if (GetDoShowTooltip()) {
                var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
                var currentLayout = languageService.GetCurrentLayout();
                if (currentLayout != null) {
                    var text = GetLanguageName(currentLayout);
                    var tooltipService = ServiceRegistry.Instance.Get<ITooltipService>();
                    var colorSet = ConfigService.LayoutColorSetArray.FirstOrDefault(x => x.LayoutId == currentLayout.LayoutId);
                    tooltipService.Push(text, new System.Drawing.Point(e.Point.X, e.Point.Y), true, colorSet);
                }
            }
        }

        private void UpdateTooltip(MouseEventArgs2 e) {
            var tooltipService = ServiceRegistry.Instance.Get<ITooltipService>();
            if (tooltipService.GetIsVisible()) {
                var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
                var currentLayout = languageService.GetCurrentLayout();
                if (currentLayout != null) {
                    var text = GetLanguageName(currentLayout);
                    var colorSet = ConfigService.LayoutColorSetArray.FirstOrDefault(x => x.LayoutId == currentLayout.LayoutId);
                    tooltipService.Push(text, new System.Drawing.Point(e.Point.X, e.Point.Y), false, colorSet);
                }
            }
        }

        #region Hook handling

        private int? HookProcedureWrapper(Func<int?> func) {
            var result = (int?) null;
            try {
                result = func();
            } catch (Exception ex) {
                Trace.TraceError(ex.ToString());
            }
            return result;
        }

        protected void Hooker_ButtonDown(object sender, MouseEventArgs2 e) {
            if (e.Buttons == MouseButtons.Left
                && CursorUtils.GetIsCurrentCursorBeam()) {
                ShowTooltip(e);
                IsLastDownHandled = true;
            } else
                IsLastDownHandled = false;
            var eventService = ServiceRegistry.Instance.Get<IEventService>();
            eventService?.RaiseMouseInput();
        }

        protected void Hooker_ButtonUp(object sender, MouseEventArgs2 e) {
            var tooltipService = ServiceRegistry.Instance.Get<ITooltipService>();
            if (e.Buttons == MouseButtons.Left
                && !IsLastDownHandled
                && !tooltipService.GetIsVisible()
                && CursorUtils.GetIsCurrentCursorBeam()) {
                ShowTooltip(e);
            }
            IsLastDownHandled = false;
            var eventService = ServiceRegistry.Instance.Get<IEventService>();
            eventService?.RaiseMouseInput();
        }

        protected void Hooker_MouseMove(object sender, MouseEventArgs2 e) {
            UpdateTooltip(e);
            var eventService = ServiceRegistry.Instance.Get<IEventService>();
            eventService?.RaiseMouseInput();
        }

        #endregion Hook handling

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                }

                Stop();

                disposedValue = true;
            }
        }

        ~MouseCursorService() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
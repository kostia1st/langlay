using System;
using System.Diagnostics;
using System.Linq;
using Product.Common;

namespace Product {
    public class AutoSwitchService : IAutoSwitchService, ILifecycled {
        private IEventService EventService { get; set; }
        private LastInputTimer Timer { get; set; }

        #region Start/Stop
        public bool IsStarted { get; private set; }

        public void Start() {
            if (!IsStarted) {
                IsStarted = true;
                Timer = new LastInputTimer {
                    OnTimer = HandleTimer
                };

                EventService = ServiceRegistry.Instance.Get<IEventService>();
                EventService.KeyboardInput += Timer.SignalInput;
                EventService.MouseInput += Timer.SignalInput;

                Timer.Start();
            }
        }

        public void Stop() {
            if (IsStarted) {
                IsStarted = false;

                EventService.KeyboardInput -= Timer.SignalInput;
                EventService.MouseInput -= Timer.SignalInput;

                Timer.Stop();
                Timer = null;
                LastFocusedWindowHandle = null;
            }
        }
        #endregion Start/Stop

        private IntPtr? LastFocusedWindowHandle { get; set; }

        private void HandleTimer() {
            var configService = ServiceRegistry.Instance.Get<IConfigService>();
            if (configService.AppBindingArray.Count > 0) {
                var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
                var currentFocusedWindowHandle = Win32.GetForegroundWindow();
                if (currentFocusedWindowHandle != IntPtr.Zero) {
                    if (
                        LastFocusedWindowHandle != null
                        && LastFocusedWindowHandle != currentFocusedWindowHandle
                    ) {
                        var text = Win32.GetWindowText(currentFocusedWindowHandle);
                        Debug.WriteLine("Active window title: " + text);
                        var binding = configService.AppBindingArray.FirstOrDefault(x => text.Contains(x.AppMask));
                        if (binding != null) {
                            var inputLayouts = languageService.GetInputLayouts();
                            var layout = inputLayouts.FirstOrDefault(x => x.LayoutId == binding.LanguageOrLayoutId);
                            if (layout != null) {
                                Debug.WriteLine($"Attempting to restore layout {layout.LanguageName} - {layout.Name}");
                                languageService.SetCurrentLayout(layout);
                            } else {
                                Debug.WriteLine($"Attempting to restore language {binding.LanguageOrLayoutId}");
                                languageService.SetCurrentLanguage(binding.LanguageOrLayoutId, true);
                            }
                        }
                    }
                    LastFocusedWindowHandle = currentFocusedWindowHandle;
                }
            }
        }
    }
}

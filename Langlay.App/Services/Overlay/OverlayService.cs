using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using Product.Common;

namespace Product {
    public class OverlayService : IOverlayService, ILifecycled {
        private IEventService EventService { get; set; }
        private IConfigService ConfigService { get; set; }

        private IDictionary<string, OverlayForm> Overlays { get; set; }
        private LastInputTimer Timer { get; set; }

        public OverlayService() {
            Overlays = new Dictionary<string, OverlayForm>();
        }

        private OverlayForm CreateOverlay(Screen screen) {
            var overlayForm = new OverlayForm {
                MillisecondsToKeepVisible = ConfigService.OverlayDuration,
                OpacityWhenVisible = ConfigService.OverlayOpacity,
                ScalingPercent = ConfigService.OverlayScale,
                DisplayLocation = ConfigService.OverlayLocation,
                RoundCorners = ConfigService.DoShowOverlayRoundCorners,
                Screen = screen
            };

            overlayForm.InitializeRenderingCoefficient();

            return overlayForm;
        }

        #region Start/Stop
        public bool IsStarted { get; private set; }

        public void Start() {
            ConfigService = ServiceRegistry.Instance.Get<IConfigService>();
            if (!IsStarted && ConfigService.DoShowOverlay) {
                IsStarted = true;
                foreach (var screen in Screen.AllScreens) {
                    if (!ConfigService.DoShowOverlayOnMainDisplayOnly || screen.Primary)
                        Overlays[screen.DeviceName] = CreateOverlay(screen);
                }

                Timer = new LastInputTimer();
                Timer.OnTimer = HandleTimer;
                Timer.Start();

                EventService = ServiceRegistry.Instance.Get<IEventService>();
                EventService.KeyboardInput += Timer.SignalInput;
                EventService.MouseInput += Timer.SignalInput;
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
        }

        public void Stop() {
            if (IsStarted) {
                IsStarted = false;

                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;

                EventService.KeyboardInput -= Timer.SignalInput;
                EventService.MouseInput -= Timer.SignalInput;

                Timer.Stop();
                Timer = null;

                PreviousLayoutHandle = null;

                foreach (var pair in Overlays) {
                    if (pair.Value != null) {
                        pair.Value.Dispose();
                    }
                }
                Overlays.Clear();
            }
        }

        #endregion Start/Stop

        private IntPtr? PreviousLayoutHandle { get; set; }

        private void HandleTimer() {
            var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
            var currentLayoutHandle = languageService.GetCurrentLayoutHandle();
            if (currentLayoutHandle != IntPtr.Zero) {
                if (PreviousLayoutHandle != null
                    && PreviousLayoutHandle != currentLayoutHandle) {
                    var currentLayout = languageService.GetCurrentLayout();
                    if (currentLayout != null)
                        PushMessage(GetLanguageName(currentLayout), currentLayout.Name);
                }
                PreviousLayoutHandle = currentLayoutHandle;
            }
        }

        private string GetLanguageName(InputLayout layout) {
            return ConfigService.DoShowLanguageNameInNative
                ? layout.LanguageNameThreeLetterNative.ToUpper()
                : layout.LanguageNameThreeLetter.ToUpper();
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, System.EventArgs e) {
            if (IsStarted) {
                Stop();
                Start();
            }
        }

        public void PushMessage(string languageName, string layoutName) {
            if (IsStarted) {
                foreach (var overlay in Overlays.Values) {
                    overlay.PushMessage(languageName, layoutName);
                }
            }
        }
    }
}
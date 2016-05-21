using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Product
{
    public class OverlayService : IOverlayService
    {
        private IConfigService ConfigService { get; set; }
        private ILanguageService LanguageService { get; set; }
        private bool IsStarted { get; set; }
        private IDictionary<string, OverlayForm> Overlays { get; set; }
        private Timer LanguageCheckTimer { get; set; }

        public OverlayService(
            IConfigService configService, ILanguageService languageService)
        {
            ConfigService = configService;
            LanguageService = languageService;
            Overlays = new Dictionary<string, OverlayForm>();
        }

        private OverlayForm CreateOverlay(Screen screen)
        {
            var overlayForm = new OverlayForm();
            overlayForm.MillisecondsToKeepVisible = ConfigService.OverlayDuration;
            overlayForm.OpacityWhenVisible = ConfigService.OverlayOpacity;
            overlayForm.ScalingPercent = ConfigService.OverlayScale;
            overlayForm.DisplayLocation = ConfigService.OverlayLocation;
            overlayForm.RoundCorners = ConfigService.DoShowOverlayRoundCorners;
            overlayForm.Screen = screen;

            overlayForm.InitializeRenderingCoefficient();

            return overlayForm;
        }

        #region Start/Stop

        public void Start()
        {
            if (!IsStarted && ConfigService.DoShowOverlay)
            {
                IsStarted = true;
                foreach (var screen in Screen.AllScreens)
                {
                    if (!ConfigService.DoShowOverlayOnMainDisplayOnly || screen.Primary)
                        Overlays[screen.DeviceName] = CreateOverlay(screen);
                }
                StartTimer();
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;

                IsStarted = false;
                StopTimer();
                foreach (var pair in Overlays)
                {
                    if (pair.Value != null)
                    {
                        pair.Value.Dispose();
                    }
                }
                Overlays.Clear();
            }
        }

        #endregion Start/Stop

        #region Timer

        private void StartTimer()
        {
            LanguageCheckTimer = new Timer();
            LanguageCheckTimer.Interval = 25;
            LanguageCheckTimer.Tick += LanguageCheckTimer_Tick;
            LanguageCheckTimer.Start();
        }

        private void StopTimer()
        {
            if (LanguageCheckTimer != null)
            {
                LanguageCheckTimer.Stop();
                LanguageCheckTimer.Tick -= LanguageCheckTimer_Tick;
                LanguageCheckTimer.Dispose();
                LanguageCheckTimer = null;
            }
        }

        private IntPtr? _previousLayoutHandle;

        private void LanguageCheckTimer_Tick(object sender, System.EventArgs e)
        {
            var currentLayoutHandle = LanguageService.GetCurrentLayoutHandle();
            if (_previousLayoutHandle != null && _previousLayoutHandle != currentLayoutHandle)
            {
                var currentLayout = LanguageService.GetCurrentLayout();
                if (currentLayout == null)
                    throw new NullReferenceException("currentLayout must not be null");
                PushMessage(GetLanguageName(currentLayout), currentLayout.Name);
            }
            _previousLayoutHandle = currentLayoutHandle;
        }

        #endregion Timer

        private string GetLanguageName(InputLayout layout)
        {
            return ConfigService.DoShowLanguageNameInNative
                ? layout.LanguageNameThreeLetterNative.ToUpper()
                : layout.LanguageNameThreeLetter.ToUpper();
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, System.EventArgs e)
        {
            if (IsStarted)
            {
                Stop();
                Start();
            }
        }

        public void PushMessage(string languageName, string layoutName)
        {
            if (IsStarted)
            {
                foreach (var overlay in Overlays.Values)
                {
                    overlay.PushMessage(languageName, layoutName);
                }
            }
        }
    }
}
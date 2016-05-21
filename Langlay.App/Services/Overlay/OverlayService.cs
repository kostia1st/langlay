using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Product
{
    public class OverlayService : IOverlayService
    {
        private IConfigService ConfigService { get; set; }
        private ILanguageService LanguageService { get; set; }
        private IEventService EventService { get; set; }
        private bool IsStarted { get; set; }
        private IDictionary<string, OverlayForm> Overlays { get; set; }
        private Timer LanguageCheckTimer { get; set; }

        public OverlayService(
            IConfigService configService, ILanguageService languageService,
            IEventService eventService)
        {
            ConfigService = configService;
            LanguageService = languageService;
            EventService = eventService;
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
                EventService.KeyboardInput += EventService_Input;
                EventService.MouseInput += EventService_Input;
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;

                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
                EventService.KeyboardInput -= EventService_Input;
                EventService.MouseInput -= EventService_Input;

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

        private const uint PeriodToCheckForLayoutSwitch = 1000;
        private Stopwatch _lastInputElapsed = new Stopwatch() { };

        private void EventService_Input()
        {
            _lastInputElapsed.Restart();
            if (GetIsTimerPaused())
                ResumeTimer();
        }

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

        private void PauseTimer()
        {
            if (LanguageCheckTimer != null)
            {
                LanguageCheckTimer.Stop();
            }
        }

        private void ResumeTimer()
        {
            if (LanguageCheckTimer != null)
            {
                LanguageCheckTimer.Start();
            }
        }

        private bool GetIsTimerPaused()
        {
            return LanguageCheckTimer != null && !LanguageCheckTimer.Enabled;
        }

        private IntPtr? _previousLayoutHandle;

        private void LanguageCheckTimer_Tick(object sender, System.EventArgs e)
        {
            if (_lastInputElapsed.IsRunning
                && _lastInputElapsed.ElapsedMilliseconds < PeriodToCheckForLayoutSwitch)
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
            else
            {
                _lastInputElapsed.Stop();
                if (!GetIsTimerPaused())
                    PauseTimer();
            }
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
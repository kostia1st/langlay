using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Product
{
    public class OverlayService : IOverlayService
    {
        private IConfigService ConfigService { get; set; }
        private bool IsStarted { get; set; }
        private IDictionary<string, OverlayForm> Overlays { get; set; }

        public OverlayService(IConfigService configService)
        {
            ConfigService = configService;
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

        public void Start()
        {
            if (!IsStarted)
            {
                if (ConfigService.DoShowOverlay)
                {
                    IsStarted = true;
                    foreach (var screen in Screen.AllScreens)
                    {
                        if (!ConfigService.DoShowOverlayOnMainDisplayOnly || screen.Primary)
                        {
                            Overlays[screen.DeviceName] = CreateOverlay(screen);
                        }
                    }
                    SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
                }
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, System.EventArgs e)
        {
            if (IsStarted)
            {
                Stop();
                Start();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;

                IsStarted = false;
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

using System.Collections.Generic;
using System.Windows.Forms;

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
            overlayForm.MillisecondsToKeepVisible = ConfigService.OverlayMilliseconds;
            overlayForm.OpacityWhenVisible = ConfigService.OverlayOpacity;
            overlayForm.DisplayLocation = ConfigService.OverlayLocation;
            overlayForm.Screen = screen;
            return overlayForm;
        }

        public void Start()
        {
            if (!IsStarted)
            {
                if (ConfigService.ShowOverlay)
                {
                    IsStarted = true;
                    foreach (var screen in Screen.AllScreens)
                    {
                        if (!ConfigService.ShowOverlayOnMainDisplayOnly || screen.Primary)
                        {
                            Overlays[screen.DeviceName] = CreateOverlay(screen);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
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

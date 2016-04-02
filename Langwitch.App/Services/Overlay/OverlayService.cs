namespace Product
{
    public class OverlayService : IOverlayService
    {
        private IConfigService ConfigService { get; set; }
        private bool IsStarted { get; set; }
        private OverlayForm OverlayForm { get; set; }

        public OverlayService(IConfigService configService)
        {
            ConfigService = configService;
        }

        public void Start()
        {
            if (!IsStarted)
            {
                if (ConfigService.ShowOverlay)
                {
                    IsStarted = true;
                    OverlayForm = new OverlayForm();
                    OverlayForm.MillisecondsToKeepVisible = ConfigService.OverlayMilliseconds;
                }
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                if (OverlayForm != null)
                {
                    OverlayForm.Dispose();
                    OverlayForm = null;
                }
            }
        }

        public void PushMessage(string languageName, string layoutName)
        {
            if (IsStarted)
            {
                OverlayForm.PushMessage(languageName, layoutName);
            }
        }
    }
}

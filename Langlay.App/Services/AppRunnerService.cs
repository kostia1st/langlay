using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class AppRunnerService : IAppRunnerService
    {
        private bool _doRereadAndRun;
        private bool _isExiting;
        private AppMessageFilter _messageFilter;

        public bool IsExiting
        {
            get { return _isExiting; }
        }

        public AppRunnerService()
        {
            _messageFilter = new AppMessageFilter
            {
                OnClose = delegate { ExitApplication(); },
            };
        }

        public IConfigService ReadConfig()
        {
            // We read the global config first, then the user's one (higher
            // priority) and finally the command line (the highest priority).
            var configService = new ConfigService();
            configService.ReadFromConfigFile(false);
            configService.ReadFromConfigFile(true);
            configService.ReadFromCommandLineArguments();
            return configService;
        }

        public void ReReadAndRunTheConfig()
        {
            _doRereadAndRun = true;
            Application.Exit();
        }

        public void ExitApplication()
        {
            _isExiting = true;
            Application.Exit();
        }

        /// <summary>
        /// Runs the given config for one time.
        /// </summary>
        /// <returns>true if a restart requested</returns>
        public bool RunTheConfig(IConfigService configService)
        {
            // Here we make sure that registry contains the proper value in
            // the Startup section
            WindowsStartupUtils.WriteRunValue(configService.DoRunAtWindowsStartup);
            var eventService = new EventService();
            var settingsService = new SettingsService(configService, this);

            var languageService = new LanguageService(configService);
            var overlayService = new OverlayService(configService, languageService, eventService);
            var hotkeyService = new HookedHotkeyService(configService, languageService, eventService);
            var tooltipService = new TooltipService(configService);
            var mouseCursorService = new MouseCursorService(
                configService, languageService, tooltipService, eventService);
            var trayService = new TrayService(configService, settingsService, this);

            ILanguageSetterService languageSetterService;
            if (configService.SwitchMethod == SwitchMethod.InputSimulation)
                languageSetterService = new SimulatorLanguageSetterService(hotkeyService, languageService);
            else
                languageSetterService = new MessageLanguageSetterService();

            languageService.LanguageSetterService = languageSetterService;

            settingsService.Start();
            overlayService.Start();
            hotkeyService.Start();
            tooltipService.Start();
            mouseCursorService.Start();
            trayService.Start();

            try
            {
                // Make the app react properly to external events.
                // Registration of a filter is required per each application Run.
                Application.AddMessageFilter(_messageFilter);

                // Here we check if we need to show the settings up
                // immediately (it's likely the first app run)
                if (configService.DoShowSettingsOnce)
                    settingsService.ShowSettings();

                Application.Run();
            }
            finally
            {
                Application.RemoveMessageFilter(_messageFilter);
                trayService.Stop();
                mouseCursorService.Stop();
                tooltipService.Stop();
                hotkeyService.Stop();
                overlayService.Stop();
                settingsService.Stop();
            }
            var doRereadAndRun = _doRereadAndRun;
            _doRereadAndRun = false;
            return doRereadAndRun;
        }
    }
}
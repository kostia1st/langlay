using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class AppRunnerService : IAppRunnerService
    {
        private bool _doRereadAndRun;

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
            Application.Exit();
            _doRereadAndRun = true;
        }

        public void RunTheConfig(IConfigService configService)
        {
            // Here we make sure that registry contains the proper value in
            // the Startup section
            WindowsStartupUtils.WriteRunValue(configService.DoRunAtWindowsStartup);

            var settingsService = new SettingsService(configService, this);

            var overlayService = new OverlayService(configService);
            var languageService = new LanguageService(configService, overlayService);
            var hotkeyService = new HookedHotkeyService(configService, languageService);
            var tooltipService = new TooltipService(configService);
            var mouseCursorService = new MouseCursorService(
                configService, languageService, tooltipService);
            var trayService = new TrayService(configService, settingsService);

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
                // Here we check if we need to show the settings up immediately
                // (it's likely the first app run)
                if (configService.DoShowSettingsOnce)
                    settingsService.ShowSettings();
                Application.Run();
            }
            finally
            {
                trayService.Stop();
                mouseCursorService.Stop();
                tooltipService.Stop();
                hotkeyService.Stop();
                overlayService.Stop();
                settingsService.Stop();
            }
            if (_doRereadAndRun)
            {
                _doRereadAndRun = false;
                configService = ReadConfig();
                RunTheConfig(configService);
            }
        }
    }
}
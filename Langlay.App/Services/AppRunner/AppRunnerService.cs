using System.Collections.Generic;
using System.Windows.Forms;
using Product.Common;

namespace Product {
    public class AppRunnerService : IAppRunnerService {
        private readonly AppMessageFilter _messageFilter;
        private bool _doRereadAndRun;
        public bool IsExiting { get; private set; }

        public AppRunnerService() {
            _messageFilter = new AppMessageFilter {
                OnClose = delegate { ExitApplication(); },
            };
        }

        public IConfigService ReadConfig() {
            // We read the global config first, then the user's one (higher
            // priority) and finally the command line (the highest priority).
            var configService = new ConfigService();
            configService.ReadFromConfigFile(false);
            configService.ReadFromConfigFile(true);
            configService.ReadFromCommandLineArguments();
            return configService;
        }

        public void ReReadAndRunTheConfig() {
            _doRereadAndRun = true;
            Application.Exit();
        }

        public void ExitApplication() {
            IsExiting = true;
            Application.Exit();
        }

        private IList<object> CreateServices() {
            var configService = ServiceRegistry.Instance.Get<IConfigService>();
            return new object[] {
                new EventService(),
                new SettingsService(),
                new LanguageService(),
                new OverlayService(),
                new HookedHotkeyService(),
                new TooltipService(),
                new MouseCursorService(),
                new TrayService(),
                new AutoSwitchService(),
                configService.SwitchMethod == SwitchMethod.InputSimulation
                    ? (ILanguageSetterService) new SimulatorLanguageSetterService()
                    : (ILanguageSetterService) new MessageLanguageSetterService()
            };
        }

        /// <summary>
        /// Runs the given config for one time.
        /// </summary>
        /// <returns>true if a restart requested</returns>
        public bool RunTheConfig() {
            var configService = ServiceRegistry.Instance.Get<IConfigService>();
            // Here we make sure that registry contains the proper value in
            // the Startup section
            WindowsStartupUtils.WriteRunValue(configService.DoRunAtWindowsStartup);

            var services = CreateServices();
            ServiceRegistry.Instance.RegisterMany(services);

            var lifecycledServices = ServiceRegistry.Instance.GetMany<ILifecycled>();
            foreach (var service in lifecycledServices) {
                service.Start();
            }

            try {
                // Make the app react properly to external events.
                // Registration of a filter is required per each application Run.
                Application.AddMessageFilter(_messageFilter);

                // Here we check if we need to show the settings up
                // immediately (it's likely the first app run)
                if (configService.DoShowSettingsOnce) {
                    var settingsService = ServiceRegistry.Instance.Get<ISettingsService>();
                    settingsService.ShowSettings();
                }

                Application.Run();
            } finally {
                Application.RemoveMessageFilter(_messageFilter);

                foreach (var service in lifecycledServices) {
                    service.Stop();
                }
                ServiceRegistry.Instance.UnregisterMany(services);
            }
            var doRereadAndRun = _doRereadAndRun;
            _doRereadAndRun = false;
            return doRereadAndRun;
        }
    }
}
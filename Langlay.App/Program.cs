using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    internal static class Program
    {
        private static IConfigService InitializeConfig()
        {
            // We read the global config first, then the user's one (higher priority)
            // and finally the command line (the highest priority).
            var configService = new ConfigService();
            configService.ReadFromConfigFile(false);
            configService.ReadFromConfigFile(true);
            configService.ReadFromCommandLineArguments();
            return configService;
        }

        private static bool IsAppInitialized { get; set; }

        private static void InitializeApp()
        {
            if (!IsAppInitialized)
            {
                IsAppInitialized = true;

                // We must let Windows know that our app is DPI aware,
                // so the sizes and coordinates don't get scaled behind the scene
                Win32.SetProcessDPIAware();

                // Make the app react properly to external events
                Application.AddMessageFilter(new AppMessageFilter
                {
                    OnClose = delegate { Application.Exit(); },
                    OnRestart = delegate { Application.Restart(); }
                });
            }
        }

        private static void RunTheConfig(IConfigService configService)
        {
            // Here we make sure that registry contains the proper value
            // in the Startup section
            WindowsStartupUtils.WriteRunValue(configService.DoRunAtWindowsStartup);

            // Here we check if we need to show the settings up immediately
            // (it's likely the first app run)
            if (configService.DoShowSettingsOnce)
                AppUtils.ShowSettings();

            var overlayService = new OverlayService(configService);
            var languageService = new LanguageService(configService, overlayService);
            var hotkeyService = new HookedHotkeyService(configService, languageService);
            var tooltipService = new TooltipService(configService);
            var mouseCursorService = new MouseCursorService(configService, tooltipService);
            var trayService = new TrayService(configService);

            ILanguageSetterService languageSetterService;
            if (configService.SwitchMethod == SwitchMethod.InputSimulation)
                languageSetterService = new SimulatorLanguageSetterService(hotkeyService);
            else
                languageSetterService = new MessageLanguageSetterService();

            languageService.LanguageSetterService = languageSetterService;

            overlayService.Start();
            hotkeyService.Start();
            tooltipService.Start();
            mouseCursorService.Start();
            trayService.Start();

            try
            {
                Application.Run();
            }
            finally
            {
                trayService.Stop();
                mouseCursorService.Stop();
                tooltipService.Stop();
                hotkeyService.Stop();
                overlayService.Stop();
            }
        }

        [STAThread]
        private static void Main()
        {
            try
            {
                var configService = InitializeConfig();

                var uniquenessService = new UniquenessService(
                    Application.ProductName, configService.DoForceThisInstance,
                    delegate { ProcessUtils.StopMainApp(); });
                uniquenessService.Run(delegate
                {
                    InitializeApp();
                    RunTheConfig(configService);
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
#if DEBUG
                MessageBox.Show(ex.ToString());
#endif

                // In the release environment we don't show exceptions up
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                // We read the global config first, then the user's one (higher priority)
                // and finally the command line (the highest priority).
                var configService = new ConfigService();
                configService.ReadFromConfigFile(false);
                configService.ReadFromConfigFile(true);
                configService.ReadFromCommandLineArguments();

                var uniquenessService = new UniquenessService(
                    Application.ProductName, configService.DoForceThisInstance,
                    delegate { ProcessUtils.StopMainApp(); });
                uniquenessService.Run(delegate
                {
                    // We must let Windows know that our app is DPI aware,
                    // so the sizes and coordinates don't get scaled behind the scene
                    Win32.SetProcessDPIAware();

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

                    Application.AddMessageFilter(new AppMessageFilter
                    {
                        OnClose = delegate { Application.Exit(); },
                        OnRestart = delegate { Application.Restart(); }
                    });

                    overlayService.Start();
                    hotkeyService.Start();
                    tooltipService.Start();
                    mouseCursorService.Start();
                    trayService.Start();

                    try
                    {
                        // Here we make sure that registry contains the proper value in the Startup section
                        WindowsStartupUtils.WriteRunValue(configService.DoRunAtWindowsStartup);

                        // Here we check if we need to show the settings up immediately
                        // (it's likely the first app run)
                        if (configService.DoShowSettingsOnce)
                            AppUtils.ShowSettings();

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
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                var configService = new ConfigService();
                configService.ReadFromConfigFile(false);
                configService.ReadFromConfigFile(true);
                configService.ReadFromCommandLineArguments();

                var uniquenessService = new UniquenessService(
                    Application.ProductName, configService.DoForceThisInstance,
                    () => {
                        ProcessUtils.StopMainApp();
                    });
                uniquenessService.Run(delegate
                {
                    var overlayService = new OverlayService(configService);
                    var languageService = new LanguageService(configService, overlayService);
                    var hotkeyService = new HookedHotkeyService(configService, languageService);
                    var tooltipService = new TooltipService(configService);
                    var mouseCursorService = new MouseCursorService(configService, tooltipService);

                    ILanguageSetterService languageSetterService;
                    if (configService.SwitchMethod == SwitchMethod.InputSimulation)
                        languageSetterService = new SimulatorLanguageSetterService(hotkeyService);
                    else
                        languageSetterService = new MessageLanguageSetterService();

                    languageService.LanguageSetterService = languageSetterService;

                    var trayService = new TrayService(configService);

                    Application.AddMessageFilter(new AppMessageFilter
                    {
                        OnClose = delegate { Application.Exit(); },
                        OnRestart = delegate { Application.Restart(); }
                    });

                    hotkeyService.Start();
                    trayService.Start();
                    overlayService.Start();
                    tooltipService.Start();
                    mouseCursorService.Start();

                    WindowsStartupUtils.WriteRunValue(configService.DoRunAtWindowsStartup);

                    if (configService.DoShowSettingsOnce)
                        AppUtils.ShowSettings();

                    try
                    {
                        Application.Run();
                    }
                    catch (Exception)
                    {
#if DEBUG
                        throw;
#else
                        // Do nothing O_o as of yet.
#endif
                    }
                    finally
                    {
                        mouseCursorService.Stop();
                        tooltipService.Stop();
                        trayService.Stop();
                        hotkeyService.Stop();
                        overlayService.Stop();
                    }
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }
    }
}

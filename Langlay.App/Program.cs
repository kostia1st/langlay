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
            var uniquenessService = new UniquenessService(Application.ProductName);
            uniquenessService.RunOrIgnore(delegate
            {
                var configService = new ConfigService();
                try
                {
                    configService.ReadFromConfigFile(false);
                    configService.ReadFromConfigFile(true);
                    configService.ReadFromCommandLineArguments();

                    var startupService = new WinStartupService(configService);
                    var settingsService = new SettingsService(configService);

                    var overlayService = new OverlayService(configService);
                    var languageService = new LanguageService(configService, overlayService);
                    var hotkeyService = new HookedHotkeyService(configService, languageService);

                    ILanguageSetterService languageSetterService;
                    if (configService.SwitchMethod == SwitchMethod.InputSimulation)
                        languageSetterService = new SimulatorLanguageSetterService(hotkeyService);
                    else
                        languageSetterService = new MessageLanguageSetterService();

                    languageService.LanguageSetterService = languageSetterService;

                    var trayService = new TrayService(configService, settingsService)
                    {
                        OnExit = delegate { Application.Exit(); }
                    };

                    Application.AddMessageFilter(new AppMessageFilter
                    {
                        OnClose = delegate { Application.Exit(); },
                        OnRestart = delegate { Application.Restart(); }
                    });

                    hotkeyService.Start();
                    trayService.Start();
                    overlayService.Start();
                    startupService.ResolveStartup();
                    settingsService.ResolveFirstRun();

                    try
                    {
                        Application.Run();
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        throw;
#else
                        // Do nothing O_o as of yet.
#endif
                    }
                    finally
                    {
                        trayService.Stop();
                        hotkeyService.Stop();
                        overlayService.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            });
        }
    }
}

using System;
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
                configService.ReadFromConfigFile();
                configService.ReadFromCommandLineArguments();

                var startupService = new WinStartupService(configService);
                var settingsService = new SettingsService(configService);

                var overlayService = new OverlayService(configService);
                var languageService = new LanguageService(configService, overlayService);
                var hotkeyService = new HotkeyService(configService, languageService);

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

                try
                {
                    hotkeyService.Start();
                    trayService.Start();
                    overlayService.Start();
                    startupService.ResolveStartup();
                    settingsService.ResolveFirstRun();

                    Application.Run();
                }
                catch (Exception ex)
                {
                    // Do nothing O_o as of yet.
                }
                finally
                {
                    trayService.Stop();
                    hotkeyService.Stop();
                    overlayService.Stop();
                }
            });
        }
    }
}

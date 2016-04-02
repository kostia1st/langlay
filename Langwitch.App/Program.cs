using System;
using System.Threading;
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
                var isExiting = false;

                var configService = new ConfigService();
                configService.ReadFromConfigFile();
                configService.ReadFromCommandLineArguments();

                var startupService = new WinStartupService(configService);

                ILanguageSetterService languageSetterService;
                if (configService.SwitchMethod == SwitchMethod.InputSimulation)
                    languageSetterService = new SimulatorLanguageSetterService();
                else
                    languageSetterService = new MessageLanguageSetterService();

                var overlayService = new OverlayService(configService);
                var languageService = new LanguageService(configService, overlayService, languageSetterService);
                var hotkeyService = new HotkeyService(configService, languageService);
                var trayService = new TrayService(configService)
                {
                    OnExit = delegate { isExiting = true; }
                };

                Application.AddMessageFilter(new AppMessageFilter { OnClose = delegate { isExiting = true; } });

                try
                {
                    hotkeyService.Start();
                    trayService.Start();
                    overlayService.Start();
                    startupService.ResolveStartup();

                    RunUntil(() => !isExiting);
                }
                finally
                {
                    trayService.Stop();
                    hotkeyService.Stop();
                    overlayService.Stop();
                }
            });
        }

        private static void RunUntil(Func<bool> predicate)
        {
            while (true)
            {
                // This is really questionable, and seems it causes some functional issues.
                // To check it out, increase the value to 500 for instance.
                Thread.Sleep(5);
                try
                {
                    Application.DoEvents();

                    if (!predicate())
                        // Quitting the loop must be just enough
                        break;
                }
                catch (Exception ex)
                {
                    // Do nothing O_o as of yet.
                    break;
                }
            }
        }

    }
}

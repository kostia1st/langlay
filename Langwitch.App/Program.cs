using System;
using System.Threading;
using System.Windows.Forms;

namespace Langwitch
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var uniquenessService = new UniquenessService("Langwitch");
            uniquenessService.RunOrIgnore(delegate
            {
                var isExiting = false;

                var configService = new ConfigService();
                configService.ReadFromConfigFile();
                configService.ReadFromCommandLineArguments();

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

                try
                {

                    hotkeyService.Start();
                    trayService.Start();
                    overlayService.Start();
                    while (true)
                    {
                        // This is really questionable, and seems it causes some functional issues.
                        // To check it out, increase the value to 500 for instance.
                        Thread.Sleep(5);
                        try
                        {
                            Application.DoEvents();
                            if (isExiting)
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

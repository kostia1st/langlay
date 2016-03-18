using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Langwitch
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var isExiting = false;

            var configService = new ConfigService();
            var hotkeyService = new HotkeyService(configService);
            var trayService = new TrayService(configService)
            {
                OnExit = delegate { isExiting = true; }
            };
            try
            {
                configService.ReadFromConfigFile();
                configService.ReadFromCommandLineArguments();
                hotkeyService.Start();
                trayService.Start();
                while (true)
                {
                    Thread.Sleep(1);
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
            }
        }

    }
}

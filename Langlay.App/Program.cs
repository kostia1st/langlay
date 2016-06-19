using System;
using System.Windows.Forms;
using Product.Common;

#if TRACE

using System.Diagnostics;
#endif

namespace Product
{
    internal static class Program
    {
        private static bool IsAppInitialized { get; set; }

        private static void InitializeApp()
        {
            if (!IsAppInitialized)
            {
                IsAppInitialized = true;

                // We must let Windows know that our app is DPI aware, so
                // the sizes and coordinates don't get scaled behind the scene
                Win32.SetProcessDPIAware();
            }
        }

        [STAThread]
        private static void Main()
        {
            try
            {
                var appRunnerService = new AppRunnerService();
                var configService = appRunnerService.ReadConfig();

                var uniquenessService = new UniquenessService(
                    Application.ProductName, configService.DoForceThisInstance,
                    delegate { ProcessUtils.StopOtherMainApp(); });
                uniquenessService.Run(delegate
                {
                    InitializeApp();
                    var restartRequested = appRunnerService.RunTheConfig(configService);
                    while (restartRequested)
                    {
                        configService = appRunnerService.ReadConfig();
                        restartRequested = appRunnerService.RunTheConfig(configService);
                    }
                });
            }
            catch (Exception ex)
            {
#if TRACE
                Trace.TraceError(ex.ToString());
#elif DEBUG
                MessageBox.Show(ex.ToString());
#endif

                // In the release environment we don't show exceptions up
            }
        }
    }
}
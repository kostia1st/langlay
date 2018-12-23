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
                ServiceRegistry.Instance = new ServiceRegistry();
                var appRunnerService = ServiceRegistry.Instance.Register(new AppRunnerService());
                var configService = ServiceRegistry.Instance.Register(appRunnerService.ReadConfig());

                var uniquenessService = new UniquenessService(
                    Application.ProductName, configService.DoForceThisInstance,
                    delegate { ProcessUtils.StopOtherMainApp(); });
                uniquenessService.Run(delegate
                {
                    InitializeApp();
                    var restartRequested = appRunnerService.RunTheConfig();
                    while (restartRequested)
                    {
                        ServiceRegistry.Instance.Unregister(configService);
                        configService = ServiceRegistry.Instance.Register(appRunnerService.ReadConfig());

                        restartRequested = appRunnerService.RunTheConfig();
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
                // TODO: write to log
            }
        }
    }
}
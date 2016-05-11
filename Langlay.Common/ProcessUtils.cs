using System;
using System.Diagnostics;
using System.Linq;

namespace Product.Common
{
    public static class ProcessUtils
    {
        public static bool StopMainApp()
        {
            var result = false;
            var mainAppProcesses = Process.GetProcessesByName(AppSpecific.MainAppProcessName);
            var currentProcess = Process.GetCurrentProcess();
            var process = mainAppProcesses
                .FirstOrDefault(x => x.Id != currentProcess.Id);
#if DEBUG
            if (process == null)
            {
                process = Process
                    .GetProcessesByName(AppSpecific.MainAppProcessNameDebug)
                    .FirstOrDefault(x => x.Id != currentProcess.Id);
            }
#endif

            if (process != null)
            {
                var thread = process.Threads.Cast<ProcessThread>().FirstOrDefault();
                if (thread != null)
                    Win32.PostThreadMessage(thread.Id, Win32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                else
                    process.CloseMainWindow();

                if (!process.WaitForExit(2000))
                    process.Kill();

                result = true;
            }
            return result;
        }

        public static void StartMainApp(
            bool forceThisInstance = false,
            bool runAsAdmin = false)
        {
            var location = PathUtils.GetAppDirectory();
            var fullFilename =
                System.IO.Path.Combine(location, AppSpecific.MainAppFilename);
            var psi = new ProcessStartInfo
            {
                FileName = fullFilename,
                WorkingDirectory = location,
                UseShellExecute = runAsAdmin,
                Arguments = forceThisInstance ? $"--{ArgumentNames.ForceThisInstance}:true" : string.Empty,
                Verb = runAsAdmin ? "runas" : string.Empty,
                LoadUserProfile = true
            };
            Process.Start(psi);
        }
    }
}
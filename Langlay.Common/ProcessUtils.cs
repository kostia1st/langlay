using System;
using System.Diagnostics;
using System.Linq;

namespace Product.Common
{
    public static class ProcessUtils
    {
        public static bool StopOtherMainApp()
        {
            var result = false;
            var mainAppProcesses = Process.GetProcessesByName(AppSpecific.MainAppProcessName);
            var currentProcess = Process.GetCurrentProcess();
            var processes = mainAppProcesses
                .Where(x => x.Id != currentProcess.Id)
                .ToList();
#if DEBUG
            if (!processes.Any())
            {
                processes = Process
                    .GetProcessesByName(AppSpecific.MainAppProcessNameDebug)
                    .Where(x => x.Id != currentProcess.Id)
                    .ToList();
            }
#endif

            foreach (var process in processes)
            {
                var thread = process.Threads.Cast<ProcessThread>().FirstOrDefault();
                if (thread != null)
                    Win32.PostThreadMessage(thread.Id, Win32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                else
                    process.CloseMainWindow();

                if (!process.WaitForExit(2000))
                    process.Kill();

                result |= true;
            }
            return result;
        }

        public static void StartMainApp(
            string arguments,
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
                Arguments = arguments,
                Verb = runAsAdmin ? "runas" : string.Empty,
                LoadUserProfile = true
            };
            Process.Start(psi);
        }
    }
}
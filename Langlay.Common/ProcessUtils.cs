using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Product.Common
{
    public static class ProcessUtils
    {
        public static bool StopMainApp()
        {
            var process = Process.GetProcessesByName(AppSpecific.MainAppProcessName).FirstOrDefault();
#if DEBUG
            if (process == null)
                process = Process.GetProcessesByName(AppSpecific.MainAppProcessName + ".vshost").FirstOrDefault();
#endif

            if (process != null)
            {
                var thread = process.Threads.Cast<ProcessThread>().FirstOrDefault();
                if (thread != null)
                {
                    Win32.PostThreadMessage(thread.Id, Win32.WM_USER_RESTART, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    process.CloseMainWindow();
                }
                if (!process.WaitForExit(2000))
                {
                    process.Kill();

                }
                return true;
            }
            return false;
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
                Arguments = forceThisInstance ? "--ForceThisInstance:true" : string.Empty,
                Verb = runAsAdmin ? "runas" : string.Empty,
                LoadUserProfile = true
            };
            Process.Start(psi);
        }

    }
}

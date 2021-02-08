using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Product.Common {
    public class ProcessInfo {
        private Process _process;

        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public bool HasExited {
            get {
                try {
                    return _process.HasExited;
                } catch {
                    return true;
                }
            }
        }

        public ProcessInfo(Process process) {
            _process = process;
            ProcessId = process.Id;
            // This is an expensive operation
            ProcessName = process.ProcessName;
        }
    }

    public static class ProcessUtils {
        public const string ProcessName_Idle = "Idle";

        public static bool StopOtherMainApp() {
            var result = false;
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process
                .GetProcessesByName(AppSpecific.MainAppProcessName)
                .Where(x => x.Id != currentProcess.Id)
                .ToList();
#if DEBUG
            if (!processes.Any()) {
                processes = Process
                    .GetProcessesByName(AppSpecific.MainAppProcessNameDebug)
                    .Where(x => x.Id != currentProcess.Id)
                    .ToList();
            }
#endif

            foreach (var process in processes) {
                process.CloseMainWindow();

                var thread = process.Threads
                    .Cast<ProcessThread>()
                    .FirstOrDefault();

                if (thread != null) {
                    Win32.PostThreadMessage(thread.Id, Win32.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                }

                if (!process.WaitForExit(2000))
                    process.Kill();

                result |= true;
            }

            return result;
        }

        public static void StartMainApp(
            string arguments,
            bool runAsAdmin = false) {
            var location = PathUtils.GetAppDirectory();
            var fullFilename =
                System.IO.Path.Combine(location, AppSpecific.MainAppFilename);
            var psi = new ProcessStartInfo {
                FileName = fullFilename,
                WorkingDirectory = location,
                UseShellExecute = runAsAdmin,
                Arguments = arguments,
                Verb = runAsAdmin ? "runas" : string.Empty,
                LoadUserProfile = true
            };
            Process.Start(psi);
        }

        private static IDictionary<int, ProcessInfo> _processCache = new Dictionary<int, ProcessInfo>();
        public static ProcessInfo GetProcessById(int processId) {
            if (!_processCache.ContainsKey(processId)) {
                // This is very expensive, thus we attempt to cache it.
                var process = Process.GetProcessById(processId);
                if (process != null)
                    _processCache[processId] = new ProcessInfo(process);
            }
            return _processCache[processId];
        }
    }
}
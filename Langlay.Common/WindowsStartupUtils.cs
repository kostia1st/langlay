using Microsoft.Win32;

namespace Product.Common
{
    public static class WindowsStartupUtils
    {
        private const string RegistryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static bool CheckRunValue()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, false))
            {
                return key.GetValue(AppSpecific.MainAppTitle, null) != null;
            }
        }

        private static void WriteRunValue(
            bool shouldRun, string pathToExecutable)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                if (shouldRun && !string.IsNullOrEmpty(pathToExecutable))
                    key.SetValue(AppSpecific.MainAppTitle, pathToExecutable);
                else
                    key.DeleteValue(AppSpecific.MainAppTitle, false);
            }
        }

        public static void WriteRunValue(bool shouldRun)
        {
            WriteRunValue(shouldRun, PathUtils.GetAppExecutable());
        }
    }
}
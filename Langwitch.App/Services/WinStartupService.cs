using System.Windows.Forms;
using Microsoft.Win32;

namespace Product
{
    public class WinStartupService
    {
        private const string RegistryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private IConfigService ConfigService { get; set; }
        public WinStartupService(IConfigService configService)
        {
            ConfigService = configService;
        }

        public bool CheckRunValue()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, false))
            {
                return key.GetValue(Application.ProductName, null) != null;
            }
        }

        private void WriteRunValue(
            bool shouldRun, string pathToExecutable)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                if (shouldRun && !string.IsNullOrEmpty(pathToExecutable))
                    key.SetValue(Application.ProductName, pathToExecutable);
                else
                    key.DeleteValue(Application.ProductName, false);
            }
        }

        public void ResolveStartup()
        {
            WriteRunValue(ConfigService.DoRunAtWindowsStartup, Application.ExecutablePath);
        }
    }
}

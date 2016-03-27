using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Langwitch
{
    public class WinStartupService
    {
        private const string AppName = "Langwitch";
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
                return key.GetValue(AppName, null) != null;
            }
        }

        private void WriteRunValue(
            bool shouldRun, string pathToExecutable)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                if (shouldRun && !string.IsNullOrEmpty(pathToExecutable))
                    key.SetValue(AppName, pathToExecutable);
                else
                    key.DeleteValue(AppName, false);
            }
        }

        public void ResolveStartup()
        {
            WriteRunValue(ConfigService.DoRunAtWindowsStartup, Application.ExecutablePath);
        }
    }
}

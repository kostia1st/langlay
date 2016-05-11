using System.Diagnostics;
using System.IO;

namespace Product.Common
{
    public static class AppUtils
    {
        public static void ShowSettings()
        {
            var location = PathUtils.GetAppDirectory();
            var fullFileName = Path.Combine(location, AppSpecific.SettingsAppFilename);
            var psi = new ProcessStartInfo
            {
                FileName = fullFileName,
                WorkingDirectory = location,
                LoadUserProfile = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }
    }
}
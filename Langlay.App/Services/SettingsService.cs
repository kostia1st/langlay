using System.Diagnostics;
using System.IO;
using Product.Common;

namespace Product
{
    public class SettingsService : ISettingsService
    {
        private IConfigService ConfigService { get; set; }

        public SettingsService(IConfigService configService)
        {
            ConfigService = configService;
        }

        public void ShowSettings()
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

        public void ResolveFirstRun()
        {
            if (ConfigService.DoShowSettingsOnce)
                ShowSettings();
        }
    }
}

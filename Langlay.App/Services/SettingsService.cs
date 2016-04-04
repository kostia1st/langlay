using System.Diagnostics;
using System.IO;
using System.Reflection;
using Product.Common;

namespace Product
{
    public class SettingsService: ISettingsService
    {
        private IConfigService ConfigService { get; set; }

        public SettingsService(IConfigService configService)
        {
            ConfigService = configService;
        }

        public void ShowSettings()
        {
            var settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), AppSpecific.SettingsPath);
            Process.Start(settingsPath);
        }

        public void ResolveFirstRun()
        {
            if (ConfigService.DoShowSettingsOnce)
                ShowSettings();
        }
    }
}

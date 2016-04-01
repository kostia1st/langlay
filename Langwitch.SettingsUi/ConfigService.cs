using System.Configuration;
using Product.Common;

namespace Product.SettingsUi
{
    public class ConfigService : ConfigServiceBase
    {
        public ConfigService()
            : base(ConfigurationManager.OpenExeConfiguration("langwitch.exe.config"))
        {
        }
    }
}

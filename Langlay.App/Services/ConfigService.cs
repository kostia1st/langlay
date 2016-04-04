using System.Configuration;
using Product.Common;

namespace Product
{
    public class ConfigService: ConfigServiceBase, IConfigService
    {
        public ConfigService()
            : base(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None))
        {
        }
    }
}

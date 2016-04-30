using System.IO;
using System.Reflection;

namespace Product.Common
{
    public static class PathUtils
    {
        public static string GetAppDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}

using System.IO;
using System.Reflection;

namespace Product.Common
{
    public static class PathUtils
    {
        public static string GetAppExecutable()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        public static string GetAppDirectory()
        {
            return Path.GetDirectoryName(GetAppExecutable());
        }
    }
}

using System;

namespace NRLS_API.Core.Helpers
{
    public class DirectoryHelper
    {
        //useful to find the varyiing location between local and docker envs
        public static string GetBaseDirectory()
        {
            var appPath = AppContext.BaseDirectory;
            var pathEnd = appPath.LastIndexOf("bin");
            var isBinPath = !(pathEnd < 0);
            var basePath = isBinPath ? appPath.Substring(0, pathEnd) : appPath;
            var pathAffix = isBinPath ? @"..\" : "";

            return basePath;
        }
    }
}

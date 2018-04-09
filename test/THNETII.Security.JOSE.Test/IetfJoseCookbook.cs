using System;
using System.IO;
using System.Reflection;

namespace THNETII
{
    public static class IetfJoseCookbook
    {
        private static string assemblyFilePath = typeof(IetfJoseCookbook).GetTypeInfo().Assembly.Location;
        private static string assemblyDirectory = GetAssemblyDirectory();

        private static string GetAssemblyDirectory()
        {
            try { return Path.GetDirectoryName(assemblyFilePath); }
            catch (ArgumentException) { return null; }
            catch (PathTooLongException) { return null; }
        }

        public static string GetCookbookFilePath(string filename)
        {
            if (File.Exists(filename))
                return filename;
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
                return null;
            var filePathNextToAssembly = Path.Combine(assemblyDirectory, filename);
            if (File.Exists(filePathNextToAssembly))
                return filePathNextToAssembly;
            return null;
        }
    }
}

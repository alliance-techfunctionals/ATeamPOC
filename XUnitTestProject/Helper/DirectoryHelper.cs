using System.Collections.Generic;
using System.Linq;

namespace XsdToObjectTree.UnitTest.Helper
{
    using System.IO;

    public static class DirectoryHelper
    {
        public static List<string> GetFiles(string filePath, string filter)
        {
            var di = new DirectoryInfo(filePath);
            var files = di.GetFiles(filter).Select(e => e.FullName).ToList().OrderBy(f => f).ToList();
            return files;
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static string GetFileNameFromDirectoryPath(string path)
        {
            return Path.GetFileName(path);
        }


        public static void CreateIfNotExistDirectoryParent(string fileFullPath)
        {
            var file = new FileInfo(fileFullPath);
            file.Directory?.Create();
        }

        public static string GetDirectoryPath(string basepath, string filename)
        {
            return Path.Combine(basepath, filename);
        }
    }
}
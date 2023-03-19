using System;
using System.IO;

namespace OriDeModLoader.Util
{
    public class FileUtil
    {
        public static void TouchFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                System.IO.File.Create(fileName).Close();

            System.IO.File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
        }
    }

    public class PathUtil
    {
        public static string Combine(params string[] paths)
        {
            string path = "";
            for (int i = 0; i < paths.Length; i++)
                path = Path.Combine(path, paths[i]);
            return path;
        }
    }
}

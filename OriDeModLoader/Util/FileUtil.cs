using System;

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
}

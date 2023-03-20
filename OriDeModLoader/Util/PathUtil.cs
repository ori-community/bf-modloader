using System.IO;
using System.Reflection;

namespace OriDeModLoader.Util
{
    public class PathUtil
    {
        public static string Combine(params string[] paths)
        {
            string path = "";
            for (int i = 0; i < paths.Length; i++)
                path = Path.Combine(path, paths[i]);
            return path;
        }

        // These are only accurate for the mod loader itself, not individual mods
        internal static string CurrentDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static string MakeAbsolute(params string[] paths) => Path.GetFullPath(Path.Combine(CurrentDir, Combine(paths)));
    }
}

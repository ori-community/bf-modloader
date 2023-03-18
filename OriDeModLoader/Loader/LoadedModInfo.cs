namespace OriDeModLoader
{
    public class LoadedModInfo
    {
        public IMod Mod { get; }
        public string Path { get; }

        public LoadedModInfo(IMod mod, string path)
        {
            Mod = mod;
            Path = path;
        }

        public string DirName => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(this.Path));
    }
}

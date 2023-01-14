using System.IO;
using BFModLoader.ModLoader;
using OriDeModLoader.Loader;

namespace OriDeModLoader
{
    public class ModWatcher
    {
        
        private  ModLoader ModLoader { get; set; }
        public ModWatcher(ModLoader modLoader)
        {
            ModLoader = modLoader;
        }

        public void StartWatching()
        {
            var watcher = new FileSystemWatcher(ModLoader.ModDir);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.dll";

            watcher.Created += (sender, args) =>
            {
                ModLoader.LoadMod(args.FullPath);
            };
            watcher.Deleted += (sender, args) =>
            {
                ModLoader.UnloadMod(args.FullPath);
            };
            watcher.Changed += (sender, args) =>
            {
                ModLoader.LoadMod(args.FullPath, true);
            };
            watcher.Error += (sender, error) =>
            {
                Logger.Log("FileSystemWatcher errored: " + error);
            };
            watcher.EnableRaisingEvents = true;
        }

    }
}

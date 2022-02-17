using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace OriDeModLoader
{
    public static class Loader
    {
        private static List<IMod> loadedMods = new List<IMod>();
        private static FileSystemWatcher watcher;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.GetName().Name == "Assembly-CSharp")
            {
                Patch();

                watcher = new FileSystemWatcher("Mods", "*dll");
                watcher.Changed += Watcher_Changed;
                watcher.Created += Watcher_Changed;

                LoadMods();
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            foreach (var loadedMod in loadedMods)
                loadedMod.Unload();

            LoadMods();
        }

        private static void Patch()
        {
            var harmony = new Harmony("com.oride.modloader");
            harmony.PatchAll();
        }

        private static void LoadMods()
        {
            const string modsDir = "Mods";
            if (Directory.Exists(modsDir))
            {
                LoadFromAssemblies(Directory.GetFiles(modsDir, "*Lib.dll")); // todo actually good dependency resolution
                LoadFromAssemblies(Directory.GetFiles(modsDir, "*.dll").Where(m => !m.EndsWith("Lib.dll")));
            }
            else
            {
                Log("Could not find mods directory");
            }
        }

        private static void LoadFromAssemblies(IEnumerable<string> modFiles)
        {
            foreach (var s in modFiles)
            {
                Log("Loading " + s);
                try
                {
                    var assembly = Assembly.LoadFrom(s);
                    var modTypes = assembly.GetTypes().Where(t => typeof(IMod).IsAssignableFrom(t));

                    foreach (var modType in modTypes)
                    {
                        Log($"Loading {modType}");
                        var mod = (IMod)Activator.CreateInstance(modType);
                        loadedMods.Add(mod);

                        Log($"Initialising {mod.Name}");
                        mod.Init();
                    }
                }
                catch (Exception ex)
                {
                    Log($"Failed to load mod: {ex}");
                    throw;
                }
            }
        }

        public static void Log(string message)
        {
            using (var sw = new StreamWriter("loader.log", true))
                sw.WriteLine(message);
        }
    }
}

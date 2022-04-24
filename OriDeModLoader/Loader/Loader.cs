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
        private static readonly List<IMod> loadedMods = new List<IMod>();

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.GetName().Name == "Assembly-CSharp")
            {
                var harmony = new Harmony("com.oride.modloader");
                harmony.PatchAll();

                LoadMods();
            }
        }

        internal static void ReloadStrings()
        {
            foreach (var mod in loadedMods)
                Strings.InitSingle(mod.Name, GameSettings.Instance.Language);
        }

        private static void LoadMods()
        {
            const string modsDir = "Mods";
            if (Directory.Exists(modsDir))
            {
                LoadFromAssemblies(Directory.GetFiles(modsDir, "*.dll"));
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
                        Log($"Instantiating {modType}");
                        var mod = (IMod)Activator.CreateInstance(modType);
                        loadedMods.Add(mod);

                        Strings.InitSingle(mod.Name, Language.English); // English is primary fallback

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

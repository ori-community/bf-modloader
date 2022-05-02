using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BFModLoader.Util;
using HarmonyLib;
using OriDeModLoader.Util;

namespace OriDeModLoader
{
    public static class EntryPoint
    {
        private static readonly List<IMod> loadedMods = new List<IMod>();

        private const string ModLoaderDir = "C:\\moon\\Ori and the Blind Forest Mod Loader"; 
        
        public static void BootModLoader()
        { 
            //Attach a terminal to our game 
            ConsoleUtil.CreateConsole();
            FileUtil.TouchFile(ModLoaderDir + "\\modloader-heartbeat");
            
            //Patch time
            var harmony = new Harmony("com.oride.modloader");
            harmony.PatchAll();

            LoadMods();
        }
        internal static void ReloadStrings()
        {
            foreach (var mod in loadedMods)
                Strings.InitSingle(mod.Name, GameSettings.Instance.Language);
        }

        private static void LoadMods()
        {
            const string modsDir = ModLoaderDir + "\\mods";
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
            Console.Out.WriteLine(message);
        }
    }
}

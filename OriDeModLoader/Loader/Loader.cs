using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BaseModLib;
using BFModLoader.Util;
using HarmonyLib;
using OriDeModLoader.Util;
using SimpleJSON;
using UnityEngine;

namespace OriDeModLoader
{
    public static class EntryPoint
    {
        internal static readonly List<LoadedModInfo> loadedMods = new List<LoadedModInfo>();

        public static void BootModLoader()
        {
            FileUtil.TouchFile("modloader-heartbeat");

            Log("Booting mod loader");

            Application.logMessageReceived += LogCallback;

            //Patch time
            var harmony = new Harmony("com.oride.modloader");
            harmony.PatchAll();
            TitleScreenModMenu.Init();

            SettingsFile.LoadFromFile();

            try
            {
                LoadMods();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        public static void BootModLoaderWithDebug()
        {
            //Attach a terminal to our game
            ConsoleUtil.CreateConsole();
            BootModLoader();
        }

        internal static void ReloadStrings()
        {
            foreach (var mod in loadedMods)
                Strings.InitSingle(mod.DirName, GameSettings.Instance.Language);
        }

        private static void LoadMods()
        {
            string modsDir = PathUtil.MakeAbsolute("..");
            string manifestPath = Path.Combine(modsDir, "manifest.json");

            if (File.Exists(manifestPath))
            {
                Log("Reading " + manifestPath);

                var json = JSON.Parse(File.ReadAllText(manifestPath));
                foreach (var mod in json)
                {
                    string id = mod.Key;
                    if (id == "ModLoader")
                        continue;

                    if (mod.Value["enabled"].AsBool == true)
                    {
                        LoadMod(PathUtil.Combine(modsDir, id, "mod.json"));
                    }
                }
            }
            else
            {
                Log("Could not find manifest");
            }
        }

        private static void LoadMod(string modManifestPath)
        {
            var json = JSON.Parse(File.ReadAllText(modManifestPath));
            string dir = Path.GetDirectoryName(modManifestPath);

            if (json["name"] == "Mod Loader")
                return;

            var entrypoint = json["entrypoint"];
            if (entrypoint == null)
                return;

            // Load mod dependencies
            // TODO find a way to avoid this
            var preload = json["preload"];
            if (preload != null)
            {
                var preloadArray = preload.AsArray;
                foreach (var str in preloadArray)
                {
                    // ...
                }
            }

            foreach (var dllNode in entrypoint.AsArray)
            {
                var path = dllNode.Value.Value;
                LoadModAssembly(Path.Combine(dir, path));
            }
        }

        private static void LoadModAssembly(string path)
        {
            Log("Loading " + path);
            try
            {
                var assembly = Assembly.LoadFrom(path);
                var modTypes = assembly.GetTypes().Where(t => typeof(IMod).IsAssignableFrom(t));

                foreach (var modType in modTypes)
                {
                    var mod = (IMod)Activator.CreateInstance(modType);
                    var modInfo = new LoadedModInfo(mod, path);
                    loadedMods.Add(modInfo);

                    Strings.InitSingle(modInfo.DirName, Language.English); // English is primary fallback

                    Log($"Initialising {mod.Name}");
                    mod.Init();
                    Log($"Initialised {mod.Name}");
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to load mod: {ex}");
                throw;
            }
        }

        private static void LogCallback(string condition, string stackTrace, LogType type)
        {
            Log($"{Time.time:F5} [{type}] {condition}");
            if (!string.IsNullOrEmpty(stackTrace))
                Log(stackTrace);
        }

        public static void Log(string message)
        {
            Console.Out.WriteLine(message);
        }
    }
}

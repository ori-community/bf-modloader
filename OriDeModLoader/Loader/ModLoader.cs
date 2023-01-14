using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BFModLoader.ModLoader;
using HarmonyLib;
using OriDeModLoader.BFModLoader.ModLoader;

namespace OriDeModLoader.Loader
{
    public class ModLoader
    {
        private readonly Dictionary<string, ModState> _loadedModsById = new Dictionary<string, ModState>();
        private readonly Dictionary<string, ModState> _loadedModsByFile = new Dictionary<string, ModState>();

        public Dictionary<string, ModState>.ValueCollection Mods => _loadedModsById.Values;

        public const string ModDir = "mods";
        public const string ConfigDir = "mods";
        
        private ModWatcher watcher;

        public void Start()
        {
            watcher = new ModWatcher(this);
            watcher.StartWatching();
            ScanModDirectory();
        }

        private void ScanModDirectory()
        {
            Logger.Log("Scanning for mods");
            if (!Directory.Exists(ModDir))
            {
                Directory.CreateDirectory(ModDir);
            }

            if (!Directory.Exists(ConfigDir))
            {
                Directory.CreateDirectory(ConfigDir);
            }

            foreach (var file in Directory.GetFiles(ModDir, "*.dll"))
            {
                Logger.Log("Found file: " + file);
                LoadMod(file);
            }
        }
        
         public void LoadMod(string file, bool allowReload = false)
        {
            Logger.Log("Loading assembly: " + file);
            if (!IsMod(file))
            {
                Logger.Log("Assembly " + file + " cannot be loaded as a mod");
                return;
            }

            try
            {
                if (_loadedModsByFile.ContainsKey(file))
                {
                    if (allowReload)
                    {
                        UnloadMod(_loadedModsByFile[file]);
                    }
                    else
                    {
                        Logger.Log("Assembly " + file + " is already loaded");
                        return;
                    }
                }

                //Loading from in-memory Byte-Array will not place a lock on the file
                var modAssembly = Assembly.Load(File.ReadAllBytes(file));
                var modType = modAssembly.GetTypes()
                    .FirstOrDefault(type => !type.IsAbstract && typeof(IMod).IsAssignableFrom(type))?.FullName;

                if (modType != null && modAssembly.CreateInstance(modType) is IMod mod)
                {
                    if (_loadedModsById.TryGetValue(mod.ModID, out ModState loadedMod))
                    {
                        if (allowReload)
                        {
                            UnloadMod(loadedMod);
                        }
                        else
                        {
                            Logger.Log("Attempted to load mod " + mod.ModID + " multiple times", LogLevel.Warn);
                            return;
                        }
                    }

                    Harmony.DEBUG = true;
                    var harmony = new Harmony(mod.ModID + Guid.NewGuid());
                    _loadedModsById[mod.ModID] = new ModState
                    {
                        Mod = mod,
                        Harmony = harmony,
                        File = file,
                    };
                    _loadedModsByFile[file] = _loadedModsById[mod.ModID];

                    harmony.PatchAll(modAssembly);
                    mod.Init();
                    Logger.Log("Loaded mod " + mod.ModID + " with version " + mod.Version);
                }
                else
                {
                    Logger.Log("No mod present in assembly " + file, LogLevel.Error);
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.Log(e, fullLog: true);
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Logger.Log(assembly.FullName);
                }
            }
        }

        public void UnloadMod(ModState modState)
        {
            modState.Mod.Unload();
            modState.Harmony.UnpatchAll();
            _loadedModsById.Remove(modState.Mod.ModID);
            _loadedModsByFile.Remove(modState.File);

            Logger.Log("Unloaded mod " + modState.Mod.ModID);
        }

        public void UnloadMod(string file)
        {
            if (file == null)
                return;
            Logger.Log("Unloading file " + file);
            if (!_loadedModsByFile.TryGetValue(file, out var mod)) return;
            UnloadMod(mod);
        }

        private bool IsMod(string dll)
        {
            return true;
        }
    }
}

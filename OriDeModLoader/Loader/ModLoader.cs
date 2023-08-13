using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BFModLoader.ModLoader;
using HarmonyLib;
using OriDeModLoader.BFModLoader.ModLoader;
using OriDeModLoader.UIExtensions;

namespace OriDeModLoader.Loader
{
    public class ModLoader
    {
        private ModLoader()
        {
        }

        public static ModLoader Instance { get; } = new ModLoader();
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
                var modTypes = modAssembly.GetTypes()
                    .Where(type => !type.IsAbstract && typeof(IMod).IsAssignableFrom(type)).ToList();

                if (!modTypes.Any())
                    Logger.Log("No mod present in assembly " + file, LogLevel.Error);
                
                foreach (var modType in modTypes)
                {
                    if (modAssembly.CreateInstance(modType.FullName) is IMod mod)
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
                                continue;
                            }
                        }
                        
                        Harmony.DEBUG = true;
                        var harmony = new Harmony(mod.ModID + Guid.NewGuid());
                        var state = new ModState(mod, harmony, file);
                        _loadedModsById[mod.ModID] = state;
                        _loadedModsByFile[file] = state;
                        AccessTools.GetTypesFromAssembly(modAssembly).Where(type => (type.Namespace ?? "").StartsWith(modType.Namespace ?? "")).Do(type =>
                            harmony.CreateClassProcessor(type).Patch());
                        
                        foreach (var screen in mod.GetSettings())
                        {
                            var menu = CustomMenuManager.RegisterOptionsScreen<SettingsListOptionScreen>(screen.name, 3, options => options.Init(screen.settings));
                            state.Settings.Add(menu);
                        }
                        
                        mod.Init();
                        Logger.Log("Loaded mod " + mod.ModID + " with version " + mod.Version);
                    }
                
                    else
                    {
                        Logger.Log("Could not instantiate mod of type " + modType, LogLevel.Error);
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.Log(e, fullLog: true);
                Logger.Log(e.Data.ToString());
                try
                {
                    foreach (var eType in e.Types)
                    {
                        Logger.Log(eType.AssemblyQualifiedName);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, fullLog: true);
                }

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Logger.Log(assembly.FullName);
                }
            }
            catch (Exception e)
            {
                
                Logger.Log(e, fullLog: true);
                
            }
        }

        public void UnloadMod(ModState modState)
        {
            
            foreach (var def in modState.Settings)
            {
                CustomMenuManager.UnregisterSettingsScreen(def);
            }

            modState.Mod.Unload();
            modState.Harmony.UnpatchAll(modState.Harmony.Id);
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

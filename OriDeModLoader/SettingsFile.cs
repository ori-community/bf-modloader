using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BFModLoader.ModLoader;
using UnityEngine;
using Logger = BFModLoader.ModLoader.Logger;

namespace BaseModLib
{
    public static class SettingsFile
    {
        private const string ConfigDir = "configs";

        private static Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<string,string>>>> AllSettings = new Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<string,string>>>>();

        public static void RegisterSettingsGroup(string modName, string screenName, IEnumerable<SettingBase> settings)
        {
            if (!AllSettings.ContainsKey(modName))
                AllSettings.Add(modName, new Dictionary<string, Dictionary<string, KeyValuePair<string,string>>>());
            if (!AllSettings[modName].ContainsKey(screenName))
                AllSettings[modName].Add(screenName, new  Dictionary<string, KeyValuePair<string,string>>());

            var sets = AllSettings[modName][screenName];
            foreach (var set in settings) {
                try
                {
                    set.ModName = modName;
                    set.ScreenName = screenName;
                    if(sets.TryGetValue(set.Id, out var strset))
                        set.Parse(strset.Key);
                }
                catch (Exception e)
                {
                    Logger.Log($"failed to parse setting {set}: {e}");
                }

            }

        }
        public static void LoadFromDir()
        {
            if (!Directory.Exists(ConfigDir))
            {
                Directory.CreateDirectory(ConfigDir);
                return;
            }

            foreach (var dirName in Directory.GetDirectories(ConfigDir))
            {
                var modName = Path.GetFileName(dirName);
                AllSettings[modName] = new Dictionary<string, Dictionary<string, KeyValuePair<string,string>>>();
                foreach (var fileName in Directory.GetFiles(dirName))
                {
                    if (!fileName.EndsWith(".txt"))
                        continue;
                    var screenName = Path.GetFileName(fileName).Replace(".txt", "");
                    AllSettings[modName][screenName] = new Dictionary<string, KeyValuePair<string,string>>();
                    try {
                        using (var file = new StreamReader(fileName)) {
                            while (!file.EndOfStream) {
                                string line = file.ReadLine();
                                if (string.IsNullOrEmpty(line))
                                    continue;
                                var withoutComments = line.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries)[0];

                                var parts = withoutComments.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length != 2) {
                                    Logger.Log($"malformed settings line '{line}' in file {fileName}");
                                    continue;
                                }
                                AllSettings[modName][screenName][parts[0]] = new KeyValuePair<string,string>(parts[1], "");
                            }
                        }
                    }
                    catch(Exception e) {
                        Logger.Log($"malformed settings file {fileName}: {e}", LogLevel.Error);
                    }
                }
            }
        }

        public static void WriteToFiles() {
            foreach (var modAndGroups in AllSettings)
            {
                foreach (var screenAndSets in modAndGroups.Value)
                {
                    var path = $"configs/{modAndGroups.Key}";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    try
                    {
                        var settingsMap = screenAndSets.Value;
                        using (var file = new StreamWriter($"{path}/{screenAndSets.Key}.txt", false))
                            foreach (var setting in settingsMap)
                                file.WriteLine($"{setting.Key}: {setting.Value.Key.PadRight(50 - (setting.Key + setting.Value.Key).Length)}// {setting.Value.Value}");
                    } catch(Exception e)
                    {
                        Logger.Log($"error writing settings file for mod {modAndGroups.Key}: {e}", LogLevel.Error);
                    }
                }
            }
        }

        public static void Update(IEnumerable<SettingBase> settings)
        {
            foreach (var setting in settings)
            {
                AllSettings[setting.ModName][setting.ScreenName][setting.Id] = new KeyValuePair<string,string>(setting.ToString(), setting.Tooltip);
            }
            WriteToFiles();
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace BaseModLib
{
    public static class SettingsFile
    {
        private static string FilePath => Path.GetFullPath(Path.Combine("..", "settings.txt"));

        private static Dictionary<string, string> settingsMap = new Dictionary<string, string>();

        public static void LoadFromFile()
        {
            if (!File.Exists(FilePath))
                return;

            settingsMap = new Dictionary<string, string>();

            try
            {
                using (var file = new StreamReader(FilePath))
                {
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var parts = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 2)
                            continue;

                        settingsMap[parts[0]] = parts[1];
                    }
                }
            }
            catch
            {
                // TODO how to handle?
            }
        }

        public static void WriteToFile()
        {
            using (var file = new StreamWriter(FilePath, false))
            {
                foreach (var setting in settingsMap)
                    file.WriteLine($"{setting.Key}: {setting.Value}");
            }

            // TODO handle write error
        }

        public static void Update(IEnumerable<SettingBase> settings)
        {
            foreach (var setting in settings)
                settingsMap[setting.ID] = setting.ToString();

            WriteToFile();
        }

        public static string GetValue(string key) => settingsMap.ContainsKey(key) ? settingsMap[key] : null;
    }
}

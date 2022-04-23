using System.Collections.Generic;
using System.IO;

namespace OriDeModLoader
{
    public class Strings
    {
        private static readonly Dictionary<string, string> strings = new Dictionary<string, string>();

        internal static void InitSingle(string section, Language language)
        {
            // Game/Mods/strings/rando/en.txt
            // Game/Override/strings/rando/en.txt
            Load($"Mods/strings/{section}/{language}.txt");
            Load($"Override/strings/{section}/{language}.txt");
        }

        private static void Load(string path)
        {
            if (!File.Exists(path))
                return;

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    int comment = line.IndexOf('#');
                    if (comment >= 0)
                        line = line.Substring(0, comment);

                    if (string.IsNullOrEmpty(line))
                        continue;

                    int equals = line.IndexOf('=');
                    if (equals < 0)
                        continue;

                    strings[line.Substring(0, equals).Trim()] = line.Substring(equals + 1).Trim();
                }
            }
        }

        public static string Get(string key, params object[] args)
        {
            if (!strings.ContainsKey(key))
                return "WARNING: Unknown string " + key;

            return string.Format(strings[key], args);
        }
    }
}

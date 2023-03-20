﻿using System.Collections.Generic;
using System.IO;
using OriDeModLoader.Util;

namespace OriDeModLoader
{
    public class Strings
    {
        private static readonly Dictionary<string, string> strings = new Dictionary<string, string>();

        internal static void InitSingle(string modDir, Language language)
        {
            Load(PathUtil.MakeAbsolute($"../{modDir}/strings/{language}.txt"));
            //Load($"Override/strings/{section}/{language}.txt");
        }

        private static void Load(string path)
        {
            if (!File.Exists(path))
                return;

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line[0] == '#')
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

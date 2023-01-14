using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriDeModLoader
{
    public static class HarmonyHelper
    {
        public const bool ContinueExecution = true;
        public const bool StopExecution = false;
    }

    public static class TransformExtensions
    {
        public static string GetPath(this Transform transform)
        {
            List<string> parts = new List<string>();
            Transform t = transform;
            while (t != null)
            {
                parts.Add(t.name);
                t = t.parent;
            }

            parts.Reverse();
            return string.Join("/", parts.ToArray());
        }
    }

    public static class FileUtil
    {
        public static void TouchFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                System.IO.File.Create(fileName).Close();

            System.IO.File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
        }
    }
}

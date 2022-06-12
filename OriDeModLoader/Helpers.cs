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
}

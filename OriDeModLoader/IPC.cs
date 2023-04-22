using System;
using System.Collections.Generic;

namespace OriDeModLoader
{
    /// <summary>
    /// Inter-plugin communication
    /// Use this class to communication between different mods without needing to include them as dependencies.
    /// Useful for functionality that depends on other mods but is not required.
    /// Namespacing keys is recommended.
    /// </summary>
    public static class IPC
    {
        public delegate void ChangeCallback(string key, object value);

        private static readonly Dictionary<string, object> items = new Dictionary<string, object>();
        private static readonly Dictionary<string, ChangeCallback> changeCallbacks = new Dictionary<string, ChangeCallback>();

        public static void SetValue(string key, object value)
        {
            Console.WriteLine("SetValue: " + key);
            items[key] = value;
            if (changeCallbacks.TryGetValue(key, out ChangeCallback changeCallback))
                changeCallback(key, value);
        }
        public static object GetValue(string key) { return items[key]; }
        public static bool TryGetValue(string key, out object value) { return items.TryGetValue(key, out value); }

        public static void RegisterListener(string key, ChangeCallback callback)
        {
            Console.WriteLine("RegisterListener: " + key);
            if (!changeCallbacks.ContainsKey(key))
                changeCallbacks[key] = callback;
            else
                changeCallbacks[key] += callback;
        }

        public static void UnregisterListener(string key, ChangeCallback callback)
        {
            if (changeCallbacks.ContainsKey(key))
                changeCallbacks[key] -= callback;
        }
    }
}

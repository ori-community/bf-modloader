using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseModLib
{
    public class Controllers
    {
        internal struct Tuple { internal Type type; internal Guid guid; internal Action<MonoBehaviour> callback; internal string group; }
        internal static List<Tuple> controllers = new List<Tuple>();


        public static void Add<T>(string saveGuid = null, string group = null, Action<MonoBehaviour> callback = null) where T : MonoBehaviour
        {
            controllers.Add(new Tuple
            {
                type = typeof(T),
                guid = saveGuid == null ? Guid.Empty : new Guid(saveGuid),
                group = group,
                callback = callback
            });
        }
    }

    [HarmonyPatch(typeof(GameController), nameof(GameController.Awake))]
    internal class HookControllers
    {
        static Dictionary<string, GameObject> groups = new Dictionary<string, GameObject>();
        static GameObject GetGroup(string name, GameController gameController)
        {
            if (groups.ContainsKey(name))
                return groups[name];

            var go = new GameObject(name);
            go.transform.SetParent(gameController.transform);
            groups[name] = go;
            return go;
        }

        static void Postfix(GameController __instance)
        {
            foreach (var controller in Controllers.controllers)
            {
                var go = GetGroup(controller.group ?? "Custom Controllers", __instance);
                var c = go.AddComponent(controller.type) as MonoBehaviour;

                if (c is SaveSerialize ss)
                {
                    if (controller.guid == Guid.Empty)
                        throw new Exception($"The component {controller.type.Name} must have a unique Save Guid if it inherits SaveSerialize");

                    ss.MoonGuid = new MoonGuid(controller.guid);
                    ss.RegisterToSaveSceneManager(__instance.GetComponent<SaveSceneManager>());
                }

                controller.callback?.Invoke(c);
            }
        }
    }
}

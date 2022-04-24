using BaseModLib;
using Game;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriDeModLoader
{
    public class SceneBootstrap : MonoBehaviour
    {
        public static void RegisterHandler(Action<SceneBootstrap> callback, string group = "Bootstrap")
        {
            Controllers.Add<SceneBootstrap>(group: group, callback: mb => callback(mb as SceneBootstrap));
        }


        void Awake()
        {
            Events.Scheduler.OnSceneRootPreEnabled.Add(OnSceneRootPreEnabled);
            Hooks.OnSceneRootUnloaded += OnSceneRootUnloaded;
        }

        void OnSceneRootPreEnabled(SceneRoot sceneRoot)
        {
            if (!loadedScenes.Contains(sceneRoot.name) && BootstrapActions.ContainsKey(sceneRoot.name))
            {
                BootstrapActions[sceneRoot.name].Invoke(sceneRoot);
                loadedScenes.Add(sceneRoot.name);
            }
        }

        void OnSceneRootUnloaded(string name)
        {
            loadedScenes.Remove(name);
        }

        public Dictionary<string, Action<SceneRoot>> BootstrapActions = new Dictionary<string, Action<SceneRoot>>();
        private HashSet<string> loadedScenes = new HashSet<string>();
    }
}

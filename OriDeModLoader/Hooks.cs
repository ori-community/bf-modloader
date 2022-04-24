using System;
using HarmonyLib;

namespace OriDeModLoader
{
    public static class Hooks
    {
        public static Action OnControllerInitialise;
        public static Action OnStartNewGame;
        public static Action<string> OnSceneRootUnloaded;
        //public Action OnLoadSaveFile;
        //public Action OnDeath;
        //public Action OnRespawn;
    }

    [HarmonyPatch(typeof(GameController), nameof(GameController.Awake))]
    internal class Hook_OnControllerInitialise
    {
        private static void Postfix()
        {
            Hooks.OnControllerInitialise?.Invoke();
        }
    }

    [HarmonyPatch(typeof(GameController), nameof(GameController.SetupGameplay))]
    internal class Hook_OnStartNewGame
    {
        private static void Postfix()
        {
            Hooks.OnStartNewGame?.Invoke();
        }
    }

    [HarmonyPatch(typeof(SceneRoot), nameof(SceneRoot.Unload))]
    internal class Hook_OnSceneRootUnload
    {
        private static void Postfix(SceneRoot __instance)
        {
            Hooks.OnSceneRootUnloaded?.Invoke(__instance.name);
        }
    }
}

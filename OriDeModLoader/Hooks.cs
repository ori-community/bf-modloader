using HarmonyLib;
using System;

namespace OriDeModLoader
{
    public static class Hooks
    {
        public static Action OnStartNewGame;
        //public Action OnLoadSaveFile;
        //public Action OnDeath;
        //public Action OnRespawn;
    }

    [HarmonyPatch(typeof(GameController), nameof(GameController.SetupGameplay))]
    internal class Hook_OnStartNewGame
    {
        static void Postfix()
        {
            Hooks.OnStartNewGame?.Invoke();
        }
    }
}

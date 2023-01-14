using HarmonyLib;

namespace OriDeModLoader.Loader
{
    [HarmonyPatch(typeof(GameSettings), nameof(GameSettings.LoadSettings))]
    internal class AfterLoadSettings
    {
        private static bool once = false;
        private static void Postfix()
        {
            if (once)
                return;

            EntryPoint.ReloadStrings();
            Game.Events.Scheduler.OnGameLanguageChange.Add(EntryPoint.ReloadStrings);
            once = true;
        }
    }
}

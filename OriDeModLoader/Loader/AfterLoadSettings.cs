using HarmonyLib;

namespace OriDeModLoader
{
    [HarmonyPatch(typeof(GameSettings), nameof(GameSettings.LoadSettings))]
    class AfterLoadSettings
    {
        static void Postfix()
        {
            Loader.ReloadStrings();
            Game.Events.Scheduler.OnGameLanguageChange.Add(Loader.ReloadStrings);
        }
    }
}

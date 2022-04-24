using HarmonyLib;

namespace OriDeModLoader
{
    [HarmonyPatch(typeof(GameSettings), nameof(GameSettings.LoadSettings))]
    internal class AfterLoadSettings
    {
        private static void Postfix()
        {
            Loader.ReloadStrings();
            Game.Events.Scheduler.OnGameLanguageChange.Add(Loader.ReloadStrings);
        }
    }
}

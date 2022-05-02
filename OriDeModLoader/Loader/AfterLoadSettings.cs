using HarmonyLib;

namespace OriDeModLoader
{
    [HarmonyPatch(typeof(GameSettings), nameof(GameSettings.LoadSettings))]
    internal class AfterLoadSettings
    {
        private static void Postfix()
        {
            EntryPoint.ReloadStrings();
            Game.Events.Scheduler.OnGameLanguageChange.Add(EntryPoint.ReloadStrings);
        }
    }
}

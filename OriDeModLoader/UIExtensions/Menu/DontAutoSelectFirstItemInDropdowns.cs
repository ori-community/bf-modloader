using HarmonyLib;

namespace OriDeModLoader.UIExtensions
{
    [HarmonyPatch(typeof(CleverMenuItemSelectionManager), nameof(CleverMenuItemSelectionManager.SetIndexToFirst))]
    static class DontAutoSelectFirstItemInDropdowns
    {
        static bool Prefix(CleverMenuItemSelectionManager __instance)
        {
            // Don't auto select item 0 because then we can't auto select what we want it to start as
            // Built in options (i.e. resolution and language) already set themselves to 0 anyway when opened
            if (__instance.GetComponent<CleverMenuOptionsList>())
                return HarmonyHelper.StopExecution;
            return HarmonyHelper.ContinueExecution;
        }
    }
}

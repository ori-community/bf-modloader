using HarmonyLib;

namespace OriDeModLoader.Loader
{
    public class ModEvents
    {
        [HarmonyPatch(typeof(GameController), nameof(GameController.FixedUpdate))]
        internal class FixedUpdateEvent
        {
            private static void Postfix()
            {
                foreach (var mod in ModLoader.Instance.Mods)
                {
                    mod.Mod.FixedUpdate();
                }
            }
        }
    }
}

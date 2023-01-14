namespace OriDeModLoader
{
    using HarmonyLib;

    namespace BFModLoader.ModLoader
    {
        public struct ModState
        {
            public IMod Mod;
            public Harmony Harmony;
            public string File;
        }
    }
}

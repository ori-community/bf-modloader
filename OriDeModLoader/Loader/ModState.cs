using System.Collections.Generic;
using OriDeModLoader.UIExtensions;

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
            public List<CustomOptionsScreenDef> Settings;

            public ModState(IMod mod, Harmony harmony, string file) : this()
            {
                Mod = mod;
                Harmony = harmony;
                File = file;
                Settings = new List<CustomOptionsScreenDef>();
            }
        }
    }
}

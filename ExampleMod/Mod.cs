using System;
using BFModLoader.ModLoader;
using HarmonyLib;
using OriDeModLoader;

namespace ExampleMod
{
    public class Mod : IMod
    {
        public string Version => "0.0.1";
        public void Init()
        {
            Logger.Log("Loaded");
        }

        public void Unload()
        {
            Logger.Log("Killed");
        }

        public string Name { get; }
        public string ModID => "xemsys.bf.examplemod";
        
        /*protected override List<SettingBase> GetSettings() => new List<SettingBase> {FlipSetting};
        public static readonly BoolSetting FlipSetting = new BoolSetting("example.flip", "Make Ori always/never jump", false);*/
    }


    [HarmonyPatch(typeof(SeinJump))]
    [HarmonyPatch("get_HasSharplyTurnedAround")]
    class FlipPatch
    {
        static bool Prefix(SeinJump __instance, ref bool __result)
        {
            __result = true;//Mod.FlipSetting.Value;
            return false;
        }
    }
}

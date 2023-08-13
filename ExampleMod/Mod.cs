using System.Collections.Generic;
using BaseModLib;
using HarmonyLib;
using OriDeModLoader;
using UnityEngine;
using Logger = BFModLoader.ModLoader.Logger;

namespace ExampleMod
{
    public class Mod : BaseMod
    {
        public override string Version => "0.0.1";
        public override void Init()
        {
            KeySetting.OnValueChanged += val =>
                Logger.Log(KeySetting.ToString());
        }
        public override void FixedUpdate()
        {
            if (KeySetting.IsJustPressed()){
                //Logger.Log("Just Pressed!");
            }else if (KeySetting.IsPressed()){
                //Logger.Log("Held!");
            }
        }

        public override string Name => "Example Mod";
        public override string ModID => "xemsys.bf.examplemod";

        public override List<SettingsScreenConfig> GetSettings() => new List<SettingsScreenConfig>
        {
            this.DefaultSettingsScreen(FlipSetting, KeySetting),
        };
        
        public static readonly BoolSetting FlipSetting = new BoolSetting("example.flip", "Always flip", "Make Ori always/never flip", false);
        public static readonly KeybindSetting KeySetting = new KeybindSetting("example.button", "Random Bind", "Testing Binds", KeyCode.O, delegate { Logger.Log("Bind Pressed!!!"); });
    }


    [HarmonyPatch(typeof(SeinJump))]
    [HarmonyPatch("get_HasSharplyTurnedAround")]
    class FlipPatch
    {
        static bool Prefix(ref bool __result)
        {
            if (!Mod.FlipSetting.Value)
            {
                return true;
            }

            __result = true;
            return false;

        }
    }
}

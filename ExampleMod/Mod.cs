using System.Collections.Generic;
using BaseModLib;
using HarmonyLib;
using OriDeModLoader;
using UnityEngine;
using Logger = BFModLoader.ModLoader.Logger;

namespace ExampleMod
{
    public class Mod : IMod
    {
        public string Version => "0.0.1";
        public void Init()
        {
            Logger.Log("Loaded");

            KeySetting.OnValueChanged += val =>
                Logger.Log(KeySetting.ToString());
        }

        public void Unload()
        {
            Logger.Log("Killed");
        }

        public virtual void FixedUpdate()
        {
            if (KeySetting.IsJustPressed()){
                Logger.Log("Just Pressed!");
            }else if (KeySetting.IsPressed()){
                Logger.Log("Held!");
            }
        }

        public string Name => "Example Mod";
        public string ModID => "xemsys.bf.examplemod";

        public List<SettingBase> GetSettings() => new List<SettingBase> {FlipSetting, KeySetting};
        public static readonly BoolSetting FlipSetting = new BoolSetting("example.flip", "Always flip", "Make Ori always/never flip", false);
        public static readonly KeybindSetting KeySetting = new KeybindSetting("example.button", "Random Bind", "Testing Binds", KeyCode.A);
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

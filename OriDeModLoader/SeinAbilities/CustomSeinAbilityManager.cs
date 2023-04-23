using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace OriDeModLoader.CustomSeinAbilities
{
    public static class CustomSeinAbilityManager
    {
        private class CustomSeinAbilityDef { public Type type; public Guid guid; }

        private static readonly List<CustomSeinAbility> customAbilities = new List<CustomSeinAbility>();
        private static readonly List<CustomSeinAbilityDef> abilityDefs = new List<CustomSeinAbilityDef>();

        /// <summary>Adds a custom ability. Sein abilities are controllers that exist on Sein instead of globally, and UpdateCharacterState will not execute if Sein is suspended.</summary>
        /// <typeparam name="T">The type of the ability to add</typeparam>
        /// <param name="saveGuid">Required to be unique, even if nothing is saved.</param>
        public static void Add<T>(string saveGuid) where T : CustomSeinAbility
        {
            abilityDefs.Add(new CustomSeinAbilityDef { type = typeof(T), guid = new Guid(saveGuid) });
        }

        internal static void UpdateStateActive(SeinLogicCycle logicCycle)
        {
            foreach (var a in customAbilities)
                a.SetStateActive(a.AllowAbility(logicCycle));
        }

        internal static void UpdateCharacterState()
        {
            foreach (var a in customAbilities)
                CharacterState.UpdateCharacterState(a);
        }

        internal static void Reset(SeinCharacter sein)
        {
            customAbilities.Clear();

            var go = new GameObject("Custom Abilities");
            go.transform.parent = sein.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            foreach (var def in abilityDefs)
            {
                var instance = (CustomSeinAbility) go.gameObject.AddComponent(def.type);
                instance.MoonGuid = new MoonGuid(def.guid);
                customAbilities.Add(instance);
            }
        }
    }

    [HarmonyPatch]
    static class AddCustomSeinAbilityPatch
    {
        [HarmonyPrefix, HarmonyPatch(typeof(SeinCharacter), nameof(SeinCharacter.Awake))]
        static void AddCustomAbilities(SeinCharacter __instance)
        {
            CustomSeinAbilityManager.Reset(__instance);
        }
    }
}

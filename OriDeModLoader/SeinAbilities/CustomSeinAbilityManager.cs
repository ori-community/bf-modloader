using BaseModLib;
using System.Collections.Generic;

namespace SeinAbilities
{
    public static class CustomSeinAbilityManager
    {
        private static List<CustomSeinAbility> customAbilities = new List<CustomSeinAbility>();

        /// <summary>Adds a custom ability</summary>
        /// <typeparam name="T">The type of the ability to add</typeparam>
        /// <param name="saveGuid">Required to be unique, even if nothing is saved. Recommended to also reset save data when starting a new game.</param>
        public static void Add<T>(string saveGuid) where T : CustomSeinAbility
        {
            Controllers.Add<T>(saveGuid, "Sein Abilities", c => customAbilities.Add(c as CustomSeinAbility));
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
    }
}

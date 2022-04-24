using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace OriDeModLoader.CustomSeinAbilities
{
    [HarmonyPatch(typeof(SeinLogicCycle), nameof(SeinLogicCycle.FixedUpdate))]
    internal class SeinLogicCyclePatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            bool needsUpdateActive = true;

            var updateCharacterStateMethod = AccessTools.Method(typeof(CharacterState), nameof(CharacterState.UpdateCharacterState), parameters: new [] { typeof(CharacterState) });

            for (int i = 0; i < codes.Count; i++)
            {
                if (needsUpdateActive && codes[i].IsLdarg(0) && codes[i + 3].Calls(updateCharacterStateMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.Call(typeof(CustomSeinAbilityManager), nameof(CustomSeinAbilityManager.UpdateStateActive));
                    needsUpdateActive = false;
                }

                yield return codes[i];
            }
        }

        static void Postfix()
        {
            CustomSeinAbilityManager.UpdateCharacterState();
        }
    }
}

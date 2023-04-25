using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CatlikeCoding.TextBox;
using HarmonyLib;
using UnityEngine;

namespace OriDeModLoader
{
    [HarmonyPatch]
    internal class MoreTextColours
    {
        [HarmonyPrefix, HarmonyPatch(typeof(TextBox), "ApplyStyleStatement")]
        private static bool ApplyTextStyles(StringBuilder ___styleStatementBuffer, Stack<AppliedTextStyle> ___styleStack, ref AppliedTextStyle ___currentStyle)
        {
            // <#hexcode>This will be the colour specified in hexcode</>
            if (___styleStatementBuffer.Length > 0 && ___styleStatementBuffer[0] == '#')
            {
                Console.WriteLine(___styleStatementBuffer.ToString());
                if (ColorUtility.TryParseHtmlString(___styleStatementBuffer.ToString(), out Color colour))
                {
                    var newStyle = new TextStyle()
                    {
                        color = colour,
                        name = "custom " + ___styleStatementBuffer.ToString(),

                        // Setting these to null/false will cause the style to inherit from the base style
                        absoluteFontScale = true,
                        font = null,
                        fontScale = 1,
                        hasColor = true,
                        hasFontScale = false,
                        hasLetterSpacing = false,
                        hasLineScale = false,
                        letterSpacing = 0,
                        lineScale = 1,
                        renderer = null,
                        rendererId = -1
                    };

                    Console.WriteLine(newStyle.color);

                    ___styleStack.Push(___currentStyle);
                    ___currentStyle.ApplyOnTop(newStyle);

                    return HarmonyHelper.StopExecution;
                }
            }
            return HarmonyHelper.ContinueExecution;
        }

        [HarmonyTranspiler, HarmonyPatch(typeof(MessageParserUtility), "ProcessColorsInString")]
        private static IEnumerable<CodeInstruction> IgnoreHashInColourCodeStyle(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            bool done = false;

            Label enterIntoBranch = ilg.DefineLabel();

            var method = AccessTools.Method(typeof(StringBuilder), "get_Chars");

            var codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                if (!done && codes[i].Calls(method))
                {
                    yield return codes[i];
                    yield return codes[i + 1];
                    yield return codes[i + 2];

                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5); // i
                    yield return new CodeInstruction(OpCodes.Brfalse_S, enterIntoBranch); // if (i == 0) goto enterIntoBranch; (i.e. if i == 0 then we should treat the # like yellow)
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // builder
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5); // builder, i
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1); // builder, i, 1
                    yield return new CodeInstruction(OpCodes.Sub); // builder, i - 1
                    yield return CodeInstruction.Call(typeof(StringBuilder), method.Name); // builder[i - 1]
                    yield return new CodeInstruction(OpCodes.Ldc_I4_S, 0x3C); // builder[i - 1], '<'
                    yield return new CodeInstruction(OpCodes.Beq_S, codes[i + 2].operand); // if (builder[i - 1] == '<') goto "else if (builder[i] == '*')"
                    yield return new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { enterIntoBranch } };

                    i += 2;

                    done = true;
                    continue;
                }

                yield return codes[i];
            }
        }
    }
}

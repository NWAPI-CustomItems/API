using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;
using InventorySystem;
using static HarmonyLib.AccessTools;
using UnityEngine;
using static HarmonyLib.Code;
using NWAPI.CustomItems.API.Extensions;

namespace NWAPI.CustomItems.Patches
{
    /// <summary>
    /// Patch to fix grenade explode event in <see cref="FlashbangGrenade"/>.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ServerFuseEnd))]
    public class FixGrenadeExplodedEventOnFlashbangPatch
    {
        // This is the Transpiler method that Harmony will execute instead of the original method
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new(instructions);

            var label = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new (OpCodes.Ldarg_0),
                // Load the attacker.
                new (OpCodes.Ldfld,  Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PreviousOwner))),

                // Current Stack: Attacker

                // Load the position. base.transform.position ie. base -> Transform -> Position
                new (OpCodes.Ldarg_0), // flashbang instance
                // Get the grenade.Transform
                new (OpCodes.Callvirt, PropertyGetter(typeof(FlashbangGrenade), nameof(FlashbangGrenade.transform))),
                // Get the Transform.Position
                new (OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.position))),

                // Current Stack: Attacker, Position

                // Load the settings reference.
                new (OpCodes.Ldarg_0),

                // Current Stack: Attacker, Position, SettingsReference

                new (OpCodes.Newobj, typeof(GrenadeExplodedEvent)),

                // Current Stack: Event Arg
                new (OpCodes.Callvirt, Method(typeof(EventManager), nameof(EventManager.ExecuteEvent), new Type[] {typeof(bool) })),

                // Current stack: bool
                new (OpCodes.Brtrue, label),
                new (OpCodes.Ret)
            });

            newInstructions[11].WithLabels(label);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i].Log(i, -1, Plugin.Instance.Config.DebugMode, true, new Dictionary<ushort, ushort>() { { 0, 11 } });

            yield break;
        }
    }
}

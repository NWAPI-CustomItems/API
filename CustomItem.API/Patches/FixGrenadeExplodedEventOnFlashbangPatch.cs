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
using InventorySystem.Items.Pickups;
using Footprinting;

namespace NWAPI.CustomItems.Patches
{
    /// <summary>
    /// Patch to fix grenade explode event in <see cref="FlashbangGrenade"/>.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ServerFuseEnd))]
    public class FixGrenadeExplodedEventOnFlashbangPatch
    {
        private static bool Prefix(FlashbangGrenade __instance)
        {
            if (!EventManager.ExecuteEvent(new GrenadeExplodedEvent(__instance.PreviousOwner, __instance.transform.position, __instance)))
            {
                return false;
            }

            return true;
        }

        // This is the Transpiler method that Harmony will execute instead of the original method
        /*private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new List<CodeInstruction>(instructions);

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
                new (OpCodes.Call, Method(typeof(FixGrenadeExplodedEventOnFlashbangPatch), nameof(FixGrenadeExplodedEventOnFlashbangPatch.RunNorthwoodEvent))),

                // Current stack: bool
                new (OpCodes.Brtrue, label),
                new (OpCodes.Ret),
                new CodeInstruction(OpCodes.Nop).WithLabels(label)
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i].Log(i, -1, Plugin.Instance.Config.DebugMode, true, new Dictionary<ushort, ushort>() { { 0, 11 } });

            yield break;
        }

        private static bool RunNorthwoodEvent(Footprint thrower, Vector3 pos, ItemPickupBase grenade)
        {
            return PluginAPI.Events.EventManager.ExecuteEvent(new GrenadeExplodedEvent(thrower,pos, grenade));
        }*/
    }
}

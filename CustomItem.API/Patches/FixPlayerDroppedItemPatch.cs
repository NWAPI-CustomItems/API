using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Pickups;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static HarmonyLib.AccessTools;

namespace NWAPI.CustomItems.Patches
{
    /// <summary>
    /// The most horrible transpiler you will ever see in your life.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]
    internal static class FixPlayerDroppedItemPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new(instructions);

            for (int i = 0; i < newInstructions.Count; i++)
            {
                var instruction = newInstructions[i];
                if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo method && method.Name == nameof(InventoryExtensions.ServerDropItem))
                {
                    yield return instruction;

                    var saveInstruction = newInstructions[++i];
                    yield return saveInstruction;

                    if (saveInstruction.opcode != OpCodes.Stloc_1)
                    {
                        PluginAPI.Core.Log.Error($"Variable is not saved in index 1");
                        continue;
                    }
                    Label endLabel = generator.DefineLabel();
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub)));
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, Method(typeof(FixPlayerDroppedItemPatch), nameof(FixPlayerDroppedItemPatch.RunNwMethod)));
                    yield return new CodeInstruction(OpCodes.Brtrue, endLabel);
                    yield return new CodeInstruction(OpCodes.Ret);
                    yield return new CodeInstruction(OpCodes.Nop).WithLabels(endLabel);

                    continue;
                }

                yield return instruction;
            }

        }

        // Doing Northwood's job...
        private static bool RunNwMethod(ReferenceHub hub, ItemPickupBase? item)
        {
            return PluginAPI.Events.EventManager.ExecuteEvent(new PlayerDroppedItemEvent(hub, item));
        }
    }
}

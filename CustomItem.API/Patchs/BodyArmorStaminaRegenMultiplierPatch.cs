using HarmonyLib;
using InventorySystem.Items.Armor;
using InventorySystem.Items.ThrowableProjectiles;
using NWAPI.CustomItems.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWAPI.CustomItems.Patchs
{
    [HarmonyPatch(typeof(BodyArmor), nameof(BodyArmor.StaminaRegenMultiplier), MethodType.Getter)]
    public class BodyArmorStaminaRegenMultiplierPatch
    {
        public static void Postfix(BodyArmor __instance, ref float __result)
        {
            if(CustomItem.TryGet(__instance.ItemSerial, out var customItem) && customItem is CustomArmor armor)
            {
                __result = armor.StaminaRegenMultiplier;
            }
        }
    }

    [HarmonyPatch(typeof(BodyArmor), nameof(BodyArmor.SprintingDisabled), MethodType.Getter)]
    public class BodyArmorSprintDisablePatch
    {
        public static void Postfix(BodyArmor __instance, ref bool __result)
        {
            if (CustomItem.TryGet(__instance.ItemSerial, out var customItem) && customItem is CustomArmor armor)
            {
                __result = armor.SprintDisable;
            }
        }
    }
}

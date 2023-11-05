using HarmonyLib;
using InventorySystem.Items.Armor;
using NWAPI.CustomItems.API.Features;

namespace NWAPI.CustomItems.Patches
{
    [HarmonyPatch(typeof(BodyArmor), nameof(BodyArmor.StaminaRegenMultiplier), MethodType.Getter)]
    public class BodyArmorStaminaRegenMultiplierPatch
    {
        public static void Postfix(BodyArmor __instance, ref float __result)
        {
            if (CustomItem.TryGet(__instance.ItemSerial, out var customItem) && customItem is CustomArmor armor)
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

using HarmonyLib;
using InventorySystem.Items.Armor;
using NWAPI.CustomItems.API.Features;

namespace NWAPI.CustomItems.Patches
{
    /// <summary>
    /// Harmony patch for the getter of the 'StaminaRegenMultiplier' property in the 'BodyArmor' class.
    /// This patch allows custom armor items to modify the stamina regeneration multiplier.
    /// </summary>
    [HarmonyPatch(typeof(BodyArmor), nameof(BodyArmor.StaminaRegenMultiplier), MethodType.Getter)]
    public class BodyArmorStaminaRegenMultiplierPatch
    {
        /// <summary>
        /// A postfix method that intercepts the 'StaminaRegenMultiplier' getter and modifies the result
        /// based on the custom armor's properties.
        /// </summary>
        /// <param name="__instance">The 'BodyArmor' instance being accessed.</param>
        /// <param name="__result">The original result of the property getter.</param>
        public static void Postfix(BodyArmor __instance, ref float __result)
        {
            if (CustomItem.TryGet(__instance.ItemSerial, out var customItem) && customItem is CustomArmor armor)
            {
                __result = armor.StaminaRegenMultiplier;
            }
        }
    }

    /// <summary>
    /// Harmony patch for the getter of the 'SprintingDisabled' property in the 'BodyArmor' class.
    /// This patch allows custom armor items to modify the sprinting disable property.
    /// </summary>
    [HarmonyPatch(typeof(BodyArmor), nameof(BodyArmor.SprintingDisabled), MethodType.Getter)]
    public class BodyArmorSprintDisablePatch
    {
        /// <summary>
        /// A postfix method that intercepts the 'SprintingDisabled' getter and modifies the result
        /// based on the custom armor's properties.
        /// </summary>
        /// <param name="__instance">The 'BodyArmor' instance being accessed.</param>
        /// <param name="__result">The original result of the property getter.</param>
        public static void Postfix(BodyArmor __instance, ref bool __result)
        {
            if (CustomItem.TryGet(__instance.ItemSerial, out var customItem) && customItem is CustomArmor armor)
            {
                __result = armor.SprintDisable;
            }
        }
    }
}

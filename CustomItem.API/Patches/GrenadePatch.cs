using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using NWAPI.CustomItems.API.Components;
using NWAPI.CustomItems.API.Features;
using PluginAPI.Events;

namespace NWAPI.CustomItems.Patches
{
    /// <summary>
    /// Patch to make grenades explode on contact.
    /// </summary>
    [HarmonyPatch(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))]
    public class GrenadePatch
    {
        private static void Prefix(TimeGrenade __instance)
        {
            if (CustomItem.TryGet(__instance.Info.Serial, out var customItem) && customItem is CustomGrenade customGrenade)
            {
                if (customGrenade.ExplodeOnCollision)
                {
                    __instance.gameObject.AddComponent<ExplodeOnCollision>();
                    return;
                }

                __instance.TargetTime = NetworkTime.time + customGrenade.FuseTime;
            }
        }
    }

    /// <summary>
    /// Patch to fix grenade explode event in <see cref="FlashbangGrenade"/>.
    /// </summary>
    //[HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ServerFuseEnd))]
    public class FixGrenadeExplodedEventOnFlashbang
    {
        private static bool Prefix(FlashbangGrenade __instance)
        {
            if (!EventManager.ExecuteEvent(new GrenadeExplodedEvent(__instance.PreviousOwner, __instance.transform.position, __instance)))
            {
                return false;
            }

            return true;
        }
    }
}

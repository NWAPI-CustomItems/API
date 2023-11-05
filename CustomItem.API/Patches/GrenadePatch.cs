using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using NWAPI.CustomItems.API.Components;
using NWAPI.CustomItems.API.Features;

namespace NWAPI.CustomItems.Patches
{
    [HarmonyPatch(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))]
    public class GrenadePatch
    {
        public static void Prefix(TimeGrenade __instance)
        {
            if (CustomItem.TryGet(__instance.Info.Serial, out var customItem) && customItem is CustomGrenade customGrenade)
            {
                if (customGrenade.ExplodeOnCollision)
                {
                    __instance.gameObject.AddComponent<ExplodeOnCollision>();
                }

                __instance.TargetTime = NetworkTime.time + customGrenade.FuseTime;
            }
        }
    }
}

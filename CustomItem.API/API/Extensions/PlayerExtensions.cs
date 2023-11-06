using InventorySystem.Items.ThrowableProjectiles;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PluginAPI.Core;
using UnityEngine;

namespace NWAPI.CustomItems.API.Extensions
{
    /// <summary>
    /// A set of tools for <see cref="Player"/>
    /// </summary>
    public static class PlayerExtensions
    {
        /// <summary>
        /// Gets a random spawn position of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to get the spawn point from.</param>
        public static Vector3 GetRandomSpawnLocation(this RoleTypeId roleType)
        {
            if (!PlayerRoleLoader.TryGetRoleTemplate(roleType, out PlayerRoleBase @base))
                return Vector3.zero;

            if (@base is not IFpcRole fpc)
                return Vector3.zero;

            ISpawnpointHandler spawn = fpc.SpawnpointHandler;
            if (spawn is null)
                return Vector3.zero;

            if (!spawn.TryGetSpawnpoint(out Vector3 pos, out float _))
                return Vector3.zero;

            return pos;
        }

        /// <summary>
        /// Throws a throwable item with the option to apply full force.
        /// </summary>
        /// <param name="player">The player throwing the item.</param>
        /// <param name="item">The throwable item to be thrown.</param>
        /// <param name="fullForce">Indicates whether the item should be thrown with full force.</param>
        /// <returns>The thrown throwable item.</returns>
        public static ThrowableItem ThrowItem(this Player player, ThrowableItem item, bool fullForce)
        {
            item.Owner = player.ReferenceHub;
            item.ThrowItem(fullForce);
            return item;
        }
    }
}

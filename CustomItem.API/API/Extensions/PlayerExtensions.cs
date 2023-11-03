using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;
using UnityEngine;

namespace NWAPI.CustomItems.API.Extensions
{
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
    }
}

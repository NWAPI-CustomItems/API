using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core;
using UnityEngine;

namespace NWAPI.CustomItems.API.Extensions
{
    /// <summary>
    ///  A collection of extension methods for creating and managing ragdolls ingame.
    /// </summary>
    public static class RagdollExtensions
    {
        /// <summary>
        /// Gets a new <see cref="BasicRagdoll"/> instance for the specified role type, name, and death reason.
        /// </summary>
        /// <param name="roleTypeId">The <see cref="RoleTypeId"/> representing the role type for the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="deathReason">The reason for the death of the ragdoll.</param>
        /// <param name="spawn">Determines if the ragdoll should be spawned in the network (default is true).</param>
        /// <param name="position">The position at which to spawn the ragdoll (default is null, which results in the default position).</param>
        /// <param name="rotation">The rotation of the ragdoll when spawned (default is null, which results in the default rotation).</param>
        /// <param name="hub">The <see cref="ReferenceHub"/> associated with the ragdoll (default is null).</param>
        /// <returns>A new <see cref="BasicRagdoll"/> instance, or null if the role type is not a ragdoll role or does not exist.</returns>
        public static BasicRagdoll? GetRagdoll(this RoleTypeId roleTypeId, string name, string deathReason, bool spawn = true, Vector3? position = null, Vector3? rotation = null, ReferenceHub? hub = null)
        {
            if (!PlayerRoleLoader.TryGetRoleTemplate(roleTypeId, out PlayerRoleBase rolebase) || rolebase is not IRagdollRole ragdoll)
                return null;

            GameObject gameObject = UnityEngine.Object.Instantiate(ragdoll.Ragdoll.gameObject, position ?? Vector3.zero, Quaternion.Euler(rotation ?? Vector3.zero));

            if (gameObject.TryGetComponent(out BasicRagdoll component))
            {
                component.NetworkInfo = new RagdollData(hub, new UniversalDamageHandler(0.0f, DeathTranslations.Unknown), roleTypeId, position ?? Vector3.zero, Quaternion.Euler(rotation ?? Vector3.zero), name, NetworkTime.time);
            }

            if (spawn)
                NetworkServer.Spawn(gameObject);

            return component;
        }

        /// <summary>
        /// Moves the position of a BasicRagdoll to the specified Vector3 position.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to be moved.</param>
        /// <param name="position">The new position to set for the ragdoll.</param>
        public static void MoveRagdoll(this BasicRagdoll ragdoll, Vector3 position)
        {
            NetworkServer.UnSpawn(ragdoll.gameObject);

            ragdoll.transform.position = position;

            NetworkServer.Spawn(ragdoll.gameObject);
        }

        /// <summary>
        /// Rotates a BasicRagdoll to the specified Quaternion rotation.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to be rotated.</param>
        /// <param name="rotation">The new rotation to set for the ragdoll.</param>
        public static void RotateRagdoll(this BasicRagdoll ragdoll, Quaternion rotation)
        {
            NetworkServer.UnSpawn(ragdoll.gameObject);

            ragdoll.transform.rotation = rotation;

            NetworkServer.Spawn(ragdoll.gameObject);
        }

        /// <summary>
        /// Scales a BasicRagdoll to the specified Vector3 scale.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to be scaled.</param>
        /// <param name="scale">The new scale to set for the ragdoll.</param>
        public static void ScaleRagdoll(this BasicRagdoll ragdoll, Vector3 scale)
        {
            NetworkServer.UnSpawn(ragdoll.gameObject);

            ragdoll.transform.localScale = scale;

            NetworkServer.Spawn(ragdoll.gameObject);
        }

        /// <summary>
        /// Checks if a BasicRagdoll has been consumed by a ZombieConsumeAbility.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to check for consumption status.</param>
        /// <returns>True if the ragdoll has been consumed, false otherwise.</returns>
        public static bool IsConsumed(this BasicRagdoll ragdoll)
        {
            return ZombieConsumeAbility.ConsumedRagdolls.Contains(ragdoll);
        }

        /// <summary>
        /// Sets the consumption status of a BasicRagdoll.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to update the consumption status for.</param>
        /// <param name="value"></param>
        public static void SetIsConsumed(this BasicRagdoll ragdoll, bool value)
        {
            if (value && !ZombieConsumeAbility.ConsumedRagdolls.Contains(ragdoll))
                ZombieConsumeAbility.ConsumedRagdolls.Add(ragdoll);
            else if (!value && ZombieConsumeAbility.ConsumedRagdolls.Contains(ragdoll))
                ZombieConsumeAbility.ConsumedRagdolls.Remove(ragdoll);
        }

        /// <summary>
        /// Spawns a BasicRagdoll in the network.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to be spawned.</param>
        public static void SpawnRagdoll(this BasicRagdoll ragdoll) => NetworkServer.Spawn(ragdoll.gameObject);

        /// <summary>
        /// Unspawns a BasicRagdoll from the network.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to be unspawned.</param>
        public static void UnSpawnRagdoll(this BasicRagdoll ragdoll) => NetworkServer.UnSpawn(ragdoll.gameObject);

        /// <summary>
        /// Destroys a BasicRagdoll object.
        /// </summary>
        /// <param name="ragdoll">The BasicRagdoll to be destroyed.</param>
        public static void DestroyRagdoll(this BasicRagdoll ragdoll) => UnityEngine.Object.Destroy(ragdoll.gameObject);


        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="networkInfo">The data associated with the ragdoll.</param>
        /// <param name="ragdoll">Created ragdoll. Will be <see langword="null"/> if method retunred <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if ragdoll was successfully created. Otherwise, false.</returns>
        public static bool TryCreate(RagdollData networkInfo, out BasicRagdoll ragdoll)
        {
            ragdoll = null!;

            if (networkInfo.RoleType.GetRoleBase() is not IRagdollRole ragdollRole)
                return false;

            GameObject modelRagdoll = ragdollRole.Ragdoll.gameObject;

            if (modelRagdoll == null || !UnityEngine.Object.Instantiate(modelRagdoll).TryGetComponent(out BasicRagdoll basicRagdoll))
                return false;

            basicRagdoll.NetworkInfo = networkInfo;
            basicRagdoll.MoveRagdoll(networkInfo.StartPosition);
            basicRagdoll.RotateRagdoll(networkInfo.StartRotation);

            ragdoll = basicRagdoll;
            return true;
        }

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="networkInfo">The data associated with the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static BasicRagdoll? CreateAndSpawn(RagdollData networkInfo)
        {
            if (!TryCreate(networkInfo, out BasicRagdoll doll))
                return null;

            doll.SpawnRagdoll();

            return doll;
        }

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="damageHandler">The damage handler responsible for the ragdoll's death.</param>
        /// <param name="ragdoll">Created ragdoll. Will be <see langword="null"/> if method retunred <see langword="false"/>.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static bool TryCreate(RoleTypeId roleType, string name, DamageHandlerBase damageHandler, out BasicRagdoll ragdoll, Player? owner = null)
            => TryCreate(new(owner?.ReferenceHub ?? ReferenceHub.HostHub, damageHandler, roleType, default, default, name, NetworkTime.time), out ragdoll);

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="deathReason">The reason the ragdoll died.</param>
        /// <param name="ragdoll">Created ragdoll. Will be <see langword="null"/> if method retunred <see langword="false"/>.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static bool TryCreate(RoleTypeId roleType, string name, string deathReason, out BasicRagdoll ragdoll, Player? owner = null)
            => TryCreate(roleType: roleType, name: name, damageHandler: new CustomReasonDamageHandler(deathReason), out ragdoll, owner);

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="damageHandler">The damage handler responsible for the ragdoll's death.</param>
        /// <param name="position">The position of the ragdoll.</param>
        /// <param name="rotation">The rotation of the ragdoll.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static BasicRagdoll? CreateAndSpawn(RoleTypeId roleType, string name, DamageHandlerBase damageHandler, Vector3 position, Quaternion rotation, Player? owner = null)
            => CreateAndSpawn(new(owner?.ReferenceHub ?? ReferenceHub.HostHub, damageHandler, roleType, position, rotation, name, NetworkTime.time));

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="deathReason">The reason the ragdoll died.</param>
        /// <param name="position">The position of the ragdoll.</param>
        /// <param name="rotation">The rotation of the ragdoll.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static BasicRagdoll? CreateAndSpawn(RoleTypeId roleType, string name, string deathReason, Vector3 position, Quaternion rotation, Player? owner = null)
            => CreateAndSpawn(roleType, name, new CustomReasonDamageHandler(deathReason), position, rotation, owner);
    }
}

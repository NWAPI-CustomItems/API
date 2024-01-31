using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerStatsSystem;
using PluginAPI.Core;
using RelativePositioning;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace NWAPI.CustomItems.API.Extensions
{
    /// <summary>
    ///  A collection of extension methods for <see cref="Player"/>
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

        /// <summary>
        /// Remove the current item in the player's hand.
        /// </summary>
        /// <param name="player">The player whose item should be removed.</param>
        public static void RemoveCurrentItem(this Player player)
        {
            if (player.CurrentItem != null)
            {
                player.ReferenceHub.inventory.ServerRemoveItem(player.CurrentItem.ItemSerial, player.CurrentItem.PickupDropModel);
            }
        }

        //TODO: Remove this when NW accepts the Pullrequest https://github.com/northwood-studios/NwPluginAPI/pull/227 | Probably in 1 year because there are pull request from more than 6 months ago.
        /// <summary>
        /// Removes a specific item from the player's inventory.
        /// </summary>
        /// <param name="player">The player whose item should be removed.</param>
        /// <param name="item">The item to be removed.</param>
        public static void RemoveItemFix(this Player player, ItemBase item)
        {
            if (item is null)
                return;

            player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, item.PickupDropModel);
        }

        //TODO: Remove this when NW accepts the Pullrequest https://github.com/northwood-studios/NwPluginAPI/pull/227 | Probably in 1 year because there are pull request from more than 6 months ago.
        /// <summary>
        /// Removes a specific item from the player's inventory based on <see cref="ItemType"/>.
        /// </summary>
        /// <param name="player">The player whose item should be removed.</param>
        /// <param name="item">The type of item to be removed.</param>
        public static void RemoveItemFix(this Player player, ItemType item)
        {
            var itemToRemove = player.Items.FirstOrDefault(i => i.ItemTypeId == item);

            if (itemToRemove != null)
            {
                player.ReferenceHub.inventory.ServerRemoveItem(itemToRemove.ItemSerial, itemToRemove.PickupDropModel);
            }
        }

        /// <summary>
        /// Sets the scale of a player's to the specified Vector3 scale. 
        /// If the current scale matches the provided scale, no changes are made.
        /// </summary>
        /// <param name="target">The player whose GameObject's scale should be set.</param>
        /// <param name="scale">The new scale to be applied to the player's GameObject.</param>
        public static void SetPlayerScale(this Player target, Vector3 scale)
        {
            if (target.GameObject.transform.localScale == scale)
                return;

            try
            {
                target.GameObject.transform.localScale = scale;

                foreach (var player in Player.GetPlayers())
                {
                    NetworkServer.SendSpawnMessage(target.ReferenceHub.networkIdentity, player.Connection);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error on {nameof(SetPlayerScale)}: {e.Message}");
            }
        }

        /// <summary>
        /// Kills the player using a damage handler.
        /// </summary>
        /// <param name="player">The player to be killed.</param>
        /// <param name="dmg">The damage handler responsible for the player's death.</param>
        public static void Kill(this Player player, DamageHandlerBase dmg)
        {
            player.ReferenceHub.playerStats.KillPlayer(dmg);
        }

        /// <summary>
        /// Triggers an explosion effect at the player's position.
        /// </summary>
        /// <param name="player">The player at whose position the explosion effect should be triggered.</param>
        public static void ExplodeEffect(this Player player)
        {
            ExplosionUtils.ServerSpawnEffect(player.Position, ItemType.GrenadeHE);
        }

        /// <summary>
        /// Triggers an explosion using the player's reference hub.
        /// </summary>
        /// <param name="player">The player whose reference hub should be used to trigger the explosion.</param>
        public static void Explode(this Player player)
        {
            ExplosionUtils.ServerExplode(player.ReferenceHub);
        }

        /// <summary>
        /// Inflicts damage to a player equal to their artificial health, effectively breaking their shield.
        /// </summary>
        /// <param name="player">The player whose shield should be broken.</param>
        public static void BreakShield(this Player player)
        {
            // Inflict damage to the player equal to their artificial health, effectively breaking their shield.
            player.Damage(player.ArtificialHealth, "Shield Breaker");
        }

        /// <summary>
        /// Gets the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static PlayerRoleBase? GetRoleBase(this RoleTypeId roleType) => roleType.TryGetRoleBase(out PlayerRoleBase roleBase) ? roleBase : null;

        /// <summary>
        /// Tries to get the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <param name="roleBase">The <see cref="PlayerRoleBase"/> to return.</param>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static bool TryGetRoleBase(this RoleTypeId roleType, out PlayerRoleBase roleBase) => PlayerRoleLoader.TryGetRoleTemplate(roleType, out roleBase);

        /// <summary>
        /// Gets a <see cref="RoleTypeId">role's</see> <see cref="Color"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to get the color of.</param>
        /// <returns>The <see cref="Color"/> of the role.</returns>
        public static Color GetColor(this RoleTypeId roleType) => (roleType.GetRoleBase()?.RoleColor) ?? Color.white;

        /// <summary>
        /// Gets the starting items of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>An <see cref="Array"/> of <see cref="ItemType"/> that the role receives on spawn. Will be empty for classes that do not spawn with items.</returns>
        public static ItemType[] GetStartingInventory(this RoleTypeId roleType)
        {
            if (StartingInventories.DefinedInventories.TryGetValue(roleType, out InventoryRoleInfo info))
                return info.Items;

            return Array.Empty<ItemType>();
        }

        /// <summary>
        /// Gets the starting ammo of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>An <see cref="Array"/> of <see cref="ItemType"/> that the role receives on spawn. Will be empty for classes that do not spawn with ammo.</returns>
        public static Dictionary<ItemType, ushort> GetStartingAmmo(this RoleTypeId roleType)
        {
            if (StartingInventories.DefinedInventories.TryGetValue(roleType, out InventoryRoleInfo info))
                return info.Ammo.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return new();
        }

        /// <summary>
        /// Gets the full name of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The full name.</returns>
        public static string? GetFullName(this RoleTypeId typeId)
        {
            return typeId.GetRoleBase()?.RoleName;
        }

        // -----------------------------------------------------------------------
        // <copyright file="MirrorExtensions.cs" company="Exiled Team">
        // Copyright (c) Exiled Team. All rights reserved.
        // Licensed under the CC BY-SA 3.0 license.
        // </copyright>
        // -----------------------------------------------------------------------
        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, RoleTypeId type, bool skipJump = false, byte unitId = 0) => ChangeAppearance(player, type, Player.GetPlayers().Where(x => x != player), skipJump, unitId);

        // -----------------------------------------------------------------------
        // <copyright file="MirrorExtensions.cs" company="Exiled Team">
        // Copyright (c) Exiled Team. All rights reserved.
        // Licensed under the CC BY-SA 3.0 license.
        // </copyright>
        // -----------------------------------------------------------------------
        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="playersToAffect">The players who should see the changed appearance.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, RoleTypeId type, IEnumerable<Player> playersToAffect, bool skipJump = false, byte unitId = 0)
        {
            if (!player.IsConnected() || !TryGetRoleBase(type, out PlayerRoleBase roleBase))
                return;

            bool isRisky = type.GetTeam() is Team.Dead || !player.IsAlive;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteUShort(38952);
            writer.WriteUInt(player.ReferenceHub.netId);
            writer.WriteRoleType(type);

            if (roleBase is HumanRole humanRole && humanRole.UsesUnitNames)
            {
                if (player.RoleBase is not HumanRole)
                    isRisky = true;
                writer.WriteByte(unitId);
            }

            if (roleBase is FpcStandardRoleBase fpc)
            {
                if (player.RoleBase is not FpcStandardRoleBase playerfpc)
                    isRisky = true;
                else
                    fpc = playerfpc;

                fpc.FpcModule.MouseLook.GetSyncValues(0, out ushort value, out ushort _);
                writer.WriteRelativePosition(new(player.Position));
                writer.WriteUShort(value);
            }

            if (roleBase is ZombieRole)
            {
                if (player.RoleBase is not ZombieRole)
                    isRisky = true;

                writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(player.MaxHealth), ushort.MinValue, ushort.MaxValue));
            }

            foreach (Player target in playersToAffect)
            {
                if (target != player || !isRisky)
                    target.Connection.Send(writer.ToArraySegment());
                else
                    Log.Error($"Prevent Seld-Desync of {player.Nickname} with {type}");
            }

            NetworkWriterPool.Return(writer);

            // To counter a bug that makes the player invisible until they move after changing their appearance, we will teleport them upwards slightly to force a new position update for all clients.
            if (!skipJump)
                player.Position += Vector3.up * 0.25f;
        }

        /// <summary>
        /// Checks if the player is well connected to the server.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool IsConnected(this Player player)
        {
            return player.GameObject != null && player.ReferenceHub.authManager.InstanceMode == CentralAuth.ClientInstanceMode.ReadyClient;
        }
    }
}

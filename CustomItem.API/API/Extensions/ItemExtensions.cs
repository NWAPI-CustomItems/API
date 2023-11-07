using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;
using static InventorySystem.Items.ThrowableProjectiles.ThrowableItem;

namespace NWAPI.CustomItems.API.Extensions
{
    /// <summary>
    /// Provides a collection of extension methods for working with item-related objects.
    /// These methods allow for additional functionality and operations on item-related types.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Checks if the specified <see cref="ItemType"/> is a keycard.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to check.</param>
        /// <returns><see langword="true"/> if the <see cref="ItemType"/> is a keycard; otherwise, <see langword="false"/>.</returns>
        public static bool IsKeycard(this ItemType type)
        {
            var itemBase = type.GetItemBase();
            return itemBase?.Category == ItemCategory.Keycard;
        }

        /// <summary>
        /// Checks if the specified <see cref="ItemType"/> is ammo.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to check.</param>
        /// <returns><see langword="true"/> if the <see cref="ItemType"/> is ammo; otherwise, <see langword="false"/>.</returns>
        public static bool IsAmmo(this ItemType type) 
        {
            var itemBase = type.GetItemBase();
            return itemBase?.Category == ItemCategory.Ammo;
        }

        /// <summary>
        /// Checks if the specified <see cref="ItemType"/> is a firearm.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to check.</param>
        /// <returns><see langword="true"/> if the ItemType is a firearm; otherwise, <see langword="false"/>.</returns>
        public static bool IsFirearm(this ItemType type) => type switch
        {
            ItemType.GunE11SR => true,
            ItemType.GunFRMG0 => true,
            ItemType.GunCom45 => true,
            ItemType.GunCOM15 => true,
            ItemType.GunCOM18 => true,
            ItemType.GunCrossvec => true,
            ItemType.GunLogicer => true,
            ItemType.GunRevolver => true,
            ItemType.GunShotgun => true,
            ItemType.GunAK => true,
            ItemType.GunFSP9 => true,
            _ => false
        };

        /// <summary>
        /// Checks if the specified <see cref="ItemType"/> is a special weapon.
        /// </summary>
        /// <param name="type">The ItemType to check.</param>
        /// <returns><see langword="true"/> if the <see cref="ItemType"/> is a special weapon; otherwise, <see langword="false"/>.</returns>
        public static bool IsSpecialWeapon(this ItemType type) => type switch
        {
            ItemType.Jailbird => true,
            ItemType.ParticleDisruptor => true,
            ItemType.MicroHID => true,
            _ => false
        };

        /// <summary>
        /// Checks if the specified <see cref="ItemType"/> is a type of armor.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to check.</param>
        /// <returns><see langword="true"/> if the <see cref="ItemType"/> is any type of armor; otherwise, <see langword="false"/>.</returns>
        public static bool IsArmor(this ItemType type) => type switch
        {
            ItemType.ArmorCombat => true,
            ItemType.ArmorHeavy => true,
            ItemType.ArmorLight => true,
            _ => false
        };

        /// <summary>
        /// Checks if the specified ItemType is throwable.
        /// </summary>
        /// <param name="type">The ItemType to check.</param>
        /// <returns>True if the ItemType is throwable; otherwise, false.</returns>
        public static bool IsThrowable(this ItemType type) => type switch
        {
            ItemType.GrenadeFlash => true,
            ItemType.GrenadeHE => true,
            ItemType.SCP018 => true,
            ItemType.SCP2176 => true,
            _ => false,
        };

        /// <summary>
        /// Checks if the item type is a medical item.
        /// </summary>
        /// <param name="type">The item type to check.</param>
        /// <returns>True if the item is of medical category, false otherwise.</returns>
        public static bool IsMedical(this ItemType type)
        {
            var itemBase = type.GetItemBase();

            return itemBase?.Category == ItemCategory.Medical;
        }



        /// <summary>
        /// Creates a throwable item of the specified ItemType and optionally assigns it to a player's inventory.
        /// If the ItemType is not throwable, it returns null. Otherwise, it creates a new throwable item instance.
        /// </summary>
        /// <param name="type">The ItemType to create a throwable item for.</param>
        /// <param name="player">The player to assign the item to. If null, it uses the host player's inventory.</param>
        /// <returns>A ThrowableItem instance if the creation is successful, or null if the ItemType is not throwable.</returns>
        public static ThrowableItem? CreateThrowableItem(this ItemType type, Player? player = null)
        {
            // Check if the ItemType is throwable
            if (!type.IsThrowable())
            {
                return null;
            }

            // Determine the inventory to use
            Inventory inventory = player?.ReferenceHub.inventory ?? ReferenceHub.HostHub.inventory;

            // Create a new item instance with a unique identifier
            ItemBase item = inventory.CreateItemInstance(new(type, ItemSerialGenerator.GenerateNext()), true);

            if (item is ThrowableItem throwable)
            {
                return throwable;
            }

            return null;
        }

        /// <summary>
        /// Spawns and activates a throwable item as a projectile at the specified position with an optional fuse time.
        /// </summary>
        /// <param name="item">The ThrowableItem to spawn as a projectile.</param>
        /// <param name="position">The position to spawn the projectile at.</param>
        /// <param name="fuseTime">The fuse time for the projectile (defaults to 1 second).</param>
        /// <param name="owner">The player who owns the item. If null, the host player is assumed.</param>
        public static void SpawnAndActivateThrowable(this ThrowableItem item, Vector3 position, float fuseTime = 1, Player? owner = null)
        {
            // Create a new instance of the projectile.
            ThrownProjectile projectile = UnityEngine.Object.Instantiate(item.Projectile, position, default);

            // Create network information for synchronization
            PickupSyncInfo networkInfo = new(item.ItemTypeId, item.Weight, item.ItemSerial)
            {
                Locked = !item._repickupable
            };

            projectile.NetworkInfo = networkInfo;
            projectile.PreviousOwner = new(owner != null ? owner.ReferenceHub : ReferenceHub.HostHub);

            NetworkServer.Spawn(projectile.gameObject);

            if (projectile is Scp018Projectile scp018)
            {
                // Adjust position and velocity for SCP-018 projectiles
                scp018.Position = position + Vector3.up;
                scp018._lastVelocity = UnityEngine.Random.value;
            }

            if (projectile is ExplosionGrenade grenade)
            {
                // Set the fuse time for explosion grenades
                grenade._fuseTime = fuseTime;
            }

            // Activate the projectile
            projectile.ServerActivate();
        }

        /// <summary>
        /// Throws a throwable item using specified settings.
        /// </summary>
        /// <param name="item">The ThrowableItem to throw.</param>
        /// <param name="fullForce">Determines whether the item is thrown with full force (true) or weak force (false).</param>
        /// <param name="runEvent">Specifies whether or not to execute the <see cref="PlayerThrowProjectileEvent"/>.</param>
        public static void ThrowItem(this ThrowableItem item, bool fullForce = true, bool runEvent = false)
        {
            ProjectileSettings settings = fullForce ? item.FullThrowSettings : item.WeakThrowSettings;

            if (runEvent)
                PluginAPI.Events.EventManager.ExecuteEvent(new PlayerThrowProjectileEvent(item.Owner, item, settings, fullForce));

            item.ServerThrow(settings.StartVelocity, settings.UpwardsFactor, settings.StartTorque, ThrowableNetworkHandler.GetLimitedVelocity(item.Owner?.GetVelocity() ?? Vector3.one));
        }

        /// <summary>
        /// Changes the scale of an ItemPickupBase, updating its local scale to the specified value.
        /// If the provided scale is the same as the current scale, no action is taken.
        /// If the pickup is already spawned on the network, it will be temporarily unspawned to apply the scale change.
        /// </summary>
        /// <param name="pickup">The ItemPickupBase whose scale will be changed.</param>
        /// <param name="Scale">The new scale to set for the ItemPickupBase.</param>
        public static void ChangeScale(this ItemPickupBase pickup, Vector3 Scale)
        {
            if (Scale == pickup.transform.localScale)
                return;

            if (!NetworkServer.spawned.ContainsKey(pickup.netId))
            {
                pickup.gameObject.transform.localScale = Scale;
                return;
            }

            NetworkServer.UnSpawn(pickup.gameObject);
            pickup.gameObject.transform.localScale = Scale;
            NetworkServer.Spawn(pickup.gameObject);
        }

        /// <summary>
        /// Changes the attachments code of a FirearmPickup.
        /// If the provided code is zero, no action is taken.
        /// </summary>
        /// <param name="fireArm">The FirearmPickup whose attachments code will be changed.</param>
        /// <param name="code">The new attachments code to set for the FirearmPickup.</param>
        public static void ChangeAttachmentsCode(this FirearmPickup fireArm, uint code)
        {
            if (code == 0)
                return;


            fireArm.NetworkStatus = new(fireArm.NetworkStatus.Ammo, fireArm.NetworkStatus.Flags, code);
        }

        /// <summary>
        /// Changes the attachments code of a Firearm, updating its status with the specified code.
        /// If the provided code is zero, no action is taken.
        /// </summary>
        /// <param name="firearm">The Firearm to update the attachments code for.</param>
        /// <param name="code">The new attachments code to set for the Firearm.</param>
        public static void ChangeAttachmentsCode(this Firearm firearm, uint code)
        {
            if (code == 0)
                return;

            firearm.Status = new(firearm.Status.Ammo, firearm.Status.Flags, code);
        }

        /// <summary>
        /// Changes the ammo count of a FirearmPickup.
        /// If the provided ammo count is zero, no action is taken.
        /// </summary>
        /// <param name="firearm">The FirearmPickup whose ammo count will be changed.</param>
        /// <param name="ammo">The new ammo count to set for the FirearmPickup.</param>
        public static void ChangeAmmo(this FirearmPickup firearm, byte ammo)
        {
            if (ammo == 0)
                return;

            firearm.NetworkStatus = new(ammo, firearm.NetworkStatus.Flags, firearm.NetworkStatus.Attachments);
        }

        /// <summary>
        /// Changes the ammo count of a Firearm, updating its status with the specified ammo count.
        /// If the provided ammo count is zero, no action is taken.
        /// </summary>
        /// <param name="firearm">The Firearm to update the ammo count for.</param>
        /// <param name="ammo1">The new ammo count to set for the Firearm.</param>
        public static void ChangeAmmo(this Firearm firearm, byte ammo1)
        {
            if (ammo1 == 0)
                return;

            firearm.Status = new(ammo1, firearm.Status.Flags, firearm.Status.Attachments);
        }

        /// <summary>
        /// Retrieves the base item associated with a specific item type.
        /// </summary>
        /// <param name="type">The item type for which the base item is requested.</param>
        /// <returns>
        /// The base item associated with the specified item type if available; otherwise, returns null.
        /// </returns>
        public static ItemBase? GetItemBase(this ItemType type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase @base))
                return null;

            return @base;
        }
    }
}

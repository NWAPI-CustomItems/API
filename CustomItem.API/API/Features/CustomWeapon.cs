using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using NWAPI.CustomItems.API.Extensions;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Items;
using PluginAPI.Events;
using System;
using UnityEngine;

namespace NWAPI.CustomItems.API.Features
{
    /// <summary>
    /// An abstract class representing a custom weapon item with customizable properties and behaviors.
    /// </summary>
    public abstract class CustomWeapon : CustomItem
    {
        /// <summary>
        /// Gets or sets the damage inflicted by the weapon.
        /// </summary>
        public abstract float Damage { get; set; }

        /// <summary>
        /// Gets or sets the clip size of the weapon, representing the number of rounds it can hold in one magazine.
        /// </summary>
        public virtual byte ClipSize { get; set; }

        /// <summary>
        /// Gets or sets the attachments code for the weapon, which can customize its attachments.
        /// </summary>
        public virtual uint AttachmentsCode { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire is enabled for the weapon.
        /// </summary>
        public virtual bool FriendlyFire { get; set; } = false;

        /// <summary>
        /// Spawns the custom weapon at the specified position and associates it with an optional owner.
        /// </summary>
        /// <param name="position">The position to spawn the weapon.</param>
        /// <param name="itemOwner">The optional owner of the weapon.</param>
        /// <returns>The spawned ItemPickup if successful, or null if there was an issue.</returns>
        public override ItemPickup? Spawn(Vector3 position, Player? itemOwner = null)
        {
            ItemPickup? pickup = ItemPickup.Create(ModelType, position, default);

            if (pickup.OriginalObject is not FirearmPickup firearm)
            {
                Log.Debug($"{nameof(Spawn)}: {ModelType} is not a Firearm.", Plugin.Instance.Config.DebugMode);
                pickup.Destroy();
                return null;
            }

            if (AttachmentsCode != 0)
                firearm.ChangeAttachmentsCode(AttachmentsCode);

            firearm.ChangeAmmo(ClipSize);

            pickup.Weight = Weight;

            if (Scale != Vector3.one)
                pickup.OriginalObject.ChangeScale(Scale);

            if (itemOwner != null)
                pickup.OriginalObject.PreviousOwner = new(itemOwner.ReferenceHub);

            TrackedSerials.Add(pickup.Serial);

            return pickup;
        }

        /// <summary>
        /// Gives the custom weapon to a player, optionally displaying a pickup message.
        /// </summary>
        /// <param name="player">The player who receives the weapon.</param>
        /// <param name="displayMessage">Determines whether to display a pickup message to the player.</param>
        public override void Give(Player player, bool displayMessage = true)
        {
            var item = player.AddItem(ModelType);

            if (item is not Firearm firearm)
            {
                Log.Debug($"{nameof(Give)}: {Name} - {ModelType} is not a firearm", Plugin.Instance.Config.DebugMode);

                player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);
                return;
            }

            if (AttachmentsCode != 0)
                firearm.ChangeAttachmentsCode(AttachmentsCode);

            firearm.ChangeAmmo(ClipSize);

            if (displayMessage)
                ShowPickupMessage(player);

            TrackedSerials.Add(item.ItemSerial);
        }

        /// <summary>
        /// Handles the event when the custom weapon is shot by a player.
        /// </summary>
        /// <param name="ev">The PlayerShotWeaponEvent that contains information about the shot.</param>
        protected virtual void OnShoot(PlayerShotWeaponEvent ev)
        {

        }

        /// <summary>
        /// Handles the event when the custom weapon is being reloaded by a player.
        /// </summary>
        /// <param name="ev">The PlayerReloadWeaponEvent that contains information about the reload.</param>
        /// <returns>True if reloading is allowed to proceed, false if it should be canceled.</returns>
        protected virtual bool OnReloading(PlayerReloadWeaponEvent ev)
        {
            return false;
        }

        /// <summary>
        /// Handles the event when the custom weapon is used to damage a player, applying custom rules for friendly fire.
        /// </summary>
        /// <param name="ev">The PlayerDamageEvent that contains information about the damage.</param>
        [PluginEvent]
        protected virtual void OnHurting(PlayerDamageEvent ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ev.DamageHandler is FirearmDamageHandler dmg)
            {
                if (!FriendlyFire && ev.Target.Team == ev.Player.Team)
                {
                    dmg.Damage = 0;
                }
                else
                {
                    dmg.Damage = Damage;
                }
            }
        }

        // private events \\
        [PluginEvent]
        private bool OnInternalReload(PlayerReloadWeaponEvent ev)
        {
            if (!Check(ev.Firearm))
                return true;

            byte remainingClip = ev.Firearm.Status.Ammo;

            if (remainingClip >= ClipSize)
                return false;

            var ammoType = ev.Firearm.AmmoType;

            if (!(ev.Player.AmmoBag.TryGetValue(ammoType, out var value) && value > 0))
                return false;

            ev.Player.Connection.Send(new RequestMessage(ev.Firearm.ItemSerial, RequestType.Reload));
            byte amountToReload = (byte)Math.Min(ClipSize - remainingClip, ev.Player.AmmoBag[ammoType]);

            if (amountToReload <= 0)
                return false;

            ev.Player.ReferenceHub.playerEffectsController.GetEffect<CustomPlayerEffects.Invisible>().Intensity = 0;
            ev.Player.AmmoBag[ammoType] -= amountToReload;
            ev.Player.ReferenceHub.inventory.SendAmmoNextFrame = true;

            FirearmStatusFlags flags = ev.Firearm.Status.Flags;
            if ((flags & FirearmStatusFlags.MagazineInserted) == 0)
            {
                flags |= FirearmStatusFlags.MagazineInserted;
            }
            ev.Firearm.Status = new((byte)(ev.Firearm.Status.Ammo + amountToReload), flags, ev.Firearm.Status.Attachments);

            return OnReloading(ev);
        }

        [PluginEvent]
        private void OnInternalShooting(PlayerShotWeaponEvent ev)
        {
            if (!Check(ev.Firearm))
                return;

            OnShoot(ev);
        }
    }
}

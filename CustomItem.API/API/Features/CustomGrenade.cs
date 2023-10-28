using InventorySystem.Items.ThrowableProjectiles;
using NWAPI.CustomItems.API.Extensions;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using UnityEngine;

namespace NWAPI.CustomItems.API.Features
{
    /// <summary>
    /// An abstract class representing a custom grenade that can be used in the game.
    /// Custom grenades can have unique properties and behaviors.
    /// </summary>
    public abstract class CustomGrenade : CustomItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether the grenade should explode upon collision with a surface.
        /// </summary>
        public abstract bool ExplodeOnCollision { get; set; }

        /// <summary>
        /// Gets or sets the fuse time for the grenade, determining the delay before it explodes after being thrown.
        /// </summary>
        public abstract float FuseTime { get; set; }

        /// <summary>
        /// Throws a custom grenade of the specified type at the given position with customizable properties.
        /// </summary>
        /// <param name="position">The position to throw the grenade to.</param>
        /// <param name="fullForce">Determines whether the grenade is thrown with full force (true) or weak force (false).</param>
        /// <param name="weight">The weight of the grenade.</param>
        /// <param name="grenadeType">The type of grenade to throw (defaults to HE grenade).</param>
        /// <param name="owner">The player who throws the grenade (defaults to the server).</param>
        /// <returns>The thrown ThrowableItem if successful, or null if the grenade type is not throwable.</returns>
        public virtual ThrowableItem? Throw(Vector3 position, bool fullForce, float weight, ItemType grenadeType = ItemType.GrenadeHE, Player? owner = null)
        {
            if (ModelType.IsThrowable())
                grenadeType = ModelType;

            ThrowableItem? throwable = grenadeType.CreateThrowableItem();

            if (throwable is null)
                return null;

            owner ??= Server.Instance;

            throwable.Owner = owner?.ReferenceHub;
            throwable._weight = weight;
            TrackedSerials.Add(throwable.ItemSerial);

            throwable.ThrowItem(fullForce, true);
            return throwable;
        }

        /// <summary>
        /// Handles the explosion event of the custom grenade.
        /// </summary>
        /// <param name="ev">The GrenadeExplodedEvent that contains information about the explosion.</param>
        /// <returns>True if the explosion is allowed to proceed, false if it should be canceled.</returns>
        public virtual bool OnExploding(GrenadeExplodedEvent ev)
        {
            return true;
        }

        /// <summary>
        /// Handles the throw event of the custom grenade.
        /// </summary>
        /// <param name="ev">The PlayerThrowProjectileEvent that contains information about the throw.</param>
        public virtual void OnThrow(PlayerThrowProjectileEvent ev)
        {

        }

        // private events \\

        // its exploding not exploded but NW dont know how name events.
        [PluginEvent]
        private bool OnInternalGrenadeExplode(GrenadeExplodedEvent ev)
        {
            if (ev.Grenade is null || !Check(ev.Grenade))
                return true;

            return OnExploding(ev);
        }

        [PluginEvent]
        private void OnInternalThrow(PlayerThrowProjectileEvent ev)
        {
            if (ev.Item is null || !Check(ev.Item))
                return;

            OnThrow(ev);
        }
    }
}

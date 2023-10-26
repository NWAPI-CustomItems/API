using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;

namespace NWAPI.CustomItems.API.Components
{
    /// <summary>
    /// A component that triggers the explosion of the grenade upon collision with something, excluding the owner of the grenade and a open door.
    /// </summary>
    public class ExplodeOnCollision : MonoBehaviour
    {
        private bool alreadyExplode = false;

        private GameObject? Owner;

        private EffectGrenade? Grenade;

        private void Start()
        {
            Grenade = GetComponent<EffectGrenade>();

            if (Grenade is null)
            {
                Destroy(this);
                return;
            }

            if (Grenade.PreviousOwner.IsSet)
                Owner = Grenade.PreviousOwner.Hub.gameObject;
        }

        /// <summary>
        /// Called when the object collides with another object.
        /// If the collision is with the grenade's owner, no action is taken.
        /// Otherwise, it triggers the explosion of the grenade.
        /// </summary>
        /// <param name="collision">The collision data.</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == Owner)
                return;

            if (Grenade != null && !alreadyExplode)
            {
                // Prevents exploding when touching the collisions of an open door.
                if (collision.gameObject.TryGetComponent<DoorVariant>(out var door) && door.NetworkTargetState is true)
                {
                    return;
                }

                alreadyExplode = true;
                Grenade.TargetTime = NetworkTime.time + 0.05;
            }

            Destroy(this);
        }

        /// <summary>
        /// Disables the component, effectively removing it from the GameObject.
        /// </summary>
        public void Disable()
        {
            Destroy(this);
        }
    }

}

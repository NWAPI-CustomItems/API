using InventorySystem.Items.Pickups;
using MapGeneration;
using MapGeneration.Distributors;
using Mirror;
using PluginAPI.Core;
using System.Collections.Generic;
using UnityEngine;

namespace NWAPI.CustomItems.API.Features
{
    /// <summary>
    /// Represents a <see cref="Locker"/> in the map.
    /// </summary>
    public class MapLocker
    {
        /// <summary>
        /// Gets all <see cref="MapLocker"/> in the map.
        /// </summary>
        public static HashSet<MapLocker> Lockers = new();

        /// <summary>
        /// Clears and adds all lockers in the map.
        /// </summary>
        public static void CacheLockers()
        {
            Lockers.Clear();

            foreach (var locker in Map.Lockers)
            {
                new MapLocker(locker);
            }
        }

        // private fields.
        private Locker _locker;
        private RoomIdentifier? _roomIdentifier;
        private bool _roomSearched = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapLocker"/> class.
        /// </summary>
        /// <param name="locker">The Locker instance representing.</param>
        public MapLocker(Locker locker)
        {
            _locker = locker;
            Lockers.Add(this);
        }

        /// <summary>
        /// Gets locker position.
        /// </summary>
        public Vector3 Position => _locker.transform.position;

        /// <summary>
        /// Gets locker <see cref="RoomIdentifier"/> where is located..
        /// </summary>
        public RoomIdentifier? Room
        {
            get
            {
                if (_roomIdentifier is null && !_roomSearched)
                {
                    _roomIdentifier = RoomIdUtils.RoomAtPositionRaycasts(Position);
                    _roomSearched = true;
                }

                return _roomIdentifier;
            }
        }

        /// <summary>
        /// Gets locker <see cref="FacilityZone"/>.
        /// </summary>
        public FacilityZone Zone => Room?.Zone ?? FacilityZone.None;

        /// <summary>
        /// Adds loot to the locker, handling the spawning and placement of items.
        /// </summary>
        /// <param name="item">The ItemPickupBase instance representing the loot to be added.</param>
        public void AddLoot(ItemPickupBase item)
        {
            // Check if the item is already spawned on the network.
            if (NetworkServer.spawned.ContainsKey(item.netId))
                // If the item is already spawned, unspawn it.
                NetworkServer.UnSpawn(item.gameObject);


            // Log information about the loot being added to the locker.
            Log.Debug($"{nameof(AddLoot)}: Spawning {item.Info.ItemId} ({item.Info.Serial}) inside a locker {Room?.Name} | {Zone}", Plugin.Instance.Config.DebugMode);

            // Retrieve a random chamber from the locker.
            var chamber = _locker.Chambers.RandomItem();

            // Check if the chamber is null, log a debug message, and return if so.
            if (chamber is null)
            {
                Log.Debug($"{nameof(AddLoot)}: Chamber is null, attempting to add {item.Info.ItemId} ({item.Info.Serial})", Plugin.Instance.Config.DebugMode);
                return;
            }

            // Set the parent of the item to the chamber's spawn point, lock the item, and add it to the chamber's content.
            item.transform.SetParent(chamber._spawnpoint);
            item.Info.Locked = true;
            chamber._content.Add(item);

            // Trigger the distribution event if the item implements the IPickupDistributorTrigger interface.
            (item as IPickupDistributorTrigger)?.OnDistributed();

            // If the item has a Rigidbody component, make it kinematic, reset its position and rotation, and add it to the list of bodies to unfreeze.
            if (item.TryGetComponent<Rigidbody>(out var component))
            {
                component.isKinematic = true;
                component.transform.localPosition = Vector3.zero;
                component.transform.localRotation = Quaternion.identity;
                SpawnablesDistributorBase.BodiesToUnfreeze.Add(component);
            }

            // If the chamber should spawn items on the first opening, add the item to the list of items to be spawned; otherwise, spawn the item immediately.
            if (chamber._spawnOnFirstChamberOpening)
                chamber._toBeSpawned.Add(item);
            else
                ItemDistributor.SpawnPickup(item);
        }
    }
}

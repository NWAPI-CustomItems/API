using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MEC;
using NWAPI.CustomItems.API.Spawn;
using NWAPI.CustomItems.API.Struct;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Items;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;

namespace NWAPI.CustomItems.API.Features
{
    /// <summary>
    /// Represents a base class for custom items.
    /// </summary>
    public abstract class CustomItem
    {
        private static readonly HashSet<CustomItemInfo?> LookupTable = new();

        /// <summary>
        /// Gets a collection of serial numbers for all custom items spawned.
        /// </summary>
        public static HashSet<ushort> AllCustomItemsSerials = new();

        /// <summary>
        /// Gets a collection of registered custom items.
        /// </summary>
        public static HashSet<CustomItem> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the unique identifier of the custom item.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom item.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the custom item.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the weight of the custom item.
        /// </summary>
        public abstract float Weight { get; set; }

        /// <summary>
        /// Gets or sets the item type for the custom item.
        /// </summary>
        public abstract ItemType ModelType { get; set; }

        /// <summary>
        /// Gets or sets the list of spawn locations and chances for each one.
        /// </summary>
        public abstract SpawnProperties? SpawnProperties { get; set; }

        /// <summary>
        /// Gets or sets the scale of the custom item. Default is (1, 1, 1).
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets a collection of serial numbers for tracked custom items.
        /// </summary>
        [YamlIgnore]
        public HashSet<ushort> TrackedSerials { get; } = new();

        #region Get

        /// <summary>
        /// Gets a <see cref="CustomItem"/> by its id.
        /// </summary>
        /// <param name="id">The unique identifier of the custom item.</param>
        /// <returns>The retrieved custom item, if found; otherwise, null.</returns>
        public static CustomItem? Get(uint id)
        {
            if (LookupTable.TryGetValue(LookupTable.FirstOrDefault(c => c.HasValue && c.Value.Id == id), out var value) && value.HasValue)
            {
                return value.Value.CustomItem;
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="CustomItem"/> by its name.
        /// </summary>
        /// <param name="name">The name of the custom item.</param>
        /// <returns>The retrieved custom item, if found; otherwise, null.</returns>
        public static CustomItem? Get(string name)
        {
            if (LookupTable.TryGetValue(LookupTable.FirstOrDefault(c => c.HasValue && c.Value.Name == name), out var value) && value.HasValue)
            {
                return value.Value.CustomItem;
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="CustomItem"/> by its .NET type.
        /// </summary>
        /// <param name="type">The .NET type of the custom item.</param>
        /// <returns>The retrieved custom item, if found; otherwise, null.</returns>
        public static CustomItem? Get(Type type)
        {
            if (LookupTable.TryGetValue(LookupTable.FirstOrDefault(c => c.HasValue && c.Value.Type == type), out var value) && value.HasValue)
            {
                return value.Value.CustomItem;
            }

            return null;
        }

        #endregion

        #region Try get

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> by its id.
        /// </summary>
        /// <param name="id">The unique identifier of the custom item.</param>
        /// <param name="customItem">The retrieved custom item, if found; otherwise, null.</param>
        /// <returns>True if the custom item is found; otherwise, false.</returns>
        public static bool TryGet(uint id, out CustomItem? customItem)
        {
            customItem = Get(id);

            return customItem != null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> by its name.
        /// </summary>
        /// <param name="name">The name of the custom item.</param>
        /// <param name="customItem">The retrieved custom item, if found; otherwise, null.</param>
        /// <returns>True if the custom item is found; otherwise, false.</returns>
        public static bool TryGet(string name, out CustomItem? customItem)
        {
            customItem = null;
            if (string.IsNullOrEmpty(name))
                return false;

            customItem = Get(name);

            if (customItem is null && uint.TryParse(name, out var id))
                customItem = Get(id);

            return customItem != null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> by its .NET type.
        /// </summary>
        /// <param name="type">The .NET type of the custom item.</param>
        /// <param name="customItem">The retrieved custom item, if found; otherwise, null.</param>
        /// <returns>True if the custom item is found; otherwise, false.</returns>
        public static bool TryGet(Type type, out CustomItem? customItem)
        {
            customItem = Get(type);

            return customItem != null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> from a player's current item.
        /// </summary>
        /// <param name="player">The player for whom to find the custom item in their hand.</param>
        /// <param name="customItem">The retrieved custom item in the player's hand, if found; otherwise, null.</param>
        /// <returns>True if the custom item is found in the player's hand; otherwise, false.</returns>
        public static bool TryGet(Player player, out CustomItem? customItem)
        {
            customItem = null;

            if (player is null)
                return false;

            customItem = Registered.FirstOrDefault(item => item.Check(player));
            return customItem != null;
        }

        /// <summary>
        /// Tries to get a collection of <see cref="CustomItem"/>s from a player's inventory.
        /// </summary>
        /// <param name="player">The player for whom to find custom items in their inventory.</param>
        /// <param name="customItems">The retrieved collection of custom items in the player's inventory, if found; otherwise, null.</param>
        /// <returns>True if custom items are found in the player's inventory; otherwise, false.</returns>
        public static bool TryGet(Player player, out IEnumerable<CustomItem>? customItems)
        {
            customItems = null;

            if (player is null)
                return false;

            customItems = Registered.Where(item => item.Check(player));

            return customItems != null;
        }

        /// <summary>
        /// Tries to check if an <see cref="Item"/> is a <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="item">The item to check for being a custom item.</param>
        /// <param name="customItem">The retrieved custom item, if it is a custom item; otherwise, null.</param>
        /// <returns>True if the item is a custom item; otherwise, false.</returns>
        public static bool TryGet(Item item, out CustomItem? customItem)
        {
            customItem = item is null ? null : Registered?.FirstOrDefault(i => i.TrackedSerials.Contains(item.Serial));

            return customItem != null;
        }

        /// <summary>
        /// Tries to check if an <see cref="ItemBase"/> is a <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="item">The item base to check for being a custom item.</param>
        /// <param name="customItem">The retrieved custom item, if it is a custom item; otherwise, null.</param>
        /// <returns>True if the item base is a custom item; otherwise, false.</returns>
        public static bool TryGet(ItemBase item, out CustomItem? customItem)
        {
            customItem = item is null ? null : Registered?.FirstOrDefault(i => i.TrackedSerials.Contains(item.ItemSerial));

            return customItem != null;
        }

        /// <summary>
        /// Tries to check if an <see cref="ItemPickup"/> is a <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="pickup">The item pickup to check for being a custom item.</param>
        /// <param name="customItem">The retrieved custom item, if it is a custom item; otherwise, null.</param>
        /// <returns>True if the item pickup is a custom item; otherwise, false.</returns>
        public static bool TryGet(ItemPickup pickup, out CustomItem? customItem)
        {
            customItem = pickup is null ? null : Registered?.FirstOrDefault(p => p.TrackedSerials.Contains(pickup.Serial));

            return customItem != null;
        }

        /// <summary>
        /// Tries to check if an <see cref="ItemPickupBase"/> is a <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="pickup">The item pickup base to check for being a custom item.</param>
        /// <param name="customItem">The retrieved custom item, if it is a custom item; otherwise, null.</param>
        /// <returns>True if the item pickup base is a custom item; otherwise, false.</returns>
        public static bool TryGet(ItemPickupBase pickup, out CustomItem? customItem)
        {
            customItem = pickup is null ? null : Registered?.FirstOrDefault(p => p.TrackedSerials.Contains(pickup.Info.Serial));

            return customItem != null;
        }

        /// <summary>
        /// Tries to check if a specified serial belongs to a <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="serial">The serial to check for belonging to a custom item.</param>
        /// <param name="customItem">The retrieved custom item, if the serial belongs to a custom item; otherwise, null.</param>
        /// <returns>True if the serial belongs to a custom item; otherwise, false.</returns>
        public static bool TryGet(ushort serial, out CustomItem? customItem)
        {
            customItem = Registered.FirstOrDefault(p => p.TrackedSerials.Contains(serial));
            return customItem != null;
        }

        #endregion

        #region Spawn

        /// <summary>
        /// Spawns an <see cref="CustomItem"/> pickup at the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the spawn location.</param>
        /// <param name="y">The Y-coordinate of the spawn location.</param>
        /// <param name="z">The Z-coordinate of the spawn location.</param>
        /// <returns>The spawned <see cref="ItemPickup"/>, if successful; otherwise, null.</returns>
        public virtual ItemPickup? Spawn(float x, float y, float z) => Spawn(new(x, y, z));

        /// <summary>
        /// Spawns an <see cref="CustomItem"/> pickup at the specified position.
        /// </summary>
        /// <param name="position">The position at which to spawn the item pickup.</param>
        /// <returns>The spawned <see cref="ItemPickup"/>, if successful; otherwise, null.</returns>
        public virtual ItemPickup? Spawn(Vector3 position) => Spawn(position);

        /// <summary>
        /// Spawns an <see cref="CustomItem"/> pickup at the specified position with an optional item owner.
        /// </summary>
        /// <param name="position">The position at which to spawn the item pickup.</param>
        /// <param name="itemOwner">The optional owner of the item pickup.</param>
        /// <returns>The spawned <see cref="ItemPickup"/>, if successful; otherwise, null.</returns>
        public virtual ItemPickup? Spawn(Vector3 position, Player? itemOwner = null) => Spawn(position, itemOwner);

        /// <summary>
        /// Spawns an <see cref="CustomItem"/> pickup at the position of a specified player with an optional item owner.
        /// </summary>
        /// <param name="player">The player whose position will be used for spawning.</param>
        /// <param name="itemOwner">The optional owner of the item pickup.</param>
        /// <returns>The spawned <see cref="ItemPickup"/>, if successful; otherwise, null.</returns>
        public virtual ItemPickup? Spawn(Player player, Player? itemOwner = null) => Spawn(player.Position, itemOwner);

        /// <summary>
        /// Spawns an <see cref="CustomItem"/> pickup at the specified position with an optional owner and rotation.
        /// </summary>
        /// <param name="position">The position at which to spawn the item pickup.</param>
        /// <param name="owner">The optional owner of the item pickup.</param>
        /// <param name="rotation">The rotation of the spawned item pickup.</param>
        /// <returns>The spawned <see cref="ItemPickup"/>, if successful; otherwise, null.</returns>
        public virtual ItemPickup? Spawn(Vector3 position, Player? owner = null, Quaternion rotation = default)
        {
            if (ModelType == ItemType.None)
                return null;

            var pickup = ItemPickup.Create(ModelType, position, rotation);

            pickup.Weight = Weight;

            if (Scale != Vector3.one)
                pickup.OriginalObject.transform.localScale = Scale;

            if (owner != null)
                pickup.OriginalObject.PreviousOwner = new(owner.ReferenceHub);

            return pickup;
        }

        /// <summary>
        /// Spawns multiple <see cref="ItemPickup"/>s at specified spawn points.
        /// </summary>
        /// <param name="spawnPoints">An enumeration of spawn points.</param>
        /// <param name="amount">The number of items to spawn.</param>
        /// <returns>The total number of items successfully spawned.</returns>
        public virtual uint Spawn(IEnumerable<Vector3> spawnPoints, uint amount)
        {
            uint spawned = 0;

            // Do the logic for spawning the items based in spawpoints

            return spawned;
        }

        #endregion

        /// <summary>
        /// Spawns all items at their designated spawn locations.
        /// </summary>
        public virtual void SpawnAll()
        {
            // do spawn logic to spawn the item in they spawn locations.
        }

        #region Give

        /// <summary>
        /// Gives the specified <see cref="Player"/> the custom item.
        /// </summary>
        /// <param name="player">The player to receive the custom item.</param>
        /// <param name="displayMessage">A boolean indicating whether to display a pickup message; true by default.</param>
        public virtual void Give(Player player, bool displayMessage = true)
        {
            var item = player.AddItem(ModelType);

            if (item is null)
                return;

            if (!TrackedSerials.Contains(item.ItemSerial))
                TrackedSerials.Add(item.ItemSerial);

            AllCustomItemsSerials.Add(item.ItemSerial);

            if (displayMessage)
                Timing.CallDelayed(.2f, () => ShowPickupMessage(player));
        }

        /// <summary>
        /// Gives the specified <see cref="ReferenceHub"/> the custom item.
        /// </summary>
        /// <param name="hub">The reference hub to receive the custom item.</param>
        /// <param name="displayMessage">A boolean indicating whether to display a pickup message; true by default.</param>
        public virtual void Give(ReferenceHub hub, bool displayMessage = true)
        {
            var item = hub.inventory.ServerAddItem(ModelType, 0);

            if (item is null)
                return;

            if (!TrackedSerials.Contains(item.ItemSerial))
                TrackedSerials.Add(item.ItemSerial);

            AllCustomItemsSerials.Add(item.ItemSerial);

            if (displayMessage)
                Timing.CallDelayed(.2f, () => ShowPickupMessage(Player.Get(hub)));
        }

        #endregion

        #region Check

        /// <summary>
        /// Checks if a specified <see cref="ItemPickup"/> is associated with a custom item.
        /// </summary>
        /// <param name="pickup">The item pickup to check for association with a custom item.</param>
        /// <returns>True if the item pickup is associated with a custom item; otherwise, false.</returns>
        public virtual bool Check(ItemPickup? pickup)
            => pickup is not null && TrackedSerials.Contains(pickup.Serial);

        /// <summary>
        /// Checks if a specified <see cref="ItemPickupBase"/> is associated with a custom item.
        /// </summary>
        /// <param name="pickupBase">The item pickup base to check for association with a custom item.</param>
        /// <returns>True if the item pickup base is associated with a custom item; otherwise, false.</returns>
        public virtual bool Check(ItemPickupBase? pickupBase)
            => pickupBase is not null && TrackedSerials.Contains(pickupBase.Info.Serial);

        /// <summary>
        /// Checks if a specified <see cref="Item"/> is associated with a custom item.
        /// </summary>
        /// <param name="item">The item to check for association with a custom item.</param>
        /// <returns>True if the item is associated with a custom item; otherwise, false.</returns>
        public virtual bool Check(Item? item)
            => item is not null && TrackedSerials.Contains(item.Serial);

        /// <summary>
        /// Checks if a specified <see cref="ItemBase"/> is associated with a custom item.
        /// </summary>
        /// <param name="itemBase">The item base to check for association with a custom item.</param>
        /// <returns>True if the item base is associated with a custom item; otherwise, false.</returns>
        public virtual bool Check(ItemBase? itemBase)
            => itemBase is not null && TrackedSerials.Contains(itemBase.ItemSerial);

        /// <summary>
        /// Checks if a specified <see cref="Player"/> is currently holding a custom item.
        /// </summary>
        /// <param name="player">The player to check for holding a custom item.</param>
        /// <returns>True if the player is holding a custom item; otherwise, false.</returns>
        public virtual bool Check(Player? player)
            => Check(player?.CurrentItem);

        /// <summary>
        /// Checks if a specified serial number is associated with a custom item.
        /// </summary>
        /// <param name="serial">The serial number to check for association with a custom item.</param>
        /// <returns>True if the serial number is associated with a custom item; otherwise, false.</returns>
        public virtual bool Check(ushort serial)
            => TrackedSerials.Contains(serial);
        #endregion

        // internal methods \\
        internal bool TryRegister()
        {
            if (!Plugin.Instance.Config.IsEnabled)
            {
                Log.Debug($"Registration of {Name} ({Id}) skipped because the plugin is disabled.", Plugin.Instance.Config.DebugMode);
                return false;
            }

            Log.Debug($"Trying to register {Name} ({Id})", Plugin.Instance.Config.DebugMode);

            if (!IsRegistered(this))
            {
                if (IsRegistered(this.Id))
                {
                    Log.Warning($"Registration of {Name} ({Id}) failed. It has the same ID as another item.");
                    return false;
                }

                Log.Debug($"Adding {Name} to the registered items hashset", Plugin.Instance.Config.DebugMode);
                Registered.Add(this);

                Init();

                Log.Debug($"{Name} ({Id}, {ModelType}) has been successfully registered.", Plugin.Instance.Config.DebugMode);
                return true;
            }

            return false;
        }

        internal bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warning($"Failed to unregister {Name} ({Id}, {ModelType}). It was not registered.");
                return false;
            }

            Log.Debug($"{Name} ({Id}, {ModelType}) has been successfully unregistered.", Plugin.Instance.Config.DebugMode);
            return true;
        }

        internal bool IsRegistered(CustomItem item)
        {
            return Registered.Contains(item);
        }

        internal bool IsRegistered(uint id)
        {
            return Registered.Any(i => i.Id == id);
        }

        // public events \\

        /// <summary>
        /// Event handler called when the owner of the custom item changes their role.
        /// </summary>
        /// <param name="ev">The event data for the role change.</param>
        public virtual void OnOwnerChangeRole(PlayerChangeRoleEvent ev)
        {

        }

        /// <summary>
        /// Event handler called when the owner of the custom item is dying.
        /// </summary>
        /// <param name="ev">The event data for the player's dying event.</param>
        public virtual void OnOwnerDying(PlayerDyingEvent ev)
        {

        }

        /// <summary>
        /// Event handler called when the owner of the custom item escapes.
        /// </summary>
        /// <param name="ev">The event data for the player's escape event.</param>
        public virtual void OnOwnerEscape(PlayerEscapeEvent ev)
        {

        }

        /// <summary>
        /// Event handler called when the owner of the custom item is being handcuffed.
        /// </summary>
        /// <param name="ev">The event data for the handcuffing event.</param>
        public virtual void OnOwnerHanducuffing(PlayerHandcuffEvent ev)
        {
            // It's always the target handcuffed.
        }

        /// <summary>
        /// Event handler called when the owner of the custom item drops the item.
        /// </summary>
        /// <param name="ev">The event data for the item dropping event.</param>
        public virtual bool OnDropped(PlayerDroppedItemEvent ev)
        {
            return true;
        }

        /// <summary>
        /// Event handler called when the owner of the custom item picks up an item from a search.
        /// </summary>
        /// <param name="ev">The event data for the item pickup event.</param>
        public virtual void OnPickedup(PlayerSearchedPickupEvent ev)
        {
            ShowPickupMessage(ev.Player);
        }

        /// <summary>
        /// Event handler called when the owner of the custom item changes their held item.
        /// </summary>
        /// <param name="ev">The event data for the item change event.</param>
        public virtual void OnChangeItem(PlayerChangeItemEvent ev)
        {
            ShowSelectMessage(ev.Player);
        }

        /// <summary>
        /// Initializes the custom item by adding its information to a lookup table and subscribing to necessary events.
        /// </summary>
        public virtual void Init()
        {
            LookupTable.Add(new(Id, Name, GetType(), this));
            SubscribeEvents();
        }

        /// <summary>
        /// Destroys the custom item by unsubscribing events and removing its information from the lookup table.
        /// </summary>
        public virtual void Destroy()
        {
            UnsubscribeEvents();

            var customItemInfo = LookupTable.FirstOrDefault(i => i.HasValue && i.Value.Id == Id);

            if (customItemInfo.HasValue)
                LookupTable.Remove(customItemInfo);
        }

        /// <summary>
        /// Subscribes to custom event handlers specific to this custom item. 
        /// This method is called after the manager is initialized to register the events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            // To register event handlers, use PluginAPI.Events.EventManager.RegisterEvents(YourPluginInstance, YourCustomItemInstance);
        }

        /// <summary>
        /// Unsubscribes from custom event handlers associated with this custom item.
        /// This method is called when the manager is being destroyed to enable the unloading of special event.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            // To unregister event handlers, use PluginAPI.Events.EventManager.UnregisterEvents(YourPluginInstance, YourCustomItemInstance);
        }

        /// <summary>
        /// Clears the hashset of item serials and Pickup serials when the server is waiting for players.
        /// </summary>
        [PluginEvent]
        protected virtual void OnWaitingForPlayers()
        {
            AllCustomItemsSerials.Clear();
            TrackedSerials.Clear();
        }

        /// <summary>
        /// Displays a pickup message to the specified player.
        /// </summary>
        /// <param name="player">The player to whom the pickup message will be displayed.</param>
        public virtual void ShowPickupMessage(Player? player)
        {
            var hint = Plugin.Instance.Config.PickupMessage;
            player?.ReceiveHint(string.Format(hint.Message, Name, Description), hint.Duration);
        }

        /// <summary>
        /// Displays a selection message to the specified player.
        /// </summary>
        /// <param name="player">The player to whom the selection message will be displayed.</param>
        public virtual void ShowSelectMessage(Player? player)
        {
            var hint = Plugin.Instance.Config.SelectMessage;
            player?.ReceiveHint(string.Format(hint.Message, Name, Description), hint.Duration);
        }

        // private events \\

        [PluginEvent]
        private void OnInternalOwnerChangingRole(PlayerChangeRoleEvent ev)
        {
            if (ev.ChangeReason == PlayerRoles.RoleChangeReason.Escaped)
                return;

            List<ItemBase> items = new(ev.Player.Items.ToList());
            foreach (var item in items)
            {
                if (!Check(item))
                    continue;

                OnOwnerChangeRole(ev);

                TrackedSerials.Remove(item.ItemSerial);

                // ev.Player.RemoveItem(item) its broken in NWAPI.
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);

                Spawn(ev.Player, ev.Player);
            }
        }

        [PluginEvent]
        private void OnInternalOwnerDying(PlayerDyingEvent ev)
        {
            List<ItemBase> items = new(ev.Player.Items.ToList());
            Vector3 position = ev.Player.Position + Vector3.up;

            foreach (var item in items)
            {
                if (!Check(item))
                    continue;

                TrackedSerials.Remove(item.ItemSerial);

                // ev.Player.RemoveItem(item) its broken in NWAPI.
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);

                OnOwnerDying(ev);

                Spawn(position, ev.Player);
            }
        }

        [PluginEvent]
        private void OnInternalOwnerEscaping(PlayerEscapeEvent ev)
        {
            List<ItemBase> items = new(ev.Player.Items.ToList());
            foreach (var item in items)
            {
                if (!Check(item))
                    continue;

                TrackedSerials.Remove(item.ItemSerial);

                // ev.Player.RemoveItem(item) its broken in NWAPI.
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);

                OnOwnerEscape(ev);

                Timing.CallDelayed(1.5f, () =>
                {
                    Spawn(ev.Player.Position, null);
                });
            }
        }

        [PluginEvent]
        private void OnInternalOwnerHandcuffing(PlayerHandcuffEvent ev)
        {
            List<ItemBase> items = new(ev.Target.Items.ToList());

            foreach (var item in items)
            {
                if (!Check(item))
                    continue;

                OnOwnerHanducuffing(ev);

                TrackedSerials.Remove(item.ItemSerial);

                // ev.Player.RemoveItem(item) its broken in NWAPI.
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);
            }
        }

        [PluginEvent]
        private bool OnInternalDropped(PlayerDroppedItemEvent ev)
        {
            if (!Check(ev.Item))
                return true;

            if (ev.Item.NetworkInfo.WeightKg != Weight)
            {
                ev.Item.NetworkInfo = new(ev.Item.NetworkInfo.ItemId, Weight, ev.Item.NetworkInfo.Serial);
            }

            return OnDropped(ev);
        }

        [PluginEvent]
        private void OnInternalPickedup(PlayerSearchedPickupEvent ev)
        {
            if (!Check(ev.Item) || ev.Player.IsInventoryFull)
                return;


            OnPickedup(ev);
        }

        [PluginEvent]
        private void OnInternalChangingItem(PlayerChangeItemEvent ev)
        {
            if (!Check(ev.NewItem))
                return;

            OnChangeItem(ev);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Name} - {ModelType} | {Id} | {Description}";

        /// <inheritdoc/>
        public static implicit operator CustomItem?(uint id)
        {
            return Get(id);
        }

        /// <inheritdoc/>
        public static implicit operator CustomItem?(string name)
        {
            return Get(name);
        }
    }
}

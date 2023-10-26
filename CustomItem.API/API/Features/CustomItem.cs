using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MEC;
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
    /// P A I N
    /// </summary>
    public abstract class CustomItem
    {
        private static readonly HashSet<CustomItemInfo?> LookupTable = new();

        public static HashSet<ushort> AllCustomItemsSerials = new();

        public static HashSet<CustomItem> Registered { get; } = new();

        // API \\
        public abstract uint Id { get; set; }

        public abstract string Name { get; set; }

        public abstract string Description { get; set; }

        public abstract float Weight { get; set; }

        public abstract ItemType ModelType { get; set; }

        public virtual Vector3 Scale { get; set; } = Vector3.one;

        [YamlIgnore]
        public HashSet<ushort> TrackedSerials { get; } = new();

        #region Get
        public static CustomItem? Get(uint id)
        {
            if (LookupTable.TryGetValue(LookupTable.FirstOrDefault(c => c.HasValue && c.Value.Id == id), out var value) && value.HasValue)
            {
                return value.Value.CustomItem;
            }

            return null;
        }

        public static CustomItem? Get(string name)
        {
            if (LookupTable.TryGetValue(LookupTable.FirstOrDefault(c => c.HasValue && c.Value.Name == name), out var value) && value.HasValue)
            {
                return value.Value.CustomItem;
            }

            return null;
        }

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

        public static bool TryGet(uint id, out CustomItem? customItem)
        {
            customItem = Get(id);

            return customItem != null;
        }

        public static bool TryGet(string name, out CustomItem? customItem)
        {
            customItem = null;
            if (string.IsNullOrEmpty(name))
                return false;

            customItem = Get(name);
            return customItem != null;
        }

        public static bool TryGet(Type type, out CustomItem? customItem)
        {
            customItem = Get(type);

            return customItem != null;
        }

        public static bool TryGet(Player player, out CustomItem? customItem)
        {
            customItem = null;

            if (player is null)
                return false;

            return false;
        }

        public static bool TryGet(Player player, out IEnumerable<CustomItem>? customItems)
        {
            customItems = null;

            if (player is null)
                return false;


            return false;
        }

        public static bool TryGet(Item item, out CustomItem? customItem)
        {
            customItem = item is null ? null : Registered?.FirstOrDefault(i => i.TrackedSerials.Contains(item.Serial));

            return customItem != null;
        }

        public static bool TryGet(ItemBase item, out CustomItem? customItem)
        {
            customItem = item is null ? null : Registered?.FirstOrDefault(i => i.TrackedSerials.Contains(item.ItemSerial));

            return customItem != null;
        }

        public static bool TryGet(ItemPickup pickup, out CustomItem? customItem)
        {
            customItem = pickup is null ? null : Registered?.FirstOrDefault(p => p.TrackedSerials.Contains(pickup.Serial));

            return customItem != null;
        }

        public static bool TryGet(ItemPickupBase pickup, out CustomItem? customItem)
        {
            customItem = pickup is null ? null : Registered?.FirstOrDefault(p => p.TrackedSerials.Contains(pickup.Info.Serial));

            return customItem != null;
        }

        public static bool TryGet(ushort serial, out CustomItem? customItem)
        {
            customItem = Registered.FirstOrDefault(p => p.TrackedSerials.Contains(serial));
            return customItem != null;
        }

        #endregion

        #region Spawn

        public virtual ItemPickup? Spawn(float x, float y, float z) => Spawn(new(x, y, z));

        public virtual ItemPickup? Spawn(Vector3 position) => Spawn(position);

        public virtual ItemPickup? Spawn(Vector3 position, Player? itemOwner = null) => Spawn(position, itemOwner);

        public virtual ItemPickup? Spawn(Player player, Player? itemOwner = null) => Spawn(player.Position, itemOwner);

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

        public virtual uint Spawn(IEnumerable<Vector3> spawnPoints, uint amount)
        {
            uint spawned = 0;

            // Do the logic for spawning the items based in spawpoints

            return spawned;
        }

        #endregion

        public virtual void SpawnAll()
        {
            // do spawn logic to spawn the item in they spawn locations.
        }

        #region Give

        public virtual void Give(Player player, bool displayMessage = true)
        {
            var item = player.AddItem(ModelType);

            if (item is null)
                return;

            if (!TrackedSerials.Contains(item.ItemSerial))
                TrackedSerials.Add(item.ItemSerial);

            if (displayMessage)
                Timing.CallDelayed(.5f, () => ShowPickupMessage(player));
        }

        #endregion

        #region Check

        public virtual bool Check(ItemPickup? pickup) => pickup is not null && TrackedSerials.Contains(pickup.Serial);

        public virtual bool Check(ItemPickupBase? pickupBase) => pickupBase is not null && TrackedSerials.Contains(pickupBase.Info.Serial);

        public virtual bool Check(Item? item) => item is not null && TrackedSerials.Contains(item.Serial);

        public virtual bool Check(ItemBase? itemBase) => itemBase is not null && TrackedSerials.Contains(itemBase.ItemSerial);

        public virtual bool Check(Player? player) => Check(player?.CurrentItem);

        public virtual bool Check(ushort serial) => TrackedSerials.Contains(serial);
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
        public virtual void OnOwnerChangeRole(PlayerChangeRoleEvent ev)
        {

        }

        public virtual void OnOwnerDying(PlayerDyingEvent ev)
        {

        }

        public virtual void OnOwnerEscape(PlayerEscapeEvent ev)
        {

        }

        public virtual void OnOwnerHanducuffing(PlayerHandcuffEvent ev)
        {
            //its always be the target.
        }

        public virtual void OnDropped(PlayerDroppedItemEvent ev)
        {

        }

        public virtual void OnPickedup(PlayerSearchedPickupEvent ev)
        {
            ShowPickupMessage(ev.Player);
        }

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
            TrackedSerials.Clear();
        }

        /// <summary>
        /// Displays a pickup message to the specified player.
        /// </summary>
        /// <param name="player">The player to whom the pickup message will be displayed.</param>
        public virtual void ShowPickupMessage(Player player)
        {
            player.ReceiveHint($"You picked up {Name}\n{Description}", 3);
        }

        /// <summary>
        /// Displays a selection message to the specified player.
        /// </summary>
        /// <param name="player">The player to whom the selection message will be displayed.</param>
        public virtual void ShowSelectMessage(Player player)
        {
            player.ReceiveHint($"You selected {Name}\n{Description}", 3);
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
        private void OnInternalDropped(PlayerDroppedItemEvent ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.Item.NetworkInfo.WeightKg != Weight)
            {
                ev.Item.NetworkInfo = new(ev.Item.NetworkInfo.ItemId, Weight, ev.Item.NetworkInfo.Serial);
            }

            OnDropped(ev);
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
    }
}

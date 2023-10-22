using InventorySystem;
using InventorySystem.Items;
using MEC;
using NWAPI.CustomItems.API.Struct;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using PluginAPI.Loader;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using YamlDotNet.Serialization;

namespace NWAPI.CustomItems.API.Features
{
    /// <summary>
    /// P A I N
    /// </summary>
    public abstract class CustomItem
    {
        private static readonly List<CustomItemInfo?> LookupTable = new();

        public static HashSet<ushort> AllCustomItemsSerials = new();

        public static HashSet<CustomItem> Registered { get; } =  new();

        // API \\
        public abstract uint Id { get; set; }

        public abstract string Name { get; set; }

        public abstract string Description { get; set; }

        public abstract float Weight { get; set; }

        public abstract ItemType ModelType { get; set; }

        public virtual Vector3 Scale { get; set; } = Vector3.one;

        [YamlIgnore]
        public HashSet<ushort> TrackedSerials { get; } = new();


        // public methods \\

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

        public virtual void Init()
        {
            LookupTable.Add(new(Id, Name, GetType()));
            SubscribeEvents();
        }

        public virtual void Destroy()
        {
            UnsubscribeEvents();

            var customItemInfo = LookupTable.FirstOrDefault(i => i.HasValue && i.Value.Id == Id);

            if (customItemInfo.HasValue)
                LookupTable.Remove(customItemInfo);
        }

        protected virtual void SubscribeEvents()
        {
            // PluginAPI.Events.EventManager.RegisterEvents(YourPluginInstance, new YourCustomItem() OR this);
        }

        protected virtual void UnsubscribeEvents()
        {
            // PluginAPI.Events.EventManager.UnregisterEvents(YourPluginInstance, YourCustomItemInstance OR this);
        }

        public virtual void ShowPickupMessage(Player player)
        {
            player.ReceiveHint("", 3);
        }

        public virtual void ShowSelectMessage(Player player)
        {
            player.ReceiveHint("", 3);
        }

        // internal methods \\

        [PluginEvent]
        private void OnInternalOwnerChangingRole(PlayerChangeRoleEvent ev)
        {
            if (ev.ChangeReason == PlayerRoles.RoleChangeReason.Escaped)
                return;

            List<ItemBase> items = new(ev.Player.Items);
            foreach (var item in items)
            {
                if (false)
                    continue;

                OnOwnerChangeRole(ev);

                TrackedSerials.Remove(item.ItemSerial);

                // ev.Player.RemoveItem(item) its broken in NWAPI.
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);

                //Spawn(ev.Player, item, ev.Player);
            }
        }

        [PluginEvent]
        private void OnInternalOwnerDying(PlayerDyingEvent ev)
        {
            List<ItemBase> items = new(ev.Player.Items);
            Vector3 position = ev.Player.Position + Vector3.up;
            foreach (var item in items)
            {
                if (false)
                    continue;

                TrackedSerials.Remove(item.ItemSerial);

                // ev.Player.RemoveItem(item) its broken in NWAPI.
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);

                OnOwnerDying(ev);

                //Spawn(ev.Player, item, ev.Player);

            }
        }

        [PluginEvent]
        private void OnInternalOwnerEscaping(PlayerEscapeEvent ev)
        {
            List<ItemBase> items = new(ev.Player.Items);
            foreach (var item in items)
            {
                if (false)
                    continue;

                TrackedSerials.Remove(item.ItemSerial);

                // ev.Player.RemoveItem(item) its broken in NWAPI.
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);

                OnOwnerEscape(ev);

                Timing.CallDelayed(1.5f, () =>
                {
                    //Spawn(ev.Player.Position, item, null)
                });
            }
        }

        [PluginEvent]
        private void OnInternalOwnerHandcuffing(PlayerHandcuffEvent ev)
        {
            List<ItemBase> items = new(ev.Target.Items);
            foreach (var item in items)
            {
                if (false)
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
            // Check if the item is a customItem

            if (ev.Item.NetworkInfo.WeightKg != Weight)
            {
                ev.Item.NetworkInfo = new(ev.Item.NetworkInfo.ItemId, Weight, ev.Item.NetworkInfo.Serial);
            }

            OnDropped(ev);
        }

        [PluginEvent]
        private void OnInternalPickedup(PlayerSearchedPickupEvent ev)
        {
            // check if the item is a custom item

            OnPickedup(ev);
        }

        [PluginEvent]
        private void OnInternalChangingItem(PlayerChangeItemEvent ev)
        {
            // check if the item its a customiten

            OnChangeItem(ev);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Name} - {ModelType} | {Id} | {Description}";
    }
}

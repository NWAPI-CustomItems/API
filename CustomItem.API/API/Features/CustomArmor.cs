using InventorySystem;
using InventorySystem.Items.Armor;
using MEC;
using NWAPI.CustomItems.API.Extensions;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace NWAPI.CustomItems.API.Features
{
    /// <summary>
    /// An abstract class representing custom armor items that can be added to a player's inventory.
    /// </summary>
    public abstract class CustomArmor : CustomItem
    {
        /// <summary>
        /// Gets or sets the multiplier for stamina usage when wearing this armor.
        /// Valid range is between 1.0 and 2.0.
        /// </summary>
        public virtual float StaminaUseMultiplier { get; set; } = 2.0f;

        /// <summary>
        /// Gets or sets the efficacy of the helmet when reducing damage.
        /// Valid range is between 1 and 100.
        /// </summary>
        public virtual int HelmetEfficacy { get; set; } = 100;

        /// <summary>
        /// Gets or sets the efficacy of the vest when reducing damage.
        /// Valid range is between 1 and 100.
        /// </summary>
        public virtual int VestEfficacy { get; set; } = 100;

        /// <summary>
        /// Gets or sets the multiplier for stamina regeneration when wearing this armor.
        /// </summary>
        public virtual float StaminaRegenMultiplier { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether to retain excess items when the armor is dropped.
        /// </summary>
        public virtual bool DontRemoveExcessOnDrop { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether sprinting is disabled when wearing this armor.
        /// </summary>
        public virtual bool SprintDisable { get; set; } = false;

        /// <summary>
        /// Gives the specified <see cref="Player"/> this custom armor item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the armor to.</param>
        /// <param name="displayMessage">Indicates whether the pickup message should be displayed.</param>
        public override void Give(Player player, bool displayMessage = true)
        {
            if (!ModelType.IsArmor())
            {
                Log.Warning($"{nameof(Give)}: {Name} -- {Id} | {ModelType} is not armor");
                return;
            }

            if (player.AddItem(ModelType) is BodyArmor armor)
            {
                SetArmorStats(armor);
            }

            if (displayMessage)
                Timing.CallDelayed(.2f, () => ShowPickupMessage(player));
        }

        /// <summary>
        /// Sets the stats for the specified <see cref="BodyArmor"/>.
        /// </summary>
        /// <param name="armor">The <see cref="BodyArmor"/> to set the stats for.</param>
        public virtual void SetArmorStats(BodyArmor armor)
        {
            armor._weight = Weight;
            armor._staminaUseMultiplier = StaminaUseMultiplier;
            armor.VestEfficacy = VestEfficacy;
            armor.HelmetEfficacy = HelmetEfficacy;

            if (!TrackedSerials.Contains(armor.ItemSerial))
                TrackedSerials.Add(armor.ItemSerial);
        }

        /// <summary>
        /// An internal event handler for when a player searches for a pickup item.
        /// </summary>
        /// <param name="ev">The <see cref="PlayerSearchedPickupEvent"/> associated with the search action.</param>
        [PluginEvent]
        private void OnInternalSearched(PlayerSearchedPickupEvent ev)
        {
            if (!Check(ev.Item))
                return;

            Timing.CallDelayed(0.4f, () =>
            {
                TrackedSerials.Remove(ev.Item.Info.Serial);
                ev.Player.ReferenceHub.inventory.ServerRemoveItem(ev.Item.Info.Serial, null);
                Give(ev.Player);
            });
        }
    }
}
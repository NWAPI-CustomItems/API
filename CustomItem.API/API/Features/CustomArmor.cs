using InventorySystem.Items.Armor;
using MEC;
using NWAPI.CustomItems.API.Extensions;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System.ComponentModel;
using System.Linq;

namespace NWAPI.CustomItems.API.Features
{
    /// <summary>
    /// An abstract class representing custom armor items that can be added to a player's inventory.
    /// </summary>
    public abstract class CustomArmor : CustomItem
    {
        /// <summary>
        /// Gets or sets the multiplier for stamina usage when wearing this armor.
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
        [Description("This is currently disabled due to a desync that is caused on the client and the server and can cause the player who has the armor to see the anticheat pull it back, the other players will see it as not running.")]
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
        /// Called when a player picks up armor. 
        /// Applies changes to the armor's statistics for the picked-up items if they are valid.
        /// </summary>
        /// <param name="ev">The event containing information about the player and the picked-up armor item.</param>
        public virtual void OnArmorPickedup(PlayerPickupArmorEvent ev)
        {
            ShowPickupMessage(ev.Player);

            Timing.CallDelayed(0.4f, () =>
            {
                foreach (var item in ev.Player.Items.ToList())
                {
                    if (Check(item) && item is BodyArmor armor)
                    {
                        SetArmorStats(armor);
                    }
                }
            });
        }

        [PluginEvent]
        private void OnInternalPickupArmor(PlayerPickupArmorEvent ev)
        {
            if (!Check(ev.Item))
                return;

            OnArmorPickedup(ev);
        }
    }
}
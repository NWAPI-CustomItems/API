using System.ComponentModel;

namespace NWAPI.CustomItems
{
    /// <summary>
    /// Represents the configuration settings for the CustomItems API plugin.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is in debug mode.
        /// </summary>
        [Description("Set to 'true' to enable logs debug in the code.")]
        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the CustomItems API is enabled.
        /// </summary>
        [Description("Set to 'true' to enable the CustomItems API.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the hint message to display when a player picks up a custom item.
        /// Use placeholders {0} and {1} for item name and description.
        /// </summary>
        [Description("Hint message displayed when a player picks up a custom item. Use placeholders {0} and {1} for item name and description.")]
        public HintMessage PickupMessage { get; set; } = new HintMessage("You have picked up a {0}\n{1}", 3f);

        /// <summary>
        /// Gets or sets the hint message to display when a player selects a custom item.
        /// Use placeholders {0} and {1} for item name and description.
        /// </summary>
        [Description("Hint message displayed when a player selects a custom item. Use placeholders {0} and {1} for item name and description.")]
        public HintMessage SelectMessage { get; set; } = new HintMessage("You have selected a {0}\n{1}", 5f);
    }


    /// <summary>
    /// Represents a hint message with a specific text and duration.
    /// </summary>
    public class HintMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HintMessage"/> class.
        /// </summary>
        public HintMessage() : this(string.Empty) 
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HintMessage"/> class.
        /// </summary>
        /// <param name="message">The text of the hint message.</param>
        /// <param name="duration">The duration in seconds for which the hint message will be displayed.</param>
        public HintMessage(string message, float duration = 5)
        {
            Message = message;
            Duration = duration;
        }

        /// <summary>
        /// Gets or sets the text of the hint message.
        /// </summary>
        [Description("The text of the hint message.")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the duration in seconds for which the hint message will be displayed.
        /// </summary>
        [Description("The duration in seconds for which the hint message will be displayed.")]
        public float Duration { get; set; }

        public override string ToString() => $"message \"{Message}\" with duration of {Duration}";
        
    }

}

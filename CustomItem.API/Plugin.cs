using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace NWAPI.CustomItems
{
    public class Plugin
    {
        public static Plugin Instance { get; private set; } = null!;

        /// <summary>
        /// Plugin config.
        /// </summary>
        [PluginConfig]
        public Config Config = null!;

        public const string PluginVersion = "0.0.1";

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("CustomItem.API", PluginVersion, "Ye", "SrLicht & Imurx")]
        private void LoadPlugin()
        {
            Instance = this;
        }

        [PluginUnload]
        private void UnLoadPlugin()
        {

        }
    }
}

using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System.Linq;

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

        /// <summary>
        /// Gets the api version.
        /// </summary>
        public const string PluginVersion = "0.0.1";

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("NWAPI.CustomItems.API", PluginVersion, "Ye", "SrLicht & Imurx")]
        private void LoadPlugin()
        {
            Instance = this;

            if (!Config.IsEnabled)
            {
                Log.Warning("NWAPI.CustomItems.API has been disabled due to configuration settings.");

                var handler = PluginAPI.Loader.AssemblyLoader.InstalledPlugins.FirstOrDefault(p => p.GetType() == this.GetType());
                handler?.Unload();
                return;
            }

            PluginAPI.Events.EventManager.RegisterEvents(Instance, new Handlers.MapHandler());
        }

        [PluginUnload]
        private void UnLoadPlugin()
        {
            PluginAPI.Events.EventManager.UnregisterAllEvents(Instance);
        }
    }
}

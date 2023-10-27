using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System.Linq;

namespace NWAPI.CustomItems
{
    /// <summary>
    /// No me rompas las pelotas con la documentacion la concha de la lora Visual
    /// </summary>
    public class Plugin
    {
        /// <summary>
        /// Gets the singleton instance of the plugin.
        /// </summary>
        public static Plugin Instance { get; private set; } = null!;

        /// <summary>
        /// Gets the Harmony instance used for patching and unpatching.
        /// </summary>
        public static Harmony Harmony { get; private set; } = null!;

        /// <summary>
        /// Plugin config.
        /// </summary>
        [PluginConfig]
        public Config Config = null!;

        /// <summary>
        /// Gets the api version.
        /// </summary>
        public const string Version = "0.0.1";

        /// <summary>
        /// Harmony id used to track assembly patchs.
        /// </summary>
        private static string HarmonyId = "";

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("NWAPI.CustomItems.API", Version, "Ye", "SrLicht & Imurx")]
        private void LoadPlugin()
        {
            Instance = this;
            HarmonyId = $"NWAPI.CustomItems.API.{Version}";
            Harmony = new(HarmonyId);

            if (!Config.IsEnabled)
            {
                Log.Warning("NWAPI.CustomItems.API has been disabled due to configuration settings.");

                var handler = PluginAPI.Loader.AssemblyLoader.InstalledPlugins.FirstOrDefault(p => p.GetType() == this.GetType());
                handler?.Unload();
                return;
            }

            try
            {
                Harmony.PatchAll();
            }
            catch (HarmonyException e)
            {
                Log.Error($"Error on patching: {e}");
            }
            
            PluginAPI.Events.EventManager.RegisterEvents(Instance, new Handlers.MapHandler());
        }

        [PluginUnload]
        private void UnLoadPlugin()
        {
            // Prevents unpatching all patches of the plugins by using HarmonyId.
            Harmony.UnpatchAll(HarmonyId);
            PluginAPI.Events.EventManager.UnregisterAllEvents(Instance);
        }
    }
}

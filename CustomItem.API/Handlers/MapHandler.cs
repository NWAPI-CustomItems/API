using MEC;
using NWAPI.CustomItems.API.Features;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace NWAPI.CustomItems.Handlers
{
    /// <summary>
    /// Handler of map related events.
    /// </summary>
    public class MapHandler
    {
        /// <summary>
        /// On MapGeneration spawn all customitems.
        /// </summary>
        /// <param name="_"></param>
        [PluginEvent]
        public void OnMapGenerated(MapGeneratedEvent _)
        {
            MapLocker.CacheLockers();

            Timing.CallDelayed(1, () =>
            {
                foreach (var customItem in CustomItem.Registered)
                    customItem?.SpawnAll();
            });

        }
    }
}

using NWAPI.CustomItems.API.Features;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace NWAPI.CustomItems.Handlers
{
    public class MapHandler
    {
        [PluginEvent]
        public void OnMapGenerated(MapGeneratedEvent _)
        {
            foreach (var customItem in CustomItem.Registered)
                customItem?.SpawnAll();
        }
    }
}

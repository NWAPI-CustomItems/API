using NWAPI.CustomItems.API.Features;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

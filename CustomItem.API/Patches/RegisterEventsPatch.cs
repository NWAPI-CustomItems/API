using HarmonyLib;
using InventorySystem;
using NWAPI.CustomItems.API.Features.Attributes;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NWAPI.CustomItems.Patches
{
    /// <summary>
    /// A harmony patch to search events in all bases class if the class have [CustomItem] attritube.
    /// </summary>
    [HarmonyPatch(typeof(EventManager))]
    public static class RegisterEventsPatch
    {
        /// <summary>
        /// Gets the private method "RegisterEvents(object plugin, object eventHandler)" to patch.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod(MethodBase original)
        {
            return typeof(EventManager).GetMethods().Where(m => m.Name == "RegisterEvents" && m.IsStatic && m.GetParameters().Count() is 2).FirstOrDefault();
        }

        private static bool Prefix(object plugin, object eventHandler)
        {
            if (eventHandler.GetType().IsDefined(typeof(CustomItemAttribute), true))
            {
                Type currentType = eventHandler.GetType();
                Type pluginType = plugin.GetType();

                while (currentType != null)
                {
                    foreach (var method in currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        foreach (var attribute in method.GetCustomAttributes<Attribute>())
                        {
                            switch (attribute)
                            {
                                case PluginEvent pluginEvent:
                                    var eventParameters = method.GetParameters().Select(p => p.ParameterType.IsByRef ? p.ParameterType.GetElementType() : p.ParameterType).ToArray();

                                    var targetType = pluginEvent.EventType;

                                    if (eventParameters.Length != 0 && targetType == ServerEventType.None)
                                        EventManager.TypeToEvent.TryGetValue(eventParameters[0], out targetType);

                                    if (!EventManager.Events.TryGetValue(targetType, out Event ev))
                                    {
                                        Log.Error($"Event &6{targetType}&r is not registered in manager method {method.Name}! ( create issue on github )");
                                        continue;
                                    }

                                    bool isDefaultMethod = true;
                                    if (!ValidateEvent(ev, eventParameters, ref isDefaultMethod))
                                    {
                                        Log.Error($"Event &6{method.Name}&r (&6{targetType}&r) in plugin &6{pluginType.FullName}&r contains wrong parameters\n - &6{(string.Join(", ", eventParameters.Select(p => p.Name)))}\n - Required:\n - &6{(string.Join(", ", ev.Parameters.Select(p => p.BaseType.Name)))}.");
                                        continue;
                                    }

                                    ev.RegisterInvoker(pluginType, eventHandler, method, isDefaultMethod);

                                    Log.Debug($"Registered event &6{method.Name}&r (&6{targetType}&r) in plugin &6{pluginType.FullName}&r!", Log.DebugMode);
                                    break;
                            }
                        }
                    }

                    currentType = currentType.BaseType;
                }

                return false;
            }


            return true;
        }

        private static bool ValidateEvent(Event ev, Type[] parameters, ref bool isDefaultMethod)
        {
            var requiredParameters = ev.Parameters.Select(x => x.BaseType).ToArray();

            if (parameters.Length == 1 && parameters[0] == ev.EventArgType)
            {
                isDefaultMethod = false;
                return true;
            }

            if (parameters.Length != requiredParameters.Length)
                return false;

            for (int x = 0; x < requiredParameters.Length; x++)
            {
                if (requiredParameters[x].IsInterface)
                {
                    if (!requiredParameters[x].IsAssignableFrom(parameters[x]))
                        return false;
                }
                else if (requiredParameters[x] != parameters[x])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

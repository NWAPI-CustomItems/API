using CommandSystem;
using NWAPI.CustomItems.API.Features;
using System;
using System.Linq;

namespace NWAPI.CustomItems.Commands.SubCommands
{
    internal sealed class List : ICommand
    {
        public string Command { get; } = "list";

        public string[] Aliases { get; } = { "l" };

        public string Description { get; } = "Gets a list of all currently registered custom items.";

        public static List Instance { get; } = new();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (CustomItem.Registered.IsEmpty())
            {
                response = "There are no custom items registered on this server.";
                return false;
            }

            response = "\n[Registered custom items (" + CustomItem.Registered.Count + ")]\n";

            foreach (var customItem in CustomItem.Registered.OrderBy(item => item.Id))
            {
                response += $"[ {customItem.Id,-5} | {customItem.Name} ({customItem.ModelType}) ]\n";
            }

            return true;
        }
    }
}

using CommandSystem;
using NWAPI.CustomItems.API.Features;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Linq;

namespace NWAPI.CustomItems.Commands.SubCommands
{
    internal sealed class Give : ICommand, IUsageProvider
    {
        public string Command { get; } = "give";

        public string[] Aliases { get; } = { "g" };

        public string Description { get; } = "Gives a custom item";

        public string[] Usage { get; } = new string[]
        {
            "custom item id/custom item name",
            "PlayerId/All/*"
        };

        public static Give Instance { get; } = new();

        /// <summary>
        /// Determines whether a player can receive a item.
        /// </summary>
        /// <param name="player">The player to check for eligibility.</param>
        private bool CanReceiveItem(Player player) => player.IsAlive && !player.IsInventoryFull && !player.IsDisarmed;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems))
            {
                response = $" Permission Denied. Required permission is {PlayerPermissions.GivingItems}";
                return false;
            }

            if (arguments.IsEmpty())
            {
                response = $" To execute this command, you need at least 2 arguments.\nUsage: {arguments.Array[0]} {arguments.Array[1]} {this.DisplayCommandUsage()}";
                return false;
            }

            if (!CustomItem.TryGet(arguments.At(0), out CustomItem? customItem))
            {
                response = $" {arguments.At(0)} is not a valid custom item";
                return false;
            }

            if (arguments.Count is 1)
            {
                if (sender is PlayerCommandSender)
                {
                    var player = Player.Get(sender);

                    if (!CanReceiveItem(player))
                    {
                        response = " Its not possible give you a custom item";
                        return false;
                    }

                    customItem?.Give(player);
                    response = $" Giving {customItem?.Name} to {player.LogName}";
                    return true;
                }

                response = " Error getting the player. Please follow the command syntax.";
                return false;
            }

            switch (arguments.At(1).ToLowerInvariant())
            {
                case "*":
                case "all":
                    {
                        var players = Player.GetPlayers().Where(CanReceiveItem).ToList();

                        foreach (var player in players)
                            customItem?.Give(player);

                        response = $" Custom item {customItem?.Name} given to all players who can receive them ({players.Count} players).";
                        return true;
                    }
                default:
                    {

                        if (!int.TryParse(arguments.At(1), out int playerId))
                        {
                            response = $" {arguments.At(1)} is not a valid PlayerId.";
                            return false;
                        }
                        if (!Player.TryGet(playerId, out var player))
                        {
                            response = $" Unable to find player: {arguments.At(1)}.";
                            return false;
                        }

                        if (!CanReceiveItem(player))
                        {
                            response = $" {player.LogName} cannot receive a custom item.";
                            return false;
                        }
                        customItem?.Give(player);
                        response = $" Giving {customItem?.Name} to {player.LogName}.";
                        return true;
                    }
            }
        }
    }
}

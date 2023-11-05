using CommandSystem;
using NWAPI.CustomItems.API.Enums;
using NWAPI.CustomItems.API.Extensions;
using NWAPI.CustomItems.API.Features;
using PluginAPI.Core;
using System;
using UnityEngine;

namespace NWAPI.CustomItems.Commands.SubCommands
{
    internal sealed class Spawn : ICommand, IUsageProvider
    {
        public string Command { get; } = "spawn";

        public string[] Aliases { get; } = { "s" };

        public string Description { get; } = "";

        public string[] Usage { get; } = new string[]
        {
            "Custom item id/custom item name",
            "SpawnLocationType/PlayerId/Vector3"
        };

        public static Spawn Instance { get; } = new();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems))
            {
                response = $" Permission Denied. Required permission is {PlayerPermissions.GivingItems}";
                return false;
            }

            if (arguments.Count < 2 || arguments.At(0).ToLowerInvariant() == "help")
            {
                response = $" To execute this command, you need at least 2 arguments.\nUsage: {arguments.Array[0]} {arguments.Array[1]} {this.DisplayCommandUsage()}";
                return false;
            }

            if (!CustomItem.TryGet(arguments.At(0), out CustomItem? customItem))
            {
                response = $" {arguments.At(0)} is not a valid custom item";
                return false;
            }

            Vector3 spawnPosition = GetSpawnPosition(arguments);

            if (spawnPosition == Vector3.zero)
            {
                response = " Unable to determine a valid spawn position.";
                return false;
            }

            customItem?.Spawn(spawnPosition);

            response = $" {customItem?.Name} ({customItem?.ModelType}) has been spawned at {spawnPosition}.";

            return true;
        }

        private Vector3 GetSpawnPosition(ArraySegment<string> arguments)
        {
            if (Enum.TryParse(arguments.At(1), out SpawnLocationType spawnLocationType))
            {
                return spawnLocationType.GetPosition();
            }

            if (int.TryParse(arguments.At(1), out int playerId) && Player.TryGet(playerId, out Player player))
            {
                if (!player.IsAlive)
                {
                    return Vector3.zero;
                }

                return player.Position;
            }

            if (arguments.Count > 3 && Vector3Extensions.TryParse($"{arguments.At(1)} {arguments.At(2)} {arguments.At(3)}", out Vector3 position))
            {
                return position;
            }

            return Vector3.zero;
        }
    }

}

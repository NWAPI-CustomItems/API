﻿using CommandSystem;
using NWAPI.CustomItems.API.Features;
using NWAPI.CustomItems.API.Spawn;
using System;

namespace NWAPI.CustomItems.Commands.SubCommands
{
    internal sealed class Info : ICommand, IUsageProvider
    {
        /// <inheritdoc/>
        public bool SanitizeResponse => true;
        public string Command { get; } = "info";

        public string[] Aliases { get; } = { "i" };

        public string Description { get; } = "Gets all information about the specified custom item";

        public string[] Usage { get; } = new string[]
        {
            "Custom item id/custom item name"
        };

        public static Info Instance { get; } = new();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.IsEmpty())
            {
                response = $" To execute this command, you need at least 1 arguments.\nUsage: {arguments.Array[0]} {arguments.Array[1]} {this.DisplayCommandUsage()}";
                return false;
            }

            if (!CustomItem.TryGet(arguments.At(0), out CustomItem? customItem))
            {
                response = $" {arguments.At(0)} is not a valid custom item";
                return false;
            }

            response =
                $"\nName: {customItem?.Name}\n" +
                $"Description: {customItem?.Description}\n" +
                $"Id: {customItem?.Id}\n" +
                $"ModelType: {customItem?.ModelType}\n" +
                $"SpawnLocations: \n{GetSpawnLocations(customItem)}";

            return true;
        }

        private string GetSpawnLocations(CustomItem? item)
        {
            if (item is null || item?.SpawnProperties is null)
                return string.Empty;

            string response = string.Empty;
            response += "Spawn Locations (" + (item.SpawnProperties.DynamicSpawnPoints.Count + item.SpawnProperties.StaticSpawnPoints.Count) + ")\n";

            foreach (DynamicSpawnPoint spawnPoint in item.SpawnProperties.DynamicSpawnPoints)
            {
                response += spawnPoint.Name + ' ' + spawnPoint.Position + " Chance: " + spawnPoint.Chance + "%\n";
            }

            foreach (StaticSpawnPoint spawnPoint in item.SpawnProperties.StaticSpawnPoints)
            {
                response += spawnPoint.Name + ' ' + spawnPoint.Position + " Chance: " + spawnPoint.Chance + "%\n";
            }

            return response;
        }

    }
}

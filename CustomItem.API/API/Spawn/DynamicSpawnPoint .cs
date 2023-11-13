using NWAPI.CustomItems.API.Enums;
using NWAPI.CustomItems.API.Extensions;
using System;
using UnityEngine;
using YamlDotNet.Serialization;

// -----------------------------------------------------------------------
// <copyright file="DynamicSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace NWAPI.CustomItems.API.Spawn
{
    /// <summary>
    /// Handles dynamic spawn locations.
    /// </summary>
    public class DynamicSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the <see cref="SpawnLocationType"/> for this item.
        /// </summary>
        public SpawnLocationType Location { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override string Name
        {
            get => Location.ToString();
            set => throw new InvalidOperationException("The name of a dynamic spawn location cannot be changed.");
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get => Location.GetPosition();
            set => throw new InvalidOperationException("The spawn vector of a dynamic spawn location cannot be changed.");
        }

        /// <inheritdoc/>
        public override Vector3 Offset { get; set; } = Vector3.zero;
    }
}

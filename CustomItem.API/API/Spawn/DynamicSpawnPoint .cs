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
    public class DynamicSpawnPoint : SpawnPoint
    {
        public SpawnLocationType Location { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get => Location.GetPosition();
            set => throw new InvalidOperationException("The spawn vector of a dynamic spawn location cannot be changed.");
        }

        /// <inheritdoc/>
        public override Vector3? Offset { get; set; }
    }
}

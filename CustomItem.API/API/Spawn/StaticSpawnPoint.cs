using UnityEngine;
// -----------------------------------------------------------------------
// <copyright file="StaticSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace NWAPI.CustomItems.API.Spawn
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticSpawnPoint : SpawnPoint
    {
        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        public override Vector3 Position { get; set; }

        /// <inheritdoc/>
        public override Vector3? Offset { get; set; }
    }
}

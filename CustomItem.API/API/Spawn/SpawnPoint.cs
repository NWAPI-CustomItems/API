using UnityEngine;

// -----------------------------------------------------------------------
// <copyright file="SpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace NWAPI.CustomItems.API.Spawn
{
    /// <summary>
    /// An abstract class representing a spawn point.
    /// </summary>
    public abstract class SpawnPoint
    {

        /// <summary>
        /// Gets or sets this spawn point name.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the probability that this spawn point will be selected.
        /// </summary>
        public abstract float Chance { get; set; }

        /// <summary>
        /// Gets or sets the position at which an item will be spawned.
        /// </summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets an optional offset from the spawn point's position.
        /// </summary>
        public abstract Vector3 Offset { get; set; }

        /// <summary>
        /// Deconstructs the class into usable variables.
        /// </summary>
        /// <param name="chance"><inheritdoc cref="Chance"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        public void Deconstruct(out float chance, out Vector3 position)
        {
            chance = Chance;
            position = Position;
        }
    }
}

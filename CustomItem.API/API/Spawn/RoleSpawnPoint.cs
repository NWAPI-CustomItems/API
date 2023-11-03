using NWAPI.CustomItems.API.Extensions;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

// -----------------------------------------------------------------------
// <copyright file="RoleSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace NWAPI.CustomItems.API.Spawn
{
    /// <summary>
    /// Represents a spawn point associated with a specific <see cref="RoleTypeId"/>.
    /// </summary>
    public class RoleSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> used to determine the spawn position.
        /// </summary>
        public RoleTypeId RoleType { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position {
            get => RoleType.GetRandomSpawnLocation();
            set => throw new InvalidOperationException("The position of a RoleSpawnPoint cannot be changed.");
        }

        /// <inheritdoc/>
        public override Vector3? Offset { get; set; }
    }
}

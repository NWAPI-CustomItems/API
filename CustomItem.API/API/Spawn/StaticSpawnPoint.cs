﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
// -----------------------------------------------------------------------
// <copyright file="StaticSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace NWAPI.CustomItems.API.Spawn
{
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
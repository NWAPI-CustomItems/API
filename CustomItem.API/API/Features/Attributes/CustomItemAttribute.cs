using System;

namespace NWAPI.CustomItems.API.Features.Attributes;

/// <summary>
/// An attribute to easily manage CustomItem initialization.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CustomItemAttribute : Attribute { }
using System;

namespace NWAPI.CustomItems.API.Features.Attributes
{
    /// <summary>
    /// An attribute to easily manage CustomItem initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CustomItemConfigAttribute : Attribute { }
}

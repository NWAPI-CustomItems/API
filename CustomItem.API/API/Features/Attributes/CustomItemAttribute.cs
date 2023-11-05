using System;

namespace NWAPI.CustomItems.API.Features.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CustomItemAttribute : Attribute { }
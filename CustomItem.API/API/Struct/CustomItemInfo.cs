using NWAPI.CustomItems.API.Features;
using System;

namespace NWAPI.CustomItems.API.Struct
{
    /// <summary>
    /// Represents information about a custom item.
    /// </summary>
    public struct CustomItemInfo
    {
        /// <summary>
        /// Gets the unique identifier of the custom item.
        /// </summary>
        public uint Id { get; private set; }

        /// <summary>
        /// Gets the name of the custom item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the Type associated with the custom item.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the instance of the custom item.
        /// </summary>
        public CustomItem CustomItem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CustomItemInfo struct with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the custom item.</param>
        /// <param name="name">The name of the custom item.</param>
        /// <param name="type">The .NET Type associated with the custom item.</param>
        /// <param name="item">The instance of the custom item.</param>
        public CustomItemInfo(uint id, string name, Type type, CustomItem item)
        {
            Id = id;
            Name = name;
            Type = type;
            CustomItem = item;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is CustomItemInfo other)
            {
                return Id == other.Id && Name == other.Name && Type == other.Type;
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (Id, Name, Type).GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Id} -- {Name} -- {Type.FullName}";
    }
}

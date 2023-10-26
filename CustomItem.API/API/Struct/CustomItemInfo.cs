using NWAPI.CustomItems.API.Features;
using System;

namespace NWAPI.CustomItems.API.Struct
{
    public struct CustomItemInfo
    {
        public uint Id { get; private set; }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public CustomItem CustomItem { get; private set; }

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

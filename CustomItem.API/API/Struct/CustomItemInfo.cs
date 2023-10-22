using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWAPI.CustomItems.API.Struct
{
    public struct CustomItemInfo
    {

        public uint Id { get; private set; }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public CustomItemInfo(uint id, string name, Type type)
        {
            Id = id;
            Name = name;
            Type = type;
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

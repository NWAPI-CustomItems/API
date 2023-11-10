using NWAPI.CustomItems.API.Extensions;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace NWAPI.CustomItems.API.Configs.Tools
{
#nullable disable
    /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention"/>
    public class UnderscoredNamingConvention : INamingConvention
    {
        /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance"/>
        public static UnderscoredNamingConvention Instance { get; } = new();

        /// <summary>
        /// Gets the list.
        /// </summary>
        public List<object> Properties { get; } = new();

        /// <inheritdoc/>
        public string Apply(string value)
        {
            string newValue = value.ToSnakeCase();
            Properties.Add(newValue);
            return newValue;
        }
    }
}

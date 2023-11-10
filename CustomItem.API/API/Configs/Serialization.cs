using NWAPI.CustomItems.API.Configs.Converters;
using NWAPI.CustomItems.API.Configs.Tools;
using System.ComponentModel;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;
using UnderscoredNamingConvention = NWAPI.CustomItems.API.Configs.Tools.UnderscoredNamingConvention;

// <copyright file="Serializer.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Serializer.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 1:45 PM
//    Created Date:     09/11/2023 1:45 PM
// -----------------------------------------
namespace NWAPI.CustomItems.API.Configs
{
    /// <summary>
    /// A utility class for serialization and deserialization of configs.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Gets or sets the serializer for configs and translations.
        /// </summary>
        public static ISerializer Serializer { get; set; } = new SerializerBuilder()
          .WithTypeConverter(new VectorsConverter())
          .WithTypeConverter(new ColorConverter())
          .WithEventEmitter(eventEmitter => new TypeAssigningEventEmitter(eventEmitter))
          .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
          .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
          .WithNamingConvention(UnderscoredNamingConvention.Instance)
          .IgnoreFields()
          .DisableAliases()
          .Build();

        /// <summary>
        /// Gets or sets the deserializer for configs and translations.
        /// </summary>
        public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
          .WithTypeConverter(new VectorsConverter())
          .WithTypeConverter(new ColorConverter())
          .WithNamingConvention(UnderscoredNamingConvention.Instance)
          .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
          .IgnoreFields()
          .IgnoreUnmatchedProperties()
          .Build();

        /// <summary>
        /// Gets or sets the quotes wrapper type.
        /// </summary>
        [Description("Indicates in which quoted strings in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
        public static ScalarStyle ScalarStyle { get; set; } = ScalarStyle.SingleQuoted;
        /// <summary>
        /// Gets or sets the quotes wrapper type.
        /// </summary>
        [Description("Indicates in which quoted strings with multiline in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
        public static ScalarStyle MultiLineScalarStyle { get; set; } = ScalarStyle.Literal;
    }
}

﻿// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         CommentPropertyDescriptor.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 2:12 PM
//    Created Date:     09/11/2023 2:12 PM
// -----------------------------------------
using System;
using System.ComponentModel;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace NWAPI.CustomItems.API.Configs.Tools
{
#nullable disable
    /// <summary>
    /// Source: https://dotnetfiddle.net/8M6iIE.
    /// </summary>
    public sealed class CommentsPropertyDescriptor : IPropertyDescriptor
    {
        private readonly IPropertyDescriptor baseDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="baseDescriptor">The base descriptor instance.</param>
        public CommentsPropertyDescriptor(IPropertyDescriptor baseDescriptor)
        {
            this.baseDescriptor = baseDescriptor;
            Name = baseDescriptor.Name;
        }

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public string Name { get; set; }

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public Type Type => baseDescriptor.Type;

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public Type TypeOverride
        {
            get => baseDescriptor.TypeOverride;
            set => baseDescriptor.TypeOverride = value;
        }

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public int Order { get; set; }

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public ScalarStyle ScalarStyle
        {
            get => baseDescriptor.ScalarStyle;
            set => baseDescriptor.ScalarStyle = value;
        }

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public bool CanWrite => baseDescriptor.CanWrite;

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public void Write(object target, object value)
        {
            baseDescriptor.Write(target, value);
        }

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public T GetCustomAttribute<T>()
            where T : Attribute => baseDescriptor.GetCustomAttribute<T>();

        /// <inheritdoc cref="IPropertyDescriptor"/>
        public IObjectDescriptor Read(object target)
        {
            DescriptionAttribute description = baseDescriptor.GetCustomAttribute<DescriptionAttribute>();
            return
                description is not null
                ? new CommentsObjectDescriptor(baseDescriptor.Read(target), description.Description)
                : baseDescriptor.Read(target);
        }
    }
}

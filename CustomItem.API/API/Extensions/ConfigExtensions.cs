﻿using NWAPI.CustomItems.API.Features.Attributes;
using PluginAPI.Core;
using PluginAPI.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;

namespace NWAPI.CustomItems.API.Extensions
{
    /// <summary>
    /// Helper to load customitem config.
    /// </summary>
    public static class ConfigExtensions
    {
        /// <summary>
        /// Loads or generates custom item configs.
        /// </summary>
        /// <param name="assembly">The assembly to load the config from.</param>
        /// <returns>The loaded or generated config object, or null if there's an error.</returns>
        public static object? LoadCustomItemConfig(this Assembly assembly)
        {
            // Disable nullable warnings for the sake of this.
#nullable disable

            Type entryPointType = null!;
            object entryPointInstance = null;

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsValidEntrypoint())
                    continue;

                entryPointType = type;
                entryPointInstance = Activator.CreateInstance(entryPointType);
                break;
            }

            if (entryPointType is null || entryPointInstance is null)
                return null;

            foreach (var field in entryPointType.GetFields())
            {
                if (field.GetCustomAttribute<CustomItemConfigAttribute>() is null)
                    continue;

                var pluginHandler = PluginAPI.Loader.AssemblyLoader.Plugins[assembly]?.FirstOrDefault().Value;

                if (pluginHandler is null)
                    return null;

                string targetPath = Path.Combine(pluginHandler.PluginDirectoryPath, "CustomItems.yml");

                if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                if (!File.Exists(targetPath))
                {
                    try
                    {
                        var defaultConfig = Activator.CreateInstance(field.FieldType);
                        File.WriteAllText(targetPath, API.Configs.Serialization.Serializer.Serialize(defaultConfig));
                        return defaultConfig;
                    }
                    catch (YamlException e)
                    {
                        Log.Error($"{nameof(LoadCustomItemConfig)} error on trying to generate default config: {e.Message}");
                        return null;
                    }
                }
                else
                {
                    object config;

                    try
                    {
                        var rawconfig = ReadCustomConfig(targetPath);

                        if (string.IsNullOrEmpty(rawconfig))
                            throw new Exception("rawconfig is empty");

                        config = API.Configs.Serialization.Deserializer.Deserialize(rawconfig, field.FieldType);
                    }
                    catch (YamlException ex)
                    {
                        Log.Error($"Failed deserializing config file for &2{assembly.GetName().Name}.&r\n{ex.Message} | {ex.TargetSite}");
                        return null;
                    }

                    try
                    {
                        field.SetValue(entryPointInstance, config);
                        File.WriteAllText(targetPath, API.Configs.Serialization.Serializer.Serialize(config));
                        return config;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{nameof(LoadCustomItemConfig)} error on trying to load config: {e.Message} | {e.GetType()}");
                        return null;
                    }
                }
            }

            // Re-enable nullable warnings
#nullable enable
            return null;
        }

        /// <summary>
        /// Reads all custom item config file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadCustomConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Error($"{nameof(ReadCustomConfig)}: Path to config is empty or null");
                return string.Empty;
            }

            try
            {
                if (File.Exists(path))
                    return File.ReadAllText(path);
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while reading configs from {path} path: {exception}");
            }
            return string.Empty;
        }
    }
}

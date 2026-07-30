// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DeckSurf.SDK.Interfaces;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Core
{
    /// <summary>
    /// Discovers and loads DeckSurf plugins from disk. Plugin assemblies must follow the
    /// <c>DeckSurf.Plugin.*.dll</c> naming convention and are discovered either in a
    /// <c>plugins</c> subdirectory (scanned recursively) or directly next to the host
    /// executable (top level only).
    /// </summary>
    public static partial class PluginLoader
    {
        /// <summary>
        /// Discovers and instantiates all plugins under the given base directory.
        /// </summary>
        /// <param name="baseDirectory">
        /// Directory to scan. Both <c>{baseDirectory}\plugins</c> (recursive) and
        /// <paramref name="baseDirectory"/> itself (top level, or fully recursive when
        /// <paramref name="scanBaseRecursively"/> is set) are searched; duplicate
        /// assembly file names are loaded only once, preferring the <c>plugins</c> copy.
        /// </param>
        /// <param name="onWarning">Optional callback invoked with a human-readable message for every non-fatal load failure.</param>
        /// <param name="scanBaseRecursively">
        /// When <c>true</c>, the base directory is scanned recursively - appropriate for
        /// user-configured plugin folders that contain one subdirectory per plugin.
        /// </param>
        /// <returns>The instantiated plugins. Empty when no plugin assemblies are found.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="baseDirectory"/> is null or whitespace.</exception>
        public static IReadOnlyList<IDeckSurfPlugin> LoadPlugins(string baseDirectory, Action<string> onWarning = null, bool scanBaseRecursively = false)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                throw new ArgumentException("Base directory cannot be null or whitespace.", nameof(baseDirectory));
            }

            var assemblyPattern = PluginAssemblyRegex();
            var pluginPath = Path.Combine(baseDirectory, "plugins");
            var dllPaths = new List<string>();

            // Scan the plugins/ subdirectory (standard layout for local/dev builds).
            if (Directory.Exists(pluginPath))
            {
                dllPaths.AddRange(
                    Directory.EnumerateFiles(pluginPath, "*.dll", SearchOption.AllDirectories)
                        .Where(path => assemblyPattern.IsMatch(Path.GetFileName(path))));
            }

            // Also scan the base directory itself (global tool layout where all
            // DLLs are side-by-side, or a user plugin folder when recursive).
            if (Directory.Exists(baseDirectory))
            {
                dllPaths.AddRange(
                    Directory.EnumerateFiles(baseDirectory, "*.dll", scanBaseRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                        .Where(path => assemblyPattern.IsMatch(Path.GetFileName(path))));
            }

            // Deduplicate by filename in case the same plugin appears in both locations.
            dllPaths = [.. dllPaths
                .GroupBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())];

            var plugins = new List<IDeckSurfPlugin>();

            foreach (var dllPath in dllPaths)
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(dllPath);
                }
                catch (Exception ex) when (ex is BadImageFormatException or FileLoadException or IOException)
                {
                    onWarning?.Invoke($"Failed to load plugin assembly '{Path.GetFileName(dllPath)}': {ex.Message}");
                    continue;
                }

                try
                {
                    var matchingTypes = assembly.GetExportedTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IDeckSurfPlugin).IsAssignableFrom(t));

                    foreach (var type in matchingTypes)
                    {
                        try
                        {
                            plugins.Add((IDeckSurfPlugin)Activator.CreateInstance(type));
                        }
                        catch (Exception ex) when (ex is MissingMethodException or MemberAccessException or TargetInvocationException or TypeLoadException or InvalidCastException)
                        {
                            onWarning?.Invoke($"Failed to instantiate plugin type '{type.FullName}': {ex.Message}");
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    onWarning?.Invoke($"Failed to enumerate types in assembly '{assembly.GetName().Name}': {ex.Message}");
                }
            }

            return plugins;
        }

        /// <summary>
        /// Instantiates the commands of a plugin that are compatible with a given device model,
        /// based on their <see cref="CompatibleWithAttribute"/> annotations.
        /// </summary>
        /// <param name="plugin">The plugin whose commands should be loaded.</param>
        /// <param name="model">The device model the commands must be compatible with.</param>
        /// <param name="onWarning">Optional callback invoked with a human-readable message for every command that fails to instantiate.</param>
        /// <returns>The instantiated, compatible commands.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is null.</exception>
        public static IReadOnlyList<IDeckSurfCommand> LoadCompatibleCommands(IDeckSurfPlugin plugin, DeviceModel model, Action<string> onWarning = null)
        {
            ArgumentNullException.ThrowIfNull(plugin);

            var commands = new List<IDeckSurfCommand>();

            foreach (var commandType in GetCommandTypes(plugin, model))
            {
                try
                {
                    commands.Add((IDeckSurfCommand)Activator.CreateInstance(commandType));
                }
                catch (Exception ex) when (ex is MissingMethodException or MemberAccessException or TargetInvocationException or TypeLoadException or InvalidCastException)
                {
                    onWarning?.Invoke($"Failed to instantiate command '{commandType.FullName}': {ex.Message}");
                }
            }

            return commands;
        }

        /// <summary>
        /// Gets the command types declared by a plugin, optionally filtered by device model
        /// compatibility. Commands with no <see cref="CompatibleWithAttribute"/> annotations
        /// are considered compatible with every model; the attribute is only needed to
        /// restrict a command to specific hardware. Unlike <see cref="LoadCompatibleCommands"/>,
        /// this does not instantiate the commands, which makes it suitable for reading command
        /// metadata and parameter schemas without side effects.
        /// </summary>
        /// <param name="plugin">The plugin whose command types should be listed.</param>
        /// <param name="model">When set, only command types compatible with this device model are returned.</param>
        /// <returns>The command types.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is null.</exception>
        public static IReadOnlyList<Type> GetCommandTypes(IDeckSurfPlugin plugin, DeviceModel? model = null)
        {
            ArgumentNullException.ThrowIfNull(plugin);

            var commandTypes = plugin.GetSupportedCommands() ?? [];

            if (model == null)
            {
                return commandTypes;
            }

            return [.. commandTypes.Where(t =>
            {
                var attributes = t.GetCustomAttributes(typeof(CompatibleWithAttribute), inherit: false)
                    .Cast<CompatibleWithAttribute>()
                    .ToArray();
                return attributes.Length == 0 || attributes.Any(a => a.CompatibleModel == model.Value);
            })];
        }

        /// <summary>
        /// Resolves the absolute path of a command's display icon, declared with
        /// <see cref="CommandIconAttribute"/>. The attribute's path is resolved
        /// relative to the directory containing the command's assembly, so plugins
        /// can ship icon files alongside their binaries.
        /// </summary>
        /// <param name="commandType">The command type whose icon should be resolved.</param>
        /// <returns>
        /// The absolute path of the icon file, or <c>null</c> when the command
        /// declares no icon or the declared file does not exist on disk.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="commandType"/> is null.</exception>
        public static string GetCommandIconPath(Type commandType)
        {
            ArgumentNullException.ThrowIfNull(commandType);

            if (commandType.GetCustomAttributes(typeof(CommandIconAttribute), inherit: true)
                    .Cast<CommandIconAttribute>()
                    .FirstOrDefault() is not { } attribute
                || string.IsNullOrWhiteSpace(attribute.RelativePath))
            {
                return null;
            }

            var assemblyDirectory = Path.GetDirectoryName(commandType.Assembly.Location);
            if (string.IsNullOrEmpty(assemblyDirectory))
            {
                return null;
            }

            var iconPath = Path.GetFullPath(Path.Combine(assemblyDirectory, attribute.RelativePath));
            return File.Exists(iconPath) ? iconPath : null;
        }

        [GeneratedRegex(@"^DeckSurf\.Plugin\..+\.dll$", RegexOptions.IgnoreCase)]
        private static partial Regex PluginAssemblyRegex();
    }
}

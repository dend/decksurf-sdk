// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Class that is used to manage DeckSurf configuration files.
    /// </summary>
    public static class ConfigurationHelper
    {
        private const string ProfileFileName = "profile.json";

        private static readonly JsonSerializerOptions IndentedSerializerOptions = new() { WriteIndented = true };

        // The fixed superset of characters that are invalid in file names on any
        // supported platform. Path.GetInvalidFileNameChars is platform-specific,
        // and profile folders must stay portable between operating systems.
        private static readonly char[] InvalidProfileNameCharacters =
        [
            '<', '>', ':', '"', '/', '\\', '|', '?', '*',
        ];

        /// <summary>
        /// Returns the fully qualified path to the root directory that contains all
        /// DeckSurf configuration profiles.
        /// </summary>
        /// <returns>Fully qualified path to the profiles root directory.</returns>
        public static string GetProfilesRootPath()
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine([localAppDataPath, "Den.Dev", "DeckSurf", "Profiles"]);
        }

        /// <summary>
        /// Returns the fully qualified path to a given DeckSurf configuration profile.
        /// </summary>
        /// <param name="name">Name of the profile.</param>
        /// <returns>Fully qualified path to the profile JSON file.</returns>
        public static string GetProfilePath(string name)
        {
            return Path.Combine([GetProfilesRootPath(), name, ProfileFileName]);
        }

        /// <summary>
        /// Lists the names of all stored profiles that contain a profile file, sorted alphabetically.
        /// </summary>
        /// <returns>The profile names. Empty when no profiles exist.</returns>
        public static IReadOnlyList<string> ListProfiles()
        {
            var rootPath = GetProfilesRootPath();
            if (!Directory.Exists(rootPath))
            {
                return [];
            }

            return [.. Directory.EnumerateDirectories(rootPath)
                .Where(directory => File.Exists(Path.Combine(directory, ProfileFileName)))
                .Select(Path.GetFileName)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)];
        }

        /// <summary>
        /// Saves a complete configuration profile, replacing any existing profile with the
        /// same name. The profile directory is created when it does not exist.
        /// </summary>
        /// <param name="name">Name of the profile.</param>
        /// <param name="profile">The profile to save.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, whitespace, or not a valid directory name.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="profile"/> is null.</exception>
        public static void SaveProfile(string name, ConfigurationProfile profile)
        {
            ValidateProfileName(name);
            ArgumentNullException.ThrowIfNull(profile);

            var path = GetProfilePath(name);
            new FileInfo(path).Directory.Create();
            File.WriteAllText(path, JsonSerializer.Serialize(profile, IndentedSerializerOptions));
        }

        /// <summary>
        /// Deletes a stored profile and its directory.
        /// </summary>
        /// <param name="name">Name of the profile.</param>
        /// <returns><c>true</c> when the profile existed and was deleted; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, whitespace, or not a valid directory name.</exception>
        public static bool DeleteProfile(string name)
        {
            ValidateProfileName(name);

            var profileDirectory = Path.Combine(GetProfilesRootPath(), name);
            if (!Directory.Exists(profileDirectory))
            {
                return false;
            }

            Directory.Delete(profileDirectory, recursive: true);
            return true;
        }

        /// <summary>
        /// Gets the profile object for a given profile name.
        /// </summary>
        /// <param name="profile">Name of the profile.</param>
        /// <returns>Object representing the DeckSurf configuration profile.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the profile file contains invalid JSON.</exception>
        public static ConfigurationProfile GetProfile(string profile)
        {
            var path = GetProfilePath(profile);
            if (!File.Exists(path) || new FileInfo(path).Length == 0)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<ConfigurationProfile>(File.ReadAllText(path));
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"The profile '{profile}' contains corrupted or invalid JSON data.", ex);
            }
        }

        /// <summary>
        /// Stores the DeckSurf configuration profile.
        /// </summary>
        /// <param name="profile">Name of the profile.</param>
        /// <param name="deviceIndex">Zero-based index of the Stream Deck device associated with a profile.</param>
        /// <param name="mapping">Object representing buttons mapped to plugins and commands.</param>
        /// <returns>Object representing the DeckSurf configuration profile.</returns>
        public static ConfigurationProfile WriteToConfiguration(string profile, int deviceIndex, CommandMapping mapping)
        {
            var path = GetProfilePath(profile);

            // In case the profile does not exist, let's make sure that we create
            // the full path. If it already exists, this does nothing.
            new FileInfo(path).Directory.Create();

            ConfigurationProfile configurationProfile;

            // We have to make sure that the file both exists, and is not empty. If
            // the file is empty, then the deserialization will fail, and the function
            // will return NULL.
            if (File.Exists(path) && new FileInfo(path).Length != 0)
            {
                configurationProfile = JsonSerializer.Deserialize<ConfigurationProfile>(File.ReadAllText(path));
                configurationProfile.ButtonMap.Add(mapping);
                configurationProfile.DeviceIndex = deviceIndex;
            }
            else
            {
                configurationProfile = new ConfigurationProfile
                {
                    DeviceIndex = deviceIndex,
                };
                configurationProfile.ButtonMap.Add(mapping);
            }

            File.WriteAllText(path, JsonSerializer.Serialize(configurationProfile));

            return configurationProfile;
        }

        private static void ValidateProfileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Profile name cannot be null or whitespace.", nameof(name));
            }

            if (name.IndexOfAny(InvalidProfileNameCharacters) >= 0
                || name.Any(char.IsControl)
                || name is "." or "..")
            {
                throw new ArgumentException($"Profile name '{name}' contains invalid characters.", nameof(name));
            }
        }
    }
}

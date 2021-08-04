// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text.Json;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Class that is used to manage DeckSurf configuration files.
    /// </summary>
    public class ConfigurationHelper
    {
        private const string ProfileFileName = "profile.json";

        /// <summary>
        /// Returns the fully qualified path to a given DeckSurf configuration profile.
        /// </summary>
        /// <param name="name">Name of the profile.</param>
        /// <returns>Fully qualified path to the profile JSON file.</returns>
        public static string GetProfilePath(string name)
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(new string[] { localAppDataPath, "DenDev", "Deck.Surf", "Profiles", name, ProfileFileName});
        }

        /// <summary>
        /// Gets the profile object for a given profile name.
        /// </summary>
        /// <param name="profile">Name of the profile.</param>
        /// <returns>Object representing the DeckSurf configuration profile.</returns>
        public static ConfigurationProfile GetProfile(string profile)
        {
            try
            {
                var path = GetProfilePath(profile);
                if (File.Exists(path) && new FileInfo(path).Length != 0)
                {
                    return JsonSerializer.Deserialize<ConfigurationProfile>(File.ReadAllText(path));
                }

                return null;
            }
            catch
            {
                return null;
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
            try
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
            catch
            {
                return null;
            }
        }
    }
}

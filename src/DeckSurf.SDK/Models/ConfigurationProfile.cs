// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Configuration profile that represents the mapping of buttons on a Stream Deck device to plugins and commands.
    /// </summary>
    public class ConfigurationProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProfile"/> class.
        /// </summary>
        public ConfigurationProfile()
        {
            this.ButtonMap = [];
        }

        /// <summary>
        /// Gets or sets the zero-based index for the device the configuration profile is associated with.
        /// </summary>
        [JsonPropertyName("device_index")]
        public int DeviceIndex { get; set; }

        /// <summary>
        /// Gets or sets the device model associated with the configuration profile.
        /// </summary>
        [JsonPropertyName("device_model")]
        public DeviceModel DeviceModel { get; set; }

        /// <summary>
        /// Gets or sets the mapping between buttons and the commands they trigger.
        /// </summary>
        [JsonPropertyName("button_map")]
        public List<CommandMapping> ButtonMap { get; set; }
    }
}

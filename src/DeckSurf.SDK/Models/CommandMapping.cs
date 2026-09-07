// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Command mapping used to associate Stream Deck buttons with specific plugins and commands.
    /// </summary>
    public class CommandMapping
    {
        private CommandArguments commandArguments = CommandArguments.Empty;

        /// <summary>
        /// Gets or sets the ID of the plugin that is associated with a button.
        /// </summary>
        [JsonPropertyName("plugin")]
        public string Plugin { get; set; }

        /// <summary>
        /// Gets or sets the ID of the command that needs to be called when the button is initialized or pressed.
        /// </summary>
        [JsonPropertyName("command")]
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the arguments that are passed to the command. Stored in
        /// profile JSON as an object of key/value pairs; the legacy comma-separated
        /// string format is still accepted when reading. Never null. Assigning null
        /// resets the mapping to <see cref="CommandArguments.Empty"/>.
        /// </summary>
        [JsonPropertyName("command_arguments")]
        public CommandArguments CommandArguments
        {
            get => this.commandArguments;
            set => this.commandArguments = value ?? CommandArguments.Empty;
        }

        /// <summary>
        /// Gets or sets the numeric index of the button on the Stream Deck that is associated with a command.
        /// For <see cref="MappingTarget.Knob"/> mappings this is the zero-based knob index.
        /// </summary>
        [JsonPropertyName("button_index")]
        public int ButtonIndex { get; set; }

        /// <summary>
        /// Gets or sets the hardware target of the mapping. Defaults to <see cref="MappingTarget.Key"/>,
        /// which keeps profiles written before targets existed working unchanged.
        /// </summary>
        [JsonPropertyName("target")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MappingTarget Target { get; set; } = MappingTarget.Key;

        /// <summary>
        /// Gets or sets the default image from the local file system that is loaded for a given Stream Deck button.
        /// </summary>
        [JsonPropertyName("button_image_path")]
        public string ButtonImagePath { get; set; }

        /// <summary>
        /// Gets or sets the backlight color for <see cref="MappingTarget.TouchButton"/>
        /// mappings, applied through <see cref="Interfaces.IConnectedDevice.SetKeyColor"/>
        /// while the profile is active. Null leaves the host's default treatment.
        /// Stored in profile JSON as a <c>#RRGGBB</c> string.
        /// </summary>
        [JsonPropertyName("button_color")]
        [JsonConverter(typeof(DeviceColorJsonConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeviceColor? ButtonColor { get; set; }
    }
}

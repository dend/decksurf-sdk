// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Command mapping used to associate Stream Deck buttons with specific plugins and commands.
    /// </summary>
    public class CommandMapping
    {
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
        /// Gets or sets a string value representing arguments that are passed to the command.
        /// </summary>
        [JsonPropertyName("command_arguments")]
        public string CommandArguments { get; set; }

        /// <summary>
        /// Gets or sets the numeric index of the button on the Stream Deck that is associated with a command.
        /// </summary>
        [JsonPropertyName("button_index")]
        public int ButtonIndex { get; set; }

        /// <summary>
        /// Gets or sets the default image from the local file system that is loaded for a given Stream Deck button.
        /// </summary>
        [JsonPropertyName("button_image_path")]
        public string ButtonImagePath { get; set; }
    }
}

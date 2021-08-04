// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Metadata that describes the plugin.
    /// </summary>
    public class PluginMetadata
    {
        /// <summary>
        /// Gets or sets the plugin ID string.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the plugin version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the plugin author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the plugin home page.
        /// </summary>
        public string Website { get; set; }
    }
}

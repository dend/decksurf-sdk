// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Interfaces
{
    /// <summary>
    /// Interface required by DeckSurf plugins to be recognized by DeckSurf tooling.
    /// </summary>
    public interface IDSPlugin
    {
        /// <summary>
        /// Gets non-functional information about the plugin.
        /// </summary>
        public PluginMetadata Metadata { get; }

        /// <summary>
        /// Provides a list of commands that the plugin exposes.
        /// </summary>
        /// <returns>List of managed types that represent <see cref="DeckSurf.SDK.Interfaces.IDSCommand"/> implementations.</returns>
        public List<Type> GetSupportedCommands();
    }
}

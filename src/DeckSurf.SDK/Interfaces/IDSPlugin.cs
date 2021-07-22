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
        public PluginMetadata Metadata { get; }

        public List<Type> GetSupportedCommands();
    }
}

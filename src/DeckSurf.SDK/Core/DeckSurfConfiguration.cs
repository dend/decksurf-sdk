// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Logging;

#nullable enable

namespace DeckSurf.SDK.Core
{
    /// <summary>
    /// Provides global configuration for the DeckSurf SDK.
    /// </summary>
    /// <remarks>
    /// Set <see cref="LoggerFactory"/> before creating any device instances to enable
    /// structured logging throughout the SDK. When no factory is configured, all log
    /// calls are no-ops via <see cref="Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory"/>.
    /// </remarks>
    public static class DeckSurfConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="ILoggerFactory"/> used to create loggers for SDK components.
        /// </summary>
        /// <value>
        /// An <see cref="ILoggerFactory"/> instance, or <see langword="null"/> to use the default no-op logger.
        /// </value>
        public static ILoggerFactory? LoggerFactory { get; set; }
    }
}

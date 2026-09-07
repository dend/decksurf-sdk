// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Coarse health of the external resource a command depends on, reported
    /// through <see cref="Interfaces.IDeckSurfStatusProvider"/>.
    /// </summary>
    public enum CommandStatusKind
    {
        /// <summary>
        /// The command's health could not be determined.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The command's backing resource is reachable and the command is expected to work.
        /// </summary>
        Ready = 1,

        /// <summary>
        /// The command's backing resource is unreachable or misconfigured;
        /// triggering the command is expected to fail.
        /// </summary>
        Unavailable = 2,
    }
}

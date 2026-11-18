// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Interfaces
{
    /// <summary>
    /// Optional interface for commands that depend on an external resource and can
    /// report whether it is currently reachable. Configuration tooling queries it
    /// while a command is being edited, alongside
    /// <see cref="IDeckSurfChoiceProvider"/> when both are implemented, so a
    /// misconfigured host or password is visible immediately instead of when a
    /// button press silently does nothing.
    /// </summary>
    public interface IDeckSurfStatusProvider
    {
        /// <summary>
        /// Probes the command's backing resource with the given configuration.
        /// Implementations should honor the cancellation token, return within a few
        /// seconds, and report failures as <see cref="CommandStatusKind.Unavailable"/>
        /// with a reason rather than throwing.
        /// </summary>
        /// <param name="currentValues">The values the user has entered so far for the command's parameters.</param>
        /// <param name="cancellationToken">Cancellation token for the probe.</param>
        /// <returns>The status report.</returns>
        Task<CommandStatus> GetStatusAsync(CommandArguments currentValues, CancellationToken cancellationToken = default);
    }
}

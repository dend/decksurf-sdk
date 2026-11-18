// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Interfaces
{
    /// <summary>
    /// Optional interface for commands whose parameter choices are only known at
    /// runtime, for example the scene list of a running OBS instance or the
    /// audio devices present on the machine. Tooling detects the interface on
    /// commands that declare a parameter with
    /// <see cref="CommandParameterAttribute.DynamicChoices"/> and queries it to
    /// populate suggestions, keeping the field editable as free text since the
    /// backing source may be unavailable.
    /// </summary>
    public interface IDeckSurfChoiceProvider
    {
        /// <summary>
        /// Returns the currently available choices for a parameter. Implementations
        /// should honor the cancellation token and return within a few seconds; when
        /// the backing source is unreachable, returning an empty list is preferred
        /// over throwing.
        /// </summary>
        /// <param name="parameterKey">Key of the parameter being edited, as declared in its <see cref="CommandParameterAttribute"/>.</param>
        /// <param name="currentValues">
        /// The values the user has entered so far for the command's other parameters.
        /// Providers typically need these for connection settings: which host to
        /// query, which credentials to use.
        /// </param>
        /// <param name="cancellationToken">Cancellation token for the lookup.</param>
        /// <returns>The available choices, in the order they should be presented. Empty when none could be determined.</returns>
        Task<IReadOnlyList<string>> GetChoicesAsync(string parameterKey, CommandArguments currentValues, CancellationToken cancellationToken = default);
    }
}

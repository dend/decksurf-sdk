// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// A point-in-time health report from a command about the external resource it
    /// depends on: a network service, connected hardware, a running application.
    /// Produced by <see cref="Interfaces.IDeckSurfStatusProvider"/> and displayed by
    /// configuration tooling so users can tell a working setup from a broken one
    /// before pressing a button.
    /// </summary>
    public sealed class CommandStatus
    {
        private CommandStatus(CommandStatusKind kind, string message)
        {
            this.Kind = kind;
            this.Message = message;
        }

        /// <summary>
        /// Gets the coarse health classification.
        /// </summary>
        public CommandStatusKind Kind { get; }

        /// <summary>
        /// Gets the human-readable status line, e.g. <c>"Connected to OBS at
        /// 127.0.0.1:4455, 6 scenes."</c>. For <see cref="CommandStatusKind.Unavailable"/>
        /// it should state the reason and, when known, what to fix.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates a status report for a working command.
        /// </summary>
        /// <param name="message">The human-readable status line.</param>
        /// <returns>The status report.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is null or whitespace.</exception>
        public static CommandStatus Ready(string message) => Create(CommandStatusKind.Ready, message);

        /// <summary>
        /// Creates a status report for a command whose backing resource is
        /// unreachable or misconfigured.
        /// </summary>
        /// <param name="message">The human-readable status line, stating the reason.</param>
        /// <returns>The status report.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is null or whitespace.</exception>
        public static CommandStatus Unavailable(string message) => Create(CommandStatusKind.Unavailable, message);

        private static CommandStatus Create(CommandStatusKind kind, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Status message cannot be null or whitespace.", nameof(message));
            }

            return new CommandStatus(kind, message);
        }
    }
}

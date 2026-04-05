// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Interfaces
{
    /// <summary>
    /// Interface used to implement executable commands for a DeckSurf plugin.
    /// </summary>
    public interface IDeckSurfCommand : IDisposable
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Function that is executed when the command is initialized by the plugin loader.
        /// </summary>
        /// <param name="mappedCommand">Instance of a command mapped to a button.</param>
        /// <param name="mappedDevice">Connected Stream Deck device.</param>
        public void ExecuteOnActivation(CommandMapping mappedCommand, IConnectedDevice mappedDevice);

        /// <summary>
        /// Function that is executed when the command is triggered by a mapped button.
        /// </summary>
        /// <param name="mappedCommand">Instance of a command mapped to a button.</param>
        /// <param name="mappedDevice">Connected Stream Deck device.</param>
        /// <param name="activatingButton">The numeric ID of the button activating the command. -1 is used for any button on the Stream Deck surface.</param>
        public void ExecuteOnAction(CommandMapping mappedCommand, IConnectedDevice mappedDevice, int activatingButton = -1);

        /// <summary>
        /// Async version of ExecuteOnAction. Override for commands that need async operations.
        /// </summary>
        /// <param name="mappedCommand">Instance of a command mapped to a button.</param>
        /// <param name="mappedDevice">Connected Stream Deck device.</param>
        /// <param name="activatingButton">The numeric ID of the button activating the command. -1 is used for any button on the Stream Deck surface.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the async operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteOnActionAsync(CommandMapping mappedCommand, IConnectedDevice mappedDevice, int activatingButton = -1, CancellationToken cancellationToken = default)
        {
            this.ExecuteOnAction(mappedCommand, mappedDevice, activatingButton);
            return Task.CompletedTask;
        }
    }
}

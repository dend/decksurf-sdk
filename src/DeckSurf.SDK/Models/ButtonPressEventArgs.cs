// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Event arguments that are passed back to the developer when a Stream Deck button is pressed.
    /// </summary>
    public class ButtonPressEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonPressEventArgs"/> class. Default constructor for button press event arguments.
        /// </summary>
        /// <param name="id">Numeric ID of the button being pressed.</param>
        /// <param name="kind">Kind of event.</param>
        public ButtonPressEventArgs(int id, ButtonEventKind kind)
        {
            this.Id = id;
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the numeric ID of the button being pressed.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the kind of event.
        /// </summary>
        public ButtonEventKind Kind { get; }
    }
}

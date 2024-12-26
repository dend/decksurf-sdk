﻿// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Event arguments that are passed back to the developer when a Stream Deck button is pressed.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ButtonPressEventArgs"/> class. Default constructor for button press event arguments.
    /// </remarks>
    /// <param name="id">Numeric ID of the button being pressed.</param>
    /// <param name="eventKind">Kind of event.</param>
    /// <param name="buttonKind">Kind of button pressed.</param>
    public class ButtonPressEventArgs(int id, ButtonEventKind eventKind, ButtonKind buttonKind, Point tapCoordinates) : EventArgs
    {
        /// <summary>
        /// Gets the coordinates on the touch screen where the tap occurred.
        /// </summary>
        public Point TapCoordinates { get; } = tapCoordinates;

        /// <summary>
        /// Gets the kind of the button pressed.
        /// </summary>
        public ButtonKind ButtonKind { get; } = buttonKind;

        /// <summary>
        /// Gets the numeric ID of the button being pressed.
        /// </summary>
        public int Id { get; } = id;

        /// <summary>
        /// Gets the kind of event.
        /// </summary>
        public ButtonEventKind EventKind { get; } = eventKind;
    }
}

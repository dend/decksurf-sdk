// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Event arguments that are passed back to the developer when a Stream Deck button is pressed.
    /// </summary>
    /// <remarks>
    /// <para>Initializes a new instance of the <see cref="ButtonPressEventArgs"/> class. Default constructor for button press event arguments.</para>
    /// <para>Which properties are populated depends on <see cref="ButtonKind"/>:</para>
    /// <list type="bullet">
    /// <item><description><see cref="Models.ButtonKind.Button"/>: only <see cref="Id"/> and <see cref="EventKind"/> are set.</description></item>
    /// <item><description><see cref="Models.ButtonKind.Screen"/>: <see cref="TapCoordinates"/> is also set.</description></item>
    /// <item><description><see cref="Models.ButtonKind.Knob"/>: <see cref="IsKnobRotating"/> and <see cref="KnobRotationDirection"/> are also set.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="pressedButtons">List of all button indices that are currently pressed.</param>
    /// <param name="eventKind">Kind of event.</param>
    /// <param name="buttonKind">Kind of button pressed.</param>
    /// <param name="tapCoordinates">Coordinates on the touch screen where a tap occurred.</param>
    /// <param name="isKnobRotating">Determines whether a knob is being rotated.</param>
    /// <param name="knobRotationDirection">Direction of a knob being rotated.</param>
    public class ButtonPressEventArgs(IReadOnlyList<int> pressedButtons, ButtonEventKind eventKind, ButtonKind? buttonKind, TouchPoint? tapCoordinates, bool? isKnobRotating, KnobRotationDirection? knobRotationDirection) : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonPressEventArgs"/> class with a single button ID.
        /// Maintained for backward compatibility.
        /// </summary>
        /// <param name="id">Numeric ID of the button being pressed.</param>
        /// <param name="eventKind">Kind of event.</param>
        /// <param name="buttonKind">Kind of button pressed.</param>
        /// <param name="tapCoordinates">Coordinates on the touch screen where a tap occurred.</param>
        /// <param name="isKnobRotating">Determines whether a knob is being rotated.</param>
        /// <param name="knobRotationDirection">Direction of a knob being rotated.</param>
        public ButtonPressEventArgs(int id, ButtonEventKind eventKind, ButtonKind? buttonKind, TouchPoint? tapCoordinates, bool? isKnobRotating, KnobRotationDirection? knobRotationDirection)
            : this(id >= 0 ? new[] { id } : Array.Empty<int>(), eventKind, buttonKind, tapCoordinates, isKnobRotating, knobRotationDirection)
        {
        }

        /// <summary>
        /// Gets a value indicating whether a knob is being rotated.
        /// </summary>
        public bool? IsKnobRotating { get; } = isKnobRotating;

        /// <summary>
        /// Gets the knob rotation direction if the button is a knob and was rotated.
        /// </summary>
        public KnobRotationDirection? KnobRotationDirection { get; } = knobRotationDirection;

        /// <summary>
        /// Gets the coordinates on the touch screen where the tap occurred.
        /// </summary>
        public TouchPoint? TapCoordinates { get; } = tapCoordinates;

        /// <summary>
        /// Gets the kind of the button pressed.
        /// </summary>
        public ButtonKind? ButtonKind { get; } = buttonKind;

        /// <summary>
        /// Gets the numeric ID of the first button being pressed, or -1 if no buttons are pressed.
        /// For multi-button scenarios, use <see cref="PressedButtons"/> instead.
        /// </summary>
        public int Id { get; } = pressedButtons.Count > 0 ? pressedButtons[0] : -1;

        /// <summary>
        /// Gets the list of all button indices that are currently pressed.
        /// </summary>
        public IReadOnlyList<int> PressedButtons { get; } = pressedButtons;

        /// <summary>
        /// Gets the kind of event.
        /// </summary>
        public ButtonEventKind EventKind { get; } = eventKind;
    }
}

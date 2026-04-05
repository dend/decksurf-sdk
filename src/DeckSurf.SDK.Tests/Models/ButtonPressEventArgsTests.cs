// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class ButtonPressEventArgsTests
    {
        [Fact]
        public void Constructor_SetsAllProperties()
        {
            var coords = new TouchPoint(100, 200);
            var args = new ButtonPressEventArgs(
                id: 5,
                eventKind: ButtonEventKind.Down,
                buttonKind: ButtonKind.Button,
                tapCoordinates: coords,
                isKnobRotating: true,
                knobRotationDirection: KnobRotationDirection.Right);

            Assert.Equal(5, args.Id);
            Assert.Equal(ButtonEventKind.Down, args.EventKind);
            Assert.Equal(ButtonKind.Button, args.ButtonKind);
            Assert.Equal(coords, args.TapCoordinates);
            Assert.True(args.IsKnobRotating);
            Assert.Equal(KnobRotationDirection.Right, args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_WithNullOptionalProperties_SetsNulls()
        {
            var args = new ButtonPressEventArgs(
                id: 0,
                eventKind: ButtonEventKind.Up,
                buttonKind: null,
                tapCoordinates: null,
                isKnobRotating: null,
                knobRotationDirection: null);

            Assert.Equal(0, args.Id);
            Assert.Equal(ButtonEventKind.Up, args.EventKind);
            Assert.Null(args.ButtonKind);
            Assert.Null(args.TapCoordinates);
            Assert.Null(args.IsKnobRotating);
            Assert.Null(args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_EventKindDown_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Down, null, null, null, null);

            Assert.Equal(ButtonEventKind.Down, args.EventKind);
        }

        [Fact]
        public void Constructor_EventKindUp_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Up, null, null, null, null);

            Assert.Equal(ButtonEventKind.Up, args.EventKind);
        }

        [Fact]
        public void Constructor_ButtonKindButton_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Down, ButtonKind.Button, null, null, null);

            Assert.Equal(ButtonKind.Button, args.ButtonKind);
        }

        [Fact]
        public void Constructor_ButtonKindKnob_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Down, ButtonKind.Knob, null, null, null);

            Assert.Equal(ButtonKind.Knob, args.ButtonKind);
        }

        [Fact]
        public void Constructor_ButtonKindScreen_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Down, ButtonKind.Screen, null, null, null);

            Assert.Equal(ButtonKind.Screen, args.ButtonKind);
        }

        [Fact]
        public void Constructor_ButtonKindUnknown_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Down, ButtonKind.Unknown, null, null, null);

            Assert.Equal(ButtonKind.Unknown, args.ButtonKind);
        }

        [Fact]
        public void Constructor_ButtonKindNull_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Down, null, null, null, null);

            Assert.Null(args.ButtonKind);
        }

        [Fact]
        public void Constructor_TapCoordinates_AreSetCorrectly()
        {
            var coords = new TouchPoint(300, 400);
            var args = new ButtonPressEventArgs(2, ButtonEventKind.Down, ButtonKind.Screen, coords, null, null);

            Assert.NotNull(args.TapCoordinates);
            Assert.Equal(300, args.TapCoordinates.Value.X);
            Assert.Equal(400, args.TapCoordinates.Value.Y);
        }

        [Fact]
        public void Constructor_TapCoordinatesNull_ReturnsNull()
        {
            var args = new ButtonPressEventArgs(2, ButtonEventKind.Down, ButtonKind.Button, null, null, null);

            Assert.Null(args.TapCoordinates);
        }

        [Fact]
        public void Constructor_IsKnobRotatingTrue_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(3, ButtonEventKind.Down, ButtonKind.Knob, null, true, KnobRotationDirection.Left);

            Assert.True(args.IsKnobRotating);
        }

        [Fact]
        public void Constructor_IsKnobRotatingFalse_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(3, ButtonEventKind.Down, ButtonKind.Knob, null, false, KnobRotationDirection.None);

            Assert.False(args.IsKnobRotating);
        }

        [Fact]
        public void Constructor_IsKnobRotatingNull_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(3, ButtonEventKind.Down, ButtonKind.Button, null, null, null);

            Assert.Null(args.IsKnobRotating);
        }

        [Fact]
        public void Constructor_KnobRotationDirectionRight_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(4, ButtonEventKind.Down, ButtonKind.Knob, null, true, KnobRotationDirection.Right);

            Assert.Equal(KnobRotationDirection.Right, args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_KnobRotationDirectionLeft_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(4, ButtonEventKind.Down, ButtonKind.Knob, null, true, KnobRotationDirection.Left);

            Assert.Equal(KnobRotationDirection.Left, args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_KnobRotationDirectionNone_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(4, ButtonEventKind.Down, ButtonKind.Knob, null, false, KnobRotationDirection.None);

            Assert.Equal(KnobRotationDirection.None, args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_KnobRotationDirectionNull_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(4, ButtonEventKind.Down, ButtonKind.Button, null, null, null);

            Assert.Null(args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_NegativeId_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(-1, ButtonEventKind.Down, null, null, null, null);

            Assert.Equal(-1, args.Id);
        }

        [Fact]
        public void Constructor_ZeroId_IsSetCorrectly()
        {
            var args = new ButtonPressEventArgs(0, ButtonEventKind.Up, null, null, null, null);

            Assert.Equal(0, args.Id);
        }

        [Fact]
        public void InheritsFromEventArgs()
        {
            var args = new ButtonPressEventArgs(1, ButtonEventKind.Down, null, null, null, null);

            Assert.IsAssignableFrom<EventArgs>(args);
        }

        [Fact]
        public void Constructor_TypicalKnobScenario_AllPropertiesSet()
        {
            var args = new ButtonPressEventArgs(
                id: 7,
                eventKind: ButtonEventKind.Down,
                buttonKind: ButtonKind.Knob,
                tapCoordinates: null,
                isKnobRotating: true,
                knobRotationDirection: KnobRotationDirection.Left);

            Assert.Equal(7, args.Id);
            Assert.Equal(ButtonEventKind.Down, args.EventKind);
            Assert.Equal(ButtonKind.Knob, args.ButtonKind);
            Assert.Null(args.TapCoordinates);
            Assert.True(args.IsKnobRotating);
            Assert.Equal(KnobRotationDirection.Left, args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_TypicalScreenTapScenario_AllPropertiesSet()
        {
            var tapPoint = new TouchPoint(150, 75);
            var args = new ButtonPressEventArgs(
                id: 0,
                eventKind: ButtonEventKind.Down,
                buttonKind: ButtonKind.Screen,
                tapCoordinates: tapPoint,
                isKnobRotating: null,
                knobRotationDirection: null);

            Assert.Equal(0, args.Id);
            Assert.Equal(ButtonEventKind.Down, args.EventKind);
            Assert.Equal(ButtonKind.Screen, args.ButtonKind);
            Assert.Equal(tapPoint, args.TapCoordinates);
            Assert.Null(args.IsKnobRotating);
            Assert.Null(args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_SingleId_SetsPressedButtonsWithOneEntry()
        {
            var args = new ButtonPressEventArgs(3, ButtonEventKind.Down, ButtonKind.Button, null, null, null);

            Assert.Single(args.PressedButtons);
            Assert.Equal(3, args.PressedButtons[0]);
            Assert.Equal(3, args.Id);
        }

        [Fact]
        public void Constructor_NegativeId_SetsPressedButtonsEmpty()
        {
            var args = new ButtonPressEventArgs(-1, ButtonEventKind.Up, null, null, null, null);

            Assert.Empty(args.PressedButtons);
            Assert.Equal(-1, args.Id);
        }

        [Fact]
        public void Constructor_WithPressedButtonsList_SetsAllProperties()
        {
            IReadOnlyList<int> buttons = new List<int> { 1, 3, 5 };
            var args = new ButtonPressEventArgs(buttons, ButtonEventKind.Down, ButtonKind.Button, null, null, null);

            Assert.Equal(3, args.PressedButtons.Count);
            Assert.Equal(1, args.PressedButtons[0]);
            Assert.Equal(3, args.PressedButtons[1]);
            Assert.Equal(5, args.PressedButtons[2]);
            Assert.Equal(1, args.Id);
        }

        [Fact]
        public void Constructor_WithEmptyPressedButtonsList_IdIsNegativeOne()
        {
            IReadOnlyList<int> buttons = Array.Empty<int>();
            var args = new ButtonPressEventArgs(buttons, ButtonEventKind.Up, null, null, null, null);

            Assert.Empty(args.PressedButtons);
            Assert.Equal(-1, args.Id);
        }

        [Fact]
        public void Constructor_WithMultipleKnobs_AllAreReported()
        {
            IReadOnlyList<int> knobs = new List<int> { 0, 2 };
            var args = new ButtonPressEventArgs(knobs, ButtonEventKind.Down, ButtonKind.Knob, null, true, KnobRotationDirection.Right);

            Assert.Equal(2, args.PressedButtons.Count);
            Assert.Equal(0, args.PressedButtons[0]);
            Assert.Equal(2, args.PressedButtons[1]);
            Assert.Equal(0, args.Id);
            Assert.True(args.IsKnobRotating);
            Assert.Equal(KnobRotationDirection.Right, args.KnobRotationDirection);
        }

        [Fact]
        public void Constructor_WithSinglePressedButton_IdMatchesFirstEntry()
        {
            IReadOnlyList<int> buttons = new List<int> { 7 };
            var args = new ButtonPressEventArgs(buttons, ButtonEventKind.Down, ButtonKind.Button, null, null, null);

            Assert.Single(args.PressedButtons);
            Assert.Equal(7, args.Id);
        }
    }
}

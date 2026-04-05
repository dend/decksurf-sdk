// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Represents a point on a Stream Deck touch screen.
    /// </summary>
    /// <remarks>
    /// Coordinates use a top-left origin where (0,0) is the top-left corner of the touch area.
    /// X increases rightward, Y increases downward.
    /// </remarks>
    public readonly struct TouchPoint : IEquatable<TouchPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchPoint"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public TouchPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        public int Y { get; }

        public static bool operator ==(TouchPoint left, TouchPoint right) => left.Equals(right);

        public static bool operator !=(TouchPoint left, TouchPoint right) => !left.Equals(right);

        /// <inheritdoc/>
        public bool Equals(TouchPoint other) => this.X == other.X && this.Y == other.Y;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is TouchPoint tp && this.Equals(tp);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

        /// <inheritdoc/>
        public override string ToString() => $"TouchPoint(X={this.X}, Y={this.Y})";
    }
}

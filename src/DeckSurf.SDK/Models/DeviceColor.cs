// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Represents an RGB color for use with Stream Deck device keys.
    /// </summary>
    public readonly struct DeviceColor : IEquatable<DeviceColor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceColor"/> struct.
        /// </summary>
        /// <param name="r">The red component (0-255).</param>
        /// <param name="g">The green component (0-255).</param>
        /// <param name="b">The blue component (0-255).</param>
        public DeviceColor(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        /// <summary>
        /// Gets a black color (0, 0, 0).
        /// </summary>
        public static DeviceColor Black => new(0, 0, 0);

        /// <summary>
        /// Gets a red color (255, 0, 0).
        /// </summary>
        public static DeviceColor Red => new(255, 0, 0);

        /// <summary>
        /// Gets a green color (0, 128, 0).
        /// </summary>
        public static DeviceColor Green => new(0, 128, 0);

        /// <summary>
        /// Gets a blue color (0, 0, 255).
        /// </summary>
        public static DeviceColor Blue => new(0, 0, 255);

        /// <summary>
        /// Gets a white color (255, 255, 255).
        /// </summary>
        public static DeviceColor White => new(255, 255, 255);

        /// <summary>
        /// Gets a yellow color (255, 255, 0).
        /// </summary>
        public static DeviceColor Yellow => new(255, 255, 0);

        /// <summary>
        /// Gets a cyan color (0, 255, 255).
        /// </summary>
        public static DeviceColor Cyan => new(0, 255, 255);

        /// <summary>
        /// Gets a magenta color (255, 0, 255).
        /// </summary>
        public static DeviceColor Magenta => new(255, 0, 255);

        /// <summary>
        /// Gets the red component of the color.
        /// </summary>
        public byte R { get; }

        /// <summary>
        /// Gets the green component of the color.
        /// </summary>
        public byte G { get; }

        /// <summary>
        /// Gets the blue component of the color.
        /// </summary>
        public byte B { get; }

        /// <summary>
        /// Determines whether two <see cref="DeviceColor"/> values are equal.
        /// </summary>
        /// <param name="left">The first color to compare.</param>
        /// <param name="right">The second color to compare.</param>
        /// <returns><c>true</c> if the two colors are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(DeviceColor left, DeviceColor right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="DeviceColor"/> values are not equal.
        /// </summary>
        /// <param name="left">The first color to compare.</param>
        /// <param name="right">The second color to compare.</param>
        /// <returns><c>true</c> if the two colors are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(DeviceColor left, DeviceColor right) => !left.Equals(right);

        /// <inheritdoc/>
        public bool Equals(DeviceColor other) => this.R == other.R && this.G == other.G && this.B == other.B;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is DeviceColor dc && this.Equals(dc);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(this.R, this.G, this.B);

        /// <inheritdoc/>
        public override string ToString() => $"DeviceColor(R={this.R}, G={this.G}, B={this.B})";
    }
}

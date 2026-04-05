// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Represents an RGB color for use with Stream Deck device keys.
    /// </summary>
    public readonly struct DeviceColor
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
    }
}

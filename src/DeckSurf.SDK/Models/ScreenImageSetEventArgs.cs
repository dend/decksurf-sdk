// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Event arguments describing an image written to a Stream Deck screen. Lets
    /// hosts observe what the hardware displays (for example to mirror the touch
    /// strip in a live on-screen preview) without intercepting device I/O.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ScreenImageSetEventArgs"/> class.
    /// </remarks>
    /// <param name="image">The image content exactly as passed to the set operation.</param>
    /// <param name="xOffset">Horizontal offset from the left where the image was set.</param>
    /// <param name="yOffset">Vertical offset from the top where the image was set.</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    public class ScreenImageSetEventArgs(byte[] image, int xOffset, int yOffset, int width, int height) : EventArgs
    {
        /// <summary>
        /// Gets the image content exactly as passed to the set operation.
        /// </summary>
        public byte[] Image { get; } = image;

        /// <summary>
        /// Gets the horizontal offset from the left where the image was set.
        /// </summary>
        public int XOffset { get; } = xOffset;

        /// <summary>
        /// Gets the vertical offset from the top where the image was set.
        /// </summary>
        public int YOffset { get; } = yOffset;

        /// <summary>
        /// Gets the image width.
        /// </summary>
        public int Width { get; } = width;

        /// <summary>
        /// Gets the image height.
        /// </summary>
        public int Height { get; } = height;
    }
}

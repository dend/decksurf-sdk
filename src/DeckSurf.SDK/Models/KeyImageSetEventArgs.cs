// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Event arguments describing an image written to a Stream Deck key. Lets
    /// hosts observe what the hardware displays (for example to mirror keys in
    /// a live on-screen preview) without intercepting device I/O.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="KeyImageSetEventArgs"/> class.
    /// </remarks>
    /// <param name="keyId">Numeric ID of the key that was set.</param>
    /// <param name="image">The image content exactly as passed to the set operation, before any device-specific resizing or encoding.</param>
    public class KeyImageSetEventArgs(int keyId, byte[] image) : EventArgs
    {
        /// <summary>
        /// Gets the numeric ID of the key that was set.
        /// </summary>
        public int KeyId { get; } = keyId;

        /// <summary>
        /// Gets the image content exactly as passed to the set operation, before
        /// any device-specific resizing or encoding.
        /// </summary>
        public byte[] Image { get; } = image;
    }
}

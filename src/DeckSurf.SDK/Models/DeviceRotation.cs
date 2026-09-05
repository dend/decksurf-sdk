// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Represents the rotation and flip transformation applied to images sent to a Stream Deck device.
    /// </summary>
    public enum DeviceRotation
    {
        /// <summary>
        /// No rotation applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// Rotate the image 180 degrees.
        /// </summary>
        Rotate180,

        /// <summary>
        /// Rotate the image 270 degrees.
        /// </summary>
        Rotate270,

        /// <summary>
        /// Rotate the image 270 degrees, then flip it vertically. The combined result is a transpose
        /// across the main diagonal: the top-left corner stays in place while the top-right and
        /// bottom-left corners swap. This is the orientation expected by BMP-based devices such as
        /// the Stream Deck Mini, whose firmware otherwise displays rotated content mirrored.
        /// </summary>
        Rotate270FlipVertical,
    }
}

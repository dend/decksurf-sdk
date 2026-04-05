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
        /// Rotate the image 180 degrees with no flip.
        /// </summary>
        Rotate180FlipNone,

        /// <summary>
        /// Rotate the image 270 degrees with no flip.
        /// </summary>
        Rotate270FlipNone,
    }
}

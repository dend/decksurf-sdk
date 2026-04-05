// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Classifies the category of a device error.
    /// </summary>
    public enum DeviceErrorCategory
    {
        /// <summary>
        /// The error category is unknown or does not match any other classification.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The device was disconnected during an operation.
        /// </summary>
        Disconnected = 1,

        /// <summary>
        /// A communication failure occurred during USB I/O with the device.
        /// </summary>
        CommunicationFailure = 2,

        /// <summary>
        /// An error occurred while processing an image for the device.
        /// </summary>
        ImageProcessing = 3,

        /// <summary>
        /// An invalid parameter was provided to a device operation.
        /// </summary>
        InvalidParameter = 4,

        /// <summary>
        /// The device has been disposed and can no longer be used.
        /// </summary>
        DeviceDisposed = 5,
    }
}

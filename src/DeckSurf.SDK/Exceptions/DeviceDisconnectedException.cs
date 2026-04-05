// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Exceptions
{
    /// <summary>
    /// Exception thrown when a device is unplugged mid-operation.
    /// </summary>
    public class DeviceDisconnectedException : DeckSurfException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDisconnectedException"/> class.
        /// </summary>
        public DeviceDisconnectedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDisconnectedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeviceDisconnectedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDisconnectedException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DeviceDisconnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the serial number of the device that was disconnected.
        /// </summary>
        public string DeviceSerial { get; init; }
    }
}

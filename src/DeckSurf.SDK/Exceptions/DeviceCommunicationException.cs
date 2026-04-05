// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Exceptions
{
    /// <summary>
    /// Exception thrown when a USB I/O failure occurs during device communication.
    /// </summary>
    public class DeviceCommunicationException : DeckSurfException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCommunicationException"/> class.
        /// </summary>
        public DeviceCommunicationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCommunicationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeviceCommunicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCommunicationException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DeviceCommunicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the error is transient and the operation may succeed if retried.
        /// </summary>
        public bool IsTransient { get; init; }
    }
}

// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Exceptions
{
    /// <summary>
    /// Base exception for all DeckSurf SDK errors.
    /// </summary>
    public class DeckSurfException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeckSurfException"/> class.
        /// </summary>
        public DeckSurfException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckSurfException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeckSurfException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckSurfException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DeckSurfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

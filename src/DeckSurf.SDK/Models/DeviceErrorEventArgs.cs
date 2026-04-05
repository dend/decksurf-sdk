// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Event arguments that are passed when a device error occurs during operation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DeviceErrorEventArgs"/> class.
    /// </remarks>
    /// <param name="exception">The underlying exception that caused the error.</param>
    /// <param name="category">The classification of the error.</param>
    /// <param name="isTransient">Whether the error is transient and a retry might succeed.</param>
    /// <param name="recoveryHint">A suggested action for recovery.</param>
    /// <param name="operationName">The name of the operation that failed.</param>
    public class DeviceErrorEventArgs(Exception exception, DeviceErrorCategory category, bool isTransient, string recoveryHint, string operationName) : EventArgs
    {
        /// <summary>
        /// Gets the underlying exception that caused the error.
        /// </summary>
        public Exception Exception { get; } = exception;

        /// <summary>
        /// Gets the classification of the error.
        /// </summary>
        public DeviceErrorCategory Category { get; } = category;

        /// <summary>
        /// Gets a value indicating whether the error is transient and a retry might succeed.
        /// </summary>
        public bool IsTransient { get; } = isTransient;

        /// <summary>
        /// Gets a suggested action for recovering from the error.
        /// </summary>
        public string RecoveryHint { get; } = recoveryHint;

        /// <summary>
        /// Gets the name of the operation that failed (e.g., "SetKey", "ReadInput").
        /// </summary>
        public string OperationName { get; } = operationName;
    }
}

// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Event arguments that describe which Stream Deck devices were added or removed
    /// since the last time the device list was evaluated.
    /// </summary>
    public class DeviceListChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="added">The devices that were added since the last evaluation.</param>
        /// <param name="removed">The devices that were removed since the last evaluation.</param>
        public DeviceListChangedEventArgs(IReadOnlyList<DeviceInfo> added, IReadOnlyList<DeviceInfo> removed)
        {
            this.Added = added ?? throw new ArgumentNullException(nameof(added));
            this.Removed = removed ?? throw new ArgumentNullException(nameof(removed));
        }

        /// <summary>
        /// Gets the list of devices that were connected since the last evaluation.
        /// </summary>
        public IReadOnlyList<DeviceInfo> Added { get; }

        /// <summary>
        /// Gets the list of devices that were disconnected since the last evaluation.
        /// </summary>
        public IReadOnlyList<DeviceInfo> Removed { get; }
    }
}

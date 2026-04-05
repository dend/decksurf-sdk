// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// A lightweight, immutable descriptor for a Stream Deck device. Two instances are
    /// considered equal when their <see cref="Serial"/> values match, making this type
    /// suitable for use in hash-based collections when tracking device additions and removals.
    /// </summary>
    public readonly struct DeviceInfo : IEquatable<DeviceInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfo"/> struct.
        /// </summary>
        /// <param name="serial">The serial number of the device.</param>
        /// <param name="name">The friendly name of the device.</param>
        /// <param name="model">The <see cref="DeviceModel"/> of the device.</param>
        /// <param name="path">The USB HID device path.</param>
        public DeviceInfo(string serial, string name, DeviceModel model, string path)
        {
            this.Serial = serial ?? throw new ArgumentNullException(nameof(serial));
            this.Name = name ?? string.Empty;
            this.Model = model;
            this.Path = path ?? string.Empty;
        }

        /// <summary>
        /// Gets the serial number of the device.
        /// </summary>
        public string Serial { get; }

        /// <summary>
        /// Gets the friendly name of the device.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="DeviceModel"/> of the device.
        /// </summary>
        public DeviceModel Model { get; }

        /// <summary>
        /// Gets the USB HID device path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Determines whether two <see cref="DeviceInfo"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(DeviceInfo left, DeviceInfo right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="DeviceInfo"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(DeviceInfo left, DeviceInfo right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(DeviceInfo other)
        {
            return string.Equals(this.Serial, other.Serial, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is DeviceInfo other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(this.Serial);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Model} ({this.Serial})";
        }
    }
}

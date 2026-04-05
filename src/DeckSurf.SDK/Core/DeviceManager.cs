// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using DeckSurf.SDK.Exceptions;
using DeckSurf.SDK.Models;
using HidSharp;

namespace DeckSurf.SDK.Core
{
    /// <summary>
    /// Class used to manage connected Stream Deck devices.
    /// </summary>
    public static class DeviceManager
    {
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
        private static readonly int[] SupportedVids = [0x0FD9];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly

        static DeviceManager()
        {
            DeviceList.Local.Changed += (sender, e) => DeviceListChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the system detects a change in connected HID devices.
        /// Call <see cref="GetDeviceList"/> after this event to get the updated list.
        /// </summary>
        public static event EventHandler DeviceListChanged;

        /// <summary>
        /// Return a list of connected Stream Deck devices supported by DeckSurf.
        /// </summary>
        /// <returns>Read-only list containing supported devices.</returns>
        public static IReadOnlyList<ConnectedDevice> GetDeviceList()
        {
            var connectedDevices = new List<ConnectedDevice>();
            var deviceList = DeviceList.Local.GetHidDevices();

            foreach (var device in deviceList)
            {
                bool supported = IsSupported(device.VendorID, device.ProductID);
                if (supported)
                {
                    var connectedDevice = DeviceRegistry.CreateDevice(device.VendorID, device.ProductID, device.DevicePath, device.GetFriendlyName(), device.GetSerialNumber());
                    if (connectedDevice != null)
                    {
                        connectedDevices.Add(connectedDevice);
                    }
                }
            }

            return connectedDevices;
        }

        /// <summary>
        /// Gets a connected Stream Deck device based on a pre-defined configuration profile.
        /// The method prefers matching by <see cref="ConfigurationProfile.DeviceSerial"/> (which is
        /// stable across re-plugs) and falls back to <see cref="ConfigurationProfile.DeviceIndex"/>
        /// when no serial match is found.
        /// </summary>
        /// <param name="profile">An instance representing the pre-defined configuration profile.</param>
        /// <returns>The matching <see cref="ConnectedDevice"/> with its button map configured.</returns>
        /// <exception cref="DeviceNotFoundException">
        /// Thrown when no device matches the serial number or index specified in <paramref name="profile"/>.
        /// </exception>
        public static ConnectedDevice SetupDevice(ConfigurationProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            var devices = GetDeviceList();
            ConnectedDevice targetDevice = null;

            // Prefer serial number (stable across re-plugs)
            if (!string.IsNullOrEmpty(profile.DeviceSerial))
            {
                targetDevice = devices.FirstOrDefault(d => d.Serial == profile.DeviceSerial);
            }

            // Fall back to index
            if (targetDevice == null && profile.DeviceIndex >= 0 && profile.DeviceIndex < devices.Count)
            {
                targetDevice = devices[profile.DeviceIndex];
            }

            if (targetDevice == null)
            {
                throw new DeviceNotFoundException($"No device found matching serial '{profile.DeviceSerial}' or index {profile.DeviceIndex}.");
            }

            targetDevice.SetupDeviceButtonMap(profile.ButtonMap);
            return targetDevice;
        }

        /// <summary>
        /// Gets a connected Stream Deck device that matches the specified serial number.
        /// </summary>
        /// <param name="serial">The serial number of the device to find.</param>
        /// <returns>The first <see cref="ConnectedDevice"/> whose serial matches, or <c>null</c> if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serial"/> is <c>null</c> or empty.</exception>
        public static ConnectedDevice GetDeviceBySerial(string serial)
        {
            if (string.IsNullOrEmpty(serial))
            {
                throw new ArgumentNullException(nameof(serial));
            }

            return GetDeviceList().FirstOrDefault(d => d.Serial == serial);
        }

        /// <summary>
        /// Gets a connected Stream Deck device that matches the specified USB HID device path.
        /// </summary>
        /// <param name="devicePath">The USB HID device path to match.</param>
        /// <returns>The first <see cref="ConnectedDevice"/> whose path matches, or <c>null</c> if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="devicePath"/> is <c>null</c> or empty.</exception>
        public static ConnectedDevice GetDeviceByPath(string devicePath)
        {
            if (string.IsNullOrEmpty(devicePath))
            {
                throw new ArgumentNullException(nameof(devicePath));
            }

            return GetDeviceList().FirstOrDefault(d => d.Path == devicePath);
        }

        /// <summary>
        /// Determines whether a given vendor ID (VID) and product ID (PID) are supported by the SDK. VID and PID should be representing a Stream Deck device.
        /// </summary>
        /// <param name="vendorId">Device VID.</param>
        /// <param name="productId">Device PID.</param>
        /// <returns>True if device is supported, false if not.</returns>
        public static bool IsSupported(int vendorId, int productId)
        {
            if (SupportedVids.Contains(vendorId) && Enum.IsDefined(typeof(DeviceModel), (byte)productId))
            {
                return true;
            }

            return false;
        }
    }
}

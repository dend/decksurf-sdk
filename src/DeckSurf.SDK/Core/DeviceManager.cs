// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Models.Devices;
using HidSharp;

namespace DeckSurf.SDK.Core
{
    /// <summary>
    /// Class used to manage connected Stream Deck devices.
    /// </summary>
    public class DeviceManager
    {
        private static readonly int[] SupportedVids = [0x0FD9];

        /// <summary>
        /// Return a list of connected Stream Deck devices supported by DeckSurf.
        /// </summary>
        /// <returns>Enumerable containing a list of supported devices.</returns>
        public static IEnumerable<ConnectedDevice> GetDeviceList()
        {
            var connectedDevices = new List<ConnectedDevice>();
            var deviceList = DeviceList.Local.GetHidDevices();

            foreach (var device in deviceList)
            {
                bool supported = IsSupported(device.VendorID, device.ProductID);
                if (supported)
                {
                    switch ((DeviceModel)device.ProductID)
                    {
                        case DeviceModel.XL:
                            {
                                connectedDevices.Add(new StreamDeckXL(device.VendorID, device.ProductID, device.DevicePath, device.GetFriendlyName(), (DeviceModel)device.ProductID));
                                break;
                            }

                        case DeviceModel.PLUS:
                            {
                                connectedDevices.Add(new StreamDeckPlus(device.VendorID, device.ProductID, device.DevicePath, device.GetFriendlyName(), (DeviceModel)device.ProductID));
                                break;
                            }

                        case DeviceModel.MINI:
                        case DeviceModel.ORIGINAL:
                        case DeviceModel.ORIGINAL_V2:
                        default:
                            {
                                // Haven't yet implemented support for other Stream Deck device classes.
                                break;
                            }
                    }
                }
            }

            return connectedDevices;
        }

        /// <summary>
        /// Gets a connected Stream Deck device based on a pre-defined configuration profile.
        /// </summary>
        /// <param name="profile">An instance representing the pre-defined configuration profile.</param>
        /// <returns>If the call is successful, returns a Stream Deck device.</returns>
        public static ConnectedDevice SetupDevice(ConfigurationProfile profile)
        {
            try
            {
                var devices = GetDeviceList();
                if (devices != null &&
                    devices.Any() &&
                    profile.DeviceIndex <= devices.Count() - 1)
                {
                    var targetDevice = devices.ElementAt(profile.DeviceIndex);
                    targetDevice.SetupDeviceButtonMap(profile.ButtonMap, profile.DeviceModel);
                    return targetDevice;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether a given vendor ID (VID) and product ID (PID) are supported by the SDK. VID and PID should be representing a Stream Deck device.
        /// </summary>
        /// <param name="vid">Device VID.</param>
        /// <param name="pid">Device PID.</param>
        /// <returns>True if device is supported, false if not.</returns>
        public static bool IsSupported(int vid, int pid)
        {
            if (SupportedVids.Contains(vid) && Enum.IsDefined(typeof(DeviceModel), (byte)pid))
            {
                return true;
            }

            return false;
        }
    }
}

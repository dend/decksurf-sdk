// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Models.Devices;

namespace DeckSurf.SDK.Core
{
    /// <summary>
    /// Static registry that maps <see cref="DeviceModel"/> values to factory functions
    /// for creating <see cref="ConnectedDevice"/> instances.
    /// </summary>
    public static class DeviceRegistry
    {
        private static readonly Dictionary<DeviceModel, Func<int, int, string, string, string, ConnectedDevice>> DeviceFactories = new()
        {
            { DeviceModel.Original, (vid, pid, path, name, serial) => new StreamDeckOriginal(vid, pid, path, name, serial) },
            { DeviceModel.Original2019, (vid, pid, path, name, serial) => new StreamDeckOriginal2019(vid, pid, path, name, serial) },
            { DeviceModel.MK2, (vid, pid, path, name, serial) => new StreamDeckMK2(vid, pid, path, name, serial) },
            { DeviceModel.Mini, (vid, pid, path, name, serial) => new StreamDeckMini(vid, pid, path, name, serial) },
            { DeviceModel.Mini2022, (vid, pid, path, name, serial) => new StreamDeckMini2022(vid, pid, path, name, serial) },
            { DeviceModel.XL, (vid, pid, path, name, serial) => new StreamDeckXL(vid, pid, path, name, serial) },
            { DeviceModel.XL2022, (vid, pid, path, name, serial) => new StreamDeckXL2022(vid, pid, path, name, serial) },
            { DeviceModel.Plus, (vid, pid, path, name, serial) => new StreamDeckPlus(vid, pid, path, name, serial) },
            { DeviceModel.Neo, (vid, pid, path, name, serial) => new StreamDeckNeo(vid, pid, path, name, serial) },
        };

        /// <summary>
        /// Creates a <see cref="ConnectedDevice"/> instance for the given device parameters.
        /// </summary>
        /// <param name="vid">Vendor ID.</param>
        /// <param name="pid">Product ID.</param>
        /// <param name="path">Path to the USB HID device.</param>
        /// <param name="name">Name of the USB HID device.</param>
        /// <param name="serial">Serial number for the device.</param>
        /// <returns>A <see cref="ConnectedDevice"/> instance if the device model is supported; otherwise, <c>null</c>.</returns>
        public static ConnectedDevice CreateDevice(int vid, int pid, string path, string name, string serial)
        {
            var model = (DeviceModel)pid;
            if (DeviceFactories.TryGetValue(model, out var factory))
            {
                return factory(vid, pid, path, name, serial);
            }

            return null;
        }
    }
}

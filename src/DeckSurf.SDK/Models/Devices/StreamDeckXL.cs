﻿// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck XL connected device.
    /// </summary>
    public class StreamDeckXL : ConnectedDevice
    {
        public StreamDeckXL(int vid, int pid, string path, string name, DeviceModel model)
            : base(vid, pid, path, name, model)
        {
        }
    }
}

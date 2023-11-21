// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Models.Devices
{
    internal class StreamDeckV2 : ConnectedDevice
    {
        public StreamDeckV2(int vid, int pid, string path, string name, DeviceModel model) : base(vid, pid, path, name, model)
        {
        }
    }
}
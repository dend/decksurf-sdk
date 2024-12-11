// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    internal class StreamDeckPlus : ConnectedDevice
    {
        public StreamDeckPlus(int vid, int pid, string path, string name, DeviceModel model)
            : base(vid, pid, path, name, model)
        {
        }
    }
}

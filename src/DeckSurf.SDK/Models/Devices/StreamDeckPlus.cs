// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    internal class StreamDeckPlus(int vid, int pid, string path, string name) : ConnectedDevice(vid, pid, path, name)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.PLUS;

        /// <inheritdoc/>
        public override int ButtonCount => 8;
    }
}

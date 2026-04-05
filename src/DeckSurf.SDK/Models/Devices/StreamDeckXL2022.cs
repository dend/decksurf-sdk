// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck XL (2022) connected device.
    /// </summary>
    public class StreamDeckXL2022(int vid, int pid, string path, string name, string serial) : JpegButtonsDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.XL2022;

        /// <inheritdoc/>
        public override int ButtonCount => 32;

        /// <inheritdoc/>
        public override int ButtonResolution => 96;

        /// <inheritdoc/>
        public override int ButtonColumns => 8;

        /// <inheritdoc/>
        public override int ButtonRows => 4;
    }
}

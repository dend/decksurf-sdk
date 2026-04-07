// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck MK.2 connected device (Elgato model 20GBA9901).
    /// </summary>
    public class StreamDeckMK2(int vid, int pid, string path, string name, string serial) : JpegButtonsDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.MK2;

        /// <inheritdoc/>
        public override int ButtonCount => 15;

        /// <inheritdoc/>
        public override int ButtonResolution => 72;

        /// <inheritdoc/>
        public override int ButtonColumns => 5;

        /// <inheritdoc/>
        public override int ButtonRows => 3;
    }
}

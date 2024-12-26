// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Plus connected device.
    /// </summary>
    internal class StreamDeckPlus(int vid, int pid, string path, string name) : ConnectedDevice(vid, pid, path, name)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.PLUS;

        /// <inheritdoc/>
        public override int ButtonCount => 8;

        /// <inheritdoc/>
        public override bool IsButtonImageFlipRequired => false;

        /// <inheritdoc/>
        public override bool IsScreenSupported => true;

        /// <inheritdoc/>
        public override bool IsKnobSupported => true;

        /// <inheritdoc/>
        public override int ButtonResolution => 120;

        /// <inheritdoc/>
        public override int ButtonColumns => 4;

        /// <inheritdoc/>
        public override int ButtonRows => 2;

        /// <inheritdoc/>
        public override int ScreenWidth => 800;

        /// <inheritdoc/>
        public override int ScreenHeight => 100;

        /// <inheritdoc/>
        public override int ScreenSegmentWidth => 200;
    }
}

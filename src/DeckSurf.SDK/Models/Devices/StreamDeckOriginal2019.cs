﻿// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Original (2019) connected device.
    /// </summary>
    public class StreamDeckOriginal2019(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.Original2019;

        /// <inheritdoc/>
        public override int ButtonCount => 15;

        /// <inheritdoc/>
        public override bool IsButtonImageFlipRequired => true;

        /// <inheritdoc/>
        public override bool IsScreenSupported => false;

        /// <inheritdoc/>
        public override bool IsKnobSupported => false;

        /// <inheritdoc/>
        public override int ButtonResolution => 72;

        /// <inheritdoc/>
        public override int ButtonColumns => 5;

        /// <inheritdoc/>
        public override int ButtonRows => 3;

        /// <inheritdoc/>
        public override int ScreenWidth => -1;

        /// <inheritdoc/>
        public override int ScreenHeight => -1;

        /// <inheritdoc/>
        public override int ScreenSegmentWidth => -1;
    }
}
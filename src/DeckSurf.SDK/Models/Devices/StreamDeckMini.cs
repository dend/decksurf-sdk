// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Mini connected device.
    /// </summary>
    public class StreamDeckMini(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.Mini;

        /// <inheritdoc/>
        public override int ButtonCount => 6;

        /// <inheritdoc/>
        public override bool IsScreenSupported => false;

        /// <inheritdoc/>
        public override bool IsKnobSupported => false;

        /// <inheritdoc/>
        public override int ButtonResolution => 80;

        /// <inheritdoc/>
        public override int ButtonColumns => 3;

        /// <inheritdoc/>
        public override int ButtonRows => 2;

        /// <inheritdoc/>
        public override int ScreenWidth => -1;

        /// <inheritdoc/>
        public override int ScreenHeight => -1;

        /// <inheritdoc/>
        public override int ScreenSegmentWidth => -1;

        /// <inheritdoc/>
        public override ImageFormat KeyImageFormat => ImageFormat.Bmp;

        /// <inheritdoc/>
        public override int KeyImageHeaderSize => 16;

        /// <inheritdoc/>
        public override int PacketSize => 1024;

        /// <inheritdoc/>
        public override int ScreenImageHeaderSize => 16;

        /// <inheritdoc/>
        public override RotateFlipType FlipType => RotateFlipType.Rotate270FlipNone;

        /// <inheritdoc/>
        public override byte[] GetKeySetupHeader(int keyId, int sliceLength, int iteration, int remainingBytes)
        {
            byte finalizer = sliceLength == remainingBytes ? (byte)1 : (byte)0;
            var binaryLength = DataHelpers.GetLittleEndianBytesFromInt(sliceLength);
            var binaryIteration = DataHelpers.GetLittleEndianBytesFromInt(iteration);

            return
            [
                0x02,
                0x01,
                binaryIteration[0],
                binaryIteration[1],
                finalizer,
                (byte)keyId,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
            ];
        }
    }
}

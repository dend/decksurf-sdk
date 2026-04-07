// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Abstract base class for Stream Deck devices that support a screen in addition
    /// to buttons. Covers the Neo and Plus models.
    /// </summary>
    public abstract class ScreenDevice(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceImageFormat KeyImageFormat => DeviceImageFormat.Jpeg;

        /// <inheritdoc/>
        public override int KeyImageHeaderSize => 8;

        /// <inheritdoc/>
        public override int PacketSize => 1024;

        /// <inheritdoc/>
        public override DeviceRotation ImageRotation => DeviceRotation.Rotate180;

        /// <inheritdoc/>
        public override bool IsScreenSupported => true;

        /// <inheritdoc/>
        protected internal override byte[] GetKeySetupHeader(int keyId, int sliceLength, int iteration, int remainingBytes)
        {
            byte finalizer = sliceLength == remainingBytes ? (byte)1 : (byte)0;
            var binaryLength = DataHelper.GetLittleEndianBytesFromInt(sliceLength);
            var binaryIteration = DataHelper.GetLittleEndianBytesFromInt(iteration);

            return
            [
                0x02,
                0x07,
                (byte)keyId,
                finalizer,
                binaryLength[0],
                binaryLength[1],
                binaryIteration[0],
                binaryIteration[1],
            ];
        }
    }
}

// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Abstract base class for Stream Deck devices that use JPEG-encoded button images
    /// and share the standard key setup header format. Covers the Original, Original 2019,
    /// MK.2, XL, and XL 2022 models.
    /// </summary>
    public abstract class JpegButtonsDevice(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceImageFormat KeyImageFormat => DeviceImageFormat.Jpeg;

        /// <inheritdoc/>
        public override int KeyImageHeaderSize => 8;

        /// <inheritdoc/>
        public override int PacketSize => 1024;

        /// <inheritdoc/>
        public override int ScreenImageHeaderSize => 16;

        /// <inheritdoc/>
        public override DeviceRotation ImageRotation => DeviceRotation.Rotate180;

        /// <inheritdoc/>
        public override bool IsScreenSupported => false;

        /// <inheritdoc/>
        public override bool IsKnobSupported => false;

        /// <inheritdoc/>
        public override int TouchButtonCount => 0;

        /// <inheritdoc/>
        public override int ScreenWidth => -1;

        /// <inheritdoc/>
        public override int ScreenHeight => -1;

        /// <inheritdoc/>
        public override int ScreenSegmentWidth => -1;

        /// <inheritdoc/>
        public override bool SetScreen(byte[] image, int offset, int width, int height)
        {
            return false;
        }

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

        /// <inheritdoc/>
        protected override ButtonPressEventArgs HandleKeyPress(IAsyncResult result, byte[] keyPressBuffer)
        {
            int bytesRead = this.UnderlyingInputStream.EndRead(result);

            var buttonKind = GetButtonKind(new ArraySegment<byte>(keyPressBuffer, 0, 2).ToArray());
            var buttonCount = DataHelper.GetIntFromLittleEndianBytes(new ArraySegment<byte>(keyPressBuffer, 2, 2).ToArray());

            int pressedButton = Array.IndexOf(new ArraySegment<byte>(keyPressBuffer, 4, buttonCount).ToArray(), (byte)0x01);
            var eventKind = pressedButton != -1 ? ButtonEventKind.Down : ButtonEventKind.Up;

            return new ButtonPressEventArgs(pressedButton, eventKind, buttonKind, null, null, null);
        }
    }
}

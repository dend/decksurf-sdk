﻿// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck XL connected device.
    /// </summary>
    public class StreamDeckXL(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.XL;

        /// <inheritdoc/>
        public override int ButtonCount => 32;

        /// <inheritdoc/>
        public override bool IsScreenSupported => false;

        /// <inheritdoc/>
        public override bool IsKnobSupported => false;

        /// <inheritdoc/>
        public override int ButtonResolution => 96;

        /// <inheritdoc/>
        public override int ButtonColumns => 8;

        /// <inheritdoc/>
        public override int ButtonRows => 4;

        /// <inheritdoc/>
        public override int ScreenWidth => -1;

        /// <inheritdoc/>
        public override int ScreenHeight => -1;

        /// <inheritdoc/>
        public override int ScreenSegmentWidth => -1;

        /// <inheritdoc/>
        public override ImageFormat KeyImageFormat => ImageFormat.Jpeg;

        /// <inheritdoc/>
        public override int KeyImageHeaderSize => 8;

        /// <inheritdoc/>
        public override int PacketSize => 1024;

        /// <inheritdoc/>
        public override int ScreenImageHeaderSize => 16;

        /// <inheritdoc/>
        public override RotateFlipType FlipType => RotateFlipType.Rotate180FlipNone;

        /// <inheritdoc/>
        public override int TouchButtonCount => 0;

        /// <inheritdoc/>
        public override byte[] GetKeySetupHeader(int keyId, int sliceLength, int iteration, int remainingBytes)
        {
            byte finalizer = sliceLength == remainingBytes ? (byte)1 : (byte)0;
            var binaryLength = DataHelpers.GetLittleEndianBytesFromInt(sliceLength);
            var binaryIteration = DataHelpers.GetLittleEndianBytesFromInt(iteration);

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
        public override bool SetScreen(byte[] image, int offset, int width, int height)
        {
            return false;
        }

        /// <inheritdoc/>
        protected override ButtonPressEventArgs HandleKeyPress(IAsyncResult result, byte[] keyPressBuffer)
        {
            int bytesRead = this.UnderlyingInputStream.EndRead(result);

            var buttonKind = GetButtonKind(new ArraySegment<byte>(keyPressBuffer, 0, 2).ToArray());
            var buttonCount = DataHelpers.GetIntFromLittleEndianBytes(new ArraySegment<byte>(keyPressBuffer, 2, 2).ToArray());

            int pressedButton = Array.IndexOf(new ArraySegment<byte>(keyPressBuffer, 4, buttonCount).ToArray(), (byte)0x01);
            var eventKind = pressedButton != -1 ? ButtonEventKind.DOWN : ButtonEventKind.UP;

            return new ButtonPressEventArgs(pressedButton, eventKind, buttonKind, null, null, null);
        }
    }
}

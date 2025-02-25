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
    /// Implementation for a Stream Deck Mini (2022) connected device.
    /// </summary>
    public class StreamDeckMini2022(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.Mini2022;

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
                0x01,
                binaryIteration[0],
                binaryIteration[1],
                finalizer,
                (byte)keyId,
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

            if (keyPressBuffer[0] != 0x01)
            {
                return null;
            }

            var buttonData = new ArraySegment<byte>(keyPressBuffer, 1, 6).ToArray();
            int pressedButton = Array.IndexOf(buttonData, (byte)0x01);
            var eventKind = pressedButton != -1 ? ButtonEventKind.DOWN : ButtonEventKind.UP;

            return new ButtonPressEventArgs(pressedButton, eventKind, ButtonKind.Button, null, null, null);
        }
    }
}

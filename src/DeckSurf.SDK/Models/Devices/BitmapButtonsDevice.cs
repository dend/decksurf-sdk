// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Abstract base class for Stream Deck devices that use BMP-encoded button images.
    /// Covers the Mini and Mini 2022 models.
    /// </summary>
    public abstract class BitmapButtonsDevice(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceImageFormat KeyImageFormat => DeviceImageFormat.Bmp;

        /// <inheritdoc/>
        public override int KeyImageHeaderSize => 16;

        /// <inheritdoc/>
        public override int PacketSize => 1024;

        /// <inheritdoc/>
        public override int ScreenImageHeaderSize => 16;

        /// <inheritdoc/>
        public override DeviceRotation ImageRotation => DeviceRotation.Rotate270;

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
        public override void SetBrightness(byte percentage)
        {
            percentage = Math.Min(percentage, (byte)100);

            byte[] brightnessRequest = new byte[17];
            brightnessRequest[0] = 0x05;
            brightnessRequest[1] = 0x55;
            brightnessRequest[2] = 0xAA;
            brightnessRequest[3] = 0xD1;
            brightnessRequest[4] = 0x01;
            brightnessRequest[5] = percentage;

            using var stream = this.Open();
            stream.SetFeature(brightnessRequest);
        }

        /// <inheritdoc/>
        protected internal override byte[] GetKeySetupHeader(int keyId, int sliceLength, int iteration, int remainingBytes)
        {
            byte[] header = new byte[16];
            byte finalizer = (byte)(sliceLength == remainingBytes ? 1 : 0);
            var binaryIteration = DataHelper.GetLittleEndianBytesFromInt(iteration);

            header[0] = 0x02;
            header[1] = 0x01;
            header[2] = binaryIteration[0];
            header[3] = binaryIteration[1];
            header[4] = finalizer;
            header[5] = (byte)keyId;

            return header;
        }

        /// <inheritdoc/>
        protected override ButtonPressEventArgs HandleKeyPress(IAsyncResult result, byte[] keyPressBuffer)
        {
            ArgumentNullException.ThrowIfNull(keyPressBuffer);

            int bytesRead = this.UnderlyingInputStream.EndRead(result);

            if (keyPressBuffer[0] != 0x01)
            {
                return new ButtonPressEventArgs(-1, ButtonEventKind.Up, ButtonKind.Unknown, null, null, null);
            }

            var buttonData = new ArraySegment<byte>(keyPressBuffer, 1, 6).ToArray();
            int pressedButton = Array.IndexOf(buttonData, (byte)0x01);
            var eventKind = pressedButton != -1 ? ButtonEventKind.Down : ButtonEventKind.Up;

            return new ButtonPressEventArgs(pressedButton, eventKind, ButtonKind.Button, null, null, null);
        }
    }
}

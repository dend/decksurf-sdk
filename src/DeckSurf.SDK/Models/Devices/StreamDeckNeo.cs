// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Neo connected device.
    /// </summary>
    public class StreamDeckNeo(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.Neo;

        /// <inheritdoc/>
        public override int ButtonCount => 8;

        /// <inheritdoc/>
        public override bool IsScreenSupported => true;

        /// <inheritdoc/>
        public override bool IsKnobSupported => false;

        /// <inheritdoc/>
        public override int ButtonResolution => 96;

        /// <inheritdoc/>
        public override int ButtonColumns => 4;

        /// <inheritdoc/>
        public override int ButtonRows => 2;

        /// <inheritdoc/>
        public override int ScreenWidth => 248;

        /// <inheritdoc/>
        public override int ScreenHeight => 58;

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
            byte[] binaryOffset = DataHelpers.GetLittleEndianBytesFromInt(offset);
            byte[] binaryWidth = DataHelpers.GetLittleEndianBytesFromInt(width);
            byte[] binaryHeight = DataHelpers.GetLittleEndianBytesFromInt(height);

            var iteration = 0;
            var remainingBytes = image.Length;

            using var stream = this.Open();
            while (remainingBytes > 0)
            {
                var sliceLength = Math.Min(remainingBytes, (this.PacketSize - this.ScreenImageHeaderSize));
                var bytesSent = iteration * (this.PacketSize - this.ScreenImageHeaderSize);

                byte isLastChunk = sliceLength == remainingBytes ? (byte)1 : (byte)0;

                var binaryLength = DataHelpers.GetLittleEndianBytesFromInt(sliceLength);
                var binaryIteration = DataHelpers.GetLittleEndianBytesFromInt(iteration);

                byte[] header =
                [
                    0x02,
                    0x0B,
                    0x00,
                    isLastChunk,
                    binaryLength[0],
                    binaryLength[1],
                    binaryIteration[0],
                    binaryIteration[1],
                ];

                var payload = header.Concat(new ArraySegment<byte>(image, bytesSent, sliceLength)).ToArray();
                var padding = new byte[this.PacketSize - payload.Length];

                var finalPayload = payload.Concat(padding).ToArray();

                stream.Write(finalPayload);

                remainingBytes -= sliceLength;
                iteration++;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override ButtonPressEventArgs HandleKeyPress(IAsyncResult result, byte[] keyPressBuffer)
        {
            int bytesRead = this.UnderlyingInputStream.EndRead(result);

            var buttonKind = this.GetButtonKind(new ArraySegment<byte>(keyPressBuffer, 0, 2).ToArray());
            var buttonCount = DataHelpers.GetIntFromLittleEndianBytes(new ArraySegment<byte>(keyPressBuffer, 2, 2).ToArray());

            int pressedButton = Array.IndexOf(new ArraySegment<byte>(keyPressBuffer, 4, buttonCount).ToArray(), (byte)0x01);
            var eventKind = pressedButton != -1 ? ButtonEventKind.DOWN : ButtonEventKind.UP;

            return new ButtonPressEventArgs(pressedButton, eventKind, buttonKind, null, null, null);
        }
    }
}

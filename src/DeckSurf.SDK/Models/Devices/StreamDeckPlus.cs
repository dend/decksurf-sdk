// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Plus connected device.
    /// </summary>
    public class StreamDeckPlus(int vid, int pid, string path, string name, string serial)
        : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.Plus;

        /// <inheritdoc/>
        public override int ButtonCount => 8;

        /// <inheritdoc/>
        public override bool IsScreenSupported => true;

        /// <inheritdoc/>
        public override int KnobCount => 4;

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
            byte[] binaryOffset = DataHelpers.GetLittleEndianBytesFromInt(offset);
            byte[] binaryWidth = DataHelpers.GetLittleEndianBytesFromInt(width);
            byte[] binaryHeight = DataHelpers.GetLittleEndianBytesFromInt(height);

            var iteration = 0;
            var remainingBytes = image.Length;

            using var stream = this.Open();
            while (remainingBytes > 0)
            {
                var sliceLength = Math.Min(remainingBytes, this.PacketSize - this.ScreenImageHeaderSize);
                var bytesSent = iteration * (this.PacketSize - this.ScreenImageHeaderSize);

                byte isLastChunk = sliceLength == remainingBytes ? (byte)1 : (byte)0;

                var binaryLength = DataHelpers.GetLittleEndianBytesFromInt(sliceLength);
                var binaryIteration = DataHelpers.GetLittleEndianBytesFromInt(iteration);

                byte[] header =
                [
                    0x02,
                    0x0C,
                    binaryOffset[0],
                    binaryOffset[1],
                    0x00,
                    0x00,
                    binaryWidth[0],
                    binaryWidth[1],
                    binaryHeight[0],
                    binaryHeight[1],
                    isLastChunk,
                    binaryIteration[0],
                    binaryIteration[1],
                    binaryLength[0],
                    binaryLength[1],
                    0x00
                ];

                var payload = header.Concat(new ArraySegment<byte>(image, bytesSent, sliceLength)).ToArray();
                var padding = new byte[this.PacketSize - payload.Length];
                
                stream.Write([..payload, ..padding]);

                remainingBytes -= sliceLength;
                iteration++;
            }

            return true;
        }

        protected override IEnumerable<IDeckEvent> HandleInput(IAsyncResult result, byte[] buffer)
        {
            this.UnderlyingInputStream.EndRead(result);

            this._buttonStates ??= new byte[this.ButtonCount];
            this._knobStates ??= new byte[this.KnobCount];

            switch (buffer)
            {
                // Button change
                case [0x01, 0x00, ..]:
                    for (var i = 0; i < this.ButtonCount; i++)
                    {
                        if (this._buttonStates[i] != buffer[i + 4])
                        {
                            yield return buffer[i + 4] == 0 ? new ButtonUp(i) : new ButtonDown(i);
                        }

                        this._buttonStates[i] = buffer[i + 4];
                    }

                    break;

                // Screen Tapped
                case [0x01, 0x02, _, _, 0x01, _, var xTapLow, var xTapHigh, var yTapLow, var yTapHigh, ..]:
                    yield return new ScreenTap(xTapHigh << 8 | xTapLow, yTapHigh << 8 | yTapLow);
                    break;

                // Screen Long Held
                case [0x01, 0x02, _, _, 0x02, _, var xHoldLow, var xHoldHigh, var yHoldLow, var yHoldHigh, ..]:
                    yield return new ScreenHold(xHoldHigh << 8 | xHoldLow, yHoldHigh << 8 | yHoldLow);
                    break;

                // Screen Swiped
                case
                [
                    0x01, 0x02, _, _, 0x03, _, var xSwipeStartLow, var xSwipeStartHigh, var ySwipeStartLow,
                    var ySwipeStartHigh,
                    var xSwipeEndLow, var xSwipeEndHigh, var ySwipeEndLow, var ySwipeEndHigh, ..
                ]:
                    yield return new ScreenSwipe(
                        xSwipeStartHigh << 8 | xSwipeStartLow,
                        ySwipeStartHigh << 8 | ySwipeStartLow,
                        xSwipeEndHigh << 8 | xSwipeEndLow,
                        ySwipeEndHigh << 8 | ySwipeEndLow);
                    break;

                // Knob Rotated
                case [0x01, 0x03, _, _, 0x01, var rot1, var rot2, var rot3, var rot4, ..]:
                    for (var i = 0; i < this.KnobCount; i++)
                    {
                        var rot = buffer[i + 5];
                        if (rot == 0)
                        {
                            continue;
                        }

                        yield return (rot & 0x80) == 0x80
                            ? new KnobCounterClockwise(0, 256 - rot)
                            : new KnobClockwise(0, rot);
                    }

                    break;

                // Knob press change
                case [0x01, 0x03, _, _, 0x00, ..]:
                    for (var i = 0; i < this.KnobCount; i++)
                    {
                        if (this._knobStates[i] != buffer[i + 5])
                        {
                            yield return buffer[i + 5] == 0 ? new KnobUp(i) : new KnobDown(i);
                        }

                        this._knobStates[i] = buffer[i + 5];
                    }

                    break;
            }
        }
    }
}
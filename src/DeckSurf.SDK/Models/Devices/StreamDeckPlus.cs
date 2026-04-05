// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Plus connected device.
    /// </summary>
    public class StreamDeckPlus(int vid, int pid, string path, string name, string serial) : ScreenDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.Plus;

        /// <inheritdoc/>
        public override int ButtonCount => 8;

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

        /// <inheritdoc/>
        public override int ScreenImageHeaderSize => 16;

        /// <inheritdoc/>
        public override int TouchButtonCount => 0;

        /// <inheritdoc/>
        public override bool SetScreen(byte[] image, int offset, int width, int height)
        {
            byte[] binaryOffset = DataHelper.GetLittleEndianBytesFromInt(offset);
            byte[] binaryWidth = DataHelper.GetLittleEndianBytesFromInt(width);
            byte[] binaryHeight = DataHelper.GetLittleEndianBytesFromInt(height);

            var iteration = 0;
            var remainingBytes = image.Length;

            using var stream = this.Open();
            while (remainingBytes > 0)
            {
                var sliceLength = Math.Min(remainingBytes, this.PacketSize - this.ScreenImageHeaderSize);
                var bytesSent = iteration * (this.PacketSize - this.ScreenImageHeaderSize);

                byte isLastChunk = sliceLength == remainingBytes ? (byte)1 : (byte)0;

                var binaryLength = DataHelper.GetLittleEndianBytesFromInt(sliceLength);
                var binaryIteration = DataHelper.GetLittleEndianBytesFromInt(iteration);

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
                    0x00,
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
            var buttonMapOffset = 4;
            int bytesRead = this.UnderlyingInputStream.EndRead(result);

            // Let's grab the first two bytes to understand the type of button we're dealing with.
            // They can be:
            //    0x01 0x00 - Button
            //    0x01 0x02 - Touch screen
            //    0x01 0x03 - Knob
            var header = new ArraySegment<byte>(keyPressBuffer, 0, 2).ToArray();
            var buttonKind = GetButtonKind(header);
            var isKnobRotated = false;
            var knobRotationDirection = KnobRotationDirection.None;
            var buttonCount = DataHelper.GetIntFromLittleEndianBytes(new ArraySegment<byte>(keyPressBuffer, 2, 2).ToArray());

            // If this was not a touch screen, we should provide
            // dummy coordinates.
            TouchPoint touchPoint = new(-1, -1);

            if (buttonKind == ButtonKind.Screen)
            {
                var xCoord = new ArraySegment<byte>(keyPressBuffer, 6, 2).ToArray();
                var yCoord = new ArraySegment<byte>(keyPressBuffer, 8, 2).ToArray();

                touchPoint = new TouchPoint(DataHelper.GetIntFromLittleEndianBytes(xCoord), DataHelper.GetIntFromLittleEndianBytes(yCoord));
            }

            // For whatever reason, the number of knobs is reported as 5, even though
            // there are only 4 on the Stream Deck Plus. Because that's the only device
            // where that value is used today, let's make sure that we decrement by 1.
            // Also, for the knob, the header is 5 bytes long, because the fifth
            // byte tells us whether the knob is rotated or not.
            if (buttonKind == ButtonKind.Knob)
            {
                buttonCount -= 1;
                buttonMapOffset += 1;
            }

            var buttonData = new ArraySegment<byte>(keyPressBuffer, buttonMapOffset, buttonCount).ToArray();

            int pressedButton = -1;

            if (buttonKind == ButtonKind.Button || buttonKind == ButtonKind.Screen)
            {
                pressedButton = Array.IndexOf(buttonData, (byte)0x01);
            }
            else
            {
                isKnobRotated = keyPressBuffer[4] != (byte)0x00;

                pressedButton = Array.IndexOf(buttonData, (byte)0x01);
                if (pressedButton == -1)
                {
                    pressedButton = Array.IndexOf(buttonData, (byte)0xFF);

                    if (isKnobRotated)
                    {
                        knobRotationDirection = KnobRotationDirection.Left;
                    }
                }
                else
                {
                    if (isKnobRotated)
                    {
                        knobRotationDirection = KnobRotationDirection.Right;
                    }
                }
            }

            var eventKind = pressedButton != -1 ? ButtonEventKind.Down : ButtonEventKind.Up;

            return new ButtonPressEventArgs(pressedButton, eventKind, buttonKind, touchPoint, isKnobRotated, knobRotationDirection);
        }
    }
}

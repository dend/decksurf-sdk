// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Plus connected device.
    /// </summary>
    public class StreamDeckPlus(int vid, int pid, string path, string name, string serial) : ConnectedDevice(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.Plus;

        /// <inheritdoc/>
        public override int ButtonCount => 8;

        /// <inheritdoc/>
        public override bool IsScreenSupported => true;

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
            var buttonKind = this.GetButtonKind(header);
            var isKnobRotated = false;
            var knobRotationDirection = KnobRotationDirection.None;
            var buttonCount = DataHelpers.GetIntFromLittleEndianBytes(new ArraySegment<byte>(keyPressBuffer, 2, 2).ToArray());

            // If this was not a touch screen, we should provide
            // dummy coordinates.
            Point touchPoint = new() { X = -1, Y = -1 };

            if (buttonKind == ButtonKind.Screen)
            {
                var xCoord = new ArraySegment<byte>(keyPressBuffer, 6, 2).ToArray();
                var yCoord = new ArraySegment<byte>(keyPressBuffer, 8, 2).ToArray();

                touchPoint = new Point() { X = DataHelpers.GetIntFromLittleEndianBytes(xCoord), Y = DataHelpers.GetIntFromLittleEndianBytes(yCoord) };
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

            var eventKind = pressedButton != -1 ? ButtonEventKind.DOWN : ButtonEventKind.UP;

            return new ButtonPressEventArgs(pressedButton, eventKind, buttonKind, touchPoint, isKnobRotated, knobRotationDirection);
        }
    }
}

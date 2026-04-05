// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeckSurf.SDK.Exceptions;
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
        public override bool SetScreen(byte[] image, int xOffset, int yOffset, int width, int height)
        {
            return this.WriteScreenCommand(0x0C, image, xOffset, yOffset, width, height);
        }

        /// <summary>
        /// Sets a full-screen image on the Stream Deck Plus LCD.
        /// </summary>
        /// <param name="image">JPEG image data for the full 800x100 screen.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> is null.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        public void SetFullScreenImage(byte[] image)
        {
            this.WriteScreenCommand(0x08, image, 0, 0, this.ScreenWidth, this.ScreenHeight);
        }

        /// <summary>
        /// Sets the boot logo displayed when the Stream Deck Plus powers on.
        /// </summary>
        /// <param name="image">JPEG image data for the boot logo (800x100).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> is null.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        public void SetBootLogo(byte[] image)
        {
            this.WriteScreenCommand(0x09, image, 0, 0, this.ScreenWidth, this.ScreenHeight);
        }

        /// <summary>
        /// Sets the background image for the Stream Deck Plus LCD.
        /// </summary>
        /// <param name="image">JPEG image data for the background (800x100).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> is null.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        public void SetBackgroundImage(byte[] image)
        {
            this.WriteScreenCommand(0x0D, image, 0, 0, this.ScreenWidth, this.ScreenHeight);
        }

        /// <inheritdoc/>
        protected override ButtonPressEventArgs HandleKeyPress(IAsyncResult result, byte[] keyPressBuffer)
        {
            ArgumentNullException.ThrowIfNull(keyPressBuffer);

            var buttonMapOffset = 4;
            this.UnderlyingInputStream.EndRead(result);

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

            var pressedButtons = new List<int>();

            if (buttonKind == ButtonKind.Button || buttonKind == ButtonKind.Screen)
            {
                for (int i = 0; i < buttonData.Length; i++)
                {
                    if (buttonData[i] == 0x01)
                    {
                        pressedButtons.Add(i);
                    }
                }
            }
            else
            {
                isKnobRotated = keyPressBuffer[4] != (byte)0x00;

                for (int i = 0; i < buttonData.Length; i++)
                {
                    if (buttonData[i] == 0x01)
                    {
                        pressedButtons.Add(i);
                        if (isKnobRotated)
                        {
                            knobRotationDirection = KnobRotationDirection.Right;
                        }
                    }
                    else if (buttonData[i] == 0xFF)
                    {
                        pressedButtons.Add(i);
                        if (isKnobRotated)
                        {
                            knobRotationDirection = KnobRotationDirection.Left;
                        }
                    }
                }
            }

            var eventKind = pressedButtons.Count > 0 ? ButtonEventKind.Down : ButtonEventKind.Up;

            return new ButtonPressEventArgs(pressedButtons, eventKind, buttonKind, touchPoint, isKnobRotated, knobRotationDirection);
        }

        private bool WriteScreenCommand(byte command, byte[] image, int xOffset, int yOffset, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(image);

            byte[] binaryXOffset = DataHelper.GetLittleEndianBytesFromInt(xOffset);
            byte[] binaryYOffset = DataHelper.GetLittleEndianBytesFromInt(yOffset);
            byte[] binaryWidth = DataHelper.GetLittleEndianBytesFromInt(width);
            byte[] binaryHeight = DataHelper.GetLittleEndianBytesFromInt(height);

            var iteration = 0;
            var remainingBytes = image.Length;

            try
            {
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
                        command,
                        binaryXOffset[0],
                        binaryXOffset[1],
                        binaryYOffset[0],
                        binaryYOffset[1],
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
            }
            catch (ObjectDisposedException ex)
            {
                throw new DeviceDisconnectedException("Device was disconnected during screen command.", ex) { DeviceSerial = this.Serial };
            }
            catch (IOException ex)
            {
                throw new DeviceCommunicationException("USB communication error during screen command.", ex) { IsTransient = true };
            }

            return true;
        }
    }
}

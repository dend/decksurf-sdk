// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using DeckSurf.SDK.Util;
using HidSharp;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Abstract class representing a connected Stream Deck device. Use specific implementations for a given connected model.
    /// </summary>
    public abstract class ConnectedDevice
    {
        private byte[] keyPressBuffer = new byte[1024];

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedDevice"/> class.
        /// </summary>
        public ConnectedDevice()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedDevice"/> class with given device parameters.
        /// </summary>
        /// <param name="vid">Vendor ID.</param>
        /// <param name="pid">Product ID.</param>
        /// <param name="path">Path to the USB HID device.</param>
        /// <param name="name">Name of the USB HID device.</param>
        /// <param name="serial">Serial number for the device.</param>
        public ConnectedDevice(int vid, int pid, string path, string name, string serial)
        {
            this.VId = vid;
            this.Path = path;
            this.Name = name;
            this.Serial = serial;
            this.UnderlyingDevice = DeviceList.Local.GetHidDeviceOrNull(vid, pid);
        }

        /// <summary>
        /// Delegate responsible for handling Stream Deck button presses.
        /// </summary>
        /// <param name="source">The device where the button was pressed.</param>
        /// <param name="e">Information on the button press.</param>
        public delegate void ReceivedButtonPressHandler(object source, ButtonPressEventArgs e);

        /// <summary>
        /// Button press event handler.
        /// </summary>
        public event ReceivedButtonPressHandler OnButtonPress;

        /// <summary>
        /// Gets the vendor ID.
        /// </summary>
        public int VId { get; }

        /// <summary>
        /// Gets the USB HID device path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the USB HID device name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the device serial number.
        /// </summary>
        public string Serial { get; }

        /// <summary>
        /// Gets the Stream Deck device model.
        /// </summary>
        public abstract DeviceModel Model { get; }

        /// <summary>
        /// Gets the number of buttons on the connected Stream Deck device.
        /// </summary>
        public abstract int ButtonCount { get; }

        /// <summary>
        /// Gets a value indicating the flip type for the image sent to the device.
        /// </summary>
        public abstract RotateFlipType FlipType { get; }

        /// <summary>
        /// Gets a value indicating whether the Stream Deck device has a screen in addition to buttons.
        /// </summary>
        public abstract bool IsScreenSupported { get; }

        /// <summary>
        /// Gets a value indicating whether the Stream Deck has knobs.
        /// </summary>
        public abstract bool IsKnobSupported { get; }

        /// <summary>
        /// Gets a value indicating the button resolution for the Stream Deck device.
        /// </summary>
        public abstract int ButtonResolution { get; }

        /// <summary>
        /// Gets a value indicating the number of button columns for a Stream Deck device.
        /// </summary>
        public abstract int ButtonColumns { get; }

        /// <summary>
        /// Gets a value indicating the number of button rows for a Stream Deck device.
        /// </summary>
        public abstract int ButtonRows { get; }

        /// <summary>
        /// Gets screen width for the Stream Deck Plus.
        /// </summary>
        /// <remarks>
        /// Returns -1 if there is no screen supported.
        /// </remarks>
        public abstract int ScreenWidth { get; }

        /// <summary>
        /// Gets screen height for the Stream Deck device that supports a screen.
        /// </summary>
        /// <remarks>
        /// Returns -1 if there is no screen supported.
        /// </remarks>
        public abstract int ScreenHeight { get; }

        /// <summary>
        /// Gets screen width for a segment on the Stream Deck device that supports a screen.
        /// </summary>
        /// <remarks>
        /// Returns -1 if there is no screen supported.
        /// </remarks>
        public abstract int ScreenSegmentWidth { get; }

        /// <summary>
        /// Gets the image format used for individual keys on the Stream Deck device.
        /// </summary>
        public abstract ImageFormat KeyImageFormat { get; }

        /// <summary>
        /// Gets the size of the header for the packets used to set the key image.
        /// </summary>
        public abstract int KeyImageHeaderSize { get; }

        /// <summary>
        /// Gets the size of the packet used to set the image for a key or the screen.
        /// </summary>
        public abstract int PacketSize { get; }

        /// <summary>
        /// Gets the size of the header for the packets used to set the screen image.
        /// </summary>
        public abstract int ScreenImageHeaderSize { get; }

        internal HidDevice UnderlyingDevice { get; }

        internal HidStream UnderlyingInputStream { get; set; }

        /// <summary>
        /// Abstract method to get the device-specific header.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <param name="sliceLength">The length of the slice.</param>
        /// <param name="iteration">The iteration number.</param>
        /// <param name="remainingBytes">The remaining bytes to be sent.</param>
        /// <returns>The device-specific header as a byte array.</returns>
        public abstract byte[] GetKeySetupHeader(int keyId, int sliceLength, int iteration, int remainingBytes);

        /// <summary>
        /// Initialize the device and start reading the input stream.
        /// </summary>
        public void StartListening()
        {
            this.UnderlyingInputStream = this.UnderlyingDevice.Open();
            this.UnderlyingInputStream.ReadTimeout = Timeout.Infinite;
            this.UnderlyingInputStream.BeginRead(this.keyPressBuffer, 0, this.keyPressBuffer.Length, this.KeyPressCallback, null);
        }

        /// <summary>
        /// Stops listening for events for the specific device.
        /// </summary>
        public void StopListening()
        {
            this.UnderlyingInputStream.Close();
        }

        /// <summary>
        /// Open the underlying Stream Deck device.
        /// </summary>
        /// <returns>HID stream that can be read or written to.</returns>
        public HidStream Open()
        {
            return this.UnderlyingDevice.Open();
        }

        /// <summary>
        /// Clear the contents of the Stream Deck buttons.
        /// </summary>
        public void ClearButtons()
        {
            for (int i = 0; i < this.ButtonCount; i++)
            {
                this.SetKey(i, ImageHelpers.CreateBlankImage(this.ButtonResolution, Color.Black));
            }
        }

        /// <summary>
        /// Sets the brightness of the Stream Deck device display.
        /// </summary>
        /// <param name="percentage">Percentage, from 0 to 100, to which brightness should be set. Any values larger than 100 will be set to 100.</param>
        public void SetBrightness(byte percentage)
        {
            if (percentage > 100)
            {
                percentage = 100;
            }

            var brightnessRequest = new byte[]
            {
                0x03, 0x08, percentage, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };

            using var stream = this.Open();
            stream.SetFeature(brightnessRequest);
        }

        /// <summary>
        /// Sets up the button mapping to associated plugins.
        /// </summary>
        /// <param name="buttonMap">List of mappings, usually loaded from a configuration file.</param>
        public void SetupDeviceButtonMap(IEnumerable<CommandMapping> buttonMap)
        {
            foreach (var button in buttonMap)
            {
                if (button.ButtonIndex <= this.ButtonCount - 1)
                {
                    if (File.Exists(button.ButtonImagePath))
                    {
                        byte[] imageBuffer = File.ReadAllBytes(button.ButtonImagePath);

                        imageBuffer = ImageHelpers.ResizeImage(imageBuffer, this.ButtonResolution, this.ButtonResolution, this.FlipType, this.KeyImageFormat);
                        this.SetKey(button.ButtonIndex, imageBuffer);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the content of a key on a Stream Deck device.
        /// </summary>
        /// <param name="keyId">Numeric ID of the key that needs to be set.</param>
        /// <param name="image">Binary content (JPEG) of the image that needs to be set on the key. The image will be resized to match the expectations of the connected device.</param>
        /// <returns>True if succesful, false if not.</returns>
        public bool SetKey(int keyId, byte[] image)
        {
            var keyImage = ImageHelpers.ResizeImage(image, this.ButtonResolution, this.ButtonResolution, this.FlipType, this.KeyImageFormat);

            var iteration = 0;
            var remainingBytes = keyImage.Length;

            using var stream = this.Open();
            while (remainingBytes > 0)
            {
                var sliceLength = Math.Min(remainingBytes, (this.PacketSize - this.KeyImageHeaderSize));
                var bytesSent = iteration * (this.PacketSize - this.KeyImageHeaderSize);

                // Get the device-specific header
                byte[] header = this.GetKeySetupHeader(keyId, sliceLength, iteration, remainingBytes);

                var payload = header.Concat(new ArraySegment<byte>(keyImage, bytesSent, sliceLength)).ToArray();
                var padding = new byte[this.PacketSize - payload.Length];

                var finalPayload = payload.Concat(padding).ToArray();

                stream.Write(finalPayload);

                remainingBytes -= sliceLength;
                iteration++;
            }

            return true;
        }

        /// <summary>
        /// Sets the screen image for a connected Stream Deck device.
        /// </summary>
        /// <remarks>Currently only supported for the Stream Deck Plus.</remarks>
        /// <param name="image">Binary content (JPEG) of the image that needs to be set on the screen. The image will be resized to match the expectations of the connected device.</param>
        /// <param name="offset">Offset from the left where the image needs to be set. Set to zero if setting the full image.</param>
        /// <param name="width">Image height.</param>
        /// <param name="height">Image width.</param>
        /// <returns>True if succesful, false if not.</returns>
        public bool SetScreen(byte[] image, int offset, int width, int height)
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

                var finalPayload = payload.Concat(padding).ToArray();

                stream.Write(finalPayload);

                remainingBytes -= sliceLength;
                iteration++;
            }

            return true;
        }

        /// <summary>
        /// Handles the key press. Different devices carry different implementations.
        /// </summary>
        /// <param name="result">Result passed from the existing stream.</param>
        /// <param name="keyPressBuffer">Binary buffer related to the key press.</param>
        /// <returns>If successful, returns the event args related to the key press event.</returns>
        protected abstract ButtonPressEventArgs HandleKeyPress(IAsyncResult result, byte[] keyPressBuffer);

        private void KeyPressCallback(IAsyncResult result)
        {
            var args = this.HandleKeyPress(result, this.keyPressBuffer);

            if (this.OnButtonPress != null)
            {
                this.OnButtonPress(this.UnderlyingDevice, args);
            }

            Array.Clear(this.keyPressBuffer, 0, this.keyPressBuffer.Length);

            this.UnderlyingInputStream.BeginRead(this.keyPressBuffer, 0, this.keyPressBuffer.Length, this.KeyPressCallback, null);
        }

        internal ButtonKind GetButtonKind(byte[] identifier)
        {
            if (identifier.Length != 2)
            {
                return ButtonKind.Unknown;
            }

            return (identifier[0], identifier[1]) switch
            {
                (0x01, 0x00) => ButtonKind.Button,
                (0x01, 0x02) => ButtonKind.Screen,
                (0x01, 0x03) => ButtonKind.Knob,
                _ => ButtonKind.Unknown,
            };
        }
    }
}

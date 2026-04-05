// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DeckSurf.SDK.Exceptions;
using DeckSurf.SDK.Interfaces;
using DeckSurf.SDK.Util;
using HidSharp;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Abstract class representing a connected Stream Deck device. Use specific implementations for a given connected model.
    /// </summary>
    /// <remarks>
    /// This class is not thread-safe. Callers must synchronize access when invoking methods
    /// from multiple threads. In particular, <see cref="SetKey"/>, <see cref="SetBrightness"/>,
    /// and <see cref="SetKeyColor"/> should not be called concurrently for the same device.
    /// </remarks>
    public abstract class ConnectedDevice : IConnectedDevice
    {
        private byte[] keyPressBuffer = new byte[1024];
        private bool disposed;

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
            this.VendorId = vid;
            this.Path = path;
            this.Name = name;
            this.Serial = serial;
            this.UnderlyingDevice = DeviceList.Local.GetHidDevices()
                .FirstOrDefault(d => d.VendorID == vid && d.ProductID == pid && d.DevicePath == path);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConnectedDevice"/> class.
        /// </summary>
        ~ConnectedDevice()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Button press event handler.
        /// </summary>
        public event EventHandler<ButtonPressEventArgs> ButtonPressed;

        /// <summary>
        /// Event raised when the device is disconnected.
        /// </summary>
        public event EventHandler<EventArgs> DeviceDisconnected;

        /// <summary>
        /// Event raised when a device error occurs.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> DeviceErrorOccurred;

        /// <summary>
        /// Gets the vendor ID.
        /// </summary>
        public int VendorId { get; }

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
        /// Gets the count of touch buttons on the connected Stream Deck device.
        /// </summary>
        public abstract int TouchButtonCount { get; }

        /// <summary>
        /// Gets a value indicating the rotation applied to images for this device.
        /// </summary>
        public abstract DeviceRotation ImageRotation { get; }

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
        public abstract DeviceImageFormat KeyImageFormat { get; }

        /// <summary>
        /// Gets a value indicating whether the device is currently listening for button press events.
        /// </summary>
        public bool IsListening => this.UnderlyingInputStream != null && !this.disposed;

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
        /// Initialize the device and start reading the input stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the device is already listening. Call <see cref="StopListening"/> first.</exception>
        public void StartListening()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectedDevice));
            }

            if (this.UnderlyingInputStream != null)
            {
                throw new InvalidOperationException("The device is already listening. Call StopListening() before calling StartListening() again.");
            }

            this.UnderlyingInputStream = this.UnderlyingDevice.Open();
            this.UnderlyingInputStream.ReadTimeout = Timeout.Infinite;
            this.UnderlyingInputStream.BeginRead(this.keyPressBuffer, 0, this.keyPressBuffer.Length, this.KeyPressCallback, null);
        }

        /// <summary>
        /// Stops listening for events for the specific device.
        /// </summary>
        public void StopListening()
        {
            if (this.UnderlyingInputStream == null)
            {
                return;
            }

            try
            {
                this.UnderlyingInputStream.Close();
            }
            catch (ObjectDisposedException)
            {
                // Stream was already disposed; nothing to do.
            }

            this.UnderlyingInputStream = null;
        }

        /// <summary>
        /// Clear the contents of the Stream Deck buttons.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while clearing buttons.</exception>
        public void ClearButtons()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectedDevice));
            }

            for (int i = 0; i < this.ButtonCount; i++)
            {
                this.SetKey(i, ImageHelper.CreateBlankImage(this.ButtonResolution, DeviceColor.Black));
            }
        }

        /// <summary>
        /// Sets the brightness of the Stream Deck device display.
        /// </summary>
        /// <param name="percentage">Percentage, from 0 to 100, to which brightness should be set. Any values larger than 100 will be set to 100.</param>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while setting brightness.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        public virtual void SetBrightness(byte percentage)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectedDevice));
            }

            percentage = Math.Min(percentage, (byte)100);

            byte[] brightnessRequest = new byte[32];
            brightnessRequest[0] = 0x03;
            brightnessRequest[1] = 0x08;
            brightnessRequest[2] = percentage;

            try
            {
                using var stream = this.Open();
                stream.SetFeature(brightnessRequest);
            }
            catch (ObjectDisposedException ex)
            {
                throw new DeviceDisconnectedException("Device was disconnected during SetBrightness operation.", ex) { DeviceSerial = this.Serial };
            }
            catch (IOException ex)
            {
                throw new DeviceCommunicationException("USB communication error during SetBrightness.", ex) { IsTransient = true };
            }
        }

        /// <summary>
        /// Sets up the button mapping to associated plugins.
        /// </summary>
        /// <param name="buttonMap">List of mappings, usually loaded from a configuration file.</param>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="buttonMap"/> is <c>null</c>.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while setting a button image.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        public void SetupDeviceButtonMap(IEnumerable<CommandMapping> buttonMap)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectedDevice));
            }

            if (buttonMap == null)
            {
                throw new ArgumentNullException(nameof(buttonMap));
            }

            foreach (var button in buttonMap)
            {
                if (button.ButtonIndex <= this.ButtonCount - 1)
                {
                    if (!string.IsNullOrEmpty(button.ButtonImagePath) && File.Exists(button.ButtonImagePath))
                    {
                        byte[] imageBuffer;
                        try
                        {
                            imageBuffer = File.ReadAllBytes(button.ButtonImagePath);
                        }
                        catch (IOException)
                        {
                            continue;
                        }

                        imageBuffer = ImageHelper.ResizeImage(imageBuffer, this.ButtonResolution, this.ButtonResolution, this.ImageRotation, this.KeyImageFormat);
                        this.SetKey(button.ButtonIndex, imageBuffer, alreadyResized: true);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the content of a key on a Stream Deck device.
        /// </summary>
        /// <param name="keyId">Numeric ID of the key that needs to be set.</param>
        /// <param name="image">Binary content of the image (supports JPEG, PNG, BMP, GIF, and other formats recognized by ImageSharp) that needs to be set on the key. The image will be resized to match the expectations of the connected device.</param>
        /// <param name="alreadyResized">If true, the image is assumed to already be resized and will not be resized again.</param>
        /// <returns>True if successful. This method throws on failure rather than returning false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="keyId"/> is outside the valid button range.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="image"/> is null or empty.</exception>
        /// <exception cref="Exceptions.ImageProcessingException">Thrown when the image buffer is not a recognized format (only when <paramref name="alreadyResized"/> is false).</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while writing the key image.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        public bool SetKey(int keyId, byte[] image, bool alreadyResized = false)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectedDevice));
            }

            if (keyId < 0 || keyId >= this.ButtonCount)
            {
                throw new ArgumentOutOfRangeException(nameof(keyId), $"Key ID must be between 0 and {this.ButtonCount - 1}.");
            }

            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("Image must not be null or empty.", nameof(image));
            }

            var keyImage = alreadyResized ? image : ImageHelper.ResizeImage(image, this.ButtonResolution, this.ButtonResolution, this.ImageRotation, this.KeyImageFormat);

            var iteration = 0;
            var remainingBytes = keyImage.Length;

            try
            {
                using var stream = this.Open();
                while (remainingBytes > 0)
                {
                    var sliceLength = Math.Min(remainingBytes, this.PacketSize - this.KeyImageHeaderSize);
                    var bytesSent = iteration * (this.PacketSize - this.KeyImageHeaderSize);

                    // Get the device-specific header
                    byte[] header = this.GetKeySetupHeader(keyId, sliceLength, iteration, remainingBytes);

                    byte[] finalPayload = new byte[this.PacketSize];
                    Buffer.BlockCopy(header, 0, finalPayload, 0, header.Length);
                    Buffer.BlockCopy(keyImage, bytesSent, finalPayload, header.Length, sliceLength);

                    stream.Write(finalPayload);

                    remainingBytes -= sliceLength;
                    iteration++;
                }
            }
            catch (IOException ex)
            {
                throw new DeviceCommunicationException("A USB I/O failure occurred while writing the key image.", ex) { IsTransient = true };
            }
            catch (ObjectDisposedException ex)
            {
                throw new DeviceDisconnectedException("The device was disconnected during the SetKey operation.", ex) { DeviceSerial = this.Serial };
            }

            return true;
        }

        /// <summary>
        /// Sets the key color to a specified color.
        /// </summary>
        /// <remarks>
        /// Only supported on the Stream Deck Neo at this time.
        /// </remarks>
        /// <param name="index">Key index where the color must be set.</param>
        /// <param name="color">Color to set the key to.</param>
        /// <returns>True if successful. This method throws on failure rather than returning false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="index"/> does not represent a valid key.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while setting the key color.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        public bool SetKeyColor(int index, DeviceColor color)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectedDevice));
            }

            if (index < 0 || index >= this.ButtonCount + this.TouchButtonCount)
            {
                throw new IndexOutOfRangeException($"The index {index} for the touch key does not represent a real touch key.");
            }

            byte[] payload = new byte[32];

            payload[0] = 0x03;
            payload[1] = 0x06;
            payload[2] = (byte)index;
            payload[3] = color.R;
            payload[4] = color.G;
            payload[5] = color.B;

            try
            {
                using var stream = this.Open();
                stream.SetFeature(payload);
            }
            catch (IOException ex)
            {
                throw new DeviceCommunicationException("A USB I/O failure occurred while setting the key color.", ex) { IsTransient = true };
            }
            catch (ObjectDisposedException ex)
            {
                throw new DeviceDisconnectedException("The device was disconnected during the SetKeyColor operation.", ex) { DeviceSerial = this.Serial };
            }

            return true;
        }

        /// <summary>
        /// Sets the screen image for a connected Stream Deck device.
        /// </summary>
        /// <remarks>Supported on devices where <see cref="IsScreenSupported"/> is true (e.g., Stream Deck Plus and Neo).</remarks>
        /// <param name="image">Binary content of the image that needs to be set on the screen.</param>
        /// <param name="offset">Offset from the left where the image needs to be set. Set to zero if setting the full image.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <returns>True if successful. Returns false if the device does not support a screen. Throws on I/O failure.</returns>
        public abstract bool SetScreen(byte[] image, int offset, int width, int height);

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal static ButtonKind GetButtonKind(byte[] identifier)
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

        /// <summary>
        /// Gets the kind of button from a span-based identifier.
        /// </summary>
        /// <param name="identifier">A read-only span of bytes representing the button identifier.</param>
        /// <returns>The kind of button represented by the identifier.</returns>
        internal static ButtonKind GetButtonKind(ReadOnlySpan<byte> identifier)
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

        /// <summary>
        /// Open the underlying Stream Deck device.
        /// </summary>
        /// <returns>HID stream that can be read or written to.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        internal HidStream Open()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectedDevice));
            }

            if (this.UnderlyingDevice == null)
            {
                throw new InvalidOperationException("The underlying HID device is not available. The device may not have been found during initialization.");
            }

            return this.UnderlyingDevice.Open();
        }

        /// <summary>
        /// Abstract method to get the device-specific header.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <param name="sliceLength">The length of the slice.</param>
        /// <param name="iteration">The iteration number.</param>
        /// <param name="remainingBytes">The remaining bytes to be sent.</param>
        /// <returns>The device-specific header as a byte array.</returns>
        protected internal abstract byte[] GetKeySetupHeader(int keyId, int sliceLength, int iteration, int remainingBytes);

        /// <summary>
        /// Handles the key press. Different devices carry different implementations.
        /// </summary>
        /// <param name="result">Result passed from the existing stream.</param>
        /// <param name="keyPressBuffer">Binary buffer related to the key press.</param>
        /// <returns>If successful, returns the event args related to the key press event.</returns>
        protected abstract ButtonPressEventArgs HandleKeyPress(IAsyncResult result, byte[] keyPressBuffer);

        /// <summary>
        /// Releases unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.UnderlyingInputStream != null)
                {
                    try
                    {
                        this.UnderlyingInputStream.Close();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Already disposed; ignore.
                    }

                    this.UnderlyingInputStream = null;
                }
            }

            this.disposed = true;
        }

        private void KeyPressCallback(IAsyncResult result)
        {
            ButtonPressEventArgs args;

            try
            {
                args = this.HandleKeyPress(result, this.keyPressBuffer);
            }
            catch (ObjectDisposedException)
            {
                // Device was disconnected.
                this.DeviceDisconnected?.Invoke(this, EventArgs.Empty);
                return;
            }
            catch (IOException ex)
            {
                this.DeviceErrorOccurred?.Invoke(this, new DeviceErrorEventArgs(ex, DeviceErrorCategory.CommunicationFailure, isTransient: true, recoveryHint: "Check the USB connection and try reconnecting the device.", operationName: nameof(this.KeyPressCallback)));
                return;
            }

            if (args != null)
            {
                this.ButtonPressed?.Invoke(this, args);
            }

            Array.Clear(this.keyPressBuffer, 0, this.keyPressBuffer.Length);

            var stream = this.UnderlyingInputStream;
            if (stream == null)
            {
                return;
            }

            try
            {
                stream.BeginRead(this.keyPressBuffer, 0, this.keyPressBuffer.Length, this.KeyPressCallback, null);
            }
            catch (ObjectDisposedException)
            {
                // Stream was closed during callback; exit gracefully.
            }
        }
    }
}

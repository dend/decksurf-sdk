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
        private byte[] inputBuffer = new byte[1024];

        protected byte[] _buttonStates = null!;

        protected byte[] _knobStates = null!;

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
        /// Delegate responsible for handling Stream Deck button press
        /// </summary>
        /// <param name="source">The device where the button was pressed.</param>
        /// <param name="e">Information on the button press</param>
        public delegate void ReceivedButtonDownHandler(object source, ButtonDown e);

        /// <summary>
        /// Button press event handler
        /// </summary>
        public event ReceivedButtonDownHandler OnButtonDown;

        /// <summary>
        /// Delegate responsible for handling Stream Deck button release
        /// </summary>
        /// <param name="source">The device where the button was released.</param>
        /// <param name="e">Information on the button release</param>
        public delegate void ReceivedButtonUpHandler(object source, ButtonUp e);

        /// <summary>
        /// Button release event handler
        /// </summary>
        public event ReceivedButtonUpHandler OnButtonUp;

        /// <summary>
        /// Delegate responsible for handling Stream Deck knob press
        /// </summary>
        /// <param name="source">The device where the knob was pressed.</param>
        /// <param name="e">Information on the knob press</param>
        public delegate void ReceivedKnobDownHandler(object source, KnobDown e);

        /// <summary>
        /// Knob press event handler
        /// </summary>
        public event ReceivedKnobDownHandler OnKnobDown;

        /// <summary>
        /// Delegate responsible for handling Stream Deck knob release
        /// </summary>
        /// <param name="source">The device where the knob was released.</param>
        /// <param name="e">Information on the knob release</param>
        public delegate void ReceivedKnobUpHandler(object source, KnobUp e);

        /// <summary>
        /// Knob release handler
        /// </summary>
        public event ReceivedKnobUpHandler OnKnobUp;

        /// <summary>
        /// Delegate responsible for handling Stream Deck knob clockwise rotation
        /// </summary>
        /// <param name="source">The device where the knob was rotated</param>
        /// <param name="e">Information on the kno rotation</param>
        public delegate void ReceivedKnobClockwiseHandler(object source, KnobClockwise e);

        /// <summary>
        /// Knob clockwise rotation handler
        /// </summary>
        public event ReceivedKnobClockwiseHandler OnKnobClockwise;

        /// <summary>
        /// Delegate responsible for handling Stream Deck knob counterclockwise rotation
        /// </summary>
        /// <param name="source">The device where the knob was rotated</param>
        /// <param name="e">Information on the knob rotation</param>
        public delegate void ReceivedKnobCounterClockwiseHandler(object source, KnobCounterClockwise e);

        /// <summary>
        /// Knob counterclockwise rotation handler
        /// </summary>
        public event ReceivedKnobCounterClockwiseHandler OnKnobCounterClockwise;

        /// <summary>
        /// Delegate responsible for handling Stream Deck screen tap
        /// </summary>
        /// <param name="source">The device where the screen was tapped</param>
        /// <param name="e">Information on the screen tap</param>
        public delegate void ReceivedScreenTapHandler(object source, ScreenTap e);

        /// <summary>
        /// Screen tap handler
        /// </summary>
        public event ReceivedScreenTapHandler OnScreenTap;

        /// <summary>
        /// Delegate responsible for handling Stream Deck screen hold
        /// </summary>
        /// <param name="source">The device where the screen was long-held</param>
        /// <param name="e">Information on the screen hold</param>
        public delegate void ReceivedScreenHoldHandler(object source, ScreenHold e);

        /// <summary>
        /// Screen hold handler
        /// </summary>
        public event ReceivedScreenHoldHandler OnScreenHold;

        /// <summary>
        /// Delegate responsible for handling Stream Deck screen swipe
        /// </summary>
        /// <param name="source">The device where the screen was swiped</param>
        /// <param name="e">Information on the screen swipe</param>
        public delegate void ReceivedScreenSwipeHandler(object source, ScreenSwipe e);

        /// <summary>
        /// Screen swipe handler
        /// </summary>
        public event ReceivedScreenSwipeHandler OnScreenSwipe;

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
        /// Gets the count of touch buttons on the connected Stream Deck device.
        /// </summary>
        public abstract int TouchButtonCount { get; }

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
        public bool IsKnobSupported => this.KnobCount != 0;

        /// <summary>
        /// Gets a value indicating the number of knobs the Stream Deck has
        /// </summary>
        public abstract int KnobCount { get; }

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
            this.UnderlyingInputStream.BeginRead(this.inputBuffer, 0, this.inputBuffer.Length, this.InputCallback, null);
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
        public virtual void SetBrightness(byte percentage)
        {
            percentage = Math.Min(percentage, (byte)100);

            byte[] brightnessRequest = new byte[32];
            brightnessRequest[0] = 0x03;
            brightnessRequest[1] = 0x08;
            brightnessRequest[2] = percentage;

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
                var sliceLength = Math.Min(remainingBytes, this.PacketSize - this.KeyImageHeaderSize);
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
        /// Sets the key color to a specified color.
        /// </summary>
        /// <remarks>
        /// Only supported on the Stream Deck Neo at this time.
        /// </remarks>
        /// <param name="index">Key index where the color must be set.</param>
        /// <param name="color">Color to set the key to.</param>
        /// <returns>If successful, returns true. Otherwise, false (including in scenarios where it's not available).</returns>
        public bool SetKeyColor(int index, Color color)
        {
            if (Math.Min(Math.Max(index, 0), this.ButtonCount + this.TouchButtonCount - 1) != index)
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

            using var stream = this.Open();
            stream.SetFeature(payload);

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
        public abstract bool SetScreen(byte[] image, int offset, int width, int height);

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
        /// Handles the inputs. Different devices carry different implementations.
        /// </summary>
        /// <param name="result">Result passed from the existing stream.</param>
        /// <param name="buffer">Binary buffer related to the key press.</param>
        /// <returns>If successful, returns the event args related to the key press event.</returns>
        protected abstract IEnumerable<IDeckEvent> HandleInput(IAsyncResult result, byte[] buffer);

        private void InputCallback(IAsyncResult result)
        {
            foreach (var @event in this.HandleInput(result, this.inputBuffer))
            {
                switch (@event)
                {
                    case ButtonDown e: this.OnButtonDown?.Invoke(this.UnderlyingDevice, e); break;
                    case ButtonUp e: this.OnButtonUp?.Invoke(this.UnderlyingDevice, e); break;
                    case KnobDown e: this.OnKnobDown?.Invoke(this.UnderlyingDevice, e); break;
                    case KnobUp e: this.OnKnobUp?.Invoke(this.UnderlyingDevice, e); break;
                    case KnobClockwise e: this.OnKnobClockwise?.Invoke(this.UnderlyingDevice, e); break;
                    case KnobCounterClockwise e: this.OnKnobCounterClockwise?.Invoke(this.UnderlyingDevice, e); break;
                    case ScreenTap e: this.OnScreenTap?.Invoke(this.UnderlyingDevice, e); break;
                    case ScreenHold e: this.OnScreenHold?.Invoke(this.UnderlyingDevice, e); break;
                    case ScreenSwipe e: this.OnScreenSwipe?.Invoke(this.UnderlyingDevice, e); break;
                }
            }

            Array.Clear(this.inputBuffer, 0, this.inputBuffer.Length);

            this.UnderlyingInputStream.BeginRead(this.inputBuffer, 0, this.inputBuffer.Length, this.InputCallback, null);
        }
    }
}

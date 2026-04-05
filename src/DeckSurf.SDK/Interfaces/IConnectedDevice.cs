// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using DeckSurf.SDK.Models;
using HidSharp;

namespace DeckSurf.SDK.Interfaces
{
    /// <summary>
    /// Interface representing a connected Stream Deck device.
    /// </summary>
    public interface IConnectedDevice : IDisposable
    {
        /// <summary>
        /// Event raised when a button is pressed on the device.
        /// </summary>
        event EventHandler<ButtonPressEventArgs> OnButtonPress;

        /// <summary>
        /// Event raised when the device is disconnected.
        /// </summary>
        event EventHandler<EventArgs> OnDeviceDisconnected;

        /// <summary>
        /// Event raised when a device error occurs.
        /// </summary>
        event EventHandler<Exception> OnDeviceError;

        /// <summary>
        /// Gets the vendor ID.
        /// </summary>
        int VendorId { get; }

        /// <summary>
        /// Gets the USB HID device path.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the USB HID device name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the device serial number.
        /// </summary>
        string Serial { get; }

        /// <summary>
        /// Gets the Stream Deck device model.
        /// </summary>
        DeviceModel Model { get; }

        /// <summary>
        /// Gets the number of buttons on the connected Stream Deck device.
        /// </summary>
        int ButtonCount { get; }

        /// <summary>
        /// Gets the count of touch buttons on the connected Stream Deck device.
        /// </summary>
        int TouchButtonCount { get; }

        /// <summary>
        /// Gets a value indicating the flip type for the image sent to the device.
        /// </summary>
        DeviceRotation FlipType { get; }

        /// <summary>
        /// Gets a value indicating whether the Stream Deck device has a screen in addition to buttons.
        /// </summary>
        bool IsScreenSupported { get; }

        /// <summary>
        /// Gets a value indicating whether the Stream Deck has knobs.
        /// </summary>
        bool IsKnobSupported { get; }

        /// <summary>
        /// Gets a value indicating the button resolution for the Stream Deck device.
        /// </summary>
        int ButtonResolution { get; }

        /// <summary>
        /// Gets a value indicating the number of button columns for a Stream Deck device.
        /// </summary>
        int ButtonColumns { get; }

        /// <summary>
        /// Gets a value indicating the number of button rows for a Stream Deck device.
        /// </summary>
        int ButtonRows { get; }

        /// <summary>
        /// Gets screen width for the Stream Deck device that supports a screen.
        /// </summary>
        int ScreenWidth { get; }

        /// <summary>
        /// Gets screen height for the Stream Deck device that supports a screen.
        /// </summary>
        int ScreenHeight { get; }

        /// <summary>
        /// Gets screen width for a segment on the Stream Deck device that supports a screen.
        /// </summary>
        int ScreenSegmentWidth { get; }

        /// <summary>
        /// Gets the image format used for individual keys on the Stream Deck device.
        /// </summary>
        DeviceImageFormat KeyImageFormat { get; }

        /// <summary>
        /// Gets the size of the header for the packets used to set the key image.
        /// </summary>
        int KeyImageHeaderSize { get; }

        /// <summary>
        /// Gets the size of the packet used to set the image for a key or the screen.
        /// </summary>
        int PacketSize { get; }

        /// <summary>
        /// Gets the size of the header for the packets used to set the screen image.
        /// </summary>
        int ScreenImageHeaderSize { get; }

        /// <summary>
        /// Initialize the device and start reading the input stream.
        /// </summary>
        void StartListening();

        /// <summary>
        /// Stops listening for events for the specific device.
        /// </summary>
        void StopListening();

        /// <summary>
        /// Open the underlying Stream Deck device.
        /// </summary>
        /// <returns>HID stream that can be read or written to.</returns>
        HidStream Open();

        /// <summary>
        /// Clear the contents of the Stream Deck buttons.
        /// </summary>
        void ClearButtons();

        /// <summary>
        /// Sets the brightness of the Stream Deck device display.
        /// </summary>
        /// <param name="percentage">Percentage, from 0 to 100, to which brightness should be set.</param>
        void SetBrightness(byte percentage);

        /// <summary>
        /// Sets up the button mapping to associated plugins.
        /// </summary>
        /// <param name="buttonMap">List of mappings, usually loaded from a configuration file.</param>
        void SetupDeviceButtonMap(IEnumerable<CommandMapping> buttonMap);

        /// <summary>
        /// Sets the content of a key on a Stream Deck device.
        /// </summary>
        /// <param name="keyId">Numeric ID of the key that needs to be set.</param>
        /// <param name="image">Binary content of the image that needs to be set on the key.</param>
        /// <param name="resize">Indicates whether the image should be resized to match the device resolution.</param>
        /// <returns>True if successful, false if not.</returns>
        bool SetKey(int keyId, byte[] image, bool resize);

        /// <summary>
        /// Sets the key color to a specified color.
        /// </summary>
        /// <param name="index">Key index where the color must be set.</param>
        /// <param name="color">Color to set the key to.</param>
        /// <returns>If successful, returns true. Otherwise, false.</returns>
        bool SetKeyColor(int index, DeviceColor color);

        /// <summary>
        /// Sets the screen image for a connected Stream Deck device.
        /// </summary>
        /// <param name="image">Binary content of the image that needs to be set on the screen.</param>
        /// <param name="offset">Offset from the left where the image needs to be set.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <returns>True if successful, false if not.</returns>
        bool SetScreen(byte[] image, int offset, int width, int height);

        /// <summary>
        /// Gets the device-specific header for key image payloads.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <param name="sliceLength">The length of the slice.</param>
        /// <param name="iteration">The iteration number.</param>
        /// <param name="remainingBytes">The remaining bytes to be sent.</param>
        /// <returns>The device-specific header as a byte array.</returns>
        byte[] GetKeySetupHeader(int keyId, int sliceLength, int iteration, int remainingBytes);
    }
}

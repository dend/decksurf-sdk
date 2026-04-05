// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using DeckSurf.SDK.Core;
using DeckSurf.SDK.Exceptions;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Interfaces
{
    /// <summary>
    /// Interface representing a connected Stream Deck device.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are not thread-safe. Callers must synchronize access when
    /// invoking methods from multiple threads. In particular, <see cref="SetKey"/>, <see cref="SetBrightness"/>,
    /// and <see cref="SetKeyColor"/> should not be called concurrently for the same device instance.
    /// </remarks>
    public interface IConnectedDevice : IDisposable
    {
        /// <summary>
        /// Event raised when a button is pressed on the device.
        /// </summary>
        /// <remarks>
        /// The sender is the <see cref="IConnectedDevice"/> instance that detected the press.
        /// </remarks>
        event EventHandler<ButtonPressEventArgs> ButtonPressed;

        /// <summary>
        /// Event raised when the device is disconnected.
        /// </summary>
        /// <remarks>
        /// After this event fires, the device should be considered unusable. Calling methods on a disconnected device may throw <see cref="ObjectDisposedException"/>.
        /// After this event fires, the device instance is no longer usable and should be disposed.
        /// To reconnect, call <see cref="DeviceManager.GetDeviceBySerial"/> or <see cref="DeviceManager.GetDeviceList"/>
        /// to obtain a fresh device instance.
        /// </remarks>
        event EventHandler<EventArgs> DeviceDisconnected;

        /// <summary>
        /// Event raised when a device error occurs.
        /// </summary>
        /// <remarks>
        /// Subscribers can inspect <see cref="DeviceErrorEventArgs.Category"/> and <see cref="DeviceErrorEventArgs.IsTransient"/> to determine whether the error is recoverable.
        /// </remarks>
        event EventHandler<DeviceErrorEventArgs> DeviceErrorOccurred;

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
        /// Gets a value indicating the rotation applied to images for this device.
        /// </summary>
        DeviceRotation ImageRotation { get; }

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
        /// Gets a value indicating whether the device is currently listening for button press events.
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Initialize the device and start reading the input stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the device is already listening.</exception>
        void StartListening();

        /// <summary>
        /// Stops listening for events for the specific device.
        /// </summary>
        /// <remarks>This method is safe to call multiple times and will not throw exceptions.</remarks>
        void StopListening();

        /// <summary>
        /// Clear the contents of the Stream Deck buttons.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while clearing buttons.</exception>
        void ClearButtons();

        /// <summary>
        /// Sets the brightness of the Stream Deck device display.
        /// </summary>
        /// <param name="percentage">Percentage, from 0 to 100, to which brightness should be set.</param>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while setting brightness.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        void SetBrightness(byte percentage);

        /// <summary>
        /// Sets up the button mapping to associated plugins.
        /// </summary>
        /// <param name="buttonMap">List of mappings, usually loaded from a configuration file.</param>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="buttonMap"/> is <c>null</c>.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while setting a button image.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        void SetupDeviceButtonMap(IEnumerable<CommandMapping> buttonMap);

        /// <summary>
        /// Sets the content of a key on a Stream Deck device.
        /// </summary>
        /// <param name="keyId">Numeric ID of the key that needs to be set.</param>
        /// <param name="image">Binary content of the image (supports JPEG, PNG, BMP, GIF, and other formats recognized by ImageSharp) that needs to be set on the key.</param>
        /// <param name="alreadyResized">If true, the image is assumed to already be resized and will not be resized again.</param>
        /// <returns>True if successful. This method throws on failure rather than returning false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="keyId"/> is outside the valid button range.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="image"/> is null or empty.</exception>
        /// <exception cref="ImageProcessingException">Thrown when the image buffer is not a recognized format (only when <paramref name="alreadyResized"/> is false).</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while writing the key image.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        bool SetKey(int keyId, byte[] image, bool alreadyResized = false);

        /// <summary>
        /// Sets the key color to a specified color.
        /// </summary>
        /// <param name="index">Key index where the color must be set.</param>
        /// <param name="color">Color to set the key to.</param>
        /// <returns>True if successful. This method throws on failure rather than returning false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="index"/> does not represent a valid key.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while setting the key color.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        bool SetKeyColor(int index, DeviceColor color);

        /// <summary>
        /// Sets the screen image for a connected Stream Deck device.
        /// </summary>
        /// <param name="image">Binary content of the image that needs to be set on the screen.</param>
        /// <param name="offset">Offset from the left where the image needs to be set.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <returns>True if successful. Returns false if the device does not support a screen. Throws on I/O failure.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the device has been disposed.</exception>
        /// <exception cref="DeviceCommunicationException">Thrown when a USB I/O failure occurs while setting the screen image.</exception>
        /// <exception cref="DeviceDisconnectedException">Thrown when the device is disconnected during the operation.</exception>
        bool SetScreen(byte[] image, int offset, int width, int height);
    }
}

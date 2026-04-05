// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Core
{
    /// <summary>
    /// Watches for the connection and disconnection of a specific Stream Deck device
    /// identified by its serial number. Subscribe to <see cref="DeviceConnected"/> and
    /// <see cref="DeviceLost"/> to react to hardware changes without polling.
    /// </summary>
    public class DeviceWatcher : IDisposable
    {
        private readonly string deviceSerial;
        private readonly object watcherLock = new();
        private bool isRunning;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceWatcher"/> class that monitors
        /// the device with the specified serial number.
        /// </summary>
        /// <param name="deviceSerial">
        /// The serial number of the device to watch. Must not be <c>null</c> or empty.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="deviceSerial"/> is <c>null</c> or empty.
        /// </exception>
        public DeviceWatcher(string deviceSerial)
        {
            if (string.IsNullOrEmpty(deviceSerial))
            {
                throw new ArgumentException("Device serial must not be null or empty.", nameof(deviceSerial));
            }

            this.deviceSerial = deviceSerial;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DeviceWatcher"/> class.
        /// </summary>
        ~DeviceWatcher()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Event raised when the watched device is connected. The event arguments contain
        /// the fully initialized <see cref="ConnectedDevice"/> instance.
        /// </summary>
        public event EventHandler<ConnectedDevice> DeviceConnected;

        /// <summary>
        /// Event raised when the watched device is disconnected.
        /// </summary>
        public event EventHandler DeviceLost;

        /// <summary>
        /// Gets the currently connected device, or <c>null</c> if the device is not connected.
        /// </summary>
        public ConnectedDevice CurrentDevice { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the watched device is currently connected.
        /// </summary>
        public bool IsConnected => this.CurrentDevice != null;

        /// <summary>
        /// Starts watching for device connection and disconnection events.
        /// If the device is already connected when this method is called, the
        /// <see cref="DeviceConnected"/> event is raised immediately.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the watcher has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the watcher is already running.</exception>
        public void Start()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(DeviceWatcher));
            }

            lock (this.watcherLock)
            {
                if (this.isRunning)
                {
                    throw new InvalidOperationException("The watcher is already running. Call Stop() before calling Start() again.");
                }

                this.isRunning = true;
            }

            DeviceManager.DeviceListChanged += this.OnDeviceListChanged;

            // Check whether the device is already connected.
            if (DeviceManager.TryGetDeviceBySerial(this.deviceSerial, out var device))
            {
                this.CurrentDevice = device;
                this.DeviceConnected?.Invoke(this, device);
            }
        }

        /// <summary>
        /// Stops watching for device connection and disconnection events.
        /// </summary>
        public void Stop()
        {
            lock (this.watcherLock)
            {
                if (!this.isRunning)
                {
                    return;
                }

                this.isRunning = false;
            }

            DeviceManager.DeviceListChanged -= this.OnDeviceListChanged;
            this.CurrentDevice = null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Stop();
            }

            this.disposed = true;
        }

        private void OnDeviceListChanged(object sender, Models.DeviceListChangedEventArgs e)
        {
            if (e.Added.Any(d => string.Equals(d.Serial, this.deviceSerial, StringComparison.Ordinal)))
            {
                if (DeviceManager.TryGetDeviceBySerial(this.deviceSerial, out var device))
                {
                    this.CurrentDevice = device;
                    this.DeviceConnected?.Invoke(this, device);
                }
            }

            if (e.Removed.Any(d => string.Equals(d.Serial, this.deviceSerial, StringComparison.Ordinal)))
            {
                this.CurrentDevice = null;
                this.DeviceLost?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

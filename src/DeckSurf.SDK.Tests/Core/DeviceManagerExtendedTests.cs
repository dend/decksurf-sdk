// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Core;
using DeckSurf.SDK.Exceptions;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Core
{
    public class DeviceManagerExtendedTests
    {
        [Fact]
        public void GetDeviceBySerial_NullSerial_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DeviceManager.GetDeviceBySerial(null!));
        }

        [Fact]
        public void GetDeviceBySerial_EmptySerial_ThrowsDeviceNotFoundException()
        {
            Assert.Throws<DeviceNotFoundException>(() => DeviceManager.GetDeviceBySerial(string.Empty));
        }

        [Fact]
        public void GetDeviceByPath_NullPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DeviceManager.GetDeviceByPath(null!));
        }

        [Fact]
        public void GetDeviceByPath_EmptyPath_ThrowsDeviceNotFoundException()
        {
            Assert.Throws<DeviceNotFoundException>(() => DeviceManager.GetDeviceByPath(string.Empty));
        }

        [Fact]
        public void GetDeviceBySerial_NonExistentSerial_ThrowsDeviceNotFoundException()
        {
            Assert.Throws<DeviceNotFoundException>(() => DeviceManager.GetDeviceBySerial("NONEXISTENT-SERIAL-12345"));
        }

        [Fact]
        public void GetDeviceByPath_NonExistentPath_ThrowsDeviceNotFoundException()
        {
            Assert.Throws<DeviceNotFoundException>(() => DeviceManager.GetDeviceByPath("/dev/nonexistent/path/12345"));
        }

        [Fact]
        public void TryGetDeviceBySerial_NonExistentSerial_ReturnsFalse()
        {
            var result = DeviceManager.TryGetDeviceBySerial("NONEXISTENT-SERIAL-12345", out var device);

            Assert.False(result);
            Assert.Null(device);
        }

        [Fact]
        public void TryGetDeviceByPath_NonExistentPath_ReturnsFalse()
        {
            var result = DeviceManager.TryGetDeviceByPath("/dev/nonexistent/path/12345", out var device);

            Assert.False(result);
            Assert.Null(device);
        }

        [Fact]
        public void TryGetDeviceBySerial_NullSerial_ReturnsFalse()
        {
            var result = DeviceManager.TryGetDeviceBySerial(null, out var device);

            Assert.False(result);
            Assert.Null(device);
        }

        [Fact]
        public void TryGetDeviceByPath_NullPath_ReturnsFalse()
        {
            var result = DeviceManager.TryGetDeviceByPath(null, out var device);

            Assert.False(result);
            Assert.Null(device);
        }

        [Fact]
        public void TryGetDeviceBySerial_EmptySerial_ReturnsFalse()
        {
            var result = DeviceManager.TryGetDeviceBySerial(string.Empty, out var device);

            Assert.False(result);
            Assert.Null(device);
        }

        [Fact]
        public void TryGetDeviceByPath_EmptyPath_ReturnsFalse()
        {
            var result = DeviceManager.TryGetDeviceByPath(string.Empty, out var device);

            Assert.False(result);
            Assert.Null(device);
        }

        [Fact]
        public void GetDeviceList_ReturnsNonNullReadOnlyList()
        {
            var result = DeviceManager.GetDeviceList();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IReadOnlyList<ConnectedDevice>>(result);
        }

        [Fact]
        public void DeviceListChanged_CanSubscribeAndUnsubscribeWithoutThrowing()
        {
            EventHandler<DeckSurf.SDK.Models.DeviceListChangedEventArgs> handler = (sender, args) => { };

            var exception = Record.Exception(() =>
            {
                DeviceManager.DeviceListChanged += handler;
                DeviceManager.DeviceListChanged -= handler;
            });

            Assert.Null(exception);
        }
    }
}

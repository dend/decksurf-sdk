// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Core;
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
        public void GetDeviceBySerial_EmptySerial_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DeviceManager.GetDeviceBySerial(string.Empty));
        }

        [Fact]
        public void GetDeviceByPath_NullPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DeviceManager.GetDeviceByPath(null!));
        }

        [Fact]
        public void GetDeviceByPath_EmptyPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DeviceManager.GetDeviceByPath(string.Empty));
        }

        [Fact]
        public void GetDeviceBySerial_NonExistentSerial_ReturnsNull()
        {
            var result = DeviceManager.GetDeviceBySerial("NONEXISTENT-SERIAL-12345");

            Assert.Null(result);
        }

        [Fact]
        public void GetDeviceByPath_NonExistentPath_ReturnsNull()
        {
            var result = DeviceManager.GetDeviceByPath("/dev/nonexistent/path/12345");

            Assert.Null(result);
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
            EventHandler handler = (sender, args) => { };

            var exception = Record.Exception(() =>
            {
                DeviceManager.DeviceListChanged += handler;
                DeviceManager.DeviceListChanged -= handler;
            });

            Assert.Null(exception);
        }
    }
}

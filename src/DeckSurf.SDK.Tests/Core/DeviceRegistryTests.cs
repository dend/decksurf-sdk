// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Models.Devices;

namespace DeckSurf.SDK.Tests.Core
{
    public class DeviceRegistryTests
    {
        private const int TestVid = 0x0FD9;
        private const string TestPath = "test";
        private const string TestName = "Test";
        private const string TestSerial = "000";

        [Theory]
        [InlineData((byte)DeviceModel.Original, typeof(StreamDeckOriginal))]
        [InlineData((byte)DeviceModel.Original2019, typeof(StreamDeckOriginal2019))]
        [InlineData((byte)DeviceModel.MK2, typeof(StreamDeckMK2))]
        [InlineData((byte)DeviceModel.XL, typeof(StreamDeckXL))]
        [InlineData((byte)DeviceModel.XL2022, typeof(StreamDeckXL2022))]
        [InlineData((byte)DeviceModel.Mini, typeof(StreamDeckMini))]
        [InlineData((byte)DeviceModel.Mini2022, typeof(StreamDeckMini2022))]
        [InlineData((byte)DeviceModel.Plus, typeof(StreamDeckPlus))]
        [InlineData((byte)DeviceModel.Neo, typeof(StreamDeckNeo))]
        public void CreateDevice_ReturnsCorrectTypeForDeviceModel(byte pid, Type expectedType)
        {
            var device = DeviceRegistry.CreateDevice(TestVid, pid, TestPath, TestName, TestSerial);

            Assert.NotNull(device);
            Assert.IsType(expectedType, device);
        }

        [Theory]
        [InlineData((byte)DeviceModel.Original)]
        [InlineData((byte)DeviceModel.MK2)]
        [InlineData((byte)DeviceModel.Mini)]
        [InlineData((byte)DeviceModel.Plus)]
        [InlineData((byte)DeviceModel.Neo)]
        public void CreateDevice_SetsVendorIdCorrectly(byte pid)
        {
            var device = DeviceRegistry.CreateDevice(TestVid, pid, TestPath, TestName, TestSerial);

            Assert.NotNull(device);
            Assert.Equal(TestVid, device.VendorId);
        }

        [Theory]
        [InlineData((byte)DeviceModel.Original)]
        [InlineData((byte)DeviceModel.MK2)]
        [InlineData((byte)DeviceModel.Mini)]
        [InlineData((byte)DeviceModel.Plus)]
        [InlineData((byte)DeviceModel.Neo)]
        public void CreateDevice_SetsPathCorrectly(byte pid)
        {
            var device = DeviceRegistry.CreateDevice(TestVid, pid, TestPath, TestName, TestSerial);

            Assert.NotNull(device);
            Assert.Equal(TestPath, device.Path);
        }

        [Theory]
        [InlineData((byte)DeviceModel.Original)]
        [InlineData((byte)DeviceModel.MK2)]
        [InlineData((byte)DeviceModel.Mini)]
        [InlineData((byte)DeviceModel.Plus)]
        [InlineData((byte)DeviceModel.Neo)]
        public void CreateDevice_SetsNameCorrectly(byte pid)
        {
            var device = DeviceRegistry.CreateDevice(TestVid, pid, TestPath, TestName, TestSerial);

            Assert.NotNull(device);
            Assert.Equal(TestName, device.Name);
        }

        [Theory]
        [InlineData((byte)DeviceModel.Original)]
        [InlineData((byte)DeviceModel.MK2)]
        [InlineData((byte)DeviceModel.Mini)]
        [InlineData((byte)DeviceModel.Plus)]
        [InlineData((byte)DeviceModel.Neo)]
        public void CreateDevice_SetsSerialCorrectly(byte pid)
        {
            var device = DeviceRegistry.CreateDevice(TestVid, pid, TestPath, TestName, TestSerial);

            Assert.NotNull(device);
            Assert.Equal(TestSerial, device.Serial);
        }

        [Fact]
        public void CreateDevice_ReturnsNullForMK2Scissor()
        {
            var device = DeviceRegistry.CreateDevice(TestVid, (byte)DeviceModel.MK2Scissor, TestPath, TestName, TestSerial);

            Assert.Null(device);
        }

        [Fact]
        public void CreateDevice_ReturnsNullForInvalidPid()
        {
            var device = DeviceRegistry.CreateDevice(TestVid, 0xFF, TestPath, TestName, TestSerial);

            Assert.Null(device);
        }
    }
}

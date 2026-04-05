// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Models.Devices;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Devices
{
    public class DeviceValidationTests
    {
        // StreamDeckMK2: ButtonCount = 15, TouchButtonCount = 0.
        // Using fake VID/PID so no real HID device is found.
        // Validation checks should throw before any HID access is attempted.

        private static StreamDeckMK2 CreateTestDevice()
        {
            return new StreamDeckMK2(0, 0, string.Empty, string.Empty, string.Empty);
        }

        // --- SetKey validation ---

        [Fact]
        public void SetKey_NegativeKeyId_ThrowsArgumentOutOfRangeException()
        {
            var device = CreateTestDevice();
            byte[] image = ImageHelper.CreateBlankImage(10, DeviceColor.Red);

            Assert.Throws<ArgumentOutOfRangeException>(() => device.SetKey(-1, image));
        }

        [Fact]
        public void SetKey_KeyIdEqualToButtonCount_ThrowsArgumentOutOfRangeException()
        {
            var device = CreateTestDevice();
            byte[] image = ImageHelper.CreateBlankImage(10, DeviceColor.Red);

            // ButtonCount is 15, so keyId = 15 is out of range.
            Assert.Throws<ArgumentOutOfRangeException>(() => device.SetKey(15, image));
        }

        [Fact]
        public void SetKey_KeyIdGreaterThanButtonCount_ThrowsArgumentOutOfRangeException()
        {
            var device = CreateTestDevice();
            byte[] image = ImageHelper.CreateBlankImage(10, DeviceColor.Red);

            Assert.Throws<ArgumentOutOfRangeException>(() => device.SetKey(100, image));
        }

        [Fact]
        public void SetKey_NullImage_ThrowsArgumentException()
        {
            var device = CreateTestDevice();

            Assert.Throws<ArgumentException>(() => device.SetKey(0, null));
        }

        [Fact]
        public void SetKey_EmptyImage_ThrowsArgumentException()
        {
            var device = CreateTestDevice();

            Assert.Throws<ArgumentException>(() => device.SetKey(0, Array.Empty<byte>()));
        }

        // --- SetKeyColor validation ---

        [Fact]
        public void SetKeyColor_NegativeIndex_ThrowsIndexOutOfRangeException()
        {
            var device = CreateTestDevice();
            var color = DeviceColor.Red;

            Assert.Throws<IndexOutOfRangeException>(() => device.SetKeyColor(-1, color));
        }

        [Fact]
        public void SetKeyColor_IndexEqualToButtonCountPlusTouchButtonCount_ThrowsIndexOutOfRangeException()
        {
            var device = CreateTestDevice();
            var color = DeviceColor.Red;

            // MK2: ButtonCount = 15, TouchButtonCount = 0, so index = 15 is out of range.
            Assert.Throws<IndexOutOfRangeException>(() => device.SetKeyColor(15, color));
        }

        [Fact]
        public void SetKeyColor_IndexGreaterThanTotal_ThrowsIndexOutOfRangeException()
        {
            var device = CreateTestDevice();
            var color = DeviceColor.Red;

            Assert.Throws<IndexOutOfRangeException>(() => device.SetKeyColor(100, color));
        }

        // --- SetupDeviceButtonMap validation ---

        [Fact]
        public void SetupDeviceButtonMap_Null_ThrowsArgumentNullException()
        {
            var device = CreateTestDevice();

            Assert.Throws<ArgumentNullException>(() => device.SetupDeviceButtonMap(null));
        }
    }
}

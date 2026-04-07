// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;
using DeckSurf.SDK.Models.Devices;

namespace DeckSurf.SDK.Tests.Devices
{
    public class DeviceSpecTests
    {
        public static IEnumerable<object[]> GetDeviceTestData()
        {
            // Original: 15 buttons, 72px, 5x3, Jpeg, Rotate180, no screen, no knob
            yield return new object[]
            {
                new StreamDeckOriginal(0, 0, "", "", ""),
                DeviceModel.Original, 15, 72, 5, 3,
                DeviceImageFormat.Jpeg, DeviceRotation.Rotate180,
                false, false, 0, 1024, 8,
            };

            // Original2019: same as Original
            yield return new object[]
            {
                new StreamDeckOriginal2019(0, 0, "", "", ""),
                DeviceModel.Original2019, 15, 72, 5, 3,
                DeviceImageFormat.Jpeg, DeviceRotation.Rotate180,
                false, false, 0, 1024, 8,
            };

            // MK2: same as Original
            yield return new object[]
            {
                new StreamDeckMK2(0, 0, "", "", ""),
                DeviceModel.MK2, 15, 72, 5, 3,
                DeviceImageFormat.Jpeg, DeviceRotation.Rotate180,
                false, false, 0, 1024, 8,
            };

            // XL: 32 buttons, 96px, 8x4, Jpeg, Rotate180, no screen, no knob
            yield return new object[]
            {
                new StreamDeckXL(0, 0, "", "", ""),
                DeviceModel.XL, 32, 96, 8, 4,
                DeviceImageFormat.Jpeg, DeviceRotation.Rotate180,
                false, false, 0, 1024, 8,
            };

            // XL2022: same as XL
            yield return new object[]
            {
                new StreamDeckXL2022(0, 0, "", "", ""),
                DeviceModel.XL2022, 32, 96, 8, 4,
                DeviceImageFormat.Jpeg, DeviceRotation.Rotate180,
                false, false, 0, 1024, 8,
            };

            // Mini: 6 buttons, 80px, 3x2, Bmp, Rotate270, no screen, no knob
            yield return new object[]
            {
                new StreamDeckMini(0, 0, "", "", ""),
                DeviceModel.Mini, 6, 80, 3, 2,
                DeviceImageFormat.Bmp, DeviceRotation.Rotate270,
                false, false, 0, 1024, 16,
            };

            // Mini2022: same as Mini
            yield return new object[]
            {
                new StreamDeckMini2022(0, 0, "", "", ""),
                DeviceModel.Mini2022, 6, 80, 3, 2,
                DeviceImageFormat.Bmp, DeviceRotation.Rotate270,
                false, false, 0, 1024, 16,
            };

            // Neo: 8 buttons, 96px, 4x2, Jpeg, Rotate180, screen (248x58), no knob, 2 touch buttons
            yield return new object[]
            {
                new StreamDeckNeo(0, 0, "", "", ""),
                DeviceModel.Neo, 8, 96, 4, 2,
                DeviceImageFormat.Jpeg, DeviceRotation.Rotate180,
                true, false, 2, 1024, 8,
            };

            // Plus: 8 buttons, 120px, 4x2, Jpeg, Rotate180, screen (800x100), knob, 0 touch buttons
            yield return new object[]
            {
                new StreamDeckPlus(0, 0, "", "", ""),
                DeviceModel.Plus, 8, 120, 4, 2,
                DeviceImageFormat.Jpeg, DeviceRotation.Rotate180,
                true, true, 0, 1024, 8,
            };
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectModel(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedModel, device.Model);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectButtonCount(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedButtonCount, device.ButtonCount);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectButtonResolution(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedButtonResolution, device.ButtonResolution);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectButtonColumns(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedColumns, device.ButtonColumns);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectButtonRows(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedRows, device.ButtonRows);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectKeyImageFormat(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedImageFormat, device.KeyImageFormat);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectImageRotation(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedImageRotation, device.ImageRotation);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectScreenSupport(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedScreenSupported, device.IsScreenSupported);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectKnobSupport(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedKnobSupported, device.IsKnobSupported);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectTouchButtonCount(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedTouchButtonCount, device.TouchButtonCount);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectPacketSize(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedPacketSize, device.PacketSize);
        }

        [Theory]
        [MemberData(nameof(GetDeviceTestData))]
        public void Device_ReportsCorrectKeyImageHeaderSize(
            ConnectedDevice device,
            DeviceModel expectedModel, int expectedButtonCount, int expectedButtonResolution,
            int expectedColumns, int expectedRows,
            DeviceImageFormat expectedImageFormat, DeviceRotation expectedImageRotation,
            bool expectedScreenSupported, bool expectedKnobSupported,
            int expectedTouchButtonCount, int expectedPacketSize, int expectedKeyImageHeaderSize)
        {
            Assert.Equal(expectedKeyImageHeaderSize, device.KeyImageHeaderSize);
        }

        [Fact]
        public void Neo_ReportsCorrectScreenDimensions()
        {
            var device = new StreamDeckNeo(0, 0, "", "", "");

            Assert.Equal(248, device.ScreenWidth);
            Assert.Equal(58, device.ScreenHeight);
        }

        [Fact]
        public void Plus_ReportsCorrectScreenDimensions()
        {
            var device = new StreamDeckPlus(0, 0, "", "", "");

            Assert.Equal(800, device.ScreenWidth);
            Assert.Equal(100, device.ScreenHeight);
        }

        [Fact]
        public void JpegDevice_ReportsNoScreenDimensions()
        {
            var device = new StreamDeckMK2(0, 0, "", "", "");

            Assert.Equal(-1, device.ScreenWidth);
            Assert.Equal(-1, device.ScreenHeight);
        }

        [Fact]
        public void BmpDevice_ReportsNoScreenDimensions()
        {
            var device = new StreamDeckMini(0, 0, "", "", "");

            Assert.Equal(-1, device.ScreenWidth);
            Assert.Equal(-1, device.ScreenHeight);
        }
    }
}

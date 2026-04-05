// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public class ImageHelpersTests
    {
        [Fact]
        public void ResizeImage_NullBuffer_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ImageHelpers.ResizeImage(null, 72, 72, DeviceRotation.Rotate180FlipNone, DeviceImageFormat.Jpeg));
        }

        [Fact]
        public void ResizeImage_EmptyBuffer_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ImageHelpers.ResizeImage(Array.Empty<byte>(), 72, 72, DeviceRotation.Rotate180FlipNone, DeviceImageFormat.Jpeg));
        }

        [Fact]
        public void ResizeImage_ZeroWidth_ThrowsArgumentException()
        {
            byte[] validImage = ImageHelpers.CreateBlankImage(10, DeviceColor.Red);

            Assert.Throws<ArgumentException>(() =>
                ImageHelpers.ResizeImage(validImage, 0, 72, DeviceRotation.Rotate180FlipNone, DeviceImageFormat.Jpeg));
        }

        [Fact]
        public void ResizeImage_ZeroHeight_ThrowsArgumentException()
        {
            byte[] validImage = ImageHelpers.CreateBlankImage(10, DeviceColor.Red);

            Assert.Throws<ArgumentException>(() =>
                ImageHelpers.ResizeImage(validImage, 72, 0, DeviceRotation.Rotate180FlipNone, DeviceImageFormat.Jpeg));
        }

        [Fact]
        public void ResizeImage_InvalidImageData_ThrowsArgumentException()
        {
            byte[] randomBytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            Assert.Throws<ArgumentException>(() =>
                ImageHelpers.ResizeImage(randomBytes, 72, 72, DeviceRotation.Rotate180FlipNone, DeviceImageFormat.Jpeg));
        }

        [Fact]
        public void ResizeImage_ValidImage_ProducesOutputWithCorrectFormat()
        {
            byte[] validImage = ImageHelpers.CreateBlankImage(10, DeviceColor.Red);

            byte[] result = ImageHelpers.ResizeImage(validImage, 72, 72, DeviceRotation.Rotate180FlipNone, DeviceImageFormat.Jpeg);

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            // JPEG files start with magic bytes 0xFF 0xD8.
            Assert.Equal(0xFF, result[0]);
            Assert.Equal(0xD8, result[1]);
        }

        [Fact]
        public void CreateBlankImage_ZeroPixelSize_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ImageHelpers.CreateBlankImage(0, DeviceColor.Red));
        }

        [Fact]
        public void CreateBlankImage_NegativePixelSize_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ImageHelpers.CreateBlankImage(-1, DeviceColor.Red));
        }

        [Fact]
        public void CreateBlankImage_ValidSize_ProducesNonEmptyByteArray()
        {
            byte[] result = ImageHelpers.CreateBlankImage(10, DeviceColor.Red);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void CreateBlankImage_Output_StartsWithJpegMagicBytes()
        {
            byte[] result = ImageHelpers.CreateBlankImage(10, DeviceColor.Red);

            Assert.True(result.Length >= 2, "Output should be at least 2 bytes long.");
            Assert.Equal(0xFF, result[0]);
            Assert.Equal(0xD8, result[1]);
        }
    }
}

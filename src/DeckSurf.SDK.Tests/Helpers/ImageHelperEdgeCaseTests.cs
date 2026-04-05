// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public class ImageHelperEdgeCaseTests
    {
        [Fact]
        public void ResizeImage_NegativeWidth_ThrowsArgumentException()
        {
            byte[] validImage = ImageHelper.CreateBlankImage(10, DeviceColor.Red);

            var ex = Assert.Throws<ArgumentException>(() =>
                ImageHelper.ResizeImage(validImage, -1, 72, DeviceRotation.Rotate180, DeviceImageFormat.Jpeg));

            Assert.Equal("width", ex.ParamName);
        }

        [Fact]
        public void ResizeImage_NegativeHeight_ThrowsArgumentException()
        {
            byte[] validImage = ImageHelper.CreateBlankImage(10, DeviceColor.Red);

            var ex = Assert.Throws<ArgumentException>(() =>
                ImageHelper.ResizeImage(validImage, 72, -1, DeviceRotation.Rotate180, DeviceImageFormat.Jpeg));

            Assert.Equal("height", ex.ParamName);
        }

        [Fact]
        public void CreateBlankImage_NegativePixelSize_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                ImageHelper.CreateBlankImage(-5, DeviceColor.Red));

            Assert.Equal("pixelSize", ex.ParamName);
        }

        [Fact]
        public void ResizeImage_MinimumValidDimensions_Succeeds()
        {
            byte[] validImage = ImageHelper.CreateBlankImage(10, DeviceColor.Red);

            byte[] result = ImageHelper.ResizeImage(validImage, 1, 1, DeviceRotation.None, DeviceImageFormat.Jpeg);

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            // JPEG files start with magic bytes 0xFF 0xD8.
            Assert.Equal(0xFF, result[0]);
            Assert.Equal(0xD8, result[1]);
        }
    }
}

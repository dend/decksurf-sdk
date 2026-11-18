// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;
using DeckSurf.SDK.Models.Devices;
using DeckSurf.SDK.Util;
using SkiaSharp;
using Xunit;

namespace DeckSurf.SDK.Tests.Helpers
{
    /// <summary>
    /// Pixel-level tests for the orientation transforms applied by <see cref="ImageHelper.ResizeImage"/>.
    /// Regression coverage for GitHub issue #24 (Stream Deck Mini key images rendered inverted).
    /// </summary>
    public class ImageOrientationTests
    {
        private const int Size = 8;

        private static readonly SKColor Red = new(255, 0, 0);
        private static readonly SKColor Green = new(0, 255, 0);
        private static readonly SKColor Blue = new(0, 0, 255);
        private static readonly SKColor White = new(255, 255, 255);

        [Fact]
        public void ResizeImage_None_PreservesQuadrants()
        {
            var result = Transform(DeviceRotation.None);

            Assert.Equal(Red, result.TopLeft);
            Assert.Equal(Green, result.TopRight);
            Assert.Equal(Blue, result.BottomLeft);
            Assert.Equal(White, result.BottomRight);
        }

        [Fact]
        public void ResizeImage_Rotate180_SwapsOppositeCorners()
        {
            var result = Transform(DeviceRotation.Rotate180);

            Assert.Equal(White, result.TopLeft);
            Assert.Equal(Blue, result.TopRight);
            Assert.Equal(Green, result.BottomLeft);
            Assert.Equal(Red, result.BottomRight);
        }

        [Fact]
        public void ResizeImage_Rotate270_RotatesCounterClockwise()
        {
            var result = Transform(DeviceRotation.Rotate270);

            Assert.Equal(Green, result.TopLeft);
            Assert.Equal(White, result.TopRight);
            Assert.Equal(Red, result.BottomLeft);
            Assert.Equal(Blue, result.BottomRight);
        }

        [Fact]
        public void ResizeImage_Rotate270FlipVertical_TransposesAcrossMainDiagonal()
        {
            // Rotate 270 then flip vertically: top-left and bottom-right stay put,
            // top-right and bottom-left swap. This is what the Mini firmware expects.
            var result = Transform(DeviceRotation.Rotate270FlipVertical);

            Assert.Equal(Red, result.TopLeft);
            Assert.Equal(Blue, result.TopRight);
            Assert.Equal(Green, result.BottomLeft);
            Assert.Equal(White, result.BottomRight);
        }

        [Fact]
        public void ResizeImage_Rotate270FlipVertical_DiffersFromRotate270ByVerticalFlip()
        {
            var rotated = Transform(DeviceRotation.Rotate270);
            var transposed = Transform(DeviceRotation.Rotate270FlipVertical);

            Assert.Equal(rotated.TopLeft, transposed.BottomLeft);
            Assert.Equal(rotated.TopRight, transposed.BottomRight);
            Assert.Equal(rotated.BottomLeft, transposed.TopLeft);
            Assert.Equal(rotated.BottomRight, transposed.TopRight);
        }

        [Fact]
        public void StreamDeckMini_UsesRotate270FlipVertical()
        {
            var device = new StreamDeckMini(0, 0, string.Empty, string.Empty, string.Empty);

            Assert.Equal(DeviceRotation.Rotate270FlipVertical, device.ImageRotation);
        }

        [Fact]
        public void StreamDeckMini2022_UsesRotate270FlipVertical()
        {
            var device = new StreamDeckMini2022(0, 0, string.Empty, string.Empty, string.Empty);

            Assert.Equal(DeviceRotation.Rotate270FlipVertical, device.ImageRotation);
        }

        private static byte[] CreateQuadrantImage()
        {
            using var image = new SKBitmap(new SKImageInfo(Size, Size, SKColorType.Rgba8888, SKAlphaType.Opaque));
            int half = Size / 2;
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    SKColor color = (x < half, y < half) switch
                    {
                        (true, true) => Red,
                        (false, true) => Green,
                        (true, false) => Blue,
                        (false, false) => White,
                    };
                    image.SetPixel(x, y, color);
                }
            }

            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        private static Quadrants Transform(DeviceRotation rotation)
        {
            byte[] output = ImageHelper.ResizeImage(CreateQuadrantImage(), Size, Size, rotation, DeviceImageFormat.Bmp);
            Assert.Equal((byte)'B', output[0]);
            Assert.Equal((byte)'M', output[1]);
            using var image = SKBitmap.Decode(output);

            // Sample the centre of each quadrant so resampling at the edges cannot affect the result.
            int near = Size / 4;
            int far = Size - 1 - near;
            return new Quadrants(image.GetPixel(near, near), image.GetPixel(far, near), image.GetPixel(near, far), image.GetPixel(far, far));
        }

        private readonly record struct Quadrants(SKColor TopLeft, SKColor TopRight, SKColor BottomLeft, SKColor BottomRight);
    }
}

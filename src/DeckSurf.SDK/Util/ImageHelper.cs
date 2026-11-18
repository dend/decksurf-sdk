// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using DeckSurf.SDK.Models;
using SkiaSharp;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Collection of methods used for image manipulation, allowing easier Stream Deck button image preparation.
    /// </summary>
    public static class ImageHelper
    {
        private const int JpegQuality = 75;

        private const int BmpFileHeaderSize = 14;

        private const int BmpInfoHeaderSize = 40;

        /// <summary>
        /// Resize an image buffer to the expected size, in pixels.
        /// </summary>
        /// <param name="buffer">Byte array containing the image.</param>
        /// <param name="width">Target width, in pixels.</param>
        /// <param name="height">Target height, in pixels.</param>
        /// <param name="rotation">Determines the rotation for a given image.</param>
        /// <param name="format">Image format to be sent to the device.</param>
        /// <returns>Byte array representing the resized image.</returns>
        /// <exception cref="ArgumentException">Thrown when buffer is null/empty or dimensions are invalid.</exception>
        /// <exception cref="Exceptions.ImageProcessingException">Thrown when the buffer is not a recognized image format.</exception>
        public static byte[] ResizeImage(byte[] buffer, int width, int height, DeviceRotation rotation, DeviceImageFormat format)
        {
            if (buffer == null || buffer.Length == 0)
            {
                throw new ArgumentException("The image buffer cannot be null or empty.", nameof(buffer));
            }

            if (width <= 0)
            {
                throw new ArgumentException("The width must be greater than zero.", nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentException("The height must be greater than zero.", nameof(height));
            }

            using SKBitmap source = SKBitmap.Decode(buffer);
            if (source == null)
            {
                throw new Exceptions.ImageProcessingException("The provided buffer is not a recognized image format.");
            }

            // Stretch to the target size. Unpremultiplied output keeps the colour channels as they are when alpha is dropped.
            var targetInfo = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
            using SKBitmap resized = source.Resize(targetInfo, new SKSamplingOptions(SKCubicResampler.CatmullRom));
            if (resized == null)
            {
                throw new Exceptions.ImageProcessingException("The image could not be resized.");
            }

            return Encode(ApplyOrientation(ToRgb(resized), rotation), format);
        }

        /// <summary>
        /// Creates a new blank square, always encoded as JPEG.
        /// </summary>
        /// <remarks>
        /// The output is JPEG regardless of the target device. Devices whose native key format is not JPEG
        /// (for example, the Stream Deck Mini, which expects BMP) silently discard JPEG buffers passed to
        /// <see cref="Models.ConnectedDevice.SetKey(int, byte[], bool)"/> with <c>alreadyResized: true</c>.
        /// When the buffer is destined for a device key, prefer
        /// <see cref="CreateBlankImage(int, int, DeviceColor, DeviceImageFormat)"/> with the device's
        /// <see cref="Models.ConnectedDevice.KeyImageFormat"/> so the encoding matches the hardware.
        /// </remarks>
        /// <param name="pixelSize">Size, in pixels, of the square sides.</param>
        /// <param name="color">The color of the blank square to be created.</param>
        /// <returns>If successful, returns a byte array representing the JPEG representation of the blank square.</returns>
        public static byte[] CreateBlankImage(int pixelSize, DeviceColor color)
        {
            if (pixelSize <= 0)
            {
                throw new ArgumentException("The pixel size must be greater than zero.", nameof(pixelSize));
            }

            return EncodeJpeg(CreateSolid(pixelSize, pixelSize, color));
        }

        /// <summary>
        /// Creates a new blank image with the specified dimensions, color, and format.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="color">The fill color.</param>
        /// <param name="format">The output image format.</param>
        /// <returns>Byte array representing the image in the specified format.</returns>
        /// <exception cref="ArgumentException">Thrown when dimensions are not positive.</exception>
        public static byte[] CreateBlankImage(int width, int height, DeviceColor color, DeviceImageFormat format)
        {
            if (width <= 0)
            {
                throw new ArgumentException("Width must be greater than zero.", nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentException("Height must be greater than zero.", nameof(height));
            }

            return Encode(CreateSolid(width, height, color), format);
        }

        /// <summary>
        /// Converts an Icon object to a byte array.
        /// </summary>
        /// <param name="icon">Icon object containing the image.</param>
        /// <returns>Byte array, generated from an Icon object.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when called on a non-Windows operating system.</exception>
        [SupportedOSPlatform("windows")]
        public static byte[] GetImageBuffer(Icon icon)
        {
            ArgumentNullException.ThrowIfNull(icon);

            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("GetImageBuffer(Icon) is only supported on Windows.");
            }

            using MemoryStream stream = new();
            icon.Save(stream);
            return stream.ToArray();
        }

        /// <summary>
        /// Gets the Windows icon for a given file.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="width">Desired icon width.</param>
        /// <param name="height">Desired icon height.</param>
        /// <param name="options">Icon extraction flags, represented by a standard Windows API <see cref="SIIGBF"/> enum.</param>
        /// <returns>Bitmap object containing the file icon.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when called on a non-Windows operating system.</exception>
        [SupportedOSPlatform("windows")]
        public static Bitmap GetFileIcon(string fileName, int width, int height, SIIGBF options)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("GetFileIcon is only supported on Windows.");
            }

            IntPtr hBitmap = GetBitmapPointer(fileName, width, height, options);

            try
            {
                return GetBitmapFromHBitmap(hBitmap);
            }
            finally
            {
                WindowsAPIHelpers.DeleteObject(hBitmap);
            }
        }

        private static RgbImage CreateSolid(int width, int height, DeviceColor color)
        {
            var pixels = new byte[width * height * 3];
            for (int i = 0; i < pixels.Length; i += 3)
            {
                pixels[i] = color.R;
                pixels[i + 1] = color.G;
                pixels[i + 2] = color.B;
            }

            return new RgbImage(width, height, pixels);
        }

        private static RgbImage ToRgb(SKBitmap bitmap)
        {
            // The bitmap is Rgba8888: R, G, B, A per pixel. The alpha channel is dropped.
            ReadOnlySpan<byte> rgba = bitmap.GetPixelSpan();
            var pixels = new byte[bitmap.Width * bitmap.Height * 3];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    rgba.Slice((y * bitmap.RowBytes) + (x * 4), 3).CopyTo(pixels.AsSpan(((y * bitmap.Width) + x) * 3));
                }
            }

            return new RgbImage(bitmap.Width, bitmap.Height, pixels);
        }

        private static RgbImage ApplyOrientation(RgbImage image, DeviceRotation rotation)
        {
            if (rotation is not (DeviceRotation.Rotate180 or DeviceRotation.Rotate270 or DeviceRotation.Rotate270FlipVertical))
            {
                return image;
            }

            bool swapAxes = rotation != DeviceRotation.Rotate180;
            int width = swapAxes ? image.Height : image.Width;
            int height = swapAxes ? image.Width : image.Height;
            var pixels = new byte[image.Pixels.Length];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Map each output pixel back to the source pixel it comes from.
                    (int sourceX, int sourceY) = rotation switch
                    {
                        DeviceRotation.Rotate180 => (image.Width - 1 - x, image.Height - 1 - y),

                        // 270 degrees clockwise, which is 90 degrees counter-clockwise.
                        DeviceRotation.Rotate270 => (image.Width - 1 - y, x),

                        // Rotate 270, then flip top-to-bottom: a transpose. This matches the reference
                        // Stream Deck Mini transform, verified on hardware; rotation alone renders content inverted.
                        _ => (y, x),
                    };

                    image.Pixels.AsSpan(((sourceY * image.Width) + sourceX) * 3, 3).CopyTo(pixels.AsSpan(((y * width) + x) * 3));
                }
            }

            return new RgbImage(width, height, pixels);
        }

        private static byte[] Encode(RgbImage image, DeviceImageFormat format)
        {
            return format == DeviceImageFormat.Bmp ? EncodeBmp(image) : EncodeJpeg(image);
        }

        private static byte[] EncodeJpeg(RgbImage image)
        {
            var info = new SKImageInfo(image.Width, image.Height, SKColorType.Rgba8888, SKAlphaType.Opaque);
            using var bitmap = new SKBitmap(info);
            Span<byte> rgba = bitmap.GetPixelSpan();
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int target = (y * bitmap.RowBytes) + (x * 4);
                    image.Pixels.AsSpan(((y * image.Width) + x) * 3, 3).CopyTo(rgba.Slice(target, 3));
                    rgba[target + 3] = byte.MaxValue;
                }
            }

            using SKData data = bitmap.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);
            if (data == null)
            {
                throw new Exceptions.ImageProcessingException("The image could not be encoded as JPEG.");
            }

            return data.ToArray();
        }

        // Skia has no BMP encoder. This writes an uncompressed 24-bit BMP with a BITMAPINFOHEADER,
        // stored bottom-up in BGR order, which is the format Stream Deck devices with BMP keys expect.
        private static byte[] EncodeBmp(RgbImage image)
        {
            int rowSize = ((image.Width * 3) + 3) & ~3;
            int pixelDataSize = rowSize * image.Height;
            int dataOffset = BmpFileHeaderSize + BmpInfoHeaderSize;
            var bmp = new byte[dataOffset + pixelDataSize];
            Span<byte> span = bmp;

            span[0] = (byte)'B';
            span[1] = (byte)'M';
            BinaryPrimitives.WriteInt32LittleEndian(span[2..], bmp.Length);
            BinaryPrimitives.WriteInt32LittleEndian(span[10..], dataOffset);
            BinaryPrimitives.WriteInt32LittleEndian(span[14..], BmpInfoHeaderSize);
            BinaryPrimitives.WriteInt32LittleEndian(span[18..], image.Width);
            BinaryPrimitives.WriteInt32LittleEndian(span[22..], image.Height);
            BinaryPrimitives.WriteInt16LittleEndian(span[26..], 1);
            BinaryPrimitives.WriteInt16LittleEndian(span[28..], 24);
            BinaryPrimitives.WriteInt32LittleEndian(span[34..], pixelDataSize);
            BinaryPrimitives.WriteInt32LittleEndian(span[38..], 2835);
            BinaryPrimitives.WriteInt32LittleEndian(span[42..], 2835);

            for (int y = 0; y < image.Height; y++)
            {
                int row = dataOffset + ((image.Height - 1 - y) * rowSize);
                for (int x = 0; x < image.Width; x++)
                {
                    int source = ((y * image.Width) + x) * 3;
                    int target = row + (x * 3);
                    bmp[target] = image.Pixels[source + 2];
                    bmp[target + 1] = image.Pixels[source + 1];
                    bmp[target + 2] = image.Pixels[source];
                }
            }

            return bmp;
        }

        [SupportedOSPlatform("windows")]
        private static Bitmap GetBitmapFromHBitmap(IntPtr nativeHBitmap)
        {
            Bitmap bitmap = System.Drawing.Image.FromHbitmap(nativeHBitmap);

            if (System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) < 32)
            {
                return bitmap;
            }

            return CreateAlphaBitmap(bitmap, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        // Refer to Stack Overflow answer: https://stackoverflow.com/a/21752100 and https://stackoverflow.com/a/42178963
        [SupportedOSPlatform("windows")]
        private static Bitmap CreateAlphaBitmap(Bitmap sourceBitmap, System.Drawing.Imaging.PixelFormat targetPixelFormat)
        {
            Bitmap outputBitmap = new(sourceBitmap.Width, sourceBitmap.Height, targetPixelFormat);
            System.Drawing.Rectangle boundary = new(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData sourceBitmapData = sourceBitmap.LockBits(boundary, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);

            try
            {
                for (int i = 0; i <= sourceBitmapData.Height - 1; i++)
                {
                    for (int j = 0; j <= sourceBitmapData.Width - 1; j++)
                    {
                        System.Drawing.Color pixelColor = System.Drawing.Color.FromArgb(Marshal.ReadInt32(sourceBitmapData.Scan0, (sourceBitmapData.Stride * i) + (4 * j)));

                        outputBitmap.SetPixel(j, i, pixelColor);
                    }
                }
            }
            finally
            {
                sourceBitmap.UnlockBits(sourceBitmapData);
            }

            return outputBitmap;
        }

        [SupportedOSPlatform("windows")]
        private static IntPtr GetBitmapPointer(string fileName, int width, int height, SIIGBF options)
        {
            Guid itemIdentifier = new(WindowsAPIHelpers.IID_IShellItem2);
            int returnCode = WindowsAPIHelpers.SHCreateItemFromParsingName(fileName, IntPtr.Zero, ref itemIdentifier, out IShellItem nativeShellItem);

            if (returnCode != 0)
            {
                throw Marshal.GetExceptionForHR(returnCode);
            }

            SIZE nativeSize = default;
            nativeSize.Width = width;
            nativeSize.Height = height;

            HResult hr = ((IShellItemImageFactory)nativeShellItem).GetImage(nativeSize, options, out IntPtr hBitmap);

            Marshal.ReleaseComObject(nativeShellItem);

            if (hr == HResult.S_OK)
            {
                return hBitmap;
            }

            throw Marshal.GetExceptionForHR((int)hr);
        }

        private readonly record struct RgbImage(int Width, int Height, byte[] Pixels);
    }
}

// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using DeckSurf.SDK.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Collection of methods used for image manipulation, allowing easier Stream Deck button image preparation.
    /// </summary>
    public static class ImageHelper
    {
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

            Image<Rgb24> image;
            try
            {
                image = SixLabors.ImageSharp.Image.Load<Rgb24>(buffer);
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException ex)
            {
                throw new Exceptions.ImageProcessingException("The provided buffer is not a recognized image format.", ex);
            }

            using (image)
            {
                image.Mutate(ctx =>
                {
                    ctx.Resize(new ResizeOptions
                    {
                        Size = new SixLabors.ImageSharp.Size(width, height),
                        Sampler = KnownResamplers.Bicubic,
                        Mode = ResizeMode.Stretch,
                    });
                    ctx.Rotate(ToRotateMode(rotation));
                });

                using var outputStream = new MemoryStream();
                image.Save(outputStream, ToEncoder(format));
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Creates a new blank square.
        /// </summary>
        /// <param name="pixelSize">Size, in pixels, of the square sides.</param>
        /// <param name="color">The color of the blank square to be created.</param>
        /// <returns>If successful, returns a byte array representing the JPEG representation of the blank square.</returns>
        public static byte[] CreateBlankImage(int pixelSize, DeviceColor color)
        {
            if (pixelSize <= 0)
            {
                throw new ArgumentException("The pixel size must be greater than zero.", nameof(pixelSize));
            }

            using var image = new Image<Rgb24>(pixelSize, pixelSize, new Rgb24(color.R, color.G, color.B));
            using var ms = new MemoryStream();
            image.Save(ms, new JpegEncoder());
            return ms.ToArray();
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

        private static RotateMode ToRotateMode(DeviceRotation rotation)
        {
            return rotation switch
            {
                DeviceRotation.Rotate180 => RotateMode.Rotate180,
                DeviceRotation.Rotate270 => RotateMode.Rotate270,
                _ => RotateMode.None,
            };
        }

        private static IImageEncoder ToEncoder(DeviceImageFormat format)
        {
            return format switch
            {
                DeviceImageFormat.Jpeg => new JpegEncoder(),
                DeviceImageFormat.Bmp => new BmpEncoder(),
                _ => new JpegEncoder(),
            };
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
    }
}

﻿// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Collection of methods used for image manipulation, allowing easier Stream Deck button image preparation.
    /// </summary>
    public class ImageHelpers
    {
        /// <summary>
        /// Resize an image buffer to the expected size, in pixels.
        /// </summary>
        /// <param name="buffer">Byte array containing the image.</param>
        /// <param name="width">Target width, in pixels.</param>
        /// <param name="height">Target height, in pixels.</param>
        /// <param name="flip">Determines whether the image needs to be flipped upside-down.</param>
        /// <returns>Byte array representing the resized image.</returns>
        public static byte[] ResizeImage(byte[] buffer, int width, int height, bool flip)
        {
            Image currentImage = GetImage(buffer);

            var targetRectangle = new Rectangle(0, 0, width, height);
            Bitmap targetImage = new(width, height, PixelFormat.Format24bppRgb);

            targetImage.SetResolution(currentImage.HorizontalResolution, currentImage.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(targetImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(currentImage, targetRectangle, 0, 0, currentImage.Width, currentImage.Height, GraphicsUnit.Pixel);
            }

            // TODO: I am not sure if every image needs to be rotated, but
            // in my limited experiments, this seems to be the case.
            if (flip)
            {
                targetImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }

            using var bufferStream = new MemoryStream();
            targetImage.Save(bufferStream, ImageFormat.Jpeg);
            return bufferStream.ToArray();
        }

        /// <summary>
        /// Converts a byte array to an Image object.
        /// </summary>
        /// <param name="buffer">Byte array containing the image.</param>
        /// <returns>Image object, generated from a given byte array.</returns>
        public static Image GetImage(byte[] buffer)
        {
            Image image = null;
            using (MemoryStream ms = new(buffer))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }

        /// <summary>
        /// Converts an Image object to a byte array.
        /// </summary>
        /// <param name="image">Image object containing the image.</param>
        /// <returns>Byte array, generated from an Image object.</returns>
        public static byte[] GetImageBuffer(Image image)
        {
            ImageConverter converter = new();
            byte[] buffer = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return buffer;
        }

        /// <summary>
        /// Converts an Icon object to a byte array.
        /// </summary>
        /// <param name="icon">Icon object containing the image.</param>
        /// <returns>Byte array, generated from an Icon object.</returns>
        public static byte[] GetImageBuffer(Icon icon)
        {
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
        public static Bitmap GetFileIcon(string fileName, int width, int height, SIIGBF options)
        {
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

        /// <summary>
        /// Creates a new blank square.
        /// </summary>
        /// <param name="pixelSize">Size, in pixels, of the square sides.</param>
        /// <param name="color">The color of the blank square to be created.</param>
        /// <returns>If successful, returns a byte array representing the JPEG representation of the blank square.</returns>
        public static byte[] CreateBlankImage(int pixelSize, Color color)
        {
            using var image = new Bitmap(pixelSize, pixelSize);
            using var graphics = Graphics.FromImage(image);
            graphics.Clear(color);
            using MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private static Bitmap GetBitmapFromHBitmap(IntPtr nativeHBitmap)
        {
            Bitmap bitmap = Image.FromHbitmap(nativeHBitmap);

            if (Image.GetPixelFormatSize(bitmap.PixelFormat) < 32)
            {
                return bitmap;
            }

            return CreateAlphaBitmap(bitmap, PixelFormat.Format32bppArgb);
        }

        // Refer to Stack Overflow answer: https://stackoverflow.com/a/21752100 and https://stackoverflow.com/a/42178963
        private static Bitmap CreateAlphaBitmap(Bitmap sourceBitmap, PixelFormat targetPixelFormat)
        {
            Bitmap outputBitmap = new(sourceBitmap.Width, sourceBitmap.Height, targetPixelFormat);
            Rectangle boundary = new(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData sourceBitmapData = sourceBitmap.LockBits(boundary, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);

            try
            {
                for (int i = 0; i <= sourceBitmapData.Height - 1; i++)
                {
                    for (int j = 0; j <= sourceBitmapData.Width - 1; j++)
                    {
                        Color pixelColor = Color.FromArgb(Marshal.ReadInt32(sourceBitmapData.Scan0, (sourceBitmapData.Stride * i) + (4 * j)));

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

// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DeckSurf.SDK.Core;
using DeckSurf.SDK.Util;
using HidSharp;
using HidSharp.Reports;

namespace DeckSurf.SDK.Models
{
    public abstract class ConnectedDevice
    {
        private const int ButtonPressHeaderOffset = 4;

        private static readonly int ImageReportLength = 1024;
        private static readonly int ImageReportHeaderLength = 8;
        private static readonly int ImageReportPayloadLength = ImageReportLength - ImageReportHeaderLength;

        private byte[] keyPressBuffer = new byte[1024];

        public ConnectedDevice()
        {
        }

        public ConnectedDevice(int vid, int pid, string path, string name, DeviceModel model)
        {
            this.VId = vid;
            this.PId = pid;
            this.Path = path;
            this.Name = name;
            this.Model = model;
            this.UnderlyingDevice = DeviceList.Local.GetHidDeviceOrNull(this.VId, this.PId);

            this.ButtonCount = model switch
            {
                DeviceModel.XL => DeviceConstants.XLButtonCount,
                DeviceModel.MINI => DeviceConstants.MiniButtonCount,
                DeviceModel.ORIGINAL or DeviceModel.ORIGINAL_V2 => DeviceConstants.OriginalButtonCount,
                _ => 0,
            };
        }

        public delegate void ReceivedButtonPressHandler(object source, ButtonPressEventArgs e);

        public event ReceivedButtonPressHandler OnButtonPress;

        public int VId { get; set; }

        public int PId { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public DeviceModel Model { get; set; }

        public int ButtonCount { get; }

        private HidDevice UnderlyingDevice { get; }

        private HidStream UnderlyingInputStream { get; set; }

        public void InitializeDevice()
        {
            this.UnderlyingInputStream = this.UnderlyingDevice.Open();
            this.UnderlyingInputStream.ReadTimeout = Timeout.Infinite;
            this.UnderlyingInputStream.BeginRead(this.keyPressBuffer, 0, this.keyPressBuffer.Length, this.KeyPressCallback, null);
        }

        public HidStream Open()
        {
            return this.UnderlyingDevice.Open();
        }

        private void KeyPressCallback(IAsyncResult result)
        {
            int bytesRead = this.UnderlyingInputStream.EndRead(result);

            var buttonData = new ArraySegment<byte>(this.keyPressBuffer, ButtonPressHeaderOffset, ButtonCount).ToArray();
            var pressedButton = Array.IndexOf(buttonData, (byte)1);
            var buttonKind = pressedButton != -1 ? ButtonEventKind.DOWN : ButtonEventKind.UP;

            if (this.OnButtonPress != null)
            {
                this.OnButtonPress(this.UnderlyingDevice, new ButtonPressEventArgs(pressedButton, buttonKind));
            }

            Array.Clear(this.keyPressBuffer, 0, this.keyPressBuffer.Length);

            this.UnderlyingInputStream.BeginRead(this.keyPressBuffer, 0, this.keyPressBuffer.Length, this.KeyPressCallback, null);
        }

        public void ClearPanel()
        {
            for (int i = 0; i < this.ButtonCount; i++)
            {
                this.SetKey(i, DeviceConstants.XLDefaultBlackButton);
            }
        }

        public void SetBrightness(byte percentage)
        {

            if (percentage > 100)
            {
                percentage = 100;
            }

            var sleepRequest = new byte[]
            {
                0x03, 0x08, percentage, 0x9d, 0xc3, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };

            using var stream = this.Open();
            stream.SetFeature(sleepRequest);
        }

        public void SetupDeviceButtonMap(IEnumerable<CommandMapping> buttonMap)
        {
            foreach (var button in buttonMap)
            {
                if (button.ButtonIndex <= this.ButtonCount - 1)
                {
                    if (File.Exists(button.ButtonImagePath))
                    {
                        byte[] imageBuffer = File.ReadAllBytes(button.ButtonImagePath);
                        imageBuffer = ImageHelpers.ResizeImage(imageBuffer, DeviceConstants.XLButtonSize, DeviceConstants.XLButtonSize);
                        this.SetKey(button.ButtonIndex, imageBuffer);
                    }
                }
            }
        }


        /// <summary>
        /// Sets the content of a key on a Stream Deck device.
        /// </summary>
        /// <param name="device">Instance of a connected Stream Deck device.</param>
        /// <param name="keyId">Numberic ID of the key that needs to be set.</param>
        /// <param name="image">Binary content (JPEG) of the image that needs to be set on the key. The image will be resized to match the expectations of the connected device.</param>
        /// <returns>True if succesful, false if not.</returns>
        public bool SetKey(int keyId, byte[] image)
        {
            var content = image ?? DeviceConstants.XLDefaultBlackButton;

            var iteration = 0;
            var remainingBytes = content.Length;

            using (var stream = this.Open())
            {
                while (remainingBytes > 0)
                {
                    var sliceLength = Math.Min(remainingBytes, ImageReportPayloadLength);
                    var bytesSent = iteration * ImageReportPayloadLength;

                    byte finalizer = sliceLength == remainingBytes ? (byte)1 : (byte)0;
                    var bitmaskedLength = (byte)(sliceLength & 0xFF);
                    var shiftedLength = (byte)(sliceLength >> ImageReportHeaderLength);
                    var bitmaskedIteration = (byte)(iteration & 0xFF);
                    var shiftedIteration = (byte)(iteration >> ImageReportHeaderLength);

                    // TODO: This is different for different device classes, so I will need
                    // to make sure that I adjust the header format.
                    byte[] header = new byte[] { 0x02, 0x07, (byte)keyId, finalizer, bitmaskedLength, shiftedLength, bitmaskedIteration, shiftedIteration };
                    var payload = header.Concat(new ArraySegment<byte>(content, bytesSent, sliceLength)).ToArray();
                    var padding = new byte[ImageReportLength - payload.Length];

                    var finalPayload = payload.Concat(padding).ToArray();

                    stream.Write(finalPayload);

                    remainingBytes -= sliceLength;
                    iteration++;
                }
            }

            return true;
        }
    }
}

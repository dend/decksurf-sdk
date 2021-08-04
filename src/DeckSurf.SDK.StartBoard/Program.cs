using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DeckSurf.SDK.StartBoard
{
    class Program
    {

        static void Main(string[] args)
        {
            var exitSignal = new ManualResetEvent(false);
            var devices = DeviceManager.GetDeviceList();

            var device = ((List<ConnectedDevice>)devices)[0];
            //device.OnButtonPress += Device_OnButtonPress;
            //device.InitializeDevice();

            var icon = ImageHelpers.GetFileIcon("INVALID_PATH", DeviceConstants.XLButtonSize, DeviceConstants.XLButtonSize, SIIGBF.SIIGBF_ICONONLY | SIIGBF.SIIGBF_CROPTOSQUARE);
            var byteContent = ImageHelpers.GetImageBuffer(icon);
            var resizedImageBuffer = ImageHelpers.ResizeImage(byteContent, 96, 96);

            device.SetKey(1, resizedImageBuffer);
            //device.SetBrightness(29);
            //device.ClearPanel();

            Console.WriteLine("Done");
            exitSignal.WaitOne();
        }

        private static void Device_OnButtonPress(object source, ButtonPressEventArgs e)
        {
            Console.WriteLine(e.Id);
        }
    }
}

using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;
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

            //byte[] testImage = File.ReadAllBytes("G:\\run.jpg");
            //var image = ImageHelpers.ResizeImage(testImage, DeviceConstants.XLButtonSize, DeviceConstants.XLButtonSize);
            //device.SetKey(1, image);
            device.SetBrightness(29);

            Console.WriteLine("Done");
            exitSignal.WaitOne();
        }

        private static void Device_OnButtonPress(object source, ButtonPressEventArgs e)
        {
            Console.WriteLine(e.Id);
        }
    }
}

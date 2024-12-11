﻿using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DeckSurf.SDK.StartBoard
{
    class Program
    {

        static void Main(string[] args)
        {
            var exitSignal = new ManualResetEvent(false);
            var devices = DeviceManager.GetDeviceList();

            Console.WriteLine("The following Stream Deck devices are connected:");

            foreach (var connectedDevice in devices)
            {
                Console.WriteLine(connectedDevice.Name);
            }

            Console.ReadLine();

            var device = ((List<ConnectedDevice>)devices)[0];
            //device.OnButtonPress += Device_OnButtonPress;
            //device.InitializeDevice();

            // Path here is obtained from the first argument.
            byte[] testImage = File.ReadAllBytes(args[0]);
            var image = ImageHelpers.ResizeImage(testImage, DeviceConstants.XLButtonSize, DeviceConstants.XLButtonSize);
            device.SetKey(1, image);
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

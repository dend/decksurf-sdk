using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

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
                Console.WriteLine($"{connectedDevice.Name} ({connectedDevice.Serial})");
            }

            var device = ((List<ConnectedDevice>)devices)[0];
            device.StartListening();
            device.OnButtonPress += Device_OnButtonPress;

            byte[] testImage = File.ReadAllBytes(args[0]);

            var image = ImageHelpers.ResizeImage(testImage, device.ScreenWidth, device.ScreenHeight, RotateFlipType.Rotate180FlipNone, ImageFormat.Jpeg);
            device.SetScreen(image, 0, device.ScreenWidth, device.ScreenHeight);

            device.SetKey(1, testImage);

            device.SetBrightness(45);
            //device.ClearButtons();

            device.SetKeyColor(2, Color.Red);
            device.SetKeyColor(4, Color.Green);

            Console.WriteLine("Done");
            exitSignal.WaitOne();
        }

        private static void Device_OnButtonPress(object source, ButtonPressEventArgs e)
        {
            Console.WriteLine($"Button with ID {e.Id} was pressed. It's identified as {e.ButtonKind}. Event is {e.EventKind}. Is knob rotated: {e.IsKnobRotating}. Rotation direction: {e.KnobRotationDirection}.");

            if (e.TapCoordinates != null)
            {
                Console.WriteLine($"If this is a touch screen, coordinates are {e.TapCoordinates.Value.X} and {e.TapCoordinates.Value.Y}.");
            }
        }
    }
}

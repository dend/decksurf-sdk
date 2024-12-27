using DeckSurf.SDK.Core;
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
                Console.WriteLine($"{connectedDevice.Name} ({connectedDevice.Serial})");
            }

            var device = ((List<ConnectedDevice>)devices)[0];
            device.StartListening();
            device.OnButtonPress += Device_OnButtonPress;

            byte[] testImage = File.ReadAllBytes(args[0]);

            var image = ImageHelpers.ResizeImage(testImage, device.ScreenWidth, device.ScreenHeight, device.IsButtonImageFlipRequired);

            device.SetScreen(image, 250, device.ScreenWidth, device.ScreenHeight);

            var keyImage = ImageHelpers.ResizeImage(testImage, device.ButtonResolution, device.ButtonResolution, device.IsButtonImageFlipRequired);
            device.SetKey(1, keyImage);

            device.SetBrightness(29);
            //device.ClearButtons();

            Console.WriteLine("Done");
            exitSignal.WaitOne();
        }

        private static void Device_OnButtonPress(object source, ButtonPressEventArgs e)
        {
            Console.WriteLine($"Button with ID {e.Id} was pressed. It's identified as {e.ButtonKind}. Event is {e.EventKind}. If this is a touch screen, coordinates are {e.TapCoordinates.X} and {e.TapCoordinates.Y}. Is knob rotated: {e.IsKnobRotating}. Rotation direction: {e.KnobRotationDirection}.");
        }
    }
}

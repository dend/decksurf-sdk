using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DeckSurf.SDK.Core;
using DeckSurf.SDK.Exceptions;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.StartBoard
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: DeckSurf.SDK.StartBoard <image_path>");
                return;
            }

            var exitSignal = new ManualResetEvent(false);
            var devices = DeviceManager.GetDeviceList();

            Console.WriteLine("The following Stream Deck devices are connected:");

            foreach (var connectedDevice in devices)
            {
                Console.WriteLine($"{connectedDevice.Name} ({connectedDevice.Serial})");
            }

            if (devices.Count == 0)
            {
                Console.WriteLine("No Stream Deck devices found. Exiting.");
                return;
            }

            using var device = devices[0];

            device.ButtonPressed += Device_ButtonPressed;
            device.DeviceDisconnected += (sender, e) =>
            {
                Console.WriteLine("Device was disconnected.");
                exitSignal.Set();
            };
            device.DeviceErrorOccurred += (sender, e) =>
            {
                Console.WriteLine($"Device error in {e.OperationName}: {e.Category} (transient: {e.IsTransient})");
                if (e.RecoveryHint != null)
                {
                    Console.WriteLine($"  Hint: {e.RecoveryHint}");
                }
            };

            device.StartListening();

            try
            {
                byte[] testImage = File.ReadAllBytes(args[0]);

                if (device.IsScreenSupported)
                {
                    var image = ImageHelper.ResizeImage(testImage, device.ScreenWidth, device.ScreenHeight, device.ImageRotation, device.KeyImageFormat);
                    device.SetScreen(image, 0, device.ScreenWidth, device.ScreenHeight);
                }

                device.SetKey(1, testImage);
                device.SetBrightness(45);
                device.SetKeyColor(2, DeviceColor.Red);
                device.SetKeyColor(4, DeviceColor.Green);
            }
            catch (DeviceCommunicationException ex)
            {
                Console.WriteLine($"Communication error: {ex.Message} (transient: {ex.IsTransient})");
            }
            catch (DeviceDisconnectedException ex)
            {
                Console.WriteLine($"Device disconnected: {ex.Message}");
            }

            Console.WriteLine("Done. Press Ctrl+C or disconnect device to exit.");
            exitSignal.WaitOne();
        }

        private static void Device_ButtonPressed(object source, ButtonPressEventArgs e)
        {
            Console.WriteLine($"Button with ID {e.Id} was pressed. It's identified as {e.ButtonKind}. Event is {e.EventKind}. Is knob rotated: {e.IsKnobRotating}. Rotation direction: {e.KnobRotationDirection}.");

            if (e.TapCoordinates != null)
            {
                Console.WriteLine($"If this is a touch screen, coordinates are {e.TapCoordinates.Value.X} and {e.TapCoordinates.Value.Y}.");
            }
        }
    }
}

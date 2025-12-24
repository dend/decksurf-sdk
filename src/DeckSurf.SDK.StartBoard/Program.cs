using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;
using HidSharp;

namespace DeckSurf.SDK.StartBoard
{
    class Program
    {
        private static byte _red = 0;
        private static byte _green = 0;
        private static byte _blue = 0;

        private static byte _redDelta = 1;
        private static byte _greenDelta = 1;
        private static byte _blueDelta = 1;

        public static ConnectedDevice Device { get; set; }
        
        static void Main(string[] args)
        {
            var exitSignal = new ManualResetEvent(false);
            var devices = DeviceManager.GetDeviceList();
            var deviceList = devices as List<ConnectedDevice> ?? devices.ToList();

            Console.WriteLine("The following Stream Deck devices are connected:");

            foreach (var connectedDevice in deviceList)
            {
                Console.WriteLine($"{connectedDevice.Name} ({connectedDevice.Serial})");
            }

            if (deviceList.Count == 0)
            {
                Console.WriteLine("No compatible devices detected.");
                return;
            }

            var device = deviceList[0];
            Device = deviceList[0];
            device.StartListening();
            device.OnButtonDown += Device_OnButtonDown;
            device.OnButtonUp += Device_OnButtonUp;
            device.OnKnobDown += Device_OnKnobDown;
            device.OnKnobUp += Device_OnKnobUp;
            device.OnKnobClockwise += Device_OnKnobClockwise;
            device.OnKnobCounterClockwise += Device_OnKnobCounterClockwise;
            
            // byte[] testImage = File.ReadAllBytes(args[0]);
            //
            // var image = ImageHelpers.ResizeImage(testImage, device.ScreenWidth, device.ScreenHeight, RotateFlipType.Rotate180FlipNone, ImageFormat.Jpeg);
            // device.SetScreen(image, 0, device.ScreenWidth, device.ScreenHeight);
            //
            // device.SetKey(1, testImage);

            device.SetBrightness(100);
            //device.ClearButtons();

            device.SetKeyColor(2, Color.Red);
            device.SetKeyColor(4, Color.Green);

            Console.WriteLine("Done");
            exitSignal.WaitOne();
        }

        private static void UpdateButton(int button, byte red, byte green, byte blue)
        {
            Console.WriteLine($"{red} {green} {blue}");
            Device.SetKeyColor(button, Color.FromArgb(255, red, green, blue));
        }
        
        private static void Device_OnKnobCounterClockwise(object source, KnobCounterClockwise e)
        {
            Console.WriteLine($"Knob Counter Clockwise {e.KnobId} {e.Clicks}");
            var clicks = (byte)e.Clicks;
            switch (e.KnobId)
            {
                case 0: _red = (byte)int.Max(0, _red - clicks * _redDelta); break;
                case 1: _green = (byte)int.Max(0, _green - clicks * _greenDelta); break;
                case 2: _blue = (byte)int.Max(0, _blue - clicks * _blueDelta); break;
            }

            UpdateButton(0, _red, _green, _blue);
        }
        
        private static void Device_OnKnobClockwise(object source, KnobClockwise e)
        {
            Console.WriteLine($"Knob Clockwise {e.KnobId} {e.Clicks}");
            var clicks = (byte)e.Clicks;
            switch (e.KnobId)
            {
                case 0: _red = (byte)int.Min(255, _red + clicks * _redDelta); break;
                case 1: _green = (byte)int.Min(255, _green + clicks * _greenDelta); break;
                case 2: _blue = (byte)int.Min(255, _blue + clicks * _blueDelta); break;
            }

            UpdateButton(0, _red, _green, _blue);
        }
        
        private static void Device_OnKnobUp(object source, KnobUp e)
        {
            switch (e.KnobId)
            {
                case 0 : _redDelta = (byte)(11 - _redDelta); break;
                case 1 : _greenDelta = (byte)(11 - _greenDelta); break;
                case 2: _blueDelta = (byte)(11 - _blueDelta); break;
            }
            Console.WriteLine($"Knob up {e.KnobId}");
        }
        
        private static void Device_OnKnobDown(object source, KnobDown e)
        {
            Console.WriteLine($"Knob down {e.KnobId}");
        }

        private static void Device_OnButtonDown(object source, ButtonDown e)
        {
            Console.WriteLine($"Button down {e.ButtonId}");
        }

        private static void Device_OnButtonUp(object source, ButtonUp e)
        {
            Console.WriteLine($"Button up {e.ButtonId}");
        }
    }
}

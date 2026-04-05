![DeckSurf SDK Icon](images/logo-small.webp)

# DeckSurf SDK for .NET

_**Unofficial Software Development Kit for your Stream Deck, built in C# for the .NET platform.**_

>[!WARNING]
>This SDK is under active development and is currently in its **alpha stage**. That means that there _may be_ breaking changes between releases until it hits `1.0.0`.

>[!NOTE]
>For the DeckSurf tooling (_CLI and plugins_), refer to the [DeckSurf repository](https://github.com/dend/DeckSurf).

[![NuGet Version](https://img.shields.io/nuget/v/DeckSurf.SDK)](https://www.nuget.org/packages/DeckSurf.SDK)

## Installation

```bash
dotnet add package DeckSurf.SDK
```

## Quick Start

```csharp
using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;

// Enumerate connected Stream Deck devices
var devices = DeviceManager.GetDeviceList();
if (devices.Count == 0)
{
    Console.WriteLine("No Stream Deck devices found.");
    return;
}

// Use the first device
using var device = devices[0];

// Listen for button presses (filter to Down to avoid double-firing on release)
device.ButtonPressed += (sender, e) =>
{
    if (e.EventKind != ButtonEventKind.Down) return;
    Console.WriteLine($"Button {e.Id} pressed (type: {e.ButtonKind})");
};

device.StartListening();

// Set a button image (JPEG, PNG, BMP, GIF, or any ImageSharp-supported format)
// Images are automatically resized to match the device's button resolution.
byte[] image = File.ReadAllBytes("icon.png");
device.SetKey(0, image);

// Set brightness (0-100)
device.SetBrightness(80);
```

> **Tip:** Button events fire twice per physical press — once for `Down` (pressed) and once for `Up` (released). Filter on `ButtonEventKind.Down` if you only want to respond once per press.

## Button Layout

Buttons are numbered **left-to-right, top-to-bottom**, starting at 0. Use `device.ButtonColumns` and `device.ButtonRows` to understand the grid:

```
Stream Deck Original (5×3):
 0  1  2  3  4
 5  6  7  8  9
10 11 12 13 14
```

| Device | Buttons | Layout | Button Resolution |
|:---|:---|:---|:---|
| Original / Original 2019 / MK.2 | 15 | 5×3 | 72×72 px |
| XL / XL 2022 | 32 | 8×4 | 96×96 px |
| Mini / Mini 2022 | 6 | 3×2 | 80×80 px |
| Neo | 8 | 4×2 | 96×96 px |
| Plus | 8 | 4×2 | 120×120 px |

Images passed to `SetKey()` are **automatically resized** to the device's button resolution. For best results, use square images. Non-square images will be stretched.

## Error Handling

The SDK uses a structured exception model rooted in `DeckSurfException`:

| Exception | When | Key Property |
|:---|:---|:---|
| `DeviceCommunicationException` | USB I/O failure during operation | `IsTransient` — true if safe to retry |
| `DeviceDisconnectedException` | Device unplugged mid-operation | `DeviceSerial` — identifies which device |
| `DeviceNotFoundException` | Device lookup failed (serial/path not found) | — |
| `ImageProcessingException` | Unrecognized image format in `SetKey` or `ImageHelper.ResizeImage` | — |
| `ObjectDisposedException` | Method called on a disposed device | — |
| `InvalidOperationException` | `StartListening()` called when already listening | — |
| `ArgumentOutOfRangeException` | Button index out of range in `SetKey` | — |
| `IndexOutOfRangeException` | Key index out of range in `SetKeyColor` | — |

```csharp
using DeckSurf.SDK.Exceptions;

try
{
    device.SetKey(0, imageData);
}
catch (DeviceCommunicationException ex) when (ex.IsTransient)
{
    // USB I/O failure — safe to retry
}
catch (DeviceDisconnectedException ex)
{
    // Device was physically unplugged
    Console.WriteLine($"Lost device {ex.DeviceSerial}");
}
```

For event-driven error handling, subscribe to `DeviceErrorOccurred`:

```csharp
device.DeviceErrorOccurred += (sender, e) =>
{
    Console.WriteLine($"Error in {e.OperationName}: {e.Category} (transient: {e.IsTransient})");
};
```

## Device Disconnection

When a device is unplugged, the `DeviceDisconnected` event fires. After this event, the device instance is unusable — no further events will fire and all method calls will throw `ObjectDisposedException`. Dispose it and acquire a fresh instance to reconnect:

```csharp
device.DeviceDisconnected += (sender, e) =>
{
    Console.WriteLine("Device disconnected. Attempting to reconnect...");
    device.Dispose();

    // Re-enumerate to find the device again
    if (DeviceManager.TryGetDeviceBySerial(knownSerial, out var newDevice))
    {
        // Use newDevice
    }
};
```

## Multiple Devices

Use serial numbers for stable device identification across re-plugs:

```csharp
// Find a specific device
if (DeviceManager.TryGetDeviceBySerial("CL12K1A00042", out var myDevice))
{
    using (myDevice)
    {
        myDevice.StartListening();
        myDevice.SetKey(0, imageData);
    }
}

// Or throw if not found
var device = DeviceManager.GetDeviceBySerial("CL12K1A00042");

// Monitor for device connection changes
DeviceManager.DeviceListChanged += (sender, e) =>
{
    Console.WriteLine("USB device change detected — re-enumerate devices.");
};
```

## Screen Support (Plus & Neo)

Devices with LCD screens expose screen operations:

```csharp
if (device.IsScreenSupported)
{
    byte[] screenImage = File.ReadAllBytes("banner.jpg");
    var resized = ImageHelper.ResizeImage(
        screenImage,
        device.ScreenWidth,
        device.ScreenHeight,
        device.ImageRotation,
        device.KeyImageFormat);
    device.SetScreen(resized, 0, device.ScreenWidth, device.ScreenHeight);
}
```

## Thread Safety

The SDK is **not thread-safe**. All events (`ButtonPressed`, `DeviceDisconnected`, `DeviceErrorOccurred`) fire on **background threads** (thread pool). If you need to update UI or call device methods from event handlers, you must synchronize access:

```csharp
var deviceLock = new object();

// Safe to call SetKey from a button press handler
device.ButtonPressed += (sender, e) =>
{
    if (e.EventKind != ButtonEventKind.Down) return;
    lock (deviceLock)
    {
        device.SetKey(e.Id, highlightImage);
    }
};

// WPF/WinForms: marshal to UI thread
device.ButtonPressed += (sender, e) =>
{
    Dispatcher.Invoke(() => statusLabel.Text = $"Button {e.Id} pressed");
};
```

## Supported Devices

| Device | Support |
|:---|:---|
| Stream Deck XL | Full |
| Stream Deck XL (2022) | Full |
| Stream Deck Plus | Full |
| Stream Deck Original | Full |
| Stream Deck Original (2019) | Full |
| Stream Deck MK.2 | Full |
| Stream Deck Mini | Full |
| Stream Deck Mini (2022) | Full |
| Stream Deck Neo | Full |

## Platform Support

Core functionality is **cross-platform** (Windows, macOS, Linux). The SDK uses [HidSharp](https://github.com/IntergatedCircuits/HidSharp) for USB HID communication.

A small number of utility methods (`ImageHelper.GetFileIcon()`, `ImageHelper.GetImageBuffer(Icon)`) are **Windows-only** and are marked with `[SupportedOSPlatform("windows")]`. All core device operations work cross-platform.

### Platform Setup

**Windows:** Works out of the box. Close the Elgato Stream Deck software before running — it holds exclusive access to the device.

**macOS:** USB HID access may require app entitlements (`com.apple.security.device.usb`). Without this, `GetDeviceList()` will return an empty list with no error.

**Linux:** Configure udev rules for non-root USB access:
```bash
# /etc/udev/rules.d/99-streamdeck.rules
SUBSYSTEM=="usb", ATTRS{idVendor}=="0fd9", MODE="0666"
SUBSYSTEM=="hidraw", ATTRS{idVendor}=="0fd9", MODE="0666"
```
Then reload: `sudo udevadm control --reload-rules && sudo udevadm trigger`

## Troubleshooting

**`GetDeviceList()` returns empty:**
- Close the Elgato Stream Deck software (it holds exclusive USB access)
- On Linux, ensure udev rules are configured (see above)
- On macOS, check USB entitlements in your app's codesign profile
- Disconnect and reconnect the device
- Verify the device appears in your OS device manager

## Documentation

Refer to [`https://docs.deck.surf`](https://docs.deck.surf/) for tutorials and full API documentation.

## License

This project is licensed under the MIT License. See [LICENSE.md](LICENSE.md) for details.

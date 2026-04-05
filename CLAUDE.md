# CLAUDE.md — DeckSurf SDK

## Project Overview

DeckSurf SDK is an open-source .NET library for managing Elgato Stream Deck devices. It provides device enumeration, button image management, screen control, and a plugin architecture for third-party developers.

- **Target framework**: .NET 10
- **Cross-platform**: Core functionality works on Windows, macOS, and Linux. Windows-only APIs are guarded with `[SupportedOSPlatform("windows")]`.
- **Author**: Den Delimarsky

## Repository Structure

```
src/
  DeckSurf.SDK/              # Main SDK library
    Core/                    # DeviceManager, DeviceRegistry
    Exceptions/              # Custom exception hierarchy
    Interfaces/              # IConnectedDevice, IDeckSurfCommand, IDeckSurfPlugin
    Models/                  # Domain types, device implementations
      Devices/               # Per-device implementations + base classes
    Util/                    # ImageHelper, DataHelper, ConfigurationHelper
  DeckSurf.SDK.Tests/        # xUnit test project
  DeckSurf.SDK.StartBoard/   # Example/demo application
```

## Build and Test

```bash
cd src
dotnet build DeckSurf.SDK.sln        # Build all projects
dotnet test DeckSurf.SDK.Tests/      # Run tests with coverage enforcement
```

- Build uses `TreatWarningsAsErrors` — all warnings must be resolved.
- Tests enforce a minimum **50% line coverage** threshold via coverlet. The build will fail if coverage drops below this.
- Windows-only code (marked with `[SupportedOSPlatform]`) is excluded from coverage metrics.

## Code Style and Linting

- **StyleCop.Analyzers** enforces code style (ordering, documentation, naming).
- **Microsoft.CodeAnalysis.NetAnalyzers** enforces code quality (CA rules).
- `.editorconfig` at `src/` root defines formatting, naming conventions, and analyzer severity.
- `TreatWarningsAsErrors` is enabled — all warnings are build errors.
- All public and interface members must have XML documentation (`documentExposedElements` and `documentInterfaces` are enabled in `stylecop.json`).
- Use `this.` prefix for instance member access (project convention).
- Copyright header required on all `.cs` files (SA1633 enforced as error).
- Elements must be separated by blank lines (SA1516).
- Members must be ordered by kind (fields, constructors, events, properties, methods) and access level (public before private) per SA1201/SA1202.

### Analyzer Suppression Policy

- **Suppressing a warning is never the default.** Fix the code instead.
- A suppression requires **strong justification** — document the reason in a code comment or the `.csproj` NoWarn line.
- Legitimate suppression reasons: modern C# syntax not yet recognized by analyzers (e.g., SA1010 for collection expressions), deliberate architectural grouping (e.g., SA1649 for NativeStructures.cs), or rules that conflict with project conventions (e.g., SA1101 — this project uses `this.` prefix).
- Before adding a global suppression to NoWarn, try a targeted `#pragma warning disable` with a justification comment.
- When reviewing PRs, challenge any new suppression — the bar is "would this confuse a future contributor?"

## Architecture Conventions

### Device Hierarchy

```
ConnectedDevice (abstract, implements IConnectedDevice)
├── JpegButtonsDevice (abstract) — JPEG/Rotate180 devices
│   ├── StreamDeckOriginal, StreamDeckOriginal2019, StreamDeckMK2
│   ├── StreamDeckXL, StreamDeckXL2022
├── BitmapButtonsDevice (abstract) — BMP/Rotate270 devices
│   ├── StreamDeckMini, StreamDeckMini2022
├── ScreenDevice (abstract) — Devices with LCD screens
│   ├── StreamDeckNeo, StreamDeckPlus
```

- Add new devices by subclassing the appropriate base class and registering in `DeviceRegistry`.
- Device-specific properties only (Model, ButtonCount, ButtonResolution, ButtonColumns, ButtonRows).
- Shared protocol logic lives in base classes to avoid duplication.

### Adding a New Device Model

1. Add the PID to the `DeviceModel` enum.
2. Create a new class inheriting from the appropriate base (`JpegButtonsDevice`, `BitmapButtonsDevice`, or `ScreenDevice`).
3. Register it in `DeviceRegistry.cs` static constructor.
4. Add a parameterized test case in `DeviceSpecTests.cs`.

### Error Handling

- Custom exception hierarchy rooted at `DeckSurfException`.
- Validation errors throw immediately (`ArgumentException`, `ArgumentOutOfRangeException`).
- USB I/O failures throw `DeviceCommunicationException` (check `IsTransient` for retry logic).
- Device disconnection throws `DeviceDisconnectedException`.
- Events use `DeviceErrorEventArgs` with structured `Category`, `IsTransient`, and `RecoveryHint`.
- Never swallow exceptions silently — throw domain exceptions or let them propagate.

### Naming Conventions

- Events: past tense, no `On` prefix (`ButtonPressed`, `DeviceDisconnected`, `DeviceErrorOccurred`).
- Helper classes: singular (`ImageHelper`, `DataHelper`, `ConfigurationHelper`).
- Plugin interfaces: spell out the namespace (`IDeckSurfCommand`, `IDeckSurfPlugin`, not abbreviations).
- Enum values: PascalCase, no type prefix (`DeviceRotation.Rotate180`, not `DeviceRotation.DeviceRotation180`).

## Testing Requirements

### When to Write Tests

- **Every new public method or type** must have corresponding unit tests before merging.
- **Every bug fix** must include a regression test that would have caught the bug.
- **Every new device model** must have a parameterized test case in `DeviceSpecTests.cs` verifying all hardware specs.
- **Every new exception type** must be tested for construction, property access, and inheritance.

### What Must Be Tested

- Input validation (null checks, bounds checks, argument exceptions).
- Value type equality, hashing, and operators (any new struct).
- Protocol byte generation (key setup headers, packet format).
- Image processing validation (format detection, dimension checks).
- Configuration roundtrips (serialize → deserialize → verify).
- Device registry (factory registration, lookup, type correctness).

### What Cannot Be Unit Tested (Requires Hardware)

- USB HID device enumeration (`DeviceManager.GetDeviceList()` with real devices).
- HID stream read/write operations (`SetKey` I/O loop, `SetScreen`, `SetBrightness`).
- Button press event chain (`StartListening` → `HandleKeyPress` → callback).
- These paths are excluded from coverage thresholds and should be tested manually with physical devices.

### Coverage Threshold

- Minimum **50% line coverage** enforced by coverlet.
- When adding testable code, ensure the threshold is maintained or raised.
- If coverage drops below the threshold, the test run will fail.
- Target **88%+ method coverage** (currently met).

## Multi-Device Support

- Each `ConnectedDevice` instance is identified by VID + PID + DevicePath (not just VID + PID).
- Use `DeviceManager.GetDeviceBySerial()` or `GetDeviceByPath()` for stable device targeting — never hardcode indices.
- `DeviceManager.DeviceListChanged` fires when USB devices are connected or disconnected.
- `ConfigurationProfile.DeviceSerial` is preferred over `DeviceIndex` for profile matching.

## Dependencies

- **HidSharp** (2.1.0) — USB HID communication, cross-platform.
- **SixLabors.ImageSharp** (3.1.12) — Cross-platform image processing (replaces System.Drawing for core operations).
- **System.Drawing.Common** (9.0.0) — Retained only for Windows-guarded APIs (`GetFileIcon`).
- **StyleCop.Analyzers** + **Microsoft.CodeAnalysis.NetAnalyzers** — Linting (dev-only).

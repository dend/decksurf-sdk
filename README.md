![DeckSurf SDK Icon](images/logo-small.webp)

# 🌊 DeckSurf SDK for .NET

_**Unofficial Software Development Kit for your Stream Deck, built in C# for the .NET platform.**_

>[!NOTE]
>For the DeckSurf tooling (_CLI and plugins_), refer to the [DeckSurf repository](https://github.com/dend/DeckSurf).

![NuGet Version](https://img.shields.io/nuget/v/DeckSurf.SDK)

## About

The DeckSurf SDK is used to manage Stream Deck devices and create plugins for [DeckSurf tools](https://github.com/dend/DeckSurf). It is completely independent of the Elgato software and/or libraries and can be used as a standalone library.

## Installation

You can use the SDK by installing it [from NuGet](https://www.nuget.org/packages/DeckSurf.SDK):

```powershell
dotnet add package DeckSurf.SDK
```

## Supported devices

Support for the following devices is implemented:

- Stream Deck XL
- Stream Deck Plus

Future support is coming for:

- Stream Deck Neo
- Stream Deck Mini
- Stream Deck MK.2
- Stream Deck Original V2

## Documentation

Refer to [`https://docs.deck.surf`](https://docs.deck.surf/) for tutorials and SDK documentation.

## Platform compatibility

The SDK in its current implementation has a number of dependencies on Windows APIs, therefore will only work on Windows. In future releases, I am thinking of a way to rip out native components and separate them in their own package, allowing the SDK to be fully cross-platform.

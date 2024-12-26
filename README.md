![DeckSurf SDK Icon](images/logo-small.webp)

# 🌊 DeckSurf SDK

_**Unofficial Software Development Kit for your Stream Deck.**_

>[!NOTE]
>For the DeckSurf tooling (_CLI and plugins_), refer to the [DeckSurf repository](https://github.com/dend/DeckSurf).

## About

The DeckSurf SDK is used to manage Stream Deck devices and create plugins for [DeckSurf tools](https://github.com/dend/DeckSurf). It is completely independent of the Elgato software and/or libraries and can be used as a standalone library.

## Platform compatibility

The SDK in its current implementation has a number of dependencies on Windows APIs, therefore will only work on Windows. In future releases, I am thinking of a way to rip out native components and separate them in their own package, allowing the SDK to be fully cross-platform.

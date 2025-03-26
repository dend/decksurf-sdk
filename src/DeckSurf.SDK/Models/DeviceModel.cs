// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Enum represening Stream Deck device models and their respective USB product IDs (PIDs).
    /// </summary>
    public enum DeviceModel : byte
    {
        /// <summary>
        /// The original model of the Stream Deck device.
        /// </summary>
        Original = 0x0060,

        /// <summary>
        /// The <see href="https://www.elgato.com/en/stream-deck">updated original model</see> of the Stream Deck device.
        /// </summary>
        Original2019 = 0x006D,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-mk2-black">MK.2</see> of the Stream Deck device.
        /// </summary>
        MK2 = 0x0080,

        /// <summary>
        /// The MK.2 Scissor of the Stream Deck device.
        /// </summary>
        /// <remarks>
        /// Need to document what the "Scissor" variant actually is.
        /// </remarks>
        MK2Scissor = 0x00A5,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-mini">Mini model</see> of the Stream Deck device.
        /// </summary>
        Mini = 0x0063,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-mini">Mini (2022) model</see> of the Stream Deck device.
        /// </summary>
        Mini2022 = 0x0090,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-xl">XL model</see> of the Stream Deck device.
        /// </summary>
        XL = 0x006C,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-xl">XL (2022) model</see> of the Stream Deck device.
        /// </summary>
        XL2022 = 0x008F,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-plus-black">Plus model</see> of the Stream Deck device.
        /// </summary>
        Plus = 0x0084,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-neo">Neo model</see> of the Stream Deck device.
        /// </summary>
        Neo = 0x009A,
    }
}

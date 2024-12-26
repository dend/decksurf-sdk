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
        ORIGINAL = 0x0060,

        /// <summary>
        /// The <see href="https://www.elgato.com/en/stream-deck">updated original model</see> of the Stream Deck device.
        /// </summary>
        ORIGINAL_V2 = 0x006D,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-mini">Mini model</see> of the Stream Deck device.
        /// </summary>
        MINI = 0x0063,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-xl">XL model</see> of the Stream Deck device.
        /// </summary>
        XL = 0x006C,

        /// <summary>
        /// The <see href="https://www.elgato.com/us/en/p/stream-deck-plus-black">XL model</see> of the Stream Deck device.
        /// </summary>
        PLUS = 0x0084,
    }
}

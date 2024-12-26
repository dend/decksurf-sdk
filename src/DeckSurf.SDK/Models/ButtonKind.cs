// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Represents the type of button used on the Stream Deck device.
    /// </summary>
    public enum ButtonKind
    {
        /// <summary>
        /// Standard button.
        /// </summary>
        Button,

        /// <summary>
        /// Knob, as used on the Stream Deck Plus.
        /// </summary>
        Knob,

        /// <summary>
        /// Touch screen, as used on the Stream Deck Plus.
        /// </summary>
        Screen,

        /// <summary>
        /// Unknown button type.
        /// </summary>
        Unknown,
    }
}

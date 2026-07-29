// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Identifies which piece of device hardware a <see cref="CommandMapping"/> is bound to.
    /// </summary>
    public enum MappingTarget
    {
        /// <summary>
        /// A physical key on the button grid. <see cref="CommandMapping.ButtonIndex"/> is the
        /// zero-based key index, or -1 for the any-key catch-all.
        /// </summary>
        Key = 0,

        /// <summary>
        /// A rotary knob (Stream Deck Plus). <see cref="CommandMapping.ButtonIndex"/> is the
        /// zero-based knob index. Rotation and press events are delivered through
        /// <see cref="Interfaces.IDeckSurfCommand.ExecuteOnEvent"/>.
        /// </summary>
        Knob = 1,

        /// <summary>
        /// The touch screen (Stream Deck Plus and Neo). Tap events are delivered through
        /// <see cref="Interfaces.IDeckSurfCommand.ExecuteOnEvent"/>, and
        /// <see cref="CommandMapping.ButtonImagePath"/> is rendered to the screen on activation.
        /// </summary>
        Screen = 2,
    }
}

// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Describes the data type of a command parameter declared with <see cref="CommandParameterAttribute"/>.
    /// Consumers (such as graphical configuration tools) use this to decide which input control to render.
    /// </summary>
    public enum CommandParameterType
    {
        /// <summary>
        /// Free-form text value.
        /// </summary>
        String = 0,

        /// <summary>
        /// Integer numeric value. Can be constrained with <see cref="CommandParameterAttribute.MinValue"/>
        /// and <see cref="CommandParameterAttribute.MaxValue"/>.
        /// </summary>
        Integer = 1,

        /// <summary>
        /// Boolean value, serialized as "true" or "false".
        /// </summary>
        Boolean = 2,

        /// <summary>
        /// One value out of a fixed set declared in <see cref="CommandParameterAttribute.Choices"/>.
        /// </summary>
        Choice = 3,

        /// <summary>
        /// Path to a file on disk.
        /// </summary>
        FilePath = 4,

        /// <summary>
        /// Path to a folder on disk.
        /// </summary>
        FolderPath = 5,

        /// <summary>
        /// Path to an image file on disk.
        /// </summary>
        ImagePath = 6,

        // Value 7 was DurationSeconds, folded into Integer: a duration is an
        // integer with a range, and units belong in the parameter's own name
        // or description. The gap stays so compiled plugins keep their values.

        /// <summary>
        /// Sensitive free-form text, such as a password or API token. Behaves like
        /// <see cref="String"/> except that tooling masks the input field. The value
        /// is still stored as plain text in the profile file, like every other
        /// parameter value.
        /// </summary>
        Secret = 8,
    }
}

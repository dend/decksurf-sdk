// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Attribute that declares a display icon for a plugin command. The path is
    /// relative to the directory containing the plugin assembly, so a plugin can
    /// ship its icon files alongside its binaries. Hosts (such as graphical
    /// profile editors) use the icon to represent the command; the attribute has
    /// no effect on command execution.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CommandIconAttribute"/> class.
    /// </remarks>
    /// <param name="relativePath">Icon file path relative to the plugin assembly directory.</param>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CommandIconAttribute(string relativePath) : Attribute
    {
        /// <summary>
        /// Gets the icon file path relative to the plugin assembly directory.
        /// </summary>
        public string RelativePath { get; } = relativePath;
    }
}

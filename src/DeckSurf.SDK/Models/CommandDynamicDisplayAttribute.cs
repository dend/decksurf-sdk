// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Attribute that declares a command as rendering its own key or screen
    /// content at runtime (live counters, clocks, games). A user-provided static
    /// button image has no effect on such commands, so graphical hosts should not
    /// offer image configuration for them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CommandDynamicDisplayAttribute : Attribute
    {
    }
}

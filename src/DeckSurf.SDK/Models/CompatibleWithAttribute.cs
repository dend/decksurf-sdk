// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Attribute that determines the class of Stream Deck devices that a plugin is compatible with.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CompatibleWithAttribute"/> class.
    /// </remarks>
    /// <param name="model">Reference to the <see cref="DeviceModel"/> device model type.</param>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class CompatibleWithAttribute(DeviceModel model) : Attribute
    {
        /// <summary>
        /// Gets or sets the <see cref="DeviceModel"/> device model type.
        /// </summary>
        public DeviceModel CompatibleModel { get; set; } = model;
    }
}

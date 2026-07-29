// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Declares a configurable parameter for a DeckSurf command. Parameters are serialized as
    /// key=value pairs inside <see cref="CommandMapping.CommandArguments"/> and can be parsed
    /// with <see cref="Util.CommandArgumentParser"/>. Declaring parameters lets tooling (such as
    /// a graphical profile editor) generate configuration forms for a command without any
    /// command-specific knowledge.
    /// </summary>
    /// <remarks>
    /// Commands without any <see cref="CommandParameterAttribute"/> annotations are treated as
    /// having no configurable parameters. Dynamic choice lists (values only known at runtime)
    /// are not supported by this attribute; consumers should fall back to free-form input.
    /// </remarks>
    /// <param name="key">The parameter key used in the serialized command arguments string.</param>
    /// <param name="parameterType">The data type of the parameter.</param>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class CommandParameterAttribute(string key, CommandParameterType parameterType) : Attribute
    {
        /// <summary>
        /// Gets the parameter key used in the serialized command arguments string.
        /// </summary>
        public string Key { get; } = !string.IsNullOrWhiteSpace(key) ? key : throw new ArgumentException("Parameter key cannot be null or whitespace.", nameof(key));

        /// <summary>
        /// Gets the data type of the parameter.
        /// </summary>
        public CommandParameterType ParameterType { get; } = parameterType;

        /// <summary>
        /// Gets or sets the human-readable label for the parameter. Consumers fall back to
        /// <see cref="Key"/> when this is not set.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the help text describing what the parameter does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter must be provided for the
        /// command to function.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the default value for the parameter, in its string form.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the set of allowed values for a <see cref="CommandParameterType.Choice"/> parameter.
        /// </summary>
        public string[] Choices { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed value for numeric parameters. Defaults to <see cref="int.MinValue"/> (unbounded).
        /// </summary>
        public int MinValue { get; set; } = int.MinValue;

        /// <summary>
        /// Gets or sets the maximum allowed value for numeric parameters. Defaults to <see cref="int.MaxValue"/> (unbounded).
        /// </summary>
        public int MaxValue { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the relative position of the parameter in generated configuration forms.
        /// Parameters are sorted by this value, then by <see cref="Key"/>.
        /// </summary>
        public int Order { get; set; }
    }
}

// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Validates a set of parameter values against a command's declared parameter schema.
    /// </summary>
    public static class CommandParameterValidator
    {
        /// <summary>
        /// Validates parameter values against a schema. Checks that required parameters are
        /// present and non-empty, that numeric parameters parse and fall within their declared
        /// range, that boolean parameters parse, and that choice parameters use one of the
        /// declared values. File and folder paths are not checked for existence, since profiles
        /// may be shared across machines.
        /// </summary>
        /// <param name="schema">The declared parameters, as returned by <see cref="CommandSchemaReader"/>.</param>
        /// <param name="values">The parameter values keyed by parameter key.</param>
        /// <returns>A list of human-readable validation errors. Empty when the values are valid.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="schema"/> or <paramref name="values"/> is null.</exception>
        public static IReadOnlyList<string> Validate(
            IReadOnlyList<CommandParameterAttribute> schema,
            IReadOnlyDictionary<string, string> values)
        {
            ArgumentNullException.ThrowIfNull(schema);
            ArgumentNullException.ThrowIfNull(values);

            var errors = new List<string>();

            foreach (var parameter in schema)
            {
                var label = parameter.DisplayName ?? parameter.Key;
                var hasValue = values.TryGetValue(parameter.Key, out var value) && !string.IsNullOrWhiteSpace(value);

                if (!hasValue)
                {
                    if (parameter.Required)
                    {
                        errors.Add($"'{label}' is required.");
                    }

                    continue;
                }

                switch (parameter.ParameterType)
                {
                    case CommandParameterType.Integer:
                        if (!int.TryParse(value, out var numericValue))
                        {
                            errors.Add($"'{label}' must be a whole number.");
                        }
                        else if (numericValue < parameter.MinValue || numericValue > parameter.MaxValue)
                        {
                            errors.Add($"'{label}' must be between {parameter.MinValue} and {parameter.MaxValue}.");
                        }

                        break;
                    case CommandParameterType.Boolean:
                        if (!bool.TryParse(value, out _))
                        {
                            errors.Add($"'{label}' must be 'true' or 'false'.");
                        }

                        break;
                    case CommandParameterType.Choice:
                        if (parameter.Choices == null || parameter.Choices.Length == 0)
                        {
                            errors.Add($"'{label}' is a choice parameter but declares no choices.");
                        }
                        else if (!parameter.Choices.Contains(value, StringComparer.OrdinalIgnoreCase))
                        {
                            errors.Add($"'{label}' must be one of: {string.Join(", ", parameter.Choices)}.");
                        }

                        break;
                }
            }

            return errors;
        }
    }
}

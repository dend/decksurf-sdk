// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Static utility class for parsing <see cref="Models.CommandMapping.CommandArguments"/> strings
    /// into structured key-value pairs.
    /// </summary>
    public static class CommandArgumentParser
    {
        private static readonly IReadOnlyDictionary<string, string> EmptyDictionary =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        /// <summary>
        /// Parses a comma-separated key=value string into a read-only dictionary.
        /// </summary>
        /// <param name="arguments">
        /// A comma-separated string of key=value pairs.
        /// Example: "device_id=light_01,brightness=80" produces {"device_id": "light_01", "brightness": "80"}.
        /// </param>
        /// <returns>
        /// A read-only dictionary of parsed key-value pairs. Returns an empty dictionary if
        /// <paramref name="arguments"/> is <c>null</c> or empty. Keys missing an <c>=</c> delimiter
        /// are stored with an empty string value. Both keys and values are trimmed of surrounding whitespace.
        /// </returns>
        public static IReadOnlyDictionary<string, string> Parse(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                return EmptyDictionary;
            }

            var parts = arguments.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = new Dictionary<string, string>(parts.Length, StringComparer.OrdinalIgnoreCase);

            foreach (var part in parts)
            {
                var equalsIndex = part.IndexOf('=');
                if (equalsIndex < 0)
                {
                    // No '=' found — treat the entire segment as a key with an empty value.
                    var key = part.Trim();
                    if (key.Length > 0)
                    {
                        result[key] = string.Empty;
                    }
                }
                else
                {
                    var key = part.Substring(0, equalsIndex).Trim();
                    var value = part.Substring(equalsIndex + 1).Trim();
                    if (key.Length > 0)
                    {
                        result[key] = value;
                    }
                }
            }

            return new ReadOnlyDictionary<string, string>(result);
        }

        /// <summary>
        /// Tries to get a value from a command arguments string by key.
        /// </summary>
        /// <param name="arguments">A comma-separated string of key=value pairs.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key if
        /// the key is found; otherwise, <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the key was found in the parsed arguments; otherwise, <c>false</c>.</returns>
        public static bool TryGetValue(string arguments, string key, out string value)
        {
            var parsed = Parse(arguments);
            return parsed.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets a value from a command arguments string by key, or returns a default value
        /// if the key is not present.
        /// </summary>
        /// <param name="arguments">A comma-separated string of key=value pairs.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">
        /// The value to return if the key is not found. Defaults to an empty string.
        /// </param>
        /// <returns>
        /// The value associated with the specified key, or <paramref name="defaultValue"/> if the
        /// key is not found.
        /// </returns>
        public static string GetValueOrDefault(string arguments, string key, string defaultValue = "")
        {
            var parsed = Parse(arguments);
            return parsed.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Formats a set of key-value pairs into a comma-separated key=value string — the
        /// inverse of <see cref="Parse"/>. Pairs with null values are serialized with an
        /// empty value.
        /// </summary>
        /// <param name="values">The key-value pairs to serialize.</param>
        /// <returns>A comma-separated string of key=value pairs, or an empty string when <paramref name="values"/> is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a key or value contains a comma or a key contains an equals sign, since
        /// the serialization format cannot represent those characters.
        /// </exception>
        public static string Format(IReadOnlyDictionary<string, string> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            var parts = new List<string>(values.Count);

            foreach (var pair in values)
            {
                if (string.IsNullOrWhiteSpace(pair.Key))
                {
                    continue;
                }

                if (pair.Key.Contains(',') || pair.Key.Contains('='))
                {
                    throw new ArgumentException($"Parameter key '{pair.Key}' contains characters that cannot be represented in the command arguments format (',' or '=').", nameof(values));
                }

                var value = pair.Value ?? string.Empty;
                if (value.Contains(','))
                {
                    throw new ArgumentException($"Value for parameter '{pair.Key}' contains a comma, which cannot be represented in the command arguments format.", nameof(values));
                }

                parts.Add($"{pair.Key.Trim()}={value.Trim()}");
            }

            return string.Join(",", parts);
        }
    }
}

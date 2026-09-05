// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Immutable set of arguments passed to a command, keyed case-insensitively by
    /// parameter key. In profile JSON the arguments are stored as a plain object
    /// (<c>"command_arguments": { "scene": "Gameplay" }</c>), which places no
    /// restrictions on the characters a key or value may contain. The legacy
    /// comma-separated <c>key=value</c> string format is still accepted when
    /// reading profiles and is written back unchanged (see <see cref="LegacyText"/>)
    /// so existing profiles round-trip losslessly until they are edited.
    /// </summary>
    [JsonConverter(typeof(CommandArgumentsJsonConverter))]
    public sealed class CommandArguments : IReadOnlyDictionary<string, string>
    {
        private readonly Dictionary<string, string> values;

        private CommandArguments(Dictionary<string, string> values, string legacyText)
        {
            this.values = values;
            this.LegacyText = legacyText;
        }

        /// <summary>
        /// Gets an empty argument set. Used as the default for
        /// <see cref="CommandMapping.CommandArguments"/> so commands never need to
        /// null-check their arguments.
        /// </summary>
        public static CommandArguments Empty { get; } = new(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), null);

        /// <summary>
        /// Gets the original delimited string when this instance was parsed from the
        /// legacy comma-separated <c>key=value</c> format, and <c>null</c> when it was
        /// created from a JSON object or a dictionary. Commands that historically
        /// accepted a bare value instead of key/value pairs can fall back to this.
        /// </summary>
        public string LegacyText { get; }

        /// <inheritdoc/>
        public int Count => this.values.Count;

        /// <inheritdoc/>
        public IEnumerable<string> Keys => this.values.Keys;

        /// <inheritdoc/>
        public IEnumerable<string> Values => this.values.Values;

        /// <inheritdoc/>
        public string this[string key] => this.values[key];

        /// <summary>
        /// Creates an argument set from a dictionary of parameter values. Keys are
        /// compared case-insensitively; when the source dictionary contains keys that
        /// differ only in casing, the last one wins.
        /// </summary>
        /// <param name="values">The parameter values keyed by parameter key.</param>
        /// <returns>The argument set. Null values are normalized to empty strings.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        public static CommandArguments FromDictionary(IReadOnlyDictionary<string, string> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            if (values.Count == 0)
            {
                return Empty;
            }

            var copy = new Dictionary<string, string>(values.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var pair in values)
            {
                if (!string.IsNullOrEmpty(pair.Key))
                {
                    copy[pair.Key] = pair.Value ?? string.Empty;
                }
            }

            return new CommandArguments(copy, null);
        }

        /// <summary>
        /// Parses the legacy comma-separated <c>key=value</c> format. Keys and values
        /// are trimmed, segments without an <c>=</c> become keys with empty values, and
        /// duplicate keys keep the last value. The raw input is preserved in
        /// <see cref="LegacyText"/>. This format cannot represent commas in values;
        /// new profiles should use the JSON object form instead.
        /// </summary>
        /// <param name="arguments">The delimited argument string. Null and empty inputs produce <see cref="Empty"/>.</param>
        /// <returns>The parsed argument set.</returns>
        public static CommandArguments FromLegacyString(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                return Empty;
            }

            var parsed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part in arguments.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var equalsIndex = part.IndexOf('=');
                if (equalsIndex < 0)
                {
                    var bareKey = part.Trim();
                    if (bareKey.Length > 0)
                    {
                        parsed[bareKey] = string.Empty;
                    }

                    continue;
                }

                var key = part[..equalsIndex].Trim();
                if (key.Length > 0)
                {
                    parsed[key] = part[(equalsIndex + 1)..].Trim();
                }
            }

            return new CommandArguments(parsed, arguments);
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key) => this.values.ContainsKey(key);

        /// <inheritdoc/>
        public bool TryGetValue(string key, out string value) => this.values.TryGetValue(key, out value);

        /// <summary>
        /// Gets the value for a key, or a default when the key is absent.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">The value returned when the key is absent.</param>
        /// <returns>The parameter value, or <paramref name="defaultValue"/>.</returns>
        public string GetString(string key, string defaultValue = "")
        {
            return this.values.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value for a key parsed as an invariant-culture integer, or a
        /// default when the key is absent or the value does not parse.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">The value returned when the key is absent or invalid.</param>
        /// <returns>The parsed value, or <paramref name="defaultValue"/>.</returns>
        public int GetInt32(string key, int defaultValue)
        {
            return this.values.TryGetValue(key, out var value)
                && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : defaultValue;
        }

        /// <summary>
        /// Gets the value for a key parsed as a boolean (<c>true</c>/<c>false</c>,
        /// case-insensitive), or a default when the key is absent or the value does
        /// not parse.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">The value returned when the key is absent or invalid.</param>
        /// <returns>The parsed value, or <paramref name="defaultValue"/>.</returns>
        public bool GetBoolean(string key, bool defaultValue)
        {
            return this.values.TryGetValue(key, out var value)
                && bool.TryParse(value, out var parsed)
                ? parsed
                : defaultValue;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => this.values.GetEnumerator();

        /// <summary>
        /// Returns a human-readable rendering of the arguments for logs and console
        /// listings. Not a serialization format.
        /// </summary>
        /// <returns>The legacy text when present, otherwise the pairs joined as <c>key=value</c>.</returns>
        public override string ToString()
        {
            return this.LegacyText ?? string.Join(", ", this.values.Select(pair => $"{pair.Key}={pair.Value}"));
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

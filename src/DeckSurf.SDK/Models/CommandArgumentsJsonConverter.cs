// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Serializes <see cref="CommandArguments"/> to and from profile JSON. Reads
    /// both the canonical object form (<c>{ "scene": "Gameplay" }</c>) and the
    /// legacy comma-separated string form; scalar values (numbers, booleans, null)
    /// are coerced to their string representation so hand-written profiles can use
    /// natural JSON literals. Writes the object form, except for instances parsed
    /// from a legacy string, which are written back verbatim so untouched profiles
    /// round-trip without modification.
    /// </summary>
    public sealed class CommandArgumentsJsonConverter : JsonConverter<CommandArguments>
    {
        /// <inheritdoc/>
        public override CommandArguments Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return CommandArguments.Empty;

                case JsonTokenType.String:
                    return CommandArguments.FromLegacyString(reader.GetString());

                case JsonTokenType.StartObject:
                    var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    using (var document = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (var property in document.RootElement.EnumerateObject())
                        {
                            values[property.Name] = property.Value.ValueKind switch
                            {
                                JsonValueKind.String => property.Value.GetString(),
                                JsonValueKind.Number => property.Value.GetRawText(),
                                JsonValueKind.True => "true",
                                JsonValueKind.False => "false",
                                JsonValueKind.Null => string.Empty,
                                _ => throw new JsonException($"Command argument '{property.Name}' must be a string, number, boolean, or null. Nested arrays and objects are not supported."),
                            };
                        }
                    }

                    return CommandArguments.FromDictionary(values);

                default:
                    throw new JsonException("Command arguments must be a JSON object of key/value pairs, or a legacy delimited string.");
            }
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, CommandArguments value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);

            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            if (value.LegacyText != null)
            {
                writer.WriteStringValue(value.LegacyText);
                return;
            }

            writer.WriteStartObject();

            foreach (var pair in value)
            {
                writer.WriteString(pair.Key, pair.Value);
            }

            writer.WriteEndObject();
        }
    }
}

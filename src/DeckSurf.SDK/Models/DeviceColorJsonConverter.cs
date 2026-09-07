// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Serializes an optional <see cref="DeviceColor"/> as a <c>#RRGGBB</c> hex string,
    /// the form profile files use for the touch key backlight color.
    /// </summary>
    public class DeviceColorJsonConverter : JsonConverter<DeviceColor?>
    {
        /// <inheritdoc/>
        public override DeviceColor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("A device color must be a #RRGGBB hex string.");
            }

            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var hex = value.StartsWith('#') ? value[1..] : value;
            if (hex.Length != 6
                || !byte.TryParse(hex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r)
                || !byte.TryParse(hex[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g)
                || !byte.TryParse(hex[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
            {
                throw new JsonException($"'{value}' is not a valid #RRGGBB device color.");
            }

            return new DeviceColor(r, g, b);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DeviceColor? value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);

            if (value is not { } color)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue($"#{color.R:X2}{color.G:X2}{color.B:X2}");
        }
    }
}

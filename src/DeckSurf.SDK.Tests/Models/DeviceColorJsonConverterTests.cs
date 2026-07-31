// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using DeckSurf.SDK.Models;
using Xunit;

namespace DeckSurf.SDK.Tests.Models
{
    public class DeviceColorJsonConverterTests
    {
        [Fact]
        public void ButtonColor_RoundTripsAsHexString()
        {
            var mapping = new CommandMapping
            {
                Target = MappingTarget.TouchButton,
                ButtonIndex = 1,
                ButtonColor = new DeviceColor(255, 128, 0),
            };

            var json = JsonSerializer.Serialize(mapping);
            Assert.Contains("\"button_color\":\"#FF8000\"", json);

            var restored = JsonSerializer.Deserialize<CommandMapping>(json)!;
            Assert.Equal(new DeviceColor(255, 128, 0), restored.ButtonColor);
        }

        [Fact]
        public void ButtonColor_OmittedWhenNull()
        {
            var json = JsonSerializer.Serialize(new CommandMapping());
            Assert.DoesNotContain("button_color", json);
        }

        [Fact]
        public void ButtonColor_AbsentPropertyReadsAsNull()
        {
            var restored = JsonSerializer.Deserialize<CommandMapping>("{\"button_index\":0}")!;
            Assert.Null(restored.ButtonColor);
        }

        [Theory]
        [InlineData("\"#00FF7F\"", 0, 255, 127)]
        [InlineData("\"00ff7f\"", 0, 255, 127)]
        public void ButtonColor_ReadsHexWithAndWithoutHash(string colorJson, byte r, byte g, byte b)
        {
            var restored = JsonSerializer.Deserialize<CommandMapping>($"{{\"button_color\":{colorJson}}}")!;
            Assert.Equal(new DeviceColor(r, g, b), restored.ButtonColor);
        }

        [Theory]
        [InlineData("\"red\"")]
        [InlineData("\"#12345\"")]
        [InlineData("123456")]
        public void ButtonColor_RejectsInvalidValues(string colorJson)
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CommandMapping>($"{{\"button_color\":{colorJson}}}"));
        }

        [Fact]
        public void ButtonColor_NullAndEmptyStringsReadAsNull()
        {
            Assert.Null(JsonSerializer.Deserialize<CommandMapping>("{\"button_color\":null}")!.ButtonColor);
            Assert.Null(JsonSerializer.Deserialize<CommandMapping>("{\"button_color\":\"\"}")!.ButtonColor);
        }
    }
}

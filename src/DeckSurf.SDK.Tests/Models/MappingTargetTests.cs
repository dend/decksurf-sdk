// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class MappingTargetTests
    {
        [Fact]
        public void CommandMapping_DefaultsToKeyTarget()
        {
            var mapping = new CommandMapping();

            Assert.Equal(MappingTarget.Key, mapping.Target);
        }

        [Fact]
        public void CommandMapping_DeserializesLegacyJsonWithoutTarget_AsKey()
        {
            const string legacyJson = """
                {"plugin":"DeckSurf.Plugin.Barn","command":"SnakeGame","command_arguments":"","button_index":-1,"button_image_path":""}
                """;

            var mapping = JsonSerializer.Deserialize<CommandMapping>(legacyJson);

            Assert.NotNull(mapping);
            Assert.Equal(MappingTarget.Key, mapping.Target);
        }

        [Fact]
        public void CommandMapping_RoundTripsKnobTargetAsString()
        {
            var mapping = new CommandMapping
            {
                Plugin = "DeckSurf.Plugin.Barn",
                Command = "KnobBrightness",
                ButtonIndex = 2,
                Target = MappingTarget.Knob,
            };

            var json = JsonSerializer.Serialize(mapping);
            var restored = JsonSerializer.Deserialize<CommandMapping>(json);

            Assert.Contains("\"target\":\"Knob\"", json);
            Assert.NotNull(restored);
            Assert.Equal(MappingTarget.Knob, restored.Target);
            Assert.Equal(2, restored.ButtonIndex);
        }
    }
}

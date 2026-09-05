// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class CommandArgumentsJsonConverterTests
    {
        [Fact]
        public void Read_ObjectForm_ProducesEntries()
        {
            var arguments = JsonSerializer.Deserialize<CommandArguments>("""{ "scene": "Gaming, Part 2", "password": "p=w,d" }""");

            Assert.NotNull(arguments);
            Assert.Equal("Gaming, Part 2", arguments.GetString("scene"));
            Assert.Equal("p=w,d", arguments.GetString("password"));
            Assert.Null(arguments.LegacyText);
        }

        [Fact]
        public void Read_ObjectForm_CoercesScalarsToStrings()
        {
            var arguments = JsonSerializer.Deserialize<CommandArguments>("""{ "port": 4455, "secure": true, "off": false, "note": null }""");

            Assert.NotNull(arguments);
            Assert.Equal(4455, arguments.GetInt32("port", -1));
            Assert.True(arguments.GetBoolean("secure", false));
            Assert.False(arguments.GetBoolean("off", true));
            Assert.Equal(string.Empty, arguments.GetString("note", "unset"));
        }

        [Fact]
        public void Read_ObjectForm_RejectsNestedStructures()
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CommandArguments>("""{ "nested": { "a": 1 } }"""));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CommandArguments>("""{ "list": [1, 2] }"""));
        }

        [Fact]
        public void Read_LegacyStringForm_ParsesAndPreservesRawText()
        {
            var arguments = JsonSerializer.Deserialize<CommandArguments>("\"scene=Gaming,port=4455\"");

            Assert.NotNull(arguments);
            Assert.Equal("Gaming", arguments.GetString("scene"));
            Assert.Equal(4455, arguments.GetInt32("port", -1));
            Assert.Equal("scene=Gaming,port=4455", arguments.LegacyText);
        }

        [Fact]
        public void Read_NullToken_ProducesEmpty()
        {
            var mapping = JsonSerializer.Deserialize<CommandMapping>("""{ "command_arguments": null }""");

            Assert.NotNull(mapping);
            Assert.Same(CommandArguments.Empty, mapping.CommandArguments);
        }

        [Fact]
        public void Read_MissingProperty_ProducesEmpty()
        {
            var mapping = JsonSerializer.Deserialize<CommandMapping>("{}");

            Assert.NotNull(mapping);
            Assert.Same(CommandArguments.Empty, mapping.CommandArguments);
        }

        [Fact]
        public void Read_InvalidToken_Throws()
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CommandArguments>("42"));
        }

        [Fact]
        public void Write_DictionaryBackedInstance_WritesObjectForm()
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string> { ["scene"] = "Gaming, Part 2" });

            var json = JsonSerializer.Serialize(arguments);

            Assert.Equal("""{"scene":"Gaming, Part 2"}""", json);
        }

        [Fact]
        public void Write_LegacyBackedInstance_WritesOriginalStringVerbatim()
        {
            var arguments = CommandArguments.FromLegacyString("scene=Gaming,port=4455");

            var json = JsonSerializer.Serialize(arguments);

            Assert.Equal("\"scene=Gaming,port=4455\"", json);
        }

        [Fact]
        public void Roundtrip_ObjectForm_IsLossless()
        {
            var original = CommandArguments.FromDictionary(new Dictionary<string, string>
            {
                ["scene"] = "Intermission, please wait...",
                ["expr"] = "x=y==z",
                ["empty"] = string.Empty,
            });

            var roundtripped = JsonSerializer.Deserialize<CommandArguments>(JsonSerializer.Serialize(original));

            Assert.NotNull(roundtripped);
            Assert.Equal(original.OrderBy(p => p.Key), roundtripped.OrderBy(p => p.Key));
            Assert.Null(roundtripped.LegacyText);
        }

        [Fact]
        public void Roundtrip_LegacyForm_IsLossless()
        {
            var original = CommandArguments.FromLegacyString(@"C:\tools\app.exe");

            var roundtripped = JsonSerializer.Deserialize<CommandArguments>(JsonSerializer.Serialize(original));

            Assert.NotNull(roundtripped);
            Assert.Equal(@"C:\tools\app.exe", roundtripped.LegacyText);
        }
    }
}

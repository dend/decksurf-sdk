// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class CommandArgumentsTests
    {
        [Fact]
        public void Empty_HasNoEntriesAndNoLegacyText()
        {
            Assert.Empty(CommandArguments.Empty);
            Assert.Null(CommandArguments.Empty.LegacyText);
        }

        [Fact]
        public void FromDictionary_CopiesValuesCaseInsensitively()
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string>
            {
                ["Scene"] = "Gameplay",
            });

            Assert.True(arguments.TryGetValue("scene", out var value));
            Assert.Equal("Gameplay", value);
            Assert.Null(arguments.LegacyText);
        }

        [Fact]
        public void FromDictionary_PreservesCommasEqualsAndWhitespaceInValues()
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string>
            {
                ["scene"] = "Gaming, Part 2",
                ["formula"] = "a=b+c",
                ["padded"] = "  keep spaces  ",
            });

            Assert.Equal("Gaming, Part 2", arguments.GetString("scene"));
            Assert.Equal("a=b+c", arguments.GetString("formula"));
            Assert.Equal("  keep spaces  ", arguments.GetString("padded"));
        }

        [Fact]
        public void FromDictionary_NormalizesNullValuesToEmptyStrings()
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string> { ["flag"] = null! });

            Assert.Equal(string.Empty, arguments.GetString("flag", "unset"));
        }

        [Fact]
        public void FromDictionary_EmptyInputReturnsEmptyInstance()
        {
            Assert.Same(CommandArguments.Empty, CommandArguments.FromDictionary(new Dictionary<string, string>()));
        }

        [Fact]
        public void FromDictionary_NullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => CommandArguments.FromDictionary(null));
        }

        [Fact]
        public void FromDictionary_SkipsNullOrEmptyKeys()
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string>
            {
                [string.Empty] = "ignored",
                ["kept"] = "value",
            });

            Assert.Single(arguments);
            Assert.Equal("value", arguments.GetString("kept"));
        }

        [Fact]
        public void FromLegacyString_NullOrEmptyReturnsEmpty()
        {
            Assert.Same(CommandArguments.Empty, CommandArguments.FromLegacyString(null));
            Assert.Same(CommandArguments.Empty, CommandArguments.FromLegacyString(string.Empty));
        }

        [Fact]
        public void FromLegacyString_ParsesMultiplePairs()
        {
            var arguments = CommandArguments.FromLegacyString("device_id=light_01,brightness=80,color=red");

            Assert.Equal(3, arguments.Count);
            Assert.Equal("light_01", arguments.GetString("device_id"));
            Assert.Equal("80", arguments.GetString("brightness"));
            Assert.Equal("red", arguments.GetString("color"));
        }

        [Fact]
        public void FromLegacyString_TrimsKeysAndValues()
        {
            var arguments = CommandArguments.FromLegacyString("  key1  =  value1  ,  key2  =  value2  ");

            Assert.Equal("value1", arguments.GetString("key1"));
            Assert.Equal("value2", arguments.GetString("key2"));
        }

        [Fact]
        public void FromLegacyString_ValuelessSegmentBecomesKeyWithEmptyValue()
        {
            var arguments = CommandArguments.FromLegacyString("mode=fast,verbose");

            Assert.True(arguments.ContainsKey("verbose"));
            Assert.Equal(string.Empty, arguments.GetString("verbose", "unset"));
        }

        [Fact]
        public void FromLegacyString_DuplicateKeysKeepLastValue()
        {
            var arguments = CommandArguments.FromLegacyString("key=first,KEY=second");

            Assert.Single(arguments);
            Assert.Equal("second", arguments.GetString("key"));
        }

        [Fact]
        public void FromLegacyString_ValueMayContainEquals()
        {
            var arguments = CommandArguments.FromLegacyString("formula=a=b+c");

            Assert.Equal("a=b+c", arguments.GetString("formula"));
        }

        [Fact]
        public void FromLegacyString_PreservesRawInputAsLegacyText()
        {
            var arguments = CommandArguments.FromLegacyString(@"C:\tools\app.exe");

            Assert.Equal(@"C:\tools\app.exe", arguments.LegacyText);
        }

        [Fact]
        public void GetString_ReturnsDefaultWhenKeyIsAbsent()
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string> { ["host"] = "localhost" });

            Assert.Equal("fallback", arguments.GetString("missing", "fallback"));
        }

        [Theory]
        [InlineData("4455", 4455)]
        [InlineData("-12", -12)]
        [InlineData("not-a-number", 99)]
        public void GetInt32_ParsesInvariantOrReturnsDefault(string value, int expected)
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string> { ["port"] = value });

            Assert.Equal(expected, arguments.GetInt32("port", 99));
        }

        [Fact]
        public void GetInt32_ReturnsDefaultWhenKeyIsAbsent()
        {
            Assert.Equal(42, CommandArguments.Empty.GetInt32("missing", 42));
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("False", false)]
        [InlineData("yes", true)]
        public void GetBoolean_ParsesOrReturnsDefault(string value, bool expected)
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string> { ["enabled"] = value });

            Assert.Equal(expected, arguments.GetBoolean("enabled", true));
        }

        [Fact]
        public void ToString_UsesLegacyTextWhenPresent()
        {
            Assert.Equal("a=1,b=2", CommandArguments.FromLegacyString("a=1,b=2").ToString());
        }

        [Fact]
        public void ToString_JoinsPairsForDictionaryBackedInstances()
        {
            var arguments = CommandArguments.FromDictionary(new Dictionary<string, string> { ["scene"] = "Gaming" });

            Assert.Equal("scene=Gaming", arguments.ToString());
        }

        [Fact]
        public void CommandMapping_ArgumentsDefaultToEmptyAndNullAssignmentResets()
        {
            var mapping = new CommandMapping();

            Assert.Same(CommandArguments.Empty, mapping.CommandArguments);

            mapping.CommandArguments = CommandArguments.FromLegacyString("a=1");
            mapping.CommandArguments = null;

            Assert.Same(CommandArguments.Empty, mapping.CommandArguments);
        }
    }
}

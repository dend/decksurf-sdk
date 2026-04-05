// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public class CommandArgumentParserTests
    {
        // ---------------------------------------------------------------
        // Parse() tests
        // ---------------------------------------------------------------

        [Fact]
        public void Parse_NullInput_ReturnsEmptyDictionary()
        {
            var result = CommandArgumentParser.Parse(null);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_EmptyString_ReturnsEmptyDictionary()
        {
            var result = CommandArgumentParser.Parse(string.Empty);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_WhitespaceOnly_ReturnsEmptyDictionary()
        {
            var result = CommandArgumentParser.Parse("   \t  ");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SingleKeyValue_ReturnsDictionaryWithOneEntry()
        {
            var result = CommandArgumentParser.Parse("device_id=light_01");

            Assert.Single(result);
            Assert.Equal("light_01", result["device_id"]);
        }

        [Fact]
        public void Parse_MultipleKeyValues_ReturnsCorrectCountAndValues()
        {
            var result = CommandArgumentParser.Parse("device_id=light_01,brightness=80,color=red");

            Assert.Equal(3, result.Count);
            Assert.Equal("light_01", result["device_id"]);
            Assert.Equal("80", result["brightness"]);
            Assert.Equal("red", result["color"]);
        }

        [Fact]
        public void Parse_KeyWithoutEquals_StoresKeyWithEmptyValue()
        {
            var result = CommandArgumentParser.Parse("verbose");

            Assert.Single(result);
            Assert.Equal(string.Empty, result["verbose"]);
        }

        [Fact]
        public void Parse_MixedKeysWithAndWithoutEquals_ParsesBothCorrectly()
        {
            var result = CommandArgumentParser.Parse("mode=fast,verbose,count=5");

            Assert.Equal(3, result.Count);
            Assert.Equal("fast", result["mode"]);
            Assert.Equal(string.Empty, result["verbose"]);
            Assert.Equal("5", result["count"]);
        }

        [Fact]
        public void Parse_WhitespaceAroundKeysAndValues_TrimsBoth()
        {
            var result = CommandArgumentParser.Parse("  key1  =  value1  ,  key2  =  value2  ");

            Assert.Equal(2, result.Count);
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
        }

        [Fact]
        public void Parse_WhitespaceAroundKeyWithoutEquals_TrimmedKey()
        {
            var result = CommandArgumentParser.Parse("  verbose  ");

            Assert.Single(result);
            Assert.Equal(string.Empty, result["verbose"]);
        }

        [Fact]
        public void Parse_DuplicateKeys_LastValueWins()
        {
            var result = CommandArgumentParser.Parse("key=first,key=second");

            Assert.Single(result);
            Assert.Equal("second", result["key"]);
        }

        [Fact]
        public void Parse_DuplicateKeysDifferentCase_LastValueWins()
        {
            // The dictionary uses OrdinalIgnoreCase, so KEY and key are duplicates.
            var result = CommandArgumentParser.Parse("key=first,KEY=second");

            Assert.Single(result);
            Assert.Equal("second", result["key"]);
        }

        [Fact]
        public void Parse_ValueContainingEqualsSign_OnlyFirstEqualsSplits()
        {
            var result = CommandArgumentParser.Parse("formula=a=b+c");

            Assert.Single(result);
            Assert.Equal("a=b+c", result["formula"]);
        }

        [Fact]
        public void Parse_ValueContainingMultipleEqualsSign_PreservesAll()
        {
            var result = CommandArgumentParser.Parse("expr=x==y");

            Assert.Single(result);
            Assert.Equal("x==y", result["expr"]);
        }

        [Fact]
        public void Parse_CaseInsensitiveKeyLookup()
        {
            var result = CommandArgumentParser.Parse("MyKey=hello");

            Assert.Equal("hello", result["mykey"]);
            Assert.Equal("hello", result["MYKEY"]);
            Assert.Equal("hello", result["MyKey"]);
        }

        [Fact]
        public void Parse_EmptySegments_AreIgnored()
        {
            // Consecutive commas produce empty segments that are removed via RemoveEmptyEntries.
            var result = CommandArgumentParser.Parse("a=1,,b=2,,,c=3");

            Assert.Equal(3, result.Count);
            Assert.Equal("1", result["a"]);
            Assert.Equal("2", result["b"]);
            Assert.Equal("3", result["c"]);
        }

        [Fact]
        public void Parse_EqualsWithNoKey_IgnoresEntry()
        {
            // "=value" has an empty key after trim, so it should be skipped.
            var result = CommandArgumentParser.Parse("=value,key=good");

            Assert.Single(result);
            Assert.Equal("good", result["key"]);
        }

        [Fact]
        public void Parse_EqualsWithNoValue_StoresEmptyValue()
        {
            var result = CommandArgumentParser.Parse("key=");

            Assert.Single(result);
            Assert.Equal(string.Empty, result["key"]);
        }

        [Fact]
        public void Parse_ReturnsReadOnlyDictionary()
        {
            var result = CommandArgumentParser.Parse("a=1");

            Assert.IsAssignableFrom<IReadOnlyDictionary<string, string>>(result);
        }

        // ---------------------------------------------------------------
        // TryGetValue() tests
        // ---------------------------------------------------------------

        [Fact]
        public void TryGetValue_ExistingKey_ReturnsTrueAndCorrectValue()
        {
            var found = CommandArgumentParser.TryGetValue("name=Alice,age=30", "name", out var value);

            Assert.True(found);
            Assert.Equal("Alice", value);
        }

        [Fact]
        public void TryGetValue_MissingKey_ReturnsFalse()
        {
            var found = CommandArgumentParser.TryGetValue("name=Alice", "missing", out var value);

            Assert.False(found);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetValue_NullArguments_ReturnsFalse()
        {
            var found = CommandArgumentParser.TryGetValue(null, "key", out var value);

            Assert.False(found);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetValue_EmptyArguments_ReturnsFalse()
        {
            var found = CommandArgumentParser.TryGetValue(string.Empty, "key", out var value);

            Assert.False(found);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetValue_CaseInsensitiveKeyLookup()
        {
            var found = CommandArgumentParser.TryGetValue("Color=blue", "color", out var value);

            Assert.True(found);
            Assert.Equal("blue", value);
        }

        [Fact]
        public void TryGetValue_CaseInsensitiveKeyLookup_UpperCase()
        {
            var found = CommandArgumentParser.TryGetValue("color=blue", "COLOR", out var value);

            Assert.True(found);
            Assert.Equal("blue", value);
        }

        // ---------------------------------------------------------------
        // GetValueOrDefault() tests
        // ---------------------------------------------------------------

        [Fact]
        public void GetValueOrDefault_ExistingKey_ReturnsValue()
        {
            var value = CommandArgumentParser.GetValueOrDefault("host=localhost,port=8080", "port");

            Assert.Equal("8080", value);
        }

        [Fact]
        public void GetValueOrDefault_MissingKey_ReturnsEmptyStringDefault()
        {
            var value = CommandArgumentParser.GetValueOrDefault("host=localhost", "missing");

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetValueOrDefault_MissingKey_ReturnsCustomDefault()
        {
            var value = CommandArgumentParser.GetValueOrDefault("host=localhost", "port", "3000");

            Assert.Equal("3000", value);
        }

        [Fact]
        public void GetValueOrDefault_NullArguments_ReturnsEmptyStringDefault()
        {
            var value = CommandArgumentParser.GetValueOrDefault(null, "key");

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetValueOrDefault_NullArguments_ReturnsCustomDefault()
        {
            var value = CommandArgumentParser.GetValueOrDefault(null, "key", "fallback");

            Assert.Equal("fallback", value);
        }

        [Fact]
        public void GetValueOrDefault_EmptyArguments_ReturnsCustomDefault()
        {
            var value = CommandArgumentParser.GetValueOrDefault(string.Empty, "key", "default_val");

            Assert.Equal("default_val", value);
        }

        [Fact]
        public void GetValueOrDefault_ExistingKeyWithEmptyValue_ReturnsEmptyString()
        {
            // "key=" has an explicit empty value, which is different from the key being absent.
            var value = CommandArgumentParser.GetValueOrDefault("key=", "key", "fallback");

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetValueOrDefault_CaseInsensitiveLookup()
        {
            var value = CommandArgumentParser.GetValueOrDefault("Theme=dark", "theme", "light");

            Assert.Equal("dark", value);
        }
    }
}

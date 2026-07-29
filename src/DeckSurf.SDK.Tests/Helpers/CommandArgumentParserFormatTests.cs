// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public class CommandArgumentParserFormatTests
    {
        [Fact]
        public void Format_ProducesParsableString_RoundTrip()
        {
            var values = new Dictionary<string, string>
            {
                ["mode"] = "timer",
                ["duration"] = "300",
            };

            var formatted = CommandArgumentParser.Format(values);
            var parsed = CommandArgumentParser.Parse(formatted);

            Assert.Equal("mode=timer,duration=300", formatted);
            Assert.Equal(2, parsed.Count);
            Assert.Equal("timer", parsed["mode"]);
            Assert.Equal("300", parsed["duration"]);
        }

        [Fact]
        public void Format_ReturnsEmptyString_ForEmptyDictionary()
        {
            var formatted = CommandArgumentParser.Format(new Dictionary<string, string>());

            Assert.Equal(string.Empty, formatted);
        }

        [Fact]
        public void Format_SerializesNullValueAsEmpty()
        {
            var formatted = CommandArgumentParser.Format(new Dictionary<string, string> { ["key"] = null! });

            Assert.Equal("key=", formatted);
        }

        [Fact]
        public void Format_SkipsEmptyKeys()
        {
            var formatted = CommandArgumentParser.Format(new Dictionary<string, string>
            {
                [" "] = "value",
                ["real"] = "1",
            });

            Assert.Equal("real=1", formatted);
        }

        [Fact]
        public void Format_ThrowsArgumentException_ForUnrepresentableCharacters()
        {
            Assert.Throws<ArgumentException>(() => CommandArgumentParser.Format(new Dictionary<string, string> { ["a,b"] = "1" }));
            Assert.Throws<ArgumentException>(() => CommandArgumentParser.Format(new Dictionary<string, string> { ["a=b"] = "1" }));
            Assert.Throws<ArgumentException>(() => CommandArgumentParser.Format(new Dictionary<string, string> { ["a"] = "1,2" }));
        }

        [Fact]
        public void Format_ThrowsArgumentNullException_ForNullDictionary()
        {
            Assert.Throws<ArgumentNullException>(() => CommandArgumentParser.Format(null!));
        }

        [Fact]
        public void Format_AllowsEqualsSignInValue_AndParsesBack()
        {
            // '=' is representable in values because Parse splits on the FIRST '='.
            var formatted = CommandArgumentParser.Format(new Dictionary<string, string> { ["expr"] = "x=1" });
            var parsed = CommandArgumentParser.Parse(formatted);

            Assert.Equal("x=1", parsed["expr"]);
        }
    }
}

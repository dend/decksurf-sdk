// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public class CommandParameterValidatorTests
    {
        [Fact]
        public void Validate_ReturnsEmpty_ForValidValues()
        {
            var schema = new[]
            {
                new CommandParameterAttribute("mode", CommandParameterType.Choice) { Choices = ["clock", "timer"], Required = true },
                new CommandParameterAttribute("duration", CommandParameterType.Integer) { MinValue = 1, MaxValue = 3600 },
                new CommandParameterAttribute("verbose", CommandParameterType.Boolean),
            };

            var values = new Dictionary<string, string>
            {
                ["mode"] = "timer",
                ["duration"] = "300",
                ["verbose"] = "true",
            };

            var errors = CommandParameterValidator.Validate(schema, values);

            Assert.Empty(errors);
        }

        [Fact]
        public void Validate_ReportsMissingRequiredParameter()
        {
            var schema = new[]
            {
                new CommandParameterAttribute("path", CommandParameterType.FilePath) { Required = true, DisplayName = "Application path" },
            };

            var errors = CommandParameterValidator.Validate(schema, new Dictionary<string, string>());

            var error = Assert.Single(errors);
            Assert.Contains("Application path", error);
        }

        [Fact]
        public void Validate_TreatsSecretAsFreeFormText()
        {
            var schema = new[]
            {
                new CommandParameterAttribute("password", CommandParameterType.Secret) { Required = true, DisplayName = "Password" },
            };

            var missing = CommandParameterValidator.Validate(schema, new Dictionary<string, string>());
            Assert.Contains("Password", Assert.Single(missing));

            var present = CommandParameterValidator.Validate(schema, new Dictionary<string, string> { ["password"] = "s3cr=t,value" });
            Assert.Empty(present);
        }

        [Fact]
        public void Validate_AllowsMissingOptionalParameter()
        {
            var schema = new[]
            {
                new CommandParameterAttribute("duration", CommandParameterType.Integer),
            };

            var errors = CommandParameterValidator.Validate(schema, new Dictionary<string, string>());

            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("1.5")]
        public void Validate_ReportsNonNumericIntegerValue(string value)
        {
            var schema = new[] { new CommandParameterAttribute("count", CommandParameterType.Integer) };
            var values = new Dictionary<string, string> { ["count"] = value };

            var errors = CommandParameterValidator.Validate(schema, values);

            Assert.Single(errors);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("999")]
        public void Validate_ReportsOutOfRangeNumericValue(string value)
        {
            var schema = new[]
            {
                new CommandParameterAttribute("duration", CommandParameterType.Integer) { MinValue = 1, MaxValue = 600 },
            };
            var values = new Dictionary<string, string> { ["duration"] = value };

            var errors = CommandParameterValidator.Validate(schema, values);

            Assert.Single(errors);
        }

        [Fact]
        public void Validate_ReportsInvalidBoolean()
        {
            var schema = new[] { new CommandParameterAttribute("verbose", CommandParameterType.Boolean) };
            var values = new Dictionary<string, string> { ["verbose"] = "yes" };

            var errors = CommandParameterValidator.Validate(schema, values);

            Assert.Single(errors);
        }

        [Fact]
        public void Validate_ReportsValueOutsideChoices_CaseInsensitiveMatch()
        {
            var schema = new[]
            {
                new CommandParameterAttribute("mode", CommandParameterType.Choice) { Choices = ["clock", "timer"] },
            };

            var invalid = CommandParameterValidator.Validate(schema, new Dictionary<string, string> { ["mode"] = "snake" });
            var validUppercase = CommandParameterValidator.Validate(schema, new Dictionary<string, string> { ["mode"] = "TIMER" });

            Assert.Single(invalid);
            Assert.Empty(validUppercase);
        }

        [Fact]
        public void Validate_ReportsChoiceParameterWithoutDeclaredChoices()
        {
            var schema = new[] { new CommandParameterAttribute("mode", CommandParameterType.Choice) };
            var values = new Dictionary<string, string> { ["mode"] = "anything" };

            var errors = CommandParameterValidator.Validate(schema, values);

            Assert.Single(errors);
        }

        [Fact]
        public void Validate_ThrowsArgumentNullException_ForNullArguments()
        {
            var schema = Array.Empty<CommandParameterAttribute>();

            Assert.Throws<ArgumentNullException>(() => CommandParameterValidator.Validate(null!, new Dictionary<string, string>()));
            Assert.Throws<ArgumentNullException>(() => CommandParameterValidator.Validate(schema, null!));
        }
    }
}

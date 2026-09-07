// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Interfaces;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public class CommandSchemaReaderTests
    {
        [Fact]
        public void GetParameters_ReturnsEmptyList_ForUnannotatedCommand()
        {
            var parameters = CommandSchemaReader.GetParameters(typeof(UnannotatedCommand));

            Assert.Empty(parameters);
        }

        [Fact]
        public void GetParameters_ReturnsDeclaredParameters_SortedByOrderThenKey()
        {
            var parameters = CommandSchemaReader.GetParameters(typeof(AnnotatedCommand));

            Assert.Equal(3, parameters.Count);
            Assert.Equal("mode", parameters[0].Key);
            Assert.Equal("duration", parameters[1].Key);
            Assert.Equal("zebra", parameters[2].Key);
        }

        [Fact]
        public void GetParameters_ReadsAttributeProperties()
        {
            var parameters = CommandSchemaReader.GetParameters(typeof(AnnotatedCommand));

            var mode = parameters[0];
            Assert.Equal(CommandParameterType.Choice, mode.ParameterType);
            Assert.Equal("Mode", mode.DisplayName);
            Assert.True(mode.Required);
            Assert.Equal("clock", mode.DefaultValue);
            Assert.Equal(["clock", "stopwatch", "timer"], mode.Choices);

            var duration = parameters[1];
            Assert.Equal(CommandParameterType.Integer, duration.ParameterType);
            Assert.Equal(1, duration.MinValue);
            Assert.Equal(86400, duration.MaxValue);
        }

        [Fact]
        public void GetParameters_DerivedClassDeclarationWins_ForDuplicateKeys()
        {
            var parameters = CommandSchemaReader.GetParameters(typeof(DerivedCommand));

            var target = Assert.Single(parameters, p => p.Key == "target");
            Assert.Equal(CommandParameterType.FilePath, target.ParameterType);
            Assert.Equal("Derived target", target.DisplayName);

            // The base-only parameter is still inherited.
            Assert.Single(parameters, p => p.Key == "baseonly");
        }

        [Fact]
        public void GetParameters_FromInstance_MatchesTypeResult()
        {
            using var command = new AnnotatedCommand();

            var fromInstance = CommandSchemaReader.GetParameters(command);
            var fromType = CommandSchemaReader.GetParameters(typeof(AnnotatedCommand));

            Assert.Equal(fromType.Count, fromInstance.Count);
        }

        [Fact]
        public void GetParameters_ThrowsArgumentNullException_ForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => CommandSchemaReader.GetParameters((Type)null!));
            Assert.Throws<ArgumentNullException>(() => CommandSchemaReader.GetParameters((IDeckSurfCommand)null!));
        }

        [Fact]
        public void CommandParameterAttribute_ThrowsArgumentException_ForEmptyKey()
        {
            Assert.Throws<ArgumentException>(() => new CommandParameterAttribute(string.Empty, CommandParameterType.String));
            Assert.Throws<ArgumentException>(() => new CommandParameterAttribute("   ", CommandParameterType.String));
        }

        [Fact]
        public void GetParameters_DynamicChoicesFlag_DefaultsToFalseAndReadsWhenSet()
        {
            var annotated = CommandSchemaReader.GetParameters(typeof(AnnotatedCommand));
            Assert.All(annotated, p => Assert.False(p.DynamicChoices));

            var dynamic = CommandSchemaReader.GetParameters(typeof(DynamicChoiceCommand));
            Assert.True(Assert.Single(dynamic, p => p.Key == "scene").DynamicChoices);
        }

        [Fact]
        public async Task ChoiceProvider_ReceivesKeyAndCurrentValues()
        {
            using var command = new DynamicChoiceCommand();

            var choices = await ((IDeckSurfChoiceProvider)command).GetChoicesAsync(
                "scene",
                CommandArguments.FromDictionary(new Dictionary<string, string> { ["host"] = "somewhere" }));

            Assert.Equal(["scene-for-somewhere"], choices);
        }

        private class UnannotatedCommand : TestCommandBase
        {
        }

        [CommandParameter("mode", CommandParameterType.Choice, DisplayName = "Mode", Required = true, DefaultValue = "clock", Choices = ["clock", "stopwatch", "timer"], Order = 0)]
        [CommandParameter("duration", CommandParameterType.Integer, MinValue = 1, MaxValue = 86400, Order = 1)]
        [CommandParameter("zebra", CommandParameterType.String, Order = 1)]
        private class AnnotatedCommand : TestCommandBase
        {
        }

        [CommandParameter("target", CommandParameterType.String, DisplayName = "Base target")]
        [CommandParameter("baseonly", CommandParameterType.Boolean)]
        private class BaseCommand : TestCommandBase
        {
        }

        [CommandParameter("target", CommandParameterType.FilePath, DisplayName = "Derived target")]
        private class DerivedCommand : BaseCommand
        {
        }

        [CommandParameter("scene", CommandParameterType.String, DynamicChoices = true)]
        [CommandParameter("host", CommandParameterType.String)]
        private class DynamicChoiceCommand : TestCommandBase, IDeckSurfChoiceProvider
        {
            public Task<IReadOnlyList<string>> GetChoicesAsync(string parameterKey, CommandArguments currentValues, CancellationToken cancellationToken = default)
            {
                IReadOnlyList<string> choices = [$"{parameterKey}-for-{currentValues.GetString("host")}"];
                return Task.FromResult(choices);
            }
        }

        private abstract class TestCommandBase : IDeckSurfCommand
        {
            public string Name => "Test Command";

            public string Description => "Test command for schema reading.";

            public void ExecuteOnActivation(CommandMapping mappedCommand, IConnectedDevice mappedDevice)
            {
            }

            public void ExecuteOnAction(CommandMapping mappedCommand, IConnectedDevice mappedDevice, int activatingButton = -1)
            {
            }

            public void Dispose()
            {
            }
        }
    }
}

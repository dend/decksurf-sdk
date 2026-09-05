// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Interfaces;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class CommandStatusTests
    {
        [Fact]
        public void Ready_SetsKindAndMessage()
        {
            var status = CommandStatus.Ready("Connected.");

            Assert.Equal(CommandStatusKind.Ready, status.Kind);
            Assert.Equal("Connected.", status.Message);
        }

        [Fact]
        public void Unavailable_SetsKindAndMessage()
        {
            var status = CommandStatus.Unavailable("Host unreachable.");

            Assert.Equal(CommandStatusKind.Unavailable, status.Kind);
            Assert.Equal("Host unreachable.", status.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Factories_RejectEmptyMessages(string? message)
        {
            Assert.Throws<ArgumentException>(() => CommandStatus.Ready(message!));
            Assert.Throws<ArgumentException>(() => CommandStatus.Unavailable(message!));
        }

        [Fact]
        public async Task StatusProvider_ReceivesCurrentValues()
        {
            var provider = new EchoStatusProvider();

            var status = await provider.GetStatusAsync(
                CommandArguments.FromDictionary(new Dictionary<string, string> { ["host"] = "studio-pc" }));

            Assert.Equal(CommandStatusKind.Ready, status.Kind);
            Assert.Equal("probed studio-pc", status.Message);
        }

        private sealed class EchoStatusProvider : IDeckSurfStatusProvider
        {
            public Task<CommandStatus> GetStatusAsync(CommandArguments currentValues, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(CommandStatus.Ready($"probed {currentValues.GetString("host")}"));
            }
        }
    }
}

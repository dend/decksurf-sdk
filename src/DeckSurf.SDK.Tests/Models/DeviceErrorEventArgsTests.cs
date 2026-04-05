// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class DeviceErrorEventArgsTests
    {
        [Fact]
        public void Constructor_SetsAllProperties()
        {
            var exception = new InvalidOperationException("test");
            var args = new DeviceErrorEventArgs(
                exception,
                DeviceErrorCategory.CommunicationFailure,
                true,
                "Reconnect the device",
                "SetKey");

            Assert.Same(exception, args.Exception);
            Assert.Equal(DeviceErrorCategory.CommunicationFailure, args.Category);
            Assert.True(args.IsTransient);
            Assert.Equal("Reconnect the device", args.RecoveryHint);
            Assert.Equal("SetKey", args.OperationName);
        }

        [Fact]
        public void InheritsFromEventArgs()
        {
            var args = new DeviceErrorEventArgs(null, DeviceErrorCategory.Unknown, false, null, null);

            Assert.IsAssignableFrom<EventArgs>(args);
        }

        [Fact]
        public void Exception_CanBeNull()
        {
            var args = new DeviceErrorEventArgs(null, DeviceErrorCategory.Unknown, false, "hint", "op");

            Assert.Null(args.Exception);
        }

        [Fact]
        public void RecoveryHint_CanBeNull()
        {
            var args = new DeviceErrorEventArgs(new Exception(), DeviceErrorCategory.Unknown, false, null, "op");

            Assert.Null(args.RecoveryHint);
        }

        [Fact]
        public void OperationName_CanBeNull()
        {
            var args = new DeviceErrorEventArgs(new Exception(), DeviceErrorCategory.Unknown, false, "hint", null);

            Assert.Null(args.OperationName);
        }

        [Fact]
        public void IsTransient_True()
        {
            var args = new DeviceErrorEventArgs(null, DeviceErrorCategory.Unknown, true, null, null);

            Assert.True(args.IsTransient);
        }

        [Fact]
        public void IsTransient_False()
        {
            var args = new DeviceErrorEventArgs(null, DeviceErrorCategory.Unknown, false, null, null);

            Assert.False(args.IsTransient);
        }

        [Theory]
        [InlineData(DeviceErrorCategory.Unknown)]
        [InlineData(DeviceErrorCategory.Disconnected)]
        [InlineData(DeviceErrorCategory.CommunicationFailure)]
        [InlineData(DeviceErrorCategory.ImageProcessing)]
        [InlineData(DeviceErrorCategory.InvalidParameter)]
        [InlineData(DeviceErrorCategory.DeviceDisposed)]
        public void Category_AcceptsAllEnumValues(DeviceErrorCategory category)
        {
            var args = new DeviceErrorEventArgs(null, category, false, null, null);

            Assert.Equal(category, args.Category);
        }
    }
}

// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Core
{
    public class DeviceManagerTests
    {
        private const int ValidVid = 0x0FD9;
        private const int InvalidVid = 0x1234;

        [Fact]
        public void IsSupported_ValidVidAndValidPid_ReturnsTrue()
        {
            bool result = DeviceManager.IsSupported(ValidVid, (int)DeviceModel.MK2);

            Assert.True(result);
        }

        [Theory]
        [InlineData((int)DeviceModel.Original)]
        [InlineData((int)DeviceModel.Original2019)]
        [InlineData((int)DeviceModel.MK2)]
        [InlineData((int)DeviceModel.Mini)]
        [InlineData((int)DeviceModel.Mini2022)]
        [InlineData((int)DeviceModel.XL)]
        [InlineData((int)DeviceModel.XL2022)]
        [InlineData((int)DeviceModel.Plus)]
        [InlineData((int)DeviceModel.Neo)]
        public void IsSupported_ValidVidWithEachValidPid_ReturnsTrue(int pid)
        {
            bool result = DeviceManager.IsSupported(ValidVid, pid);

            Assert.True(result);
        }

        [Fact]
        public void IsSupported_ValidVidWithInvalidPid_ReturnsFalse()
        {
            bool result = DeviceManager.IsSupported(ValidVid, 0xFFFF);

            Assert.False(result);
        }

        [Fact]
        public void IsSupported_InvalidVidWithValidPid_ReturnsFalse()
        {
            bool result = DeviceManager.IsSupported(InvalidVid, (int)DeviceModel.MK2);

            Assert.False(result);
        }

        [Fact]
        public void IsSupported_InvalidVidWithInvalidPid_ReturnsFalse()
        {
            bool result = DeviceManager.IsSupported(InvalidVid, 0xFFFF);

            Assert.False(result);
        }
    }
}

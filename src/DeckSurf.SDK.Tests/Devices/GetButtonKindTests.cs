// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Devices
{
    public class GetButtonKindTests
    {
        [Fact]
        public void GetButtonKind_ButtonIdentifier_ReturnsButton()
        {
            byte[] identifier = [0x01, 0x00];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Button, result);
        }

        [Fact]
        public void GetButtonKind_ScreenIdentifier_ReturnsScreen()
        {
            byte[] identifier = [0x01, 0x02];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Screen, result);
        }

        [Fact]
        public void GetButtonKind_KnobIdentifier_ReturnsKnob()
        {
            byte[] identifier = [0x01, 0x03];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Knob, result);
        }

        [Fact]
        public void GetButtonKind_ZeroBytes_ReturnsUnknown()
        {
            byte[] identifier = [0x00, 0x00];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKind_MaxBytes_ReturnsUnknown()
        {
            byte[] identifier = [0xFF, 0xFF];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKind_SingleByte_ReturnsUnknown()
        {
            byte[] identifier = [0x01];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKind_EmptyArray_ReturnsUnknown()
        {
            byte[] identifier = [];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKind_ThreeBytes_ReturnsUnknown()
        {
            byte[] identifier = [0x01, 0x00, 0x00];

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }
    }
}

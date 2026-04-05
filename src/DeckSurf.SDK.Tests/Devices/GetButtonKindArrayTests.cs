// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Devices
{
    public class GetButtonKindArrayTests
    {
        [Fact]
        public void GetButtonKindByteArray_ButtonIdentifier_ReturnsButton()
        {
            byte[] identifier = new byte[] { 0x01, 0x00 };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Button, result);
        }

        [Fact]
        public void GetButtonKindByteArray_ScreenIdentifier_ReturnsScreen()
        {
            byte[] identifier = new byte[] { 0x01, 0x02 };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Screen, result);
        }

        [Fact]
        public void GetButtonKindByteArray_KnobIdentifier_ReturnsKnob()
        {
            byte[] identifier = new byte[] { 0x01, 0x03 };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Knob, result);
        }

        [Fact]
        public void GetButtonKindByteArray_UnknownBytes_ReturnsUnknown()
        {
            byte[] identifier = new byte[] { 0x02, 0x05 };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKindByteArray_SingleByte_ReturnsUnknown()
        {
            byte[] identifier = new byte[] { 0x01 };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKindByteArray_ThreeBytes_ReturnsUnknown()
        {
            byte[] identifier = new byte[] { 0x01, 0x00, 0xFF };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKindByteArray_EmptyArray_ReturnsUnknown()
        {
            byte[] identifier = new byte[] { };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKindByteArray_CorrectFirstByteWrongSecond_ReturnsUnknown()
        {
            byte[] identifier = new byte[] { 0x01, 0x01 };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }

        [Fact]
        public void GetButtonKindByteArray_WrongFirstByteCorrectSecond_ReturnsUnknown()
        {
            byte[] identifier = new byte[] { 0x00, 0x00 };

            var result = ConnectedDevice.GetButtonKind(identifier);

            Assert.Equal(ButtonKind.Unknown, result);
        }
    }
}

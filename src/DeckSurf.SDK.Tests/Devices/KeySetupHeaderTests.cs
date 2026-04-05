// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models.Devices;

namespace DeckSurf.SDK.Tests.Devices
{
    public class KeySetupHeaderTests
    {
        [Fact]
        public void JpegDevice_GetKeySetupHeader_FinalSlice_ReturnsCorrectHeader()
        {
            var device = new StreamDeckMK2(0, 0, "", "", "");

            // keyId=0, sliceLength=100, iteration=0, remainingBytes=100
            // finalizer=1 because sliceLength == remainingBytes
            // lenLo=100 (0x64), lenHi=0, iterLo=0, iterHi=0
            byte[] header = device.GetKeySetupHeader(0, 100, 0, 100);

            Assert.Equal(8, header.Length);
            Assert.Equal(0x02, header[0]);
            Assert.Equal(0x07, header[1]);
            Assert.Equal(0x00, header[2]); // keyId
            Assert.Equal(0x01, header[3]); // finalizer (last slice)
            Assert.Equal(0x64, header[4]); // sliceLength low byte (100)
            Assert.Equal(0x00, header[5]); // sliceLength high byte
            Assert.Equal(0x00, header[6]); // iteration low byte
            Assert.Equal(0x00, header[7]); // iteration high byte
        }

        [Fact]
        public void JpegDevice_GetKeySetupHeader_NonFinalSlice_ReturnsCorrectHeader()
        {
            var device = new StreamDeckMK2(0, 0, "", "", "");

            // keyId=3, sliceLength=100, iteration=0, remainingBytes=200
            // finalizer=0 because sliceLength != remainingBytes
            byte[] header = device.GetKeySetupHeader(3, 100, 0, 200);

            Assert.Equal(8, header.Length);
            Assert.Equal(0x02, header[0]);
            Assert.Equal(0x07, header[1]);
            Assert.Equal(0x03, header[2]); // keyId
            Assert.Equal(0x00, header[3]); // finalizer (not last slice)
            Assert.Equal(0x64, header[4]); // sliceLength low byte (100)
            Assert.Equal(0x00, header[5]); // sliceLength high byte
            Assert.Equal(0x00, header[6]); // iteration low byte
            Assert.Equal(0x00, header[7]); // iteration high byte
        }

        [Fact]
        public void BmpDevice_GetKeySetupHeader_FinalSlice_ReturnsCorrectHeader()
        {
            var device = new StreamDeckMini(0, 0, "", "", "");

            // keyId=0, sliceLength=100, iteration=0, remainingBytes=100
            // finalizer=1 because sliceLength == remainingBytes
            byte[] header = device.GetKeySetupHeader(0, 100, 0, 100);

            Assert.Equal(16, header.Length);
            Assert.Equal(0x02, header[0]);
            Assert.Equal(0x01, header[1]);
            Assert.Equal(0x00, header[2]); // iteration low byte
            Assert.Equal(0x00, header[3]); // iteration high byte
            Assert.Equal(0x01, header[4]); // finalizer (last slice)
            Assert.Equal(0x00, header[5]); // keyId
            Assert.Equal(0x00, header[6]); // padding
            Assert.Equal(0x00, header[7]); // padding

            // Remaining bytes should all be zero
            for (int i = 8; i < 16; i++)
            {
                Assert.Equal(0x00, header[i]);
            }
        }

        [Fact]
        public void BmpDevice_GetKeySetupHeader_NonFinalSlice_ReturnsCorrectHeader()
        {
            var device = new StreamDeckMini(0, 0, "", "", "");

            // keyId=3, sliceLength=100, iteration=0, remainingBytes=200
            // finalizer=0 because sliceLength != remainingBytes
            byte[] header = device.GetKeySetupHeader(3, 100, 0, 200);

            Assert.Equal(16, header.Length);
            Assert.Equal(0x02, header[0]);
            Assert.Equal(0x01, header[1]);
            Assert.Equal(0x00, header[2]); // iteration low byte
            Assert.Equal(0x00, header[3]); // iteration high byte
            Assert.Equal(0x00, header[4]); // finalizer (not last slice)
            Assert.Equal(0x03, header[5]); // keyId
            Assert.Equal(0x00, header[6]); // padding
            Assert.Equal(0x00, header[7]); // padding

            for (int i = 8; i < 16; i++)
            {
                Assert.Equal(0x00, header[i]);
            }
        }

        [Fact]
        public void ScreenDevice_GetKeySetupHeader_FinalSlice_ReturnsCorrectHeader()
        {
            var device = new StreamDeckNeo(0, 0, "", "", "");

            // keyId=0, sliceLength=100, iteration=0, remainingBytes=100
            // finalizer=1 because sliceLength == remainingBytes
            byte[] header = device.GetKeySetupHeader(0, 100, 0, 100);

            Assert.Equal(8, header.Length);
            Assert.Equal(0x02, header[0]);
            Assert.Equal(0x07, header[1]);
            Assert.Equal(0x00, header[2]); // keyId
            Assert.Equal(0x01, header[3]); // finalizer (last slice)
            Assert.Equal(0x64, header[4]); // sliceLength low byte (100)
            Assert.Equal(0x00, header[5]); // sliceLength high byte
            Assert.Equal(0x00, header[6]); // iteration low byte
            Assert.Equal(0x00, header[7]); // iteration high byte
        }

        [Fact]
        public void ScreenDevice_GetKeySetupHeader_NonFinalSlice_ReturnsCorrectHeader()
        {
            var device = new StreamDeckNeo(0, 0, "", "", "");

            // keyId=3, sliceLength=100, iteration=0, remainingBytes=200
            // finalizer=0 because sliceLength != remainingBytes
            byte[] header = device.GetKeySetupHeader(3, 100, 0, 200);

            Assert.Equal(8, header.Length);
            Assert.Equal(0x02, header[0]);
            Assert.Equal(0x07, header[1]);
            Assert.Equal(0x03, header[2]); // keyId
            Assert.Equal(0x00, header[3]); // finalizer (not last slice)
            Assert.Equal(0x64, header[4]); // sliceLength low byte (100)
            Assert.Equal(0x00, header[5]); // sliceLength high byte
            Assert.Equal(0x00, header[6]); // iteration low byte
            Assert.Equal(0x00, header[7]); // iteration high byte
        }

        [Fact]
        public void JpegDevice_GetKeySetupHeader_LargeIterationValue_EncodesLittleEndian()
        {
            var device = new StreamDeckMK2(0, 0, "", "", "");

            // iteration=300 => 0x012C => low=0x2C, high=0x01
            // sliceLength=500 => 0x01F4 => low=0xF4, high=0x01
            byte[] header = device.GetKeySetupHeader(1, 500, 300, 500);

            Assert.Equal(0x01, header[2]); // keyId
            Assert.Equal(0x01, header[3]); // finalizer (sliceLength == remainingBytes)
            Assert.Equal(0xF4, header[4]); // sliceLength low byte
            Assert.Equal(0x01, header[5]); // sliceLength high byte
            Assert.Equal(0x2C, header[6]); // iteration low byte
            Assert.Equal(0x01, header[7]); // iteration high byte
        }

        [Fact]
        public void BmpDevice_GetKeySetupHeader_LargeIterationValue_EncodesLittleEndian()
        {
            var device = new StreamDeckMini(0, 0, "", "", "");

            // iteration=300 => 0x012C => low=0x2C, high=0x01
            byte[] header = device.GetKeySetupHeader(2, 500, 300, 500);

            Assert.Equal(0x2C, header[2]); // iteration low byte
            Assert.Equal(0x01, header[3]); // iteration high byte
            Assert.Equal(0x01, header[4]); // finalizer
            Assert.Equal(0x02, header[5]); // keyId
        }
    }
}

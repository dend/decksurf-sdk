// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public class DataHelpersTests
    {
        [Theory]
        [InlineData(0, new byte[] { 0x00, 0x00 })]
        [InlineData(1, new byte[] { 0x01, 0x00 })]
        [InlineData(255, new byte[] { 0xFF, 0x00 })]
        [InlineData(256, new byte[] { 0x00, 0x01 })]
        [InlineData(1024, new byte[] { 0x00, 0x04 })]
        public void GetLittleEndianBytesFromInt_ReturnsExpectedBytes(int value, byte[] expected)
        {
            byte[] result = DataHelpers.GetLittleEndianBytesFromInt(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(255)]
        [InlineData(256)]
        [InlineData(1024)]
        [InlineData(65535)]
        public void GetIntFromLittleEndianBytes_ByteArray_RoundtripsWithGetLittleEndianBytesFromInt(int originalValue)
        {
            byte[] bytes = DataHelpers.GetLittleEndianBytesFromInt(originalValue);
            int result = DataHelpers.GetIntFromLittleEndianBytes(bytes);

            Assert.Equal(originalValue, result);
        }

        [Fact]
        public void GetIntFromLittleEndianBytes_ByteArray_ThrowsForEmptyArray()
        {
            Assert.Throws<ArgumentException>(() => DataHelpers.GetIntFromLittleEndianBytes(Array.Empty<byte>()));
        }

        [Fact]
        public void GetIntFromLittleEndianBytes_ByteArray_ThrowsForSingleByte()
        {
            Assert.Throws<ArgumentException>(() => DataHelpers.GetIntFromLittleEndianBytes(new byte[] { 0x01 }));
        }

        [Fact]
        public void GetIntFromLittleEndianBytes_ByteArray_ThrowsForThreeBytes()
        {
            Assert.Throws<ArgumentException>(() => DataHelpers.GetIntFromLittleEndianBytes(new byte[] { 0x01, 0x02, 0x03 }));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(255)]
        [InlineData(256)]
        [InlineData(1024)]
        [InlineData(65535)]
        public void GetIntFromLittleEndianBytes_Span_RoundtripsWithGetLittleEndianBytesFromInt(int originalValue)
        {
            byte[] bytes = DataHelpers.GetLittleEndianBytesFromInt(originalValue);
            ReadOnlySpan<byte> span = bytes.AsSpan();
            int result = DataHelpers.GetIntFromLittleEndianBytes(span);

            Assert.Equal(originalValue, result);
        }

        [Fact]
        public void GetIntFromLittleEndianBytes_Span_ThrowsForEmptySpan()
        {
            Assert.Throws<ArgumentException>(() => DataHelpers.GetIntFromLittleEndianBytes(ReadOnlySpan<byte>.Empty));
        }

        [Fact]
        public void GetIntFromLittleEndianBytes_Span_ThrowsForSingleByte()
        {
            byte[] data = new byte[] { 0x01 };
            Assert.Throws<ArgumentException>(() => DataHelpers.GetIntFromLittleEndianBytes((ReadOnlySpan<byte>)data));
        }

        [Fact]
        public void GetIntFromLittleEndianBytes_Span_ThrowsForThreeBytes()
        {
            byte[] data = new byte[] { 0x01, 0x02, 0x03 };
            Assert.Throws<ArgumentException>(() => DataHelpers.GetIntFromLittleEndianBytes((ReadOnlySpan<byte>)data));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(255)]
        [InlineData(256)]
        [InlineData(1024)]
        public void GetIntFromLittleEndianBytes_SpanAndByteArray_ProduceSameResult(int originalValue)
        {
            byte[] bytes = DataHelpers.GetLittleEndianBytesFromInt(originalValue);
            int fromArray = DataHelpers.GetIntFromLittleEndianBytes(bytes);
            int fromSpan = DataHelpers.GetIntFromLittleEndianBytes((ReadOnlySpan<byte>)bytes);

            Assert.Equal(fromArray, fromSpan);
        }

        [Fact]
        public void ByteArrayToString_ProducesExpectedHexFormat()
        {
            byte[] data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
            string result = DataHelpers.ByteArrayToString(data);

            Assert.Equal("DE-AD-BE-EF", result);
        }

        [Fact]
        public void ByteArrayToString_SingleByte_ProducesExpectedFormat()
        {
            byte[] data = new byte[] { 0x0A };
            string result = DataHelpers.ByteArrayToString(data);

            Assert.Equal("0A", result);
        }

        [Fact]
        public void ByteArrayToString_EmptyArray_ReturnsEmptyString()
        {
            byte[] data = Array.Empty<byte>();
            string result = DataHelpers.ByteArrayToString(data);

            Assert.Equal(string.Empty, result);
        }
    }
}

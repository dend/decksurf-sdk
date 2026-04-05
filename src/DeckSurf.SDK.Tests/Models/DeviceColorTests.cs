// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class DeviceColorTests
    {
        [Fact]
        public void Constructor_SetsRgbComponents()
        {
            var color = new DeviceColor(10, 20, 30);

            Assert.Equal(10, color.R);
            Assert.Equal(20, color.G);
            Assert.Equal(30, color.B);
        }

        [Fact]
        public void Constructor_MinValues_SetsAllToZero()
        {
            var color = new DeviceColor(0, 0, 0);

            Assert.Equal(0, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(0, color.B);
        }

        [Fact]
        public void Constructor_MaxValues_SetsAllTo255()
        {
            var color = new DeviceColor(255, 255, 255);

            Assert.Equal(255, color.R);
            Assert.Equal(255, color.G);
            Assert.Equal(255, color.B);
        }

        [Fact]
        public void Black_ReturnsZeroForAllComponents()
        {
            var black = DeviceColor.Black;

            Assert.Equal(0, black.R);
            Assert.Equal(0, black.G);
            Assert.Equal(0, black.B);
        }

        [Fact]
        public void Red_Returns255ForRedAndZeroForOthers()
        {
            var red = DeviceColor.Red;

            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);
        }

        [Fact]
        public void Green_Returns128ForGreenAndZeroForOthers()
        {
            var green = DeviceColor.Green;

            Assert.Equal(0, green.R);
            Assert.Equal(128, green.G);
            Assert.Equal(0, green.B);
        }

        [Fact]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new DeviceColor(100, 150, 200);
            var b = new DeviceColor(100, 150, 200);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Equals_DifferentR_ReturnsFalse()
        {
            var a = new DeviceColor(100, 150, 200);
            var b = new DeviceColor(101, 150, 200);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Equals_DifferentG_ReturnsFalse()
        {
            var a = new DeviceColor(100, 150, 200);
            var b = new DeviceColor(100, 151, 200);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Equals_DifferentB_ReturnsFalse()
        {
            var a = new DeviceColor(100, 150, 200);
            var b = new DeviceColor(100, 150, 201);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void EqualsObject_SameValues_ReturnsTrue()
        {
            var a = new DeviceColor(50, 100, 150);
            object b = new DeviceColor(50, 100, 150);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void EqualsObject_DifferentValues_ReturnsFalse()
        {
            var a = new DeviceColor(50, 100, 150);
            object b = new DeviceColor(51, 100, 150);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void EqualsObject_WrongType_ReturnsFalse()
        {
            var color = new DeviceColor(50, 100, 150);
            object other = "not a color";

            Assert.False(color.Equals(other));
        }

        [Fact]
        public void EqualsObject_Null_ReturnsFalse()
        {
            var color = new DeviceColor(50, 100, 150);

            Assert.False(color.Equals(null));
        }

        [Fact]
        public void GetHashCode_EqualValues_ProduceSameHash()
        {
            var a = new DeviceColor(10, 20, 30);
            var b = new DeviceColor(10, 20, 30);

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentValues_ProduceDifferentHash()
        {
            var a = new DeviceColor(10, 20, 30);
            var b = new DeviceColor(30, 20, 10);

            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void EqualityOperator_SameValues_ReturnsTrue()
        {
            var a = new DeviceColor(1, 2, 3);
            var b = new DeviceColor(1, 2, 3);

            Assert.True(a == b);
        }

        [Fact]
        public void EqualityOperator_DifferentValues_ReturnsFalse()
        {
            var a = new DeviceColor(1, 2, 3);
            var b = new DeviceColor(4, 5, 6);

            Assert.False(a == b);
        }

        [Fact]
        public void InequalityOperator_DifferentValues_ReturnsTrue()
        {
            var a = new DeviceColor(1, 2, 3);
            var b = new DeviceColor(4, 5, 6);

            Assert.True(a != b);
        }

        [Fact]
        public void InequalityOperator_SameValues_ReturnsFalse()
        {
            var a = new DeviceColor(1, 2, 3);
            var b = new DeviceColor(1, 2, 3);

            Assert.False(a != b);
        }

        [Fact]
        public void ToString_ReturnsExpectedFormat()
        {
            var color = new DeviceColor(10, 20, 30);

            Assert.Equal("DeviceColor(R=10, G=20, B=30)", color.ToString());
        }

        [Fact]
        public void ToString_StaticBlack_ReturnsExpectedFormat()
        {
            Assert.Equal("DeviceColor(R=0, G=0, B=0)", DeviceColor.Black.ToString());
        }

        [Fact]
        public void DefaultStruct_HasAllZeros()
        {
            var color = default(DeviceColor);

            Assert.Equal(0, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(0, color.B);
        }

        [Fact]
        public void DefaultStruct_EqualsBlack()
        {
            var defaultColor = default(DeviceColor);

            Assert.Equal(DeviceColor.Black, defaultColor);
        }
    }
}

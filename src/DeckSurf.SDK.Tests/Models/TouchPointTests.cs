// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class TouchPointTests
    {
        [Fact]
        public void Constructor_SetsXAndY()
        {
            var point = new TouchPoint(100, 200);

            Assert.Equal(100, point.X);
            Assert.Equal(200, point.Y);
        }

        [Fact]
        public void Constructor_ZeroCoordinates()
        {
            var point = new TouchPoint(0, 0);

            Assert.Equal(0, point.X);
            Assert.Equal(0, point.Y);
        }

        [Fact]
        public void Constructor_NegativeCoordinates_AreValid()
        {
            var point = new TouchPoint(-50, -100);

            Assert.Equal(-50, point.X);
            Assert.Equal(-100, point.Y);
        }

        [Fact]
        public void Constructor_MixedNegativeAndPositive()
        {
            var point = new TouchPoint(-10, 300);

            Assert.Equal(-10, point.X);
            Assert.Equal(300, point.Y);
        }

        [Fact]
        public void Constructor_LargeValues()
        {
            var point = new TouchPoint(int.MaxValue, int.MinValue);

            Assert.Equal(int.MaxValue, point.X);
            Assert.Equal(int.MinValue, point.Y);
        }

        [Fact]
        public void Equals_SameCoordinates_ReturnsTrue()
        {
            var a = new TouchPoint(10, 20);
            var b = new TouchPoint(10, 20);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Equals_DifferentX_ReturnsFalse()
        {
            var a = new TouchPoint(10, 20);
            var b = new TouchPoint(11, 20);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Equals_DifferentY_ReturnsFalse()
        {
            var a = new TouchPoint(10, 20);
            var b = new TouchPoint(10, 21);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Equals_BothDifferent_ReturnsFalse()
        {
            var a = new TouchPoint(10, 20);
            var b = new TouchPoint(30, 40);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void EqualsObject_SameValues_ReturnsTrue()
        {
            var a = new TouchPoint(5, 10);
            object b = new TouchPoint(5, 10);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void EqualsObject_DifferentValues_ReturnsFalse()
        {
            var a = new TouchPoint(5, 10);
            object b = new TouchPoint(6, 10);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void EqualsObject_WrongType_ReturnsFalse()
        {
            var point = new TouchPoint(5, 10);
            object other = "not a point";

            Assert.False(point.Equals(other));
        }

        [Fact]
        public void EqualsObject_Null_ReturnsFalse()
        {
            var point = new TouchPoint(5, 10);

            Assert.False(point.Equals(null));
        }

        [Fact]
        public void EqualityOperator_SameValues_ReturnsTrue()
        {
            var a = new TouchPoint(15, 25);
            var b = new TouchPoint(15, 25);

            Assert.True(a == b);
        }

        [Fact]
        public void EqualityOperator_DifferentValues_ReturnsFalse()
        {
            var a = new TouchPoint(15, 25);
            var b = new TouchPoint(25, 15);

            Assert.False(a == b);
        }

        [Fact]
        public void InequalityOperator_DifferentValues_ReturnsTrue()
        {
            var a = new TouchPoint(15, 25);
            var b = new TouchPoint(25, 15);

            Assert.True(a != b);
        }

        [Fact]
        public void InequalityOperator_SameValues_ReturnsFalse()
        {
            var a = new TouchPoint(15, 25);
            var b = new TouchPoint(15, 25);

            Assert.False(a != b);
        }

        [Fact]
        public void GetHashCode_EqualValues_ProduceSameHash()
        {
            var a = new TouchPoint(42, 99);
            var b = new TouchPoint(42, 99);

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentValues_ProduceDifferentHash()
        {
            var a = new TouchPoint(42, 99);
            var b = new TouchPoint(99, 42);

            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_IsConsistent_AcrossMultipleCalls()
        {
            var point = new TouchPoint(7, 13);

            var hash1 = point.GetHashCode();
            var hash2 = point.GetHashCode();
            var hash3 = point.GetHashCode();

            Assert.Equal(hash1, hash2);
            Assert.Equal(hash2, hash3);
        }

        [Fact]
        public void ToString_ReturnsExpectedFormat()
        {
            var point = new TouchPoint(100, 200);

            Assert.Equal("TouchPoint(X=100, Y=200)", point.ToString());
        }

        [Fact]
        public void ToString_NegativeCoordinates_ReturnsExpectedFormat()
        {
            var point = new TouchPoint(-50, -75);

            Assert.Equal("TouchPoint(X=-50, Y=-75)", point.ToString());
        }

        [Fact]
        public void ToString_ZeroCoordinates_ReturnsExpectedFormat()
        {
            var point = new TouchPoint(0, 0);

            Assert.Equal("TouchPoint(X=0, Y=0)", point.ToString());
        }

        [Fact]
        public void DefaultStruct_HasZeroCoordinates()
        {
            var point = default(TouchPoint);

            Assert.Equal(0, point.X);
            Assert.Equal(0, point.Y);
        }
    }
}

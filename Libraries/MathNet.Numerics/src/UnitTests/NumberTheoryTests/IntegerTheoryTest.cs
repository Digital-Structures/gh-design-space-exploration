﻿// <copyright file="IntegerTheoryTest.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2010 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.UnitTests.NumberTheoryTests
{
    using System;
    using NumberTheory;
    using MbUnit.Framework;

    [TestFixture]
    public class IntegerTheoryTest
    {
        [Test]
        public void TestEvenOdd32()
        {
            Assert.IsTrue(IntegerTheory.IsEven(0), "0 is even");
            Assert.IsFalse(IntegerTheory.IsOdd(0), "0 is not odd");

            Assert.IsFalse(IntegerTheory.IsEven(1), "1 is not even");
            Assert.IsTrue(IntegerTheory.IsOdd(1), "1 is odd");

            Assert.IsFalse(IntegerTheory.IsEven(-1), "-1 is not even");
            Assert.IsTrue(IntegerTheory.IsOdd(-1), "-1 is odd");

            Assert.IsFalse(IntegerTheory.IsEven(Int32.MaxValue), "Int32.Max is not even");
            Assert.IsTrue(IntegerTheory.IsOdd(Int32.MaxValue), "Int32.Max is odd");

            Assert.IsTrue(IntegerTheory.IsEven(Int32.MinValue), "Int32.Min is even");
            Assert.IsFalse(IntegerTheory.IsOdd(Int32.MinValue), "Int32.Min is not odd");
        }

        [Test]
        public void TestEvenOdd64()
        {
            Assert.IsTrue(IntegerTheory.IsEven((long)0), "0 is even");
            Assert.IsFalse(IntegerTheory.IsOdd((long)0), "0 is not odd");

            Assert.IsFalse(IntegerTheory.IsEven((long)1), "1 is not even");
            Assert.IsTrue(IntegerTheory.IsOdd((long)1), "1 is odd");

            Assert.IsFalse(IntegerTheory.IsEven((long)-1), "-1 is not even");
            Assert.IsTrue(IntegerTheory.IsOdd((long)-1), "-1 is odd");

            Assert.IsFalse(IntegerTheory.IsEven(Int64.MaxValue), "Int64.Max is not even");
            Assert.IsTrue(IntegerTheory.IsOdd(Int64.MaxValue), "Int64.Max is odd");

            Assert.IsTrue(IntegerTheory.IsEven(Int64.MinValue), "Int64.Min is even");
            Assert.IsFalse(IntegerTheory.IsOdd(Int64.MinValue), "Int64.Min is not odd");
        }

        [Test]
        public void TestIsPowerOfTwo32()
        {
            for (int i = 2; i < 31; i++)
            {
                int x = 1 << i;
                Assert.IsTrue(IntegerTheory.IsPowerOfTwo(x), x + " (+)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(x - 1), x + "-1 (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(x + 1), x + "+1 (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-x), "-" + x + " (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-x + 1), "-" + x + "+1 (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-x - 1), "-" + x + "-1 (-)");
            }

            Assert.IsTrue(IntegerTheory.IsPowerOfTwo(4), "4 (+)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(3), "3 (-)");
            Assert.IsTrue(IntegerTheory.IsPowerOfTwo(2), "2 (+)");
            Assert.IsTrue(IntegerTheory.IsPowerOfTwo(1), "1 (+)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(0), "0 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-1), "-1 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-2), "-2 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-3), "-3 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-4), "-4 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int32.MinValue), "Int32.MinValue (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int32.MinValue+1), "Int32.MinValue+1 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int32.MaxValue), "Int32.MaxValue (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int32.MaxValue-1), "Int32.MaxValue-1 (-)");
        }

        [Test]
        public void TestIsPowerOfTwo64()
        {
            for (int i = 2; i < 63; i++)
            {
                long x = ((long)1) << i;
                Assert.IsTrue(IntegerTheory.IsPowerOfTwo(x), x + " (+)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(x - 1), x + "-1 (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(x + 1), x + "+1 (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-x), "-" + x + " (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-x + 1), "-" + x + "+1 (-)");
                Assert.IsFalse(IntegerTheory.IsPowerOfTwo(-x - 1), "-" + x + "-1 (-)");
            }

            Assert.IsTrue(IntegerTheory.IsPowerOfTwo((long)4), "4 (+)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo((long)3), "3 (-)");
            Assert.IsTrue(IntegerTheory.IsPowerOfTwo((long)2), "2 (+)");
            Assert.IsTrue(IntegerTheory.IsPowerOfTwo((long)1), "1 (+)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo((long)0), "0 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo((long)-1), "-1 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo((long)-2), "-2 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo((long)-3), "-3 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo((long)-4), "-4 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int64.MinValue), "Int32.MinValue (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int64.MinValue+1), "Int32.MinValue+1 (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int64.MaxValue), "Int32.MaxValue (-)");
            Assert.IsFalse(IntegerTheory.IsPowerOfTwo(Int64.MaxValue-1), "Int32.MaxValue-1 (-)");
        }

        [Test]
        public void CeilingToPowerOfHandlesPositiveIntegersCorrectly32()
        {
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(0), "0");
            Assert.AreEqual(1, IntegerTheory.CeilingToPowerOfTwo(1), "1");
            Assert.AreEqual(2, IntegerTheory.CeilingToPowerOfTwo(2), "2");
            Assert.AreEqual(4, IntegerTheory.CeilingToPowerOfTwo(3), "3");
            Assert.AreEqual(4, IntegerTheory.CeilingToPowerOfTwo(4), "4");

            for (int i = 2; i < 31; i++)
            {
                int x = 1 << i;
                Assert.AreEqual(x, IntegerTheory.CeilingToPowerOfTwo(x), x.ToString());
                Assert.AreEqual(x, IntegerTheory.CeilingToPowerOfTwo(x - 1), x + "-1");
                Assert.AreEqual(x, IntegerTheory.CeilingToPowerOfTwo((x >> 1) + 1), x + "/2+1");
                Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(-x), "-" + x);
            }

            const int maxPowerOfTwo = 0x40000000;
            Assert.AreEqual(maxPowerOfTwo, IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo), "max");
            Assert.AreEqual(maxPowerOfTwo, IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo - 1), "max");
        }

        [Test]
        public void CeilingToPowerOfHandlesPositiveIntegersCorrectly64()
        {
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo((long)0), "0");
            Assert.AreEqual(1, IntegerTheory.CeilingToPowerOfTwo((long)1), "1");
            Assert.AreEqual(2, IntegerTheory.CeilingToPowerOfTwo((long)2), "2");
            Assert.AreEqual(4, IntegerTheory.CeilingToPowerOfTwo((long)3), "3");
            Assert.AreEqual(4, IntegerTheory.CeilingToPowerOfTwo((long)4), "4");

            for (int i = 2; i < 63; i++)
            {
                long x = ((long)1) << i;
                Assert.AreEqual(x, IntegerTheory.CeilingToPowerOfTwo(x), x.ToString());
                Assert.AreEqual(x, IntegerTheory.CeilingToPowerOfTwo(x - 1), x + "-1");
                Assert.AreEqual(x, IntegerTheory.CeilingToPowerOfTwo((x >> 1) + 1), x + "/2+1");
                Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(-x), "-" + x);
            }

            const long maxPowerOfTwo = 0x4000000000000000;
            Assert.AreEqual(maxPowerOfTwo, IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo), "max");
            Assert.AreEqual(maxPowerOfTwo, IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo - 1), "max");
        }

        [Test]
        public void CeilingToPowerOfTwoReturnsZeroForNegativeNumbers32()
        {
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(-1), "-1");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(-2), "-2");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(-3), "-3");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(-4), "-4");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(Int32.MinValue), "Int32.MinValue");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(Int32.MinValue + 1), "Int32.MinValue+1");
        }

        [Test]
        public void CeilingToPowerOfTwoReturnsZeroForNegativeNumbers64()
        {
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo((long)-1), "-1");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo((long)-2), "-2");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo((long)-3), "-3");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo((long)-4), "-4");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(Int64.MinValue), "Int64.MinValue");
            Assert.AreEqual(0, IntegerTheory.CeilingToPowerOfTwo(Int64.MinValue + 1), "Int64.MinValue+1");
        }

        [Test]
        public void CeilingToPowerOfTwoThrowsWhenResultWouldOverflow32()
        {
            Assert.Throws(
                typeof (ArgumentOutOfRangeException),
                () => IntegerTheory.CeilingToPowerOfTwo(Int32.MaxValue));

            const int maxPowerOfTwo = 0x40000000;
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo + 1));

            Assert.DoesNotThrow(
                () => IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo - 1));
        }

        [Test]
        public void CeilingToPowerOfTwoThrowsWhenResultWouldOverflow64()
        {
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.CeilingToPowerOfTwo(Int64.MaxValue));

            const long maxPowerOfTwo = 0x4000000000000000;
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo + 1));

            Assert.DoesNotThrow(
                () => IntegerTheory.CeilingToPowerOfTwo(maxPowerOfTwo - 1));
        }

        [Test]
        public void PowerOfTwoMatchesFloatingPointPower32()
        {
            for(int i=0; i<31; i++)
            {
                Assert.AreEqual(Math.Round(Math.Pow(2, i)), IntegerTheory.PowerOfTwo(i));
            }
        }

        [Test]
        public void PowerOfTwoMatchesFloatingPointPower64()
        {
            for (int i = 0; i < 63; i++)
            {
                Assert.AreEqual(Math.Round(Math.Pow(2, i)), IntegerTheory.PowerOfTwo((long)i));
            }
        }

        [Test]
        public void PowerOfTwoThrowsWhenOutOfRange32()
        {
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo(-1));

            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo(31));

            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo(Int32.MinValue));

            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo(Int32.MaxValue));

            Assert.DoesNotThrow(
                () => IntegerTheory.PowerOfTwo(30));

            Assert.DoesNotThrow(
                () => IntegerTheory.PowerOfTwo(0));
        }

        [Test]
        public void PowerOfTwoThrowsWhenOutOfRange64()
        {
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo((long)-1));

            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo((long)63));

            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo(Int64.MinValue));

            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => IntegerTheory.PowerOfTwo(Int64.MaxValue));

            Assert.DoesNotThrow(
                () => IntegerTheory.PowerOfTwo((long)62));

            Assert.DoesNotThrow(
                () => IntegerTheory.PowerOfTwo((long)0));
        }

        [Test]
        public void TestIsPerfectSquare32()
        {
            // Test all known suares
            int lastRadix = (int)Math.Floor(Math.Sqrt(Int32.MaxValue));
            for (int i = 0; i <= lastRadix; i++)
            {
                Assert.IsTrue(IntegerTheory.IsPerfectSquare(i * i), i + "^2 (+)");
            }

            // Test 1-offset from all known squares
            for (int i = 2; i <= lastRadix; i++)
            {
                Assert.IsFalse(IntegerTheory.IsPerfectSquare((i * i) - 1), i + "^2-1 (-)");
                Assert.IsFalse(IntegerTheory.IsPerfectSquare((i * i) + 1), i + "^2+1 (-)");
            }

            // Selected Cases
            Assert.IsTrue(IntegerTheory.IsPerfectSquare(100000000), "100000000 (+)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(100000001), "100000001 (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(99999999), "99999999 (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(-4), "-4 (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(Int32.MinValue), "Int32.MinValue (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(Int32.MaxValue), "Int32.MaxValue (-)");
            Assert.IsTrue(IntegerTheory.IsPerfectSquare(1), "1 (+)");
            Assert.IsTrue(IntegerTheory.IsPerfectSquare(0), "0 (+)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(-1), "-1 (-)");
        }

        [Test]
        public void TestIsPerfectSquare64()
        {
            // Test all known suares
            for (int i = 0; i < 32; i++)
            {
                long t = ((long)1) << i;
                Assert.IsTrue(IntegerTheory.IsPerfectSquare(t * t), t + "^2 (+)");
            }

            // Test 1-offset from all known squares
            for (int i = 1; i < 32; i++)
            {
                long t = ((long)1) << i;
                Assert.IsFalse(IntegerTheory.IsPerfectSquare((t * t) - 1), t + "^2-1 (-)");
                Assert.IsFalse(IntegerTheory.IsPerfectSquare((t * t) + 1), t + "^2+1 (-)");
            }

            // Selected Cases
            Assert.IsTrue(IntegerTheory.IsPerfectSquare((long)1000000000000000000), "1000000000000000000 (+)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare((long)1000000000000000001), "1000000000000000001 (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare((long)999999999999999999), "999999999999999999 (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare((long)999999999999999993), "999999999999999993 (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare((long)-4), "-4 (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(Int64.MinValue), "Int32.MinValue (-)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare(Int64.MaxValue), "Int32.MaxValue (-)");
            Assert.IsTrue(IntegerTheory.IsPerfectSquare((long)1), "1 (+)");
            Assert.IsTrue(IntegerTheory.IsPerfectSquare((long)0), "0 (+)");
            Assert.IsFalse(IntegerTheory.IsPerfectSquare((long)-1), "-1 (-)");
        }
    }
}
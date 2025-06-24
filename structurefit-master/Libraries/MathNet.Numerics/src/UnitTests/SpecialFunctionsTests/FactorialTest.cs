// <copyright file="FactorialTest.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.SpecialFunctionsTests
{
    using System;
    using MbUnit.Framework;

    [TestFixture]
    public class FactorialTest
    {
        [Test]
        public void CanComputeFactorial()
        {
            // exact
            double factorial = 1.0;
            for (int i = 1; i < 23; i++)
            {
                factorial *= i;
                AssertHelpers.AlmostEqual(factorial, SpecialFunctions.Factorial(i), 14);
                AssertHelpers.AlmostEqual(Math.Log(factorial), SpecialFunctions.FactorialLn(i), 14);
            }

            // approximation
            for (int i = 23; i < 171; i++)
            {
                factorial *= i;
                AssertHelpers.AlmostEqual(factorial, SpecialFunctions.Factorial(i), 14);
                AssertHelpers.AlmostEqual(Math.Log(factorial), SpecialFunctions.FactorialLn(i), 14);
            }
        }

        [Test]
        public void ThrowsOnNegativeArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SpecialFunctions.Factorial(Int32.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => SpecialFunctions.Factorial(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SpecialFunctions.FactorialLn(-1));
        }

        [Test]
        public void FactorialOverflowsToInfinity()
        {
            Assert.AreEqual(Double.PositiveInfinity, SpecialFunctions.Factorial(172));
            Assert.AreEqual(Double.PositiveInfinity, SpecialFunctions.Factorial(Int32.MaxValue));
        }

        [Test]
        public void FactorialLnDoesNotOverflow()
        {
            AssertHelpers.AlmostEqual(6078.2118847500501140, SpecialFunctions.FactorialLn(1 << 10), 14);
            AssertHelpers.AlmostEqual(29978.648060844048236, SpecialFunctions.FactorialLn(1 << 12), 14);
            AssertHelpers.AlmostEqual(307933.81973375485425, SpecialFunctions.FactorialLn(1 << 15), 14);
            AssertHelpers.AlmostEqual(1413421.9939462073242, SpecialFunctions.FactorialLn(1 << 17), 14);
        }

        [Test]
        public void CanComputeBinomial()
        {
            AssertHelpers.AlmostEqual(1, SpecialFunctions.Binomial(1, 1), 14);
            AssertHelpers.AlmostEqual(10, SpecialFunctions.Binomial(5, 2), 14);
            AssertHelpers.AlmostEqual(35, SpecialFunctions.Binomial(7, 3), 14);
            AssertHelpers.AlmostEqual(1, SpecialFunctions.Binomial(1, 0), 14);
            AssertHelpers.AlmostEqual(0, SpecialFunctions.Binomial(0, 1), 14);
            AssertHelpers.AlmostEqual(0, SpecialFunctions.Binomial(5, 7), 14);
            AssertHelpers.AlmostEqual(0, SpecialFunctions.Binomial(5, -7), 14);
        }

        [Test]
        public void CanComputeBinomialLn()
        {
            AssertHelpers.AlmostEqual(Math.Log(1), SpecialFunctions.BinomialLn(1, 1), 14);
            AssertHelpers.AlmostEqual(Math.Log(10), SpecialFunctions.BinomialLn(5, 2), 14);
            AssertHelpers.AlmostEqual(Math.Log(35), SpecialFunctions.BinomialLn(7, 3), 14);
            AssertHelpers.AlmostEqual(Math.Log(1), SpecialFunctions.BinomialLn(1, 0), 14);
            AssertHelpers.AlmostEqual(Math.Log(0), SpecialFunctions.BinomialLn(0, 1), 14);
            AssertHelpers.AlmostEqual(Math.Log(0), SpecialFunctions.BinomialLn(5, 7), 14);
            AssertHelpers.AlmostEqual(Math.Log(0), SpecialFunctions.BinomialLn(5, -7), 14);
        }

        [Test]
        public void CanComputeMultinomial()
        {
            AssertHelpers.AlmostEqual(1, SpecialFunctions.Multinomial(1, new int[] { 1, 0 }), 14);
            AssertHelpers.AlmostEqual(10, SpecialFunctions.Multinomial(5, new int[] { 3, 2 }), 14);
            AssertHelpers.AlmostEqual(10, SpecialFunctions.Multinomial(5, new int[] { 2, 3 }), 14);
            AssertHelpers.AlmostEqual(35, SpecialFunctions.Multinomial(7, new int[] { 3, 4 }), 14);
        }
    }
}
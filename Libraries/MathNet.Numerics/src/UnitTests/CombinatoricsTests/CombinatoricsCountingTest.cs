﻿// <copyright file="CombinatoricsCountingTest.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.CombinatoricsTests
{
    using MbUnit.Framework;

    [TestFixture]
    public class CombinatoricsCountingTest
    {
        [Test]
        [Row(0, 0, 1)]
        [Row(1, 0, 1)]
        [Row(10, 0, 1)]
        [Row(10, 2, 90)]
        [Row(10, 4, 5040)]
        [Row(10, 6, 151200)]
        [Row(10, 9, 3628800)]
        [Row(10, 10, 3628800)]
        public void CanCountVariations(int n, int k, long expected)
        {
            Assert.AreEqual(
                expected,
                Combinatorics.Variations(n, k),
                "Count the number of variations without repetition");
        }

        [Test]
        [Row(0, 1)]
        [Row(10, 11)]
        [Row(0, -1)]
        [Row(1, -1)]
        [Row(-1, 0)]
        [Row(-1, 1)]
        public void OutOfRangeVariationsMustCountToZero(int n, int k)
        {
            Assert.AreEqual(
                0,
                Combinatorics.Variations(n, k),
                "The number of variations without repetition but out of the range must be 0.");
        }

        [Test]
        [Row(0, 0, 1)]
        [Row(1, 0, 1)]
        [Row(10, 0, 1)]
        [Row(10, 2, 100)]
        [Row(10, 4, 10000)]
        [Row(10, 6, 1000000)]
        [Row(10, 9, 1000000000)]
        [Row(10, 10, 10000000000)]
        [Row(10, 11, 100000000000)]
        public void CanCountVariationsWithRepetition(int n, int k, long expected)
        {
            Assert.AreEqual(
                expected,
                Combinatorics.VariationsWithRepetition(n, k),
                "Count the number of variations with repetition");
        }

        [Test]
        [Row(0, 1)]
        [Row(0, -1)]
        [Row(1, -1)]
        [Row(-1, 0)]
        [Row(-1, 1)]
        public void OutOfRangeVariationsWithRepetitionMustCountToZero(int n, int k)
        {
            Assert.AreEqual(
                0,
                Combinatorics.VariationsWithRepetition(n, k),
                "The number of variations with repetition but out of the range must be 0.");
        }

        [Test]
        [Row(0, 0, 1)]
        [Row(1, 0, 1)]
        [Row(10, 0, 1)]
        [Row(10, 2, 45)]
        [Row(10, 4, 210)]
        [Row(10, 6, 210)]
        [Row(10, 9, 10)]
        [Row(10, 10, 1)]
        public void CanCountCombinations(int n, int k, long expected)
        {
            Assert.AreEqual(
                expected,
                Combinatorics.Combinations(n, k),
                "Count the number of combinations without repetition");
        }

        [Test]
        [Row(0, 1)]
        [Row(10, 11)]
        [Row(0, -1)]
        [Row(1, -1)]
        [Row(-1, 0)]
        [Row(-1, 1)]
        public void OutOfRangeCombinationsMustCountToZero(int n, int k)
        {
            Assert.AreEqual(
                0,
                Combinatorics.Combinations(n, k),
                "The number of combinations without repetition but out of the range must be 0.");
        }

        [Test]
        [Row(0, 0, 1)]
        [Row(1, 0, 1)]
        [Row(10, 0, 1)]
        [Row(10, 2, 55)]
        [Row(10, 4, 715)]
        [Row(10, 6, 5005)]
        [Row(10, 9, 48620)]
        [Row(10, 10, 92378)]
        [Row(10, 11, 167960)]
        public void CanCountCombinationsWithRepetition(int n, int k, long expected)
        {
            Assert.AreEqual(
                expected,
                Combinatorics.CombinationsWithRepetition(n, k),
                "Count the number of combinations with repetition");
        }

        [Test]
        [Row(0, 1)]
        [Row(0, -1)]
        [Row(1, -1)]
        [Row(-1, 0)]
        [Row(-1, 1)]
        public void OutOfRangeCombinationsWithRepetitionMustCountToZero(int n, int k)
        {
            Assert.AreEqual(
                0,
                Combinatorics.CombinationsWithRepetition(n, k),
                "The number of combinations with repetition but out of the range must be 0.");
        }

        [Test]
        [Row(0, 1)]
        [Row(1, 1)]
        [Row(2, 2)]
        [Row(8, 40320)]
        [Row(15, 1307674368000)]
        public void CanCountPermutations(int n, long expected)
        {
            Assert.AreEqual(
                expected,
                Combinatorics.Permutations(n),
                "Count the number of permutations");
        }
    }
}
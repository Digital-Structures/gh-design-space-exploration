﻿// <copyright file="ComplexTest.TextHandling.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.ComplexTests
{
    using System;
    using System.Globalization;
    using MbUnit.Framework;
    using System.Numerics;

    [TestFixture]
    public class ComplexTextHandlingTest
    {
      

        [Test]
        [Row("-1 -2i", -1, -2, "en-US")]
        [Row("-1 - 2i ", -1, -2, "de-CH")]
        public void CanParseStringToComplexWithCulture(
            string text, double expectedReal, double expectedImaginary, string cultureName)
        {
            Complex parsed = text.ToComplex(CultureInfo.GetCultureInfo(cultureName));
            Assert.AreEqual(expectedReal, parsed.Real);
            Assert.AreEqual(expectedImaginary, parsed.Imaginary);
        }

        [Test]
        [Row("1", 1, 0)]
        [Row("-1", -1, 0)]
        [Row("-i", 0, -1)]
        [Row("i", 0, 1)]
        [Row("2i", 0, 2)]
        [Row("1 + 2i", 1, 2)]
        [Row("1+2i", 1, 2)]
        [Row("1 - 2i", 1, -2)]
        [Row("1-2i", 1, -2)]
        [Row("1,2 ", 1, 2)]
        [Row("1 , 2", 1, 2)]
        [Row("1,2i", 1, 2)]
        [Row("-1, -2i", -1, -2)]
        [Row(" - 1 , - 2 i ", -1, -2)]
        [Row("(+1,2i)", 1, 2)]
        [Row("(-1 , -2)", -1, -2)]
        [Row("(-1 , -2i)", -1, -2)]
        [Row("(+1e1 , -2e-2i)", 10, -0.02)]
        [Row("(-1E1 -2e2i)", -10, -200)]
        [Row("(-1e+1 -2e2i)", -10, -200)]
        [Row("(-1e1 -2e+2i)", -10, -200)]
        [Row("(-1e-1  -2E2i)", -0.1, -200)]
        [Row("(-1e1  -2e-2i)", -10, -0.02)]
        [Row("(-1E+1 -2e+2i)", -10, -200)]
        [Row("(-1e-1,-2e-2i)", -0.1, -0.02)]
        [Row("(+1 +2i)", 1, 2)]
        [Row("(-1E+1 -2e+2i)", -10, -200)]
        [Row("(-1e-1,-2e-2i)", -0.1, -0.02)]
        public void CanTryParseStringToComplexWithInvariant(string str, double expectedReal, double expectedImaginary)
        {
            var invariantCulture = CultureInfo.InvariantCulture;
            Complex z;
            var ret = str.TryToComplex(invariantCulture, out z);
            Assert.IsTrue(ret);
            Assert.AreEqual(expectedReal, z.Real);
            Assert.AreEqual(expectedImaginary, z.Imaginary);
        }

        [Test]
        public void ParseThrowsFormatExceptionIfMissingClosingParen()
        {
            Assert.Throws<FormatException>(() => "(1,2".ToComplex());
        }

        [Test]
        public void TryParseCanHandleSymbols()
        {
            Complex z;
            var ni = NumberFormatInfo.CurrentInfo;
            var separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            var symbol = ni.NegativeInfinitySymbol + separator + ni.PositiveInfinitySymbol;
            var ret = symbol.TryToComplex(out z);
            Assert.IsTrue(ret, "A1");
            Assert.AreEqual(double.NegativeInfinity, z.Real, "A2");
            Assert.AreEqual(double.PositiveInfinity, z.Imaginary, "A3");

            symbol = ni.NaNSymbol + separator + ni.NaNSymbol;
            ret = symbol.TryToComplex(out z);
            Assert.IsTrue(ret, "B1");
            Assert.AreEqual(double.NaN, z.Real, "B2");
            Assert.AreEqual(double.NaN, z.Imaginary, "B3");

            symbol = ni.NegativeInfinitySymbol + "+" + ni.PositiveInfinitySymbol + "i";
            ret = symbol.TryToComplex(out z);
            Assert.IsTrue(ret, "C1");
            Assert.AreEqual(double.NegativeInfinity, z.Real, "C2");
            Assert.AreEqual(double.PositiveInfinity, z.Imaginary, "C3");

            symbol = ni.NaNSymbol + "+" + ni.NaNSymbol + "i";
            ret = symbol.TryToComplex(out z);
            Assert.IsTrue(ret, "D1");
            Assert.AreEqual(double.NaN, z.Real, "D2");
            Assert.AreEqual(double.NaN, z.Imaginary, "D3");

            symbol = double.MaxValue.ToString("R") + " " + double.MinValue.ToString("R") + "i";
            ret = symbol.TryToComplex(out z);
            Assert.IsTrue(ret, "E1");
            Assert.AreEqual(double.MaxValue, z.Real, "E2");
            Assert.AreEqual(double.MinValue, z.Imaginary, "E3");
        }

        [Test]
        [Row("en-US")]
        [Row("tr-TR")]
        [Row("de-DE")]
        [Row("de-CH")]
        [Row("he-IL")]
        public void TryParseCanHandleSymbolsWithCulture(string cultureName)
        {
            Complex z;
            var culture = CultureInfo.GetCultureInfo(cultureName);
            var ni = culture.NumberFormat;
            var separator = culture.TextInfo.ListSeparator;

            var symbol = ni.NegativeInfinitySymbol + separator + ni.PositiveInfinitySymbol;
            var ret = symbol.TryToComplex(culture, out z);
            Assert.IsTrue(ret, "A1");
            Assert.AreEqual(double.NegativeInfinity, z.Real, "A2");
            Assert.AreEqual(double.PositiveInfinity, z.Imaginary, "A3");

            symbol = ni.NaNSymbol + separator + ni.NaNSymbol;
            ret = symbol.TryToComplex(culture, out z);
            Assert.IsTrue(ret, "B1");
            Assert.AreEqual(double.NaN, z.Real, "B2");
            Assert.AreEqual(double.NaN, z.Imaginary, "B3");

            symbol = ni.NegativeInfinitySymbol + "+" + ni.PositiveInfinitySymbol + "i";
            ret = symbol.TryToComplex(culture, out z);
            Assert.IsTrue(ret, "C1");
            Assert.AreEqual(double.NegativeInfinity, z.Real, "C2");
            Assert.AreEqual(double.PositiveInfinity, z.Imaginary, "C3");

            symbol = ni.NaNSymbol + "+" + ni.NaNSymbol + "i";
            ret = symbol.TryToComplex(culture, out z);
            Assert.IsTrue(ret, "D1");
            Assert.AreEqual(double.NaN, z.Real, "D2");
            Assert.AreEqual(double.NaN, z.Imaginary, "D3");

            symbol = double.MaxValue.ToString("R", culture) + " " + double.MinValue.ToString("R", culture) + "i";
            ret = symbol.TryToComplex(culture, out z);
            Assert.IsTrue(ret, "E1");
            Assert.AreEqual(double.MaxValue, z.Real, "E2");
            Assert.AreEqual(double.MinValue, z.Imaginary, "E3");
        }

        [Test]
        [Row("")]
        [Row("+")]
        [Row("1-")]
        [Row("i+")]
        [Row("1/2i")]
        [Row("1i+2i")]
        [Row("i1i")]
        [Row("(1i,2)")]
        [Row("1e+")]
        [Row("1e")]
        [Row("1,")]
        [Row(",1")]
        [Row(null)]
        [Row("()")]
        [Row("(  )")]
        public void TryParseReturnsFalseWhenGivenBadValueWithInvariant(string str)
        {
            Complex z;
            var ret = str.TryToComplex(CultureInfo.InvariantCulture, out z);
            Assert.IsFalse(ret);
            Assert.AreEqual(0, z.Real);
            Assert.AreEqual(0, z.Imaginary);
        }
    }
}
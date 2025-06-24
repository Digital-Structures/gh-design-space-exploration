﻿// <copyright file="ParsevalTheoremTest.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.IntegralTransformsTests
{
    using System.Linq;
    using Distributions;
    using IntegralTransforms;
    using IntegralTransforms.Algorithms;
    using MbUnit.Framework;
    using Sampling;
    using Statistics;
    using System.Numerics;

    [TestFixture]
    public class ParsevalTheoremTest
    {
        private IContinuousDistribution _uniform = new ContinuousUniform(-1, 1);

        [Test]
        [Row(0x1000)]
        [Row(0x7FF)]
        public void FourierDefaultTransformSatisfiesParsevalsTheorem(int count)
        {
            var samples = Sample.Random((u, v) => new Complex(u, v), _uniform, count);

            var timeSpaceEnergy = (from s in samples select s.MagnitudeSquared()).Mean();

            var work = new Complex[samples.Length];
            samples.CopyTo(work, 0);

            // Default -> Symmetric Scaling
            Transform.FourierForward(work);

            var frequencySpaceEnergy = (from s in work select s.MagnitudeSquared()).Mean();

            Assert.AreApproximatelyEqual(timeSpaceEnergy, frequencySpaceEnergy, 1e-12);
        }

        [Test]
        [Row(0x40)]
        [Row(0x1F)]
        public void HartleyDefaultNaiveSatisfiesParsevalsTheorem(int count)
        {
            var samples = Sample.Random(x => x, _uniform, count);

            var timeSpaceEnergy = (from s in samples select s * s).Mean();

            var work = new double[samples.Length];
            samples.CopyTo(work, 0);

            // Default -> Symmetric Scaling
            var dht = new DiscreteHartleyTransform();
            work = dht.NaiveForward(work, HartleyOptions.Default);

            var frequencySpaceEnergy = (from s in work select s * s).Mean();

            Assert.AreApproximatelyEqual(timeSpaceEnergy, frequencySpaceEnergy, 1e-12);
        }
    }
}
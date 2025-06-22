﻿// <copyright file="MetropolisSamplerTests.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.StatisticsTests.McmcTests
{
    using System;
    using Numerics.Random;
    using Distributions;
    using Statistics.Mcmc;
    using MbUnit.Framework;

    [TestFixture]
    public class MetropolisSamplerTests
    {
        [Test]
        public void MetropolisConstructor()
        {
            var normal = new Normal(0.0, 1.0);
            var rnd = new MersenneTwister();

            var ms = new MetropolisSampler<double>(0.2, normal.Density, x => Normal.Sample(rnd, x, 0.1), 10);
            Assert.IsNotNull(ms.RandomSource);

            ms.RandomSource = rnd;
            Assert.IsNotNull(ms.RandomSource);
        }

        [Test]
        public void SampleTest()
        {
            var normal = new Normal(0.0, 1.0);
            var rnd = new MersenneTwister();

            var ms = new MetropolisSampler<double>(0.2, normal.Density, x => Normal.Sample(rnd, x, 0.1), 10);
            ms.RandomSource = rnd;

            double sample = ms.Sample();
        }

        [Test]
        public void SampleArrayTest()
        {
            var normal = new Normal(0.0, 1.0);
            var rnd = new MersenneTwister();

            var ms = new MetropolisSampler<double>(0.2, normal.Density, x => Normal.Sample(rnd, x, 0.1), 10);
            ms.RandomSource = rnd;

            double[] sample = ms.Sample(5);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullRandomNumberGenerator()
        {
            var normal = new Normal(0.0, 1.0);
            var ms = new MetropolisSampler<double>(0.2, normal.Density, x => Normal.Sample(new System.Random(), x, 0.1), 10);
            ms.RandomSource = null;
        }
    }
}

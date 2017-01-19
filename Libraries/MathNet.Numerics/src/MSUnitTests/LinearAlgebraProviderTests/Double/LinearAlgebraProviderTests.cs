﻿// <copyright file="LinearAlgebraProviderTests.cs" company="Math.NET">
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
namespace MathNet.Numerics.UnitTests.LinearAlgebraProviderTests.Double
{
    using System;
    using System.Collections.Generic;
    using Algorithms.LinearAlgebra;
    using LinearAlgebra.Double;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class for linear algebra provider tests.
    /// </summary>
    [TestClass]
    public class LinearAlgebraProviderTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearAlgebraProviderTests"/> class. 
        /// </summary>
        public LinearAlgebraProviderTests()
        {
            Provider = new ManagedLinearAlgebraProvider();
        }

        /// <summary>
        /// Gets or sets linear algebra provider to test.
        /// </summary>
        protected ILinearAlgebraProvider Provider
        {
            get;
            set;
        }

        /// <summary>
        /// The Y double test vector.
        /// </summary>
        private readonly double[] _y = new[] { 1.1, 2.2, 3.3, 4.4, 5.5 };

        /// <summary>
        /// The X double test vector.
        /// </summary>
        private readonly double[] _x = new[] { 6.6, 7.7, 8.8, 9.9, 10.1 };

        /// <summary>
        /// Test matrix to use.
        /// </summary>
        private readonly IDictionary<string, DenseMatrix> _matrices = new Dictionary<string, DenseMatrix>
                                                          {
                                                              { "Singular3x3", new DenseMatrix(new[,] { { 1.0, 1.0, 2.0 }, { 1.0, 1.0, 2.0 }, { 1.0, 1.0, 2.0 } }) },
                                                              { "Square3x3",  new DenseMatrix(new[,] { { -1.1, -2.2, -3.3 }, { 0.0, 1.1, 2.2 }, { -4.4, 5.5, 6.6 } }) },
                                                              { "Square4x4", new DenseMatrix(new[,] { { -1.1, -2.2, -3.3, -4.4 }, { 0.0, 1.1, 2.2, 3.3 }, { 1.0, 2.1, 6.2, 4.3 }, { -4.4, 5.5, 6.6, -7.7 } }) },
                                                              { "Singular4x4", new DenseMatrix(new[,] { { -1.1, -2.2, -3.3, -4.4 }, { -1.1, -2.2, -3.3, -4.4 }, { -1.1, -2.2, -3.3, -4.4 }, { -1.1, -2.2, -3.3, -4.4 } }) },
                                                              { "Tall3x2", new DenseMatrix(new[,] { { -1.1, -2.2 }, { 0.0, 1.1 }, { -4.4, 5.5 } }) },
                                                              { "Wide2x3", new DenseMatrix(new[,] { { -1.1, -2.2, -3.3 }, { 0.0, 1.1, 2.2 } }) }
                                                          };

        /// <summary>
        /// Can add a vector to scaled vector
        /// </summary>
        [TestMethod]
        public void CanAddVectorToScaledVectorDouble()
        {
            var result = new double[_y.Length];
            Array.Copy(_y, result, _y.Length);

            Provider.AddVectorToScaledVector(result, 0, _x);
            for (var i = 0; i < _y.Length; i++)
            {
                Assert.AreEqual(_y[i], result[i]);
            }

            Array.Copy(_y, result, _y.Length);
            Provider.AddVectorToScaledVector(result, 1, _x);
            for (var i = 0; i < _y.Length; i++)
            {
                Assert.AreEqual(_y[i] + _x[i], result[i]);
            }

            Array.Copy(_y, result, _y.Length);
            Provider.AddVectorToScaledVector(result, Math.PI, _x);
            for (var i = 0; i < _y.Length; i++)
            {
                Assert.AreEqual(_y[i] + (Math.PI * _x[i]), result[i]);
            }
        }

        /// <summary>
        /// Can scale an array.
        /// </summary>
        [TestMethod]
        public void CanScaleArray()
        {
            var result = new double[_y.Length];

            Array.Copy(_y, result, _y.Length);
            Provider.ScaleArray(1, result);
            for (var i = 0; i < _y.Length; i++)
            {
                Assert.AreEqual(_y[i], result[i]);
            }

            Array.Copy(_y, result, _y.Length);
            Provider.ScaleArray(Math.PI, result);
            for (var i = 0; i < _y.Length; i++)
            {
                Assert.AreEqual(_y[i] * Math.PI, result[i]);
            }
        }

        /// <summary>
        /// Can compute the dot product.
        /// </summary>
        [TestMethod]
        public void CanComputeDotProduct()
        {
            var result = Provider.DotProduct(_x, _y);
            AssertHelpers.AlmostEqual(152.35, result, 15);
        }

        /// <summary>
        /// Can add two arrays.
        /// </summary>
        [TestMethod]
        public void CanAddArrays()
        {
            var result = new double[_y.Length];
            Provider.AddArrays(_x, _y, result);
            for (var i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(_x[i] + _y[i], result[i]);
            }
        }

        /// <summary>
        /// Can subtract two arrays.
        /// </summary>
        [TestMethod]
        public void CanSubtractArrays()
        {
            var result = new double[_y.Length];
            Provider.SubtractArrays(_x, _y, result);
            for (var i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(_x[i] - _y[i], result[i]);
            }
        }

        /// <summary>
        /// Can pointwise multiply two arrays.
        /// </summary>
        [TestMethod]
        public void CanPointWiseMultiplyArrays()
        {
            var result = new double[_y.Length];
            Provider.PointWiseMultiplyArrays(_x, _y, result);
            for (var i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(_x[i] * _y[i], result[i]);
            }
        }

        /// <summary>
        /// Can compute L1 norm.
        /// </summary>
        [TestMethod]
        public void CanComputeMatrixL1Norm()
        {
            var matrix = _matrices["Square3x3"];
            var work = new double[matrix.RowCount];
            var norm = Provider.MatrixNorm(Norm.OneNorm, matrix.RowCount, matrix.ColumnCount, matrix.Data, work);
            AssertHelpers.AlmostEqual(12.1, norm, 6);
        }

        /// <summary>
        /// Can compute Frobenius norm.
        /// </summary>
        [TestMethod]
        public void CanComputeMatrixFrobeniusNorm()
        {
            var matrix = _matrices["Square3x3"];
            var work = new double[matrix.RowCount];
            var norm = Provider.MatrixNorm(Norm.FrobeniusNorm, matrix.RowCount, matrix.ColumnCount, matrix.Data, work);
            AssertHelpers.AlmostEqual(10.777754868246, norm, 8);
        }

        /// <summary>
        /// Can compute Infinity norm.
        /// </summary>
        [TestMethod]
        public void CanComputeMatrixInfinityNorm()
        {
            var matrix = _matrices["Square3x3"];
            var work = new double[matrix.RowCount];
            var norm = Provider.MatrixNorm(Norm.InfinityNorm, matrix.RowCount, matrix.ColumnCount, matrix.Data, work);
            Assert.AreEqual(16.5, norm);
        }

        /// <summary>
        /// Can compute L1 norm using a work array.
        /// </summary>
        [TestMethod]
        public void CanComputeMatrixL1NormWithWorkArray()
        {
            var matrix = _matrices["Square3x3"];
            var norm = Provider.MatrixNorm(Norm.OneNorm, matrix.RowCount, matrix.ColumnCount, matrix.Data);
            AssertHelpers.AlmostEqual(12.1, norm, 6);
        }

        /// <summary>
        /// Can compute Frobenius norm using a work array.
        /// </summary>
        [TestMethod]
        public void CanComputeMatrixFrobeniusNormWithWorkArray()
        {
            var matrix = _matrices["Square3x3"];
            var norm = Provider.MatrixNorm(Norm.FrobeniusNorm, matrix.RowCount, matrix.ColumnCount, matrix.Data);
            AssertHelpers.AlmostEqual(10.777754868246, norm, 8);
        }

        /// <summary>
        /// Can compute Infinity norm using a work array.
        /// </summary>
        [TestMethod]
        public void CanComputeMatrixInfinityNormWithWorkArray()
        {
            var matrix = _matrices["Square3x3"];
            var norm = Provider.MatrixNorm(Norm.InfinityNorm, matrix.RowCount, matrix.ColumnCount, matrix.Data);
            Assert.AreEqual(16.5, norm);
        }

        /// <summary>
        /// Can multiply two square matrices.
        /// </summary>
        [TestMethod]
        public void CanMultiplySquareMatrices()
        {
            var x = _matrices["Singular3x3"];
            var y = _matrices["Square3x3"];
            var c = new DenseMatrix(x.RowCount, y.ColumnCount);

            Provider.MatrixMultiply(x.Data, x.RowCount, x.ColumnCount, y.Data, y.RowCount, y.ColumnCount, c.Data);

            for (var i = 0; i < c.RowCount; i++)
            {
                for (var j = 0; j < c.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(x.Row(i) * y.Column(j), c[i, j], 15);
                }
            }
        }

        /// <summary>
        /// Can multiply a wide and tall matrix.
        /// </summary>
        [TestMethod]
        public void CanMultiplyWideAndTallMatrices()
        {
            var x = _matrices["Wide2x3"];
            var y = _matrices["Tall3x2"];
            var c = new DenseMatrix(x.RowCount, y.ColumnCount);

            Provider.MatrixMultiply(x.Data, x.RowCount, x.ColumnCount, y.Data, y.RowCount, y.ColumnCount, c.Data);

            for (var i = 0; i < c.RowCount; i++)
            {
                for (var j = 0; j < c.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(x.Row(i) * y.Column(j), c[i, j], 15);
                }
            }
        }

        /// <summary>
        /// Can multiply a tall and wide matrix.
        /// </summary>
        [TestMethod]
        public void CanMultiplyTallAndWideMatrices()
        {
            var x = _matrices["Tall3x2"];
            var y = _matrices["Wide2x3"];
            var c = new DenseMatrix(x.RowCount, y.ColumnCount);

            Provider.MatrixMultiply(x.Data, x.RowCount, x.ColumnCount, y.Data, y.RowCount, y.ColumnCount, c.Data);

            for (var i = 0; i < c.RowCount; i++)
            {
                for (var j = 0; j < c.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(x.Row(i) * y.Column(j), c[i, j], 15);
                }
            }
        }

        /// <summary>
        /// Can multiply two square matrices.
        /// </summary>
        [TestMethod]
        public void CanMultiplySquareMatricesWithUpdate()
        {
            var x = _matrices["Singular3x3"];
            var y = _matrices["Square3x3"];
            var c = new DenseMatrix(x.RowCount, y.ColumnCount);

            Provider.MatrixMultiplyWithUpdate(Transpose.DontTranspose, Transpose.DontTranspose, 2.2, x.Data, x.RowCount, x.ColumnCount, y.Data, y.RowCount, y.ColumnCount, 1.0, c.Data);

            for (var i = 0; i < c.RowCount; i++)
            {
                for (var j = 0; j < c.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(2.2 * x.Row(i) * y.Column(j), c[i, j], 15);
                }
            }
        }

        /// <summary>
        /// Can multiply a wide and tall matrix.
        /// </summary>
        [TestMethod]
        public void CanMultiplyWideAndTallMatricesWithUpdate()
        {
            var x = _matrices["Wide2x3"];
            var y = _matrices["Tall3x2"];
            var c = new DenseMatrix(x.RowCount, y.ColumnCount);

            Provider.MatrixMultiplyWithUpdate(Transpose.DontTranspose, Transpose.DontTranspose, 2.2, x.Data, x.RowCount, x.ColumnCount, y.Data, y.RowCount, y.ColumnCount, 1.0, c.Data);

            for (var i = 0; i < c.RowCount; i++)
            {
                for (var j = 0; j < c.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(2.2 * x.Row(i) * y.Column(j), c[i, j], 15);
                }
            }
        }

        /// <summary>
        /// Can multiply a tall and wide matrix.
        /// </summary>
        [TestMethod]
        public void CanMultiplyTallAndWideMatricesWithUpdate()
        {
            var x = _matrices["Tall3x2"];
            var y = _matrices["Wide2x3"];
            var c = new DenseMatrix(x.RowCount, y.ColumnCount);

            Provider.MatrixMultiplyWithUpdate(Transpose.DontTranspose, Transpose.DontTranspose, 2.2, x.Data, x.RowCount, x.ColumnCount, y.Data, y.RowCount, y.ColumnCount, 1.0, c.Data);

            for (var i = 0; i < c.RowCount; i++)
            {
                for (var j = 0; j < c.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(2.2 * x.Row(i) * y.Column(j), c[i, j], 15);
                }
            }
        }

        /// <summary>
        /// Can compute the Cholesky factorization.
        /// </summary>
        [TestMethod]
        public void CanComputeCholeskyFactor()
        {
            var matrix = new double[] { 1, 1, 1, 1, 1, 5, 5, 5, 1, 5, 14, 14, 1, 5, 14, 15 };
            Provider.CholeskyFactor(matrix, 4);
            Assert.AreEqual(matrix[0], 1);
            Assert.AreEqual(matrix[1], 1);
            Assert.AreEqual(matrix[2], 1);
            Assert.AreEqual(matrix[3], 1);
            Assert.AreEqual(matrix[4], 0);
            Assert.AreEqual(matrix[5], 2);
            Assert.AreEqual(matrix[6], 2);
            Assert.AreEqual(matrix[7], 2);
            Assert.AreEqual(matrix[8], 0);
            Assert.AreEqual(matrix[9], 0);
            Assert.AreEqual(matrix[10], 3);
            Assert.AreEqual(matrix[11], 3);
            Assert.AreEqual(matrix[12], 0);
            Assert.AreEqual(matrix[13], 0);
            Assert.AreEqual(matrix[14], 0);
            Assert.AreEqual(matrix[15], 1);
        }
    }
}
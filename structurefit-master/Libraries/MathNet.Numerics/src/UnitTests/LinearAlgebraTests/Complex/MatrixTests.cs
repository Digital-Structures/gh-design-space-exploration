﻿// <copyright file="MatrixTests.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
// Copyright (c) 2009-2010 Math.NET
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Complex
{
    using System;
    using System.Numerics;
    using LinearAlgebra.Complex;
    using LinearAlgebra.Generic;
    using MbUnit.Framework;

    [TestFixture]
    public abstract partial class MatrixTests : MatrixLoader
    {
        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanCloneMatrix(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            var clone = matrix.Clone();

            Assert.AreNotSame(matrix, clone);
            Assert.AreEqual(matrix.RowCount, clone.RowCount);
            Assert.AreEqual(matrix.ColumnCount, clone.ColumnCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], clone[i, j]);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanCloneMatrixUsingICloneable(string name)
        {
            var matrix = TestMatrices[name];
            var clone = (Matrix<Complex>)((ICloneable)matrix).Clone();

            Assert.AreNotSame(matrix, clone);
            Assert.AreEqual(matrix.RowCount, clone.RowCount);
            Assert.AreEqual(matrix.ColumnCount, clone.ColumnCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], clone[i, j]);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanCopyTo(string name)
        {
            var matrix = TestMatrices[name];
            var copy = CreateMatrix(matrix.RowCount, matrix.ColumnCount);
            matrix.CopyTo(copy);

            Assert.AreNotSame(matrix, copy);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], copy[i, j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void CopyToFailsWhenTargetIsNull()
        {
            var matrix = TestMatrices["Singular3x3"];
            Matrix<Complex> target = null;
            matrix.CopyTo(target);
        }

        [Test]
        [ExpectedArgumentException]
        public void CopyToFailsWhenTargetHasMoreRows()
        {
            var matrix = TestMatrices["Singular3x3"];
            var target = CreateMatrix(matrix.RowCount + 1, matrix.ColumnCount);
            matrix.CopyTo(target);
        }

        [Test]
        [ExpectedArgumentException]
        public void CopyToFailsWhenTargetHasMoreColumns()
        {
            var matrix = TestMatrices["Singular3x3"];
            var target = CreateMatrix(matrix.RowCount + 1, matrix.ColumnCount);
            matrix.CopyTo(target);
        }

        [Test]
        [Ignore]
        public void CanConvertVectorToString()
        {
        }

        [Test]
        public void CanCreateMatrix()
        {
            var expected = CreateMatrix(5, 6);
            var actual = expected.CreateMatrix(5, 6);
            Assert.AreEqual(expected.GetType(), actual.GetType(), "Matrices are same type.");
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanEquateMatrices(string name)
        {
            var matrix1 = CreateMatrix(TestData2D[name]);
            var matrix2 = CreateMatrix(TestData2D[name]);
            var matrix3 = CreateMatrix(TestData2D[name].GetLength(0), TestData2D[name].GetLength(1));
            Assert.IsTrue(matrix1.Equals(matrix1));
            Assert.IsTrue(matrix1.Equals(matrix2));
            Assert.IsFalse(matrix1.Equals(matrix3));
            Assert.IsFalse(matrix1.Equals(null));
        }

        [Test]
        [Row(0, 2)]
        [Row(2, 0)]
        [Row(0, 0)]
        [Row(-1, 1)]
        [Row(1, -1)]
        [ExpectedArgumentOutOfRangeException]
        public void ThrowsArgumentExceptionIfSizeIsNotPositive(int rows, int columns)
        {
            CreateMatrix(rows, columns);
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        public void TestingForEqualityWithNonMatrixReturnsFalse(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            Assert.IsFalse(matrix.Equals(2));
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        public void CanTestForEqualityUsingObjectEquals(string name)
        {
            var matrix1 = CreateMatrix(TestData2D[name]);
            var matrix2 = CreateMatrix(TestData2D[name]);
            Assert.IsTrue(matrix1.Equals((object)matrix2));
        }

        [Test]
        [Row(-1, 1, "Singular3x3")]
        [Row(1, -1, "Singular3x3")]
        [Row(4, 2, "Square3x3")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RangeCheckFails(int i, int j, string name)
        {
            var x = TestMatrices[name][i, j];
        }

        [Test]
        [Ignore]
        public void MatrixGetHashCode()
        {
        }

        [Test]
        public void CanClearMatrix()
        {
            var matrix = TestMatrices["Singular3x3"].Clone();
            matrix.Clear();
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(0, matrix[i, j]);
                }
            }
        }

        [Test]
        [Row(0, "Singular3x3")]
        [Row(1, "Singular3x3")]
        [Row(2, "Singular3x3")]
        [Row(2, "Square3x3")]
        public void CanGetRow(int rowIndex, string name)
        {
            var matrix = TestMatrices[name];
            var row = matrix.Row(rowIndex);

            Assert.AreEqual(matrix.ColumnCount, row.Count);
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                AssertHelpers.AreEqual(matrix[rowIndex, j], row[j]);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetRowThrowsArgumentOutOfRangeWithNegativeIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            matrix.Row(-1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetRowThrowsArgumentOutOfRangeWithOverflowingRowIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            matrix.Row(matrix.RowCount);
        }

        [Test]
        [Row(0, "Singular3x3")]
        [Row(1, "Singular3x3")]
        [Row(2, "Singular3x3")]
        [Row(2, "Square3x3")]
        public void CanGetRowWithResult(int rowIndex, string name)
        {
            var matrix = TestMatrices[name];
            var row = CreateVector(matrix.ColumnCount);
            matrix.Row(rowIndex, row);

            Assert.AreEqual(matrix.ColumnCount, row.Count);
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                AssertHelpers.AreEqual(matrix[rowIndex, j], row[j]);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetRowWithResultFailsWhenResultIsNull()
        {
            var matrix = TestMatrices["Singular3x3"];
            matrix.Row(0, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetRowWithResultThrowsArgumentOutOfRangeWithNegativeIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            var row = CreateVector(matrix.ColumnCount);
            matrix.Row(-1, row);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetRowWithResultThrowsArgumentOutOfRangeWithOverflowingRowIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            var row = CreateVector(matrix.ColumnCount);
            matrix.Row(matrix.RowCount, row);
        }

        [Test]
        [Row(0, 0, 1, "Singular3x3")]
        [Row(1, 1, 2, "Singular3x3")]
        [Row(2, 0, 3, "Singular3x3")]
        [Row(2, 0, 3, "Square3x3")]
        public void CanGetRowWithRange(int rowIndex, int start, int length, string name)
        {
            var matrix = TestMatrices[name];
            var row = matrix.Row(rowIndex, start, length);

            Assert.AreEqual(length, row.Count);
            for (var j = start; j < start + length; j++)
            {
                AssertHelpers.AreEqual(matrix[rowIndex, j], row[j - start]);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRowWithRangeResultArgumentExeptionWhenLengthIsZero()
        {
            var matrix = TestMatrices["Singular3x3"];
            var result = CreateVector(matrix.ColumnCount);
            matrix.Row(0, 0, 0, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRowWithRangeFailsWithTooSmallResultVector()
        {
            var matrix = TestMatrices["Singular3x3"];
            var result = CreateVector(matrix.ColumnCount - 1);
            matrix.Row(0, 0, 0, result);
        }

        [Test]
        [Row(0, "Singular3x3")]
        [Row(1, "Singular3x3")]
        [Row(2, "Singular3x3")]
        [Row(2, "Square3x3")]
        public void CanGetColumn(int colIndex, string name)
        {
            var matrix = TestMatrices[name];
            var col = matrix.Column(colIndex);

            Assert.AreEqual(matrix.RowCount, col.Count);
            for (var j = 0; j < matrix.RowCount; j++)
            {
                AssertHelpers.AreEqual(matrix[j, colIndex], col[j]);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetColumnThrowsArgumentOutOfRangeWithNegativeIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            matrix.Column(-1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetColumnThrowsArgumentOutOfRangeWithOverflowingRowIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            matrix.Column(matrix.ColumnCount);
        }

        [Test]
        [Row(0, "Singular3x3")]
        [Row(1, "Singular3x3")]
        [Row(2, "Singular3x3")]
        [Row(2, "Square3x3")]
        public void CanGetColumnWithResult(int colIndex, string name)
        {
            var matrix = TestMatrices[name];
            var col = CreateVector(matrix.RowCount);
            matrix.Column(colIndex, col);

            Assert.AreEqual(matrix.RowCount, col.Count);
            for (var j = 0; j < matrix.RowCount; j++)
            {
                AssertHelpers.AreEqual(matrix[j, colIndex], col[j]);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetColumnFailsWhenResultIsNull()
        {
            var matrix = TestMatrices["Singular3x3"];
            matrix.Column(0, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetColumnWithResultThrowsArgumentOutOfRangeWithNegativeIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            var column = CreateVector(matrix.ColumnCount);
            matrix.Column(-1, column);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetColumnWithResultThrowsArgumentOutOfRangeWithOverflowingRowIndex()
        {
            var matrix = TestMatrices["Singular3x3"];
            var column = CreateVector(matrix.RowCount);
            matrix.Row(matrix.ColumnCount, column);
        }

        [Test]
        [Row(0, 0, 1, "Singular3x3")]
        [Row(1, 1, 2, "Singular3x3")]
        [Row(2, 0, 3, "Singular3x3")]
        [Row(2, 0, 3, "Square3x3")]
        public void CanGetColumnWithRange(int colIndex, int start, int length, string name)
        {
            var matrix = TestMatrices[name];
            var col = matrix.Column(colIndex, start, length);

            Assert.AreEqual(length, col.Count);
            for (var j = start; j < start + length; j++)
            {
                AssertHelpers.AreEqual(matrix[j, colIndex], col[j - start]);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetColumnWithRangeResultArgumentExeptionWhenLengthIsZero()
        {
            var matrix = TestMatrices["Singular3x3"];
            var col = CreateVector(matrix.RowCount);
            matrix.Column(0, 0, 0, col);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetColumnWithRangeFailsWithTooSmallResultVector()
        {
            var matrix = TestMatrices["Singular3x3"];
            var result = CreateVector(matrix.RowCount - 1);
            matrix.Column(0, 0, matrix.RowCount, result);
        }

        [Test]
        [Row(0, "Singular3x3")]
        [Row(1, "Singular3x3")]
        [Row(2, "Singular3x3")]
        [Row(2, "Square3x3")]
        public void CanSetRow(int rowIndex, string name)
        {
            var matrix = TestMatrices[name].Clone();
            matrix.SetRow(rowIndex, CreateVector(matrix.ColumnCount));

            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(i == rowIndex ? 0.0 : TestMatrices[name][i, j], matrix[i, j]);
                }
            }
        }

        [Test]
        [Row(0, "Singular3x3")]
        [Row(1, "Singular3x3")]
        [Row(2, "Singular3x3")]
        [Row(2, "Square3x3")]
        public void CanSetColumn(int colIndex, string name)
        {
            var matrix = TestMatrices[name].Clone();
            matrix.SetColumn(colIndex, CreateVector(matrix.ColumnCount));

            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(j == colIndex ? 0.0 : TestMatrices[name][i, j], matrix[i, j]);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void UpperTriangle(string name)
        {
            var data = TestMatrices[name];
            var upper = data.UpperTriangle();
            for (var i = 0; i < data.RowCount; i++)
            {
                for (var j = 0; j < data.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(i <= j ? data[i, j] : 0, upper[i, j]);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void UpperTriangleResult(string name)
        {
            var data = TestMatrices[name];
            var upper = CreateMatrix(data.RowCount, data.ColumnCount);
            data.UpperTriangle(upper);
            for (var i = 0; i < data.RowCount; i++)
            {
                for (var j = 0; j < data.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(i <= j ? data[i, j] : 0, upper[i, j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void UpperTriangleWithResultNullShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            Matrix<Complex> result = null;
            data.UpperTriangle(result);
        }

        [Test]
        [ExpectedArgumentException]
        public void UpperTriangleWithUnEqualRowsShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            var result = CreateMatrix(data.RowCount + 1, data.ColumnCount);
            data.UpperTriangle(result);
        }

        [Test]
        [ExpectedArgumentException]
        public void UpperTriangleWithUnEqualColumnsShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            var result = CreateMatrix(data.RowCount, data.ColumnCount + 1);
            data.UpperTriangle(result);
        }

        [Test]
        public void StrictlyLowerTriangle()
        {
            foreach (var data in TestMatrices.Values)
            {
                var lower = data.StrictlyLowerTriangle();
                for (var i = 0; i < data.RowCount; i++)
                {
                    for (var j = 0; j < data.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(i > j ? data[i, j] : 0, lower[i, j]);
                    }
                }
            }
        }

        [Test]
        public void StrictlyLowerTriangleResult()
        {
            foreach (var data in TestMatrices.Values)
            {
                var lower = CreateMatrix(data.RowCount, data.ColumnCount);
                data.StrictlyLowerTriangle(lower);
                for (var i = 0; i < data.RowCount; i++)
                {
                    for (var j = 0; j < data.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(i > j ? data[i, j] : 0, lower[i, j]);
                    }
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StrictlyLowerTriangleWithNullParameterShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            Matrix<Complex> lower = null;
            data.StrictlyLowerTriangle(lower);
        }

        [Test]
        [ExpectedArgumentException]
        public void StrictlyLowerTriangleWithInvalidColumnNumberShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            var lower = CreateMatrix(data.RowCount, data.ColumnCount + 1);
            data.StrictlyLowerTriangle(lower);
        }

        [Test]
        [ExpectedArgumentException]
        public void StrictlyLowerTriangleWithInvalidRowNumberShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            var lower = CreateMatrix(data.RowCount + 1, data.ColumnCount);
            data.StrictlyLowerTriangle(lower);
        }


        [Test]
        public void StrictlyUpperTriangle()
        {
            foreach (var data in TestMatrices.Values)
            {
                var upper = data.StrictlyUpperTriangle();
                for (var i = 0; i < data.RowCount; i++)
                {
                    for (var j = 0; j < data.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(i < j ? data[i, j] : 0, upper[i, j]);
                    }
                }
            }
        }

        [Test]
        public void StrictlyUpperTriangleResult()
        {
            foreach (var data in TestMatrices.Values)
            {
                var upper = CreateMatrix(data.RowCount, data.ColumnCount);
                data.StrictlyUpperTriangle(upper);
                for (var i = 0; i < data.RowCount; i++)
                {
                    for (var j = 0; j < data.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(i < j ? data[i, j] : 0, upper[i, j]);
                    }
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StrictlyUpperTriangleWithNullParameterShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            Matrix<Complex> lower = null;
            data.StrictlyUpperTriangle(lower);
        }

        [Test]
        [ExpectedArgumentException]
        public void StrictlyUpperTriangleWithInvalidColumnNumberShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            var lower = CreateMatrix(data.RowCount, data.ColumnCount + 1);
            data.StrictlyUpperTriangle(lower);
        }

        [Test]
        [ExpectedArgumentException]
        public void StrictlyUpperTriangleWithInvalidRowNumberShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            var lower = CreateMatrix(data.RowCount + 1, data.ColumnCount);
            data.StrictlyUpperTriangle(lower);
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanTransposeMatrix(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            var transpose = matrix.Transpose();

            Assert.AreNotSame(matrix, transpose);
            Assert.AreEqual(matrix.RowCount, transpose.ColumnCount);
            Assert.AreEqual(matrix.ColumnCount, transpose.RowCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], transpose[j, i]);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanConjugateTransposeMatrix(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            var transpose = matrix.ConjugateTranspose();

            Assert.AreNotSame(matrix, transpose);
            Assert.AreEqual(matrix.RowCount, transpose.ColumnCount);
            Assert.AreEqual(matrix.ColumnCount, transpose.RowCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], transpose[j, i].Conjugate());
                }
            }
        }

        [Test]
        [Row("Singular3x3", new double[] { 1, 2, 3 })]
        [Row("Square3x3", new double[] { 1, 2, 3 })]
        [Row("Tall3x2", new double[] { 1, 2, 3 })]
        [Row("Wide2x3", new double[] { 1, 2 })]
        [Row("Singular3x3", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("Singular3x3", new double[] { 1, 2, 3, 4, 5 }, ExpectedException = typeof(ArgumentException))]
        public virtual void SetColumnWithArray(string name, double[] real)
        {
            Complex[] column = null;
            if (real != null)
            {
                column = new Complex[real.Length];
                for (int i = 0; i < real.Length; i++)
                {
                    column[i] = new Complex(real[i], 1);
                }
            }

            var matrix = TestMatrices[name];
            for (var i = 0; i < matrix.ColumnCount; i++)
            {
                matrix.SetColumn(i, column);
                for (var j = 0; j < matrix.RowCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[j, i], column[j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetColumnArrayWithInvalidColumnIndexShouldThrowException()
        {
            var matrix = TestMatrices["Square3x3"];
            Complex[] column = { 1, 2, 3 };
            matrix.SetColumn(-1, column);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetColumnArrayWithInvalidColumnIndexShouldThrowException2()
        {
            var matrix = TestMatrices["Square3x3"];
            Complex[] column = { 1, 2, 3 };
            matrix.SetColumn(matrix.ColumnCount + 1, column);
        }

        [Test]
        [Row("Singular3x3", new double[] { 1, 2, 3 })]
        [Row("Square3x3", new double[] { 1, 2, 3 })]
        [Row("Tall3x2", new double[] { 1, 2, 3 })]
        [Row("Wide2x3", new double[] { 1, 2 })]
        [Row("Singular3x3", new double[] { 1, 2, 3, 4, 5 }, ExpectedException = typeof(ArgumentException))]
        public virtual void SetColumnWithVector(string name, double[] real)
        {
            var column = new Complex[real.Length];
            for (int i = 0; i < real.Length; i++)
            {
                column[i] = new Complex(real[i], 1);
            }

            var matrix = TestMatrices[name];
            var columnVector = CreateVector(column);
            for (var i = 0; i < matrix.ColumnCount; i++)
            {
                matrix.SetColumn(i, column);
                for (var j = 0; j < matrix.RowCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[j, i], columnVector[j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetColumnWithNullVectorShouldThrowException()
        {
            var matrix = TestMatrices["Square3x3"];
            Vector<Complex> columnVector = null;
            matrix.SetColumn(1, columnVector);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetColumnVectorWithInvalidColumnIndexShouldThrowException()
        {
            var matrix = TestMatrices["Square3x3"];
            var column = CreateVector(new Complex[] { 1, 2, 3 });
            matrix.SetColumn(-1, column);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetColumnVectorWithInvalidColumnIndexShouldThrowException2()
        {
            var matrix = TestMatrices["Square3x3"];
            var column = CreateVector(new Complex[] { 1, 2, 3 });
            matrix.SetColumn(matrix.ColumnCount + 1, column);
        }

        [Test]
        public void InsertColumn()
        {
            var matrix = CreateMatrix(3, 3);
            var column = CreateVector(matrix.RowCount);
            for (var i = 0; i < column.Count; i++)
            {
                column[i] = i;
            }

            for (var k = 0; k < matrix.ColumnCount + 1; k++)
            {
                var result = matrix.InsertColumn(k, column);
                Assert.AreEqual(result.ColumnCount, matrix.ColumnCount + 1);
                for (var col = 0; col < result.ColumnCount; col++)
                {
                    for (var row = 0; row < result.RowCount; row++)
                    {
                        AssertHelpers.AreEqual(col == k ? row : 0, result[row, col]);
                    }
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertNullColumnShouldThrowExecption()
        {
            var matrix = TestMatrices["Square3x3"];
            matrix.InsertColumn(0, null);
        }

        [Test]
        [Row(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(5, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void InsertColumnWithInvalidColumnIndexShouldThrowExceptiopn(int columnIndex)
        {
            var matrix = CreateMatrix(3, 3);
            var column = CreateVector(matrix.RowCount);
            matrix.InsertColumn(columnIndex, column);
        }

        public void InsertColumnWithInvalidNumberOfElementsShouldThrowException()
        {
            var matrix = CreateMatrix(3, 3);
            var column = CreateVector(matrix.RowCount + 1);
            matrix.InsertColumn(0, column);
        }

        [Test]
        [Row("Singular3x3", new double[] { 1, 2, 3 })]
        [Row("Square3x3", new double[] { 1, 2, 3 })]
        [Row("Tall3x2", new double[] { 1, 2 })]
        [Row("Wide2x3", new double[] { 1, 2, 3 })]
        [Row("Singular3x3", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("Singular3x3", new double[] { 1, 2, 3, 4, 5 }, ExpectedException = typeof(ArgumentException))]
        public virtual void SetRowWithArray(string name, double[] real)
        {
            Complex[] row = null;
            if (real != null)
            {
                row = new Complex[real.Length];
                for (var i = 0; i < real.Length; i++)
                {
                    row[i] = new Complex(real[i], 1);
                }
            }

            var matrix = TestMatrices[name];
            for (var i = 0; i < matrix.RowCount; i++)
            {
                matrix.SetRow(i, row);
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], row[j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetRowArrayWithInvalidRowIndexShouldThrowException()
        {
            var matrix = TestMatrices["Square3x3"];
            Complex[] row = { 1, 2, 3 };
            matrix.SetRow(-1, row);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetRowArrayWithInvalidRowIndexShouldThrowException2()
        {
            var matrix = TestMatrices["Square3x3"];
            Complex[] row = { 1, 2, 3 };
            matrix.SetRow(matrix.RowCount + 1, row);
        }

        [Test]
        [Row("Singular3x3", new double[] { 1, 2, 3 })]
        [Row("Square3x3", new double[] { 1, 2, 3 })]
        [Row("Tall3x2", new double[] { 1, 2 })]
        [Row("Wide2x3", new double[] { 1, 2, 3 })]
        [Row("Singular3x3", new double[] { 1, 2, 3, 4, 5 }, ExpectedException = typeof(ArgumentException))]
        public virtual void SetRowWithVector(string name, double[] real)
        {
            var row = new Complex[real.Length];
            for (var i = 0; i < real.Length; i++)
            {
                row[i] = new Complex(real[i], 1);
            }

            var matrix = TestMatrices[name];
            var rowVector = CreateVector(row);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                matrix.SetRow(i, row);
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], rowVector[j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetRowWithNullVectorShouldThrowException()
        {
            var matrix = TestMatrices["Square3x3"];
            Vector<Complex> rowVector = null;
            matrix.SetRow(1, rowVector);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetRowVectorWithInvalidRowIndexShouldThrowException()
        {
            var matrix = TestMatrices["Square3x3"];
            var row = CreateVector(new Complex[] { 1, 2, 3 });
            matrix.SetRow(-1, row);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void SetRowVectorWithInvalidRowIndexShouldThrowException2()
        {
            var matrix = TestMatrices["Square3x3"];
            var row = CreateVector(new Complex[] { 1, 2, 3 });
            matrix.SetRow(matrix.RowCount + 1, row);
        }

        [Test]
        [Row(0, 2, 0, 2)]
        [Row(1, 1, 1, 1)]
        [Row(0, 4, 0, 2, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(0, 2, 0, 4, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(4, 2, 0, 2, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(0, 2, 4, 2, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(-1, 2, 0, 2, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(0, 2, -1, 2, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(0, -1, 0, 2, ExpectedException = typeof(ArgumentException))]
        [Row(0, 2, 0, -1, ExpectedException = typeof(ArgumentException))]
        public virtual void SetSubMatrix(int rowStart, int rowLength, int colStart, int colLength)
        {
            foreach (var matrix in TestMatrices.Values)
            {
                var subMatrix = matrix.SubMatrix(0, 2, 0, 2);
                subMatrix[0, 0] = 10.0;
                subMatrix[0, 1] = -1.0;
                subMatrix[1, 0] = 3.0;
                subMatrix[1, 1] = 4.0;
                matrix.SetSubMatrix(rowStart, rowLength, colStart, colLength, subMatrix);

                for (int i = rowStart, ii = 0; i < rowLength; i++, ii++)
                {
                    for (int j = colStart, jj = 0; j < colLength; j++, jj++)
                    {
                        AssertHelpers.AreEqual(matrix[i, j], subMatrix[ii, jj]);
                    }
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetSubMatrixWithNullSubMatrixShouldThrowException()
        {
            var data = TestMatrices["Square3x3"];
            Matrix<Complex> subMatrix = null;
            data.SetSubMatrix(0, 2, 0, 2, subMatrix);
        }

        [Test]
        [Row("Square3x3", new double[] { 1, 2, 3 })]
        [Row("Wide2x3", new double[] { 1, 2 })]
        [Row("Wide2x3", new double[] { 1, 2, 3 }, ExpectedException = typeof(ArgumentException))]
        [Row("Tall3x2", new double[] { 1, 2 })]
        public void SetDiagonalVector(string name, double[] real)
        {
            var diagonal = new Complex[real.Length];
            for (var i = 0; i < real.Length; i++)
            {
                diagonal[i] = new Complex(real[i], 1);
            }


            var matrix = TestMatrices[name];
            var vector = CreateVector(diagonal);
            matrix.SetDiagonal(vector);

            var min = Math.Min(matrix.ColumnCount, matrix.RowCount);
            Assert.AreEqual(diagonal.Length, min);

            for (var i = 0; i < vector.Count; i++)
            {
                AssertHelpers.AreEqual(vector[i], matrix[i, i]);
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetDiagonalWithNullVectorParameterShouldThrowException()
        {
            var matrix = TestMatrices["Square3x3"];
            Vector<Complex> vector = null;
            matrix.SetDiagonal(vector);
        }

        [Test]
        [Row("Square3x3", new double[] { 1, 2, 3 })]
        [Row("Wide2x3", new double[] { 1, 2 })]
        [Row("Wide2x3", new double[] { 1, 2, 3 }, ExpectedException = typeof(ArgumentException))]
        [Row("Tall3x2", new double[] { 1, 2 })]
        [Row("Square3x3", null, ExpectedException = typeof(ArgumentNullException))]
        public void SetDiagonalArray(string name, double[] real)
        {
            Complex[] diagonal = null;
            if (real != null)
            {
                diagonal  = new Complex[real.Length];
                for (var i = 0; i < real.Length; i++)
                {
                    diagonal[i] = new Complex(real[i], 1);
                }
            }

            var matrix = TestMatrices[name];
            matrix.SetDiagonal(diagonal);
            var min = Math.Min(matrix.ColumnCount, matrix.RowCount);
            Assert.AreEqual(diagonal.Length, min);
            for (var i = 0; i < diagonal.Length; i++)
            {
                AssertHelpers.AreEqual(diagonal[i], matrix[i, i]);
            }
        }

        [Test]
        public void InsertRow()
        {
            var matrix = CreateMatrix(3, 3);
            var row = CreateVector(matrix.ColumnCount);
            for (var i = 0; i < row.Count; i++)
            {
                row[i] = i;
            }

            for (var insertedRowIndex = 0; insertedRowIndex < matrix.RowCount + 1; insertedRowIndex++)
            {
                var result = matrix.InsertRow(insertedRowIndex, row);
                Assert.AreEqual(result.RowCount, matrix.ColumnCount + 1);
                for (var i = 0; i < result.RowCount; i++)
                {
                    for (var j = 0; j < result.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(i == insertedRowIndex ? row[j] : 0, result[i, j]);
                    }
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertNullRowShouldThrowExecption()
        {
            var matrix = TestMatrices["Square3x3"];
            matrix.InsertRow(0, null);
        }

        [Test]
        [Row(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(5, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void InsertRowWithInvalidRowIndexShouldThrowExceptiopn(int rowIndex)
        {
            var matrix = CreateMatrix(3, 3);
            var row = CreateVector(matrix.ColumnCount);
            matrix.InsertRow(rowIndex, row);
        }

        public void InsertRowWithInvalidNumberOfElementsShouldThrowException()
        {
            var matrix = CreateMatrix(3, 3);
            var row = CreateVector(matrix.ColumnCount + 1);
            matrix.InsertRow(0, row);
        }

        [Test]
        public void ToArray()
        {
            foreach (var data in TestMatrices.Values)
            {
                var array = data.ToArray();
                Assert.AreEqual(data.RowCount, array.GetLength(0));
                Assert.AreEqual(data.ColumnCount, array.GetLength(1));

                for (var i = 0; i < data.RowCount; i++)
                {
                    for (var j = 0; j < data.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(data[i, j], array[i, j]);
                    }
                }
            }
        }

        [Test]
        public void ToColumnWiseArray()
        {
            foreach (var data in TestMatrices.Values)
            {
                var array = data.ToColumnWiseArray();
                Assert.AreEqual(data.RowCount * data.ColumnCount, array.Length);

                for (var i = 0; i < data.RowCount; i++)
                {
                    for (var j = 0; j < data.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(data[i, j], array[j * data.RowCount + i]);
                    }
                }
            }
        }

        [Test]
        public void ToRowWiseArray()
        {
            foreach (var data in TestMatrices.Values)
            {
                var array = data.ToRowWiseArray();
                Assert.AreEqual(data.RowCount * data.ColumnCount, array.Length);

                for (var i = 0; i < data.RowCount; i++)
                {
                    for (var j = 0; j < data.ColumnCount; j++)
                    {
                        AssertHelpers.AreEqual(data[i, j], array[i * data.ColumnCount + j]);
                    }
                }
            }
        }


        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Tall3x2")]
        [MultipleAsserts]
        public virtual void CanPermuteMatrixRows(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            var matrixp = CreateMatrix(TestData2D[name]);

            var permutation = new Permutation(new[] { 2, 0, 1 });
            matrixp.PermuteRows(permutation);

            Assert.AreNotSame(matrix, matrixp);
            Assert.AreEqual(matrix.RowCount, matrixp.RowCount);
            Assert.AreEqual(matrix.ColumnCount, matrixp.ColumnCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], matrixp[permutation[i], j]);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public virtual void CanPermuteMatrixColumns(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            var matrixp = CreateMatrix(TestData2D[name]);

            var permutation = new Permutation(new[] { 2, 0, 1 });
            matrixp.PermuteColumns(permutation);

            Assert.AreNotSame(matrix, matrixp);
            Assert.AreEqual(matrix.RowCount, matrixp.RowCount);
            Assert.AreEqual(matrix.ColumnCount, matrixp.ColumnCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(matrix[i, j], matrixp[i, permutation[j]]);
                }
            }
        }

        [Test]
        public void CanAppendMatrices()
        {
            var left = CreateMatrix(TestData2D["Singular3x3"]);
            var right = CreateMatrix(TestData2D["Tall3x2"]);
            var result = left.Append(right);
            Assert.AreEqual(left.ColumnCount + right.ColumnCount, result.ColumnCount);
            Assert.AreEqual(left.RowCount, right.RowCount);

            for (var i = 0; i < result.RowCount; i++)
            {
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(j < left.ColumnCount ? left[i, j] : right[i, j - left.ColumnCount], result[i, j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void CanAppendWithRightParameterNullShouldThrowException()
        {
            var left = TestMatrices["Square3x3"];
            Matrix<Complex> right = null;
            left.Append(right);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void CanAppendWithResultParameterNullShouldThrowException()
        {
            var left = TestMatrices["Square3x3"];
            var right = TestMatrices["Tall3x2"];
            Matrix<Complex> result = null;
            left.Append(right, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void AppendingTwoMatricesWithDifferentRowCountShouldThrowException()
        {
            var left = TestMatrices["Square3x3"];
            var right = TestMatrices["Wide2x3"];
            var result = left.Append(right);
        }

        [Test]
        [ExpectedArgumentException]
        public void AppendingWithInvalidResultMatrixColumnsShouldThrowException()
        {
            var left = TestMatrices["Square3x3"];
            var right = TestMatrices["Tall3x2"];
            var result = CreateMatrix(3, 2);
            left.Append(right, result);
        }

        [Test]
        public void CanStackMatrices()
        {
            var top = TestMatrices["Square3x3"];
            var bottom = TestMatrices["Wide2x3"];
            var result = top.Stack(bottom);
            Assert.AreEqual(top.RowCount + bottom.RowCount, result.RowCount);
            Assert.AreEqual(top.ColumnCount, result.ColumnCount);

            for (var i = 0; i < result.RowCount; i++)
            {
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    AssertHelpers.AreEqual(result[i, j], i < top.RowCount ? top[i, j] : bottom[i - top.RowCount, j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StackingWithBottomParameterNullShouldThrowException()
        {
            var top = TestMatrices["Square3x3"];
            Matrix<Complex> bottom = null;
            var result = CreateMatrix(top.RowCount + top.RowCount, top.ColumnCount);
            top.Stack(bottom, result);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StackingWithResultParameterNullShouldThrowException()
        {
            var top = TestMatrices["Square3x3"];
            var bottom = TestMatrices["Square3x3"];
            Matrix<Complex> result = null;
            top.Stack(bottom, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void StackingTwoMatricesWithDifferentColumnsShouldThrowException()
        {
            var top = TestMatrices["Square3x3"];
            var lower = TestMatrices["Tall3x2"];
            var result = CreateMatrix(top.RowCount + lower.RowCount, top.ColumnCount);
            top.Stack(lower, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void StackingWithInvalidResultMatrixRowsShouldThrowException()
        {
            var top = TestMatrices["Square3x3"];
            var bottom = TestMatrices["Wide2x3"];
            var result = CreateMatrix(1, 3);
            top.Stack(bottom, result);
        }

        [Test]
        public void CanDiagonallyStackMatrics()
        {
            var top = TestMatrices["Tall3x2"];
            var bottom = TestMatrices["Wide2x3"];
            var result = top.DiagonalStack(bottom);
            Assert.AreEqual(top.RowCount + bottom.RowCount, result.RowCount);
            Assert.AreEqual(top.ColumnCount + bottom.ColumnCount, result.ColumnCount);

            for (var i = 0; i < result.RowCount; i++)
            {
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    if (i < top.RowCount && j < top.ColumnCount)
                    {
                        AssertHelpers.AreEqual(top[i, j], result[i, j]);
                    }
                    else if (i >= top.RowCount && j >= top.ColumnCount)
                    {
                        AssertHelpers.AreEqual(bottom[i - top.RowCount, j - top.ColumnCount], result[i, j]);
                    }
                    else
                    {
                        AssertHelpers.AreEqual(0, result[i, j]);
                    }
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void DiagonalStackWithLowerNullShouldThrowException()
        {
            var top = TestMatrices["Square3x3"];
            Matrix<Complex> lower = null;
            top.DiagonalStack(lower);
        }

        [Test]
        public virtual void CanDiagonallyStackMatricesWithPassingResult()
        {
            var top = TestMatrices["Tall3x2"];
            var bottom = TestMatrices["Wide2x3"];
            var result = CreateMatrix(top.RowCount + bottom.RowCount, top.ColumnCount + bottom.ColumnCount);
            top.DiagonalStack(bottom, result);
            Assert.AreEqual(top.RowCount + bottom.RowCount, result.RowCount);
            Assert.AreEqual(top.ColumnCount + bottom.ColumnCount, result.ColumnCount);

            for (var i = 0; i < result.RowCount; i++)
            {
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    if (i < top.RowCount && j < top.ColumnCount)
                    {
                        AssertHelpers.AreEqual(top[i, j], result[i, j]);
                    }
                    else if (i >= top.RowCount && j >= top.ColumnCount)
                    {
                        AssertHelpers.AreEqual(bottom[i - top.RowCount, j - top.ColumnCount], result[i, j]);
                    }
                    else
                    {
                        AssertHelpers.AreEqual(0, result[i, j]);
                    }
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void DiagonalStackWithResultNullShouldThrowException()
        {
            var top = TestMatrices["Square3x3"];
            var lower = TestMatrices["Wide2x3"];
            Matrix<Complex> result = null;
            top.DiagonalStack(lower, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void DiagonalStackWithInvalidResultMatrixShouldThrowException()
        {
            var top = TestMatrices["Square3x3"];
            var lower = TestMatrices["Wide2x3"];
            var result = CreateMatrix(top.RowCount + lower.RowCount + 2, top.ColumnCount + lower.ColumnCount);
            top.DiagonalStack(lower, result);
        }

        [Test]
        public virtual void FrobeniusNorm()
        {
            var matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(10.8819655930903, matrix.FrobeniusNorm(), 14);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(5.19052560084774, matrix.FrobeniusNorm(), 14);

            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(7.59041159967795, matrix.FrobeniusNorm(), 14);
        }

        [Test]
        public virtual void InfinityNorm()
        {
            Matrix<Complex> matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(16.7777033201323, matrix.InfinityNorm(), 14);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(7.3514039993641, matrix.InfinityNorm(), 14);

            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(10.1023756128209, matrix.InfinityNorm(), 14);
        }

        [Test]
        public virtual void L1Norm()
        {
            var matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(12.5401248319437, matrix.L1Norm(), 14);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(5.86479712463225, matrix.L1Norm(), 14);

            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(9.49338601320024, matrix.L1Norm(), 14);
        }

        [Test]
        public virtual void L2Norm()
        {
            var matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(10.638175225153, matrix.L2Norm(), 14);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(5.2058554445283, matrix.L2Norm(), 14);

            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(7.35826643761172, matrix.L2Norm(), 14);
        }
    }
}
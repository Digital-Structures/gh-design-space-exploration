﻿// <copyright file="Vector.cs" company="Math.NET">
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

namespace MathNet.Numerics.LinearAlgebra.Complex32
{
    using System;
    using Distributions;
    using Generic;
    using Properties;
    using Threading;
    using Complex32 = Numerics.Complex32;

    /// <summary>
    /// <c>Complex32</c> version of the <see cref="Vector{T}"/> class.
    /// </summary>
    public abstract class Vector : Vector<Complex32>
    {
        /// <summary>
        /// Initializes a new instance of the Vector class. 
        /// Constructs a <strong>Vector</strong> with the given size.
        /// </summary>
        /// <param name="size">
        /// The size of the <strong>Vector</strong> to construct.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="size"/> is less than one.
        /// </exception>
        protected Vector(int size) : base(size)
        {
        }

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to add.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the addition.
        /// </param>
        protected override void DoAdd(Complex32 scalar, Vector<Complex32> result)
        {
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = this[index] + scalar);
        }

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">
        /// The vector to add to this one.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the addition.
        /// </param>
        protected override void DoAdd(Vector<Complex32> other, Vector<Complex32> result)
        {
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = this[index] + other[index]);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to subtract.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the subtraction.
        /// </param>
        protected override void DoSubtract(Complex32 scalar, Vector<Complex32> result)
        {
            DoAdd(-scalar, result);
        }

        /// <summary>
        /// Subtracts another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">
        /// The vector to subtract from this one.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the subtraction.
        /// </param>
        protected override void DoSubtract(Vector<Complex32> other, Vector<Complex32> result)
        {
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = this[index] - other[index]);
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to multiply.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the multiplication.
        /// </param>
        protected override void DoMultiply(Complex32 scalar, Vector<Complex32> result)
        {
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = this[index] * scalar);
        }

        /// <summary>
        /// Divides each element of the vector by a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to divide with.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the division.
        /// </param>
        protected override void DoDivide(Complex32 scalar, Vector<Complex32> result)
        {
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = this[index] / scalar);
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Vector<Complex32> other, Vector<Complex32> result)
        {
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = this[index] * other[index]);
        }

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise divide this one by.</param>
        /// <param name="result">The vector to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Vector<Complex32> other, Vector<Complex32> result)
        {
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = this[index] / other[index]);
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">
        /// The other vector to add.
        /// </param>
        /// <returns>s
        /// The result of the addition.
        /// </returns>
        protected override Complex32 DoDotProduct(Vector<Complex32> other)
        {
            return CommonParallel.Aggregate(
                0, 
                Count, 
                i => this[i] * other[i]);
        }

        /// <summary>
        /// Returns the value of the absolute minimum element.
        /// </summary>
        /// <returns>The value of the absolute minimum element.</returns>
        public override Complex32 AbsoluteMinimum()
        {
            return this[AbsoluteMinimumIndex()].Magnitude;
        }

        /// <summary>
        /// Returns the index of the absolute minimum element.
        /// </summary>
        /// <returns>The index of absolute minimum element.</returns>   
        public override int AbsoluteMinimumIndex()
        {
            var index = 0;
            var min = this[index].Magnitude;
            for (var i = 1; i < Count; i++)
            {
                var test = this[i].Magnitude;
                if (test < min)
                {
                    index = i;
                    min = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the value of the absolute maximum element.
        /// </summary>
        /// <returns>The value of the absolute maximum element.</returns>
        public override Complex32 AbsoluteMaximum()
        {
            return this[AbsoluteMaximumIndex()].Magnitude;
        }

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>   
        public override int AbsoluteMaximumIndex()
        {
            var index = 0;
            var max = this[index].Magnitude;
            for (var i = 1; i < Count; i++)
            {
                var test = this[i].Magnitude;
                if (test > max)
                {
                    index = i;
                    max = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Computes the sum of the vector's elements.
        /// </summary>
        /// <returns>The sum of the vector's elements.</returns>
        public override Complex32 Sum()
        {
            return CommonParallel.Aggregate(
                0, 
                Count, 
                i => this[i]);
        }

        /// <summary>
        /// Computes the sum of the absolute value of the vector's elements.
        /// </summary>
        /// <returns>The sum of the absolute value of the vector's elements.</returns>
        public override Complex32 SumMagnitudes()
        {
            return CommonParallel.Aggregate(
                0, 
                Count, 
                i => this[i].Magnitude);
        }

        /// <summary>
        /// Computes the p-Norm.
        /// </summary>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// <c>Scalar ret = (sum(abs(this[i])^p))^(1/p)</c>
        /// </returns>
        public override Complex32 Norm(double p)
        {
            if (p < 0.0)
            {
                throw new ArgumentOutOfRangeException("p");
            }

            if (double.IsPositiveInfinity(p))
            {
                return CommonParallel.Select(
                    0, 
                    Count, 
                    (index, localData) => Math.Max(localData, this[index].Magnitude), 
                    Common.Max);
            }

            var sum = CommonParallel.Aggregate(
                0, 
                Count, 
                index => Math.Pow(this[index].Magnitude, p));

            return (float)Math.Pow(sum, 1.0 / p);
        }

        /// <summary>
        /// Conjugates vector and save result to <paramref name="target"/>
        /// </summary>
        /// <param name="target">Target vector</param>
        protected override void DoConjugate(Vector<Complex32> target)
        {
            CopyTo(target);
            CommonParallel.For(
                0, 
                Count, 
                index => target[index] = this[index].Conjugate());
        }

        /// <summary>
        /// Returns a negated vector.
        /// </summary>
        /// <returns>
        /// The negated vector.
        /// </returns>
        /// <remarks>
        /// Added as an alternative to the unary negation operator.
        /// </remarks>
        public override Vector<Complex32> Negate()
        {
            var result = CreateVector(Count);
            CommonParallel.For(
                0, 
                Count, 
                index => result[index] = -this[index]);

            return result;
        }

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>          
        public override int MaximumIndex()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the index of the minimum element.
        /// </summary>
        /// <returns>The index of minimum element.</returns>  
        public override int MinimumIndex()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Normalizes this vector to a unit vector with respect to the p-norm.
        /// </summary>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// This vector normalized to a unit vector with respect to the p-norm.
        /// </returns>
        public override Vector<Complex32> Normalize(double p)
        {
            if (p < 0.0)
            {
                throw new ArgumentOutOfRangeException("p");
            }

            var norm = Norm(p);
            var clone = Clone();
            if (norm.Real == 0.0f)
            {
                return clone;
            }

            clone.Multiply(1.0f / norm, clone);

            return clone;
        }

        /// <summary>
        /// Generates a vector with random elements
        /// </summary>
        /// <param name="length">Number of elements in the vector.</param>
        /// <param name="randomDistribution">Continuous Random Distribution or Source</param>
        /// <returns>
        /// A vector with n-random elements distributed according
        /// to the specified random distribution.
        /// </returns>
        /// <exception cref="ArgumentException">If the n vector is non-positive.</exception> 
        public override Vector<Complex32> Random(int length, IContinuousDistribution randomDistribution)
        {
            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            var vector = CreateVector(length);
            for (var index = 0; index < length; index++)
            {
                vector[index] = new Complex32(Convert.ToSingle(randomDistribution.Sample()), Convert.ToSingle(randomDistribution.Sample()));
            }

            return vector;
        }

        /// <summary>
        /// Generates a vector with random elements
        /// </summary>
        /// <param name="length">Number of elements in the vector.</param>
        /// <param name="randomDistribution">Continuous Random Distribution or Source</param>
        /// <returns>
        /// A vector with n-random elements distributed according
        /// to the specified random distribution.
        /// </returns>
        /// <exception cref="ArgumentException">If the n vector is not positive.</exception> 
        public override Vector<Complex32> Random(int length, IDiscreteDistribution randomDistribution)
        {
            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            var vector = CreateVector(length);
            for (var index = 0; index < length; index++)
            {
                vector[index] = new Complex32(Convert.ToSingle(randomDistribution.Sample()), Convert.ToSingle(randomDistribution.Sample()));
            }

            return vector;
        }
    }
}

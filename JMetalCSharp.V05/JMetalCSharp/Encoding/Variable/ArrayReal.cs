using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;
using System.Text;

namespace JMetalCSharp.Encoding.Variable
{
	public class ArrayReal : Core.Variable
	{
		/// <summary>
		/// Problem using the type
		/// </summary>
		private Problem problem;

		/// <summary>
		/// Stores an array of real values
		/// </summary>
		public double[] Array { get; set; }

		/// <summary>
		/// Stores the length of the array
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public ArrayReal()
		{
			this.problem = null;
			Size = 0;
			Array = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="size">Size of the array</param>
		/// <param name="problem"></param>
		public ArrayReal(int size, Problem problem)
		{
			this.problem = problem;
			Size = size;
			Array = new double[Size];

			for (int i = 0; i < Size; i++)
			{
				double val = JMetalRandom.NextDouble() * (this.problem.UpperLimit[i] - this.problem.LowerLimit[i]) + this.problem.LowerLimit[i];
				Array[i] = val;
			}
		}

		public ArrayReal(ArrayReal arrayReal)
		{
			this.problem = arrayReal.problem;
			Size = arrayReal.Size;
			Array = new double[Size];

			arrayReal.Array.CopyTo(Array, 0);
		}

		public override Core.Variable DeepCopy()
		{
			return new ArrayReal(this);
		}

		public double GetLowerBound(int index)
		{
			if ((index >= 0) && (index < Size))
			{
				return this.problem.LowerLimit[index];
			}
			else
			{
				IndexOutOfRangeException ex = new IndexOutOfRangeException();
				Logger.Log.Error("ArrayReal.GetLowerBound", ex);
				throw ex;
			}
		}

		public double GetUpperBound(int index)
		{
			if ((index >= 0) && (index < Size))
			{
				return this.problem.UpperLimit[index];
			}
			else
			{
				IndexOutOfRangeException ex = new IndexOutOfRangeException();
				Logger.Log.Error("ArrayReal.GetUpperBound", ex);
				throw ex;
			}
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < Size; i += 1)
			{
				result.Append(Array[i] + (i < Size - 1 ? " " : ""));
			}

			return result.ToString();
		}
	}
}

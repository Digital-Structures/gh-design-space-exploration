using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using System;

namespace JMetalCSharp.Utils.Wrapper
{
	/// <summary>
	/// Wrapper for accessing real-coded solutions
	/// </summary>
	public class XReal
	{
		private Solution solution;
		private SolutionType type;

		/// <summary>
		/// Constructor
		/// </summary>
		public XReal()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="solution">solution</param>
		public XReal(Solution solution)
		{
			type = solution.Type;
			this.solution = solution;
		}

		/// <summary>
		/// Gets value of a encodings.variable
		/// </summary>
		/// <param name="index">Index of the encodings.variable</param>
		/// <returns>The value of the encodings.variable</returns>
		public double GetValue(int index)
		{
			double result;
			Type type = this.type.GetType();

			if (type == typeof(RealSolutionType))
			{
				result = ((Real)solution.Variable[index]).Value;
			}
			else if (type == typeof(BinaryRealSolutionType))
			{
				result = ((BinaryReal)solution.Variable[index]).Value;
			}
			else if (type == typeof(ArrayRealSolutionType))
			{
				result = ((ArrayReal)solution.Variable[0]).Array[index];
			}
			else if (type == typeof(ArrayRealAndBinarySolutionType))
			{
				result = ((ArrayReal)solution.Variable[0]).Array[index];
			}
			else
			{
				result = 0;
				Logger.Log.Error("XReal.GetValue(): Type not valid");
			}

			return result;
		}

		/// <summary>
		/// Sets the value of a encodings.variable
		/// </summary>
		/// <param name="index">Index of the encodings.variable</param>
		/// <param name="value">Value to be assigned</param>
		public void SetValue(int index, double value)
		{
			Type type = this.type.GetType();

			if (type == typeof(RealSolutionType))
			{
				((Real)solution.Variable[index]).Value = value;
			}
			else if (type == typeof(ArrayRealSolutionType))
			{
				((ArrayReal)solution.Variable[0]).Array[index] = value;
			}
			else if (type == typeof(ArrayRealAndBinarySolutionType))
			{
				((ArrayReal)solution.Variable[0]).Array[index] = value;
			}
			else
			{
				Logger.Log.Error("XReal.SetValue(): Type not valid");
			}
		}

		/// <summary>
		/// Gets the lower bound of a encodings.variable
		/// </summary>
		/// <param name="index">Index of the encodings.variable</param>
		/// <returns>The lower bound of the encodings.variable</returns>
		public double GetLowerBound(int index)
		{
			Type type = this.type.GetType();
			double result = 0;
			if (type == typeof(RealSolutionType))
			{
				result = ((Real)solution.Variable[index]).LowerBound;
			}
			else if (type == typeof(BinaryRealSolutionType))
			{
				result = ((BinaryReal)solution.Variable[index]).LowerBound;
			}
			else if (type == typeof(ArrayRealSolutionType))
			{
				result = (double)((ArrayReal)solution.Variable[0]).GetLowerBound(index);
			}
			else if (type == typeof(ArrayRealAndBinarySolutionType))
				result = (double)((ArrayReal)solution.Variable[0]).GetLowerBound(index);
			else
			{
				Logger.Log.Error("XReal.GetLowerBound(): Type not valid");
			}
			return result;
		}

		/// <summary>
		/// Gets the upper bound of a encodings.variable
		/// </summary>
		/// <param name="index">Index of the encodings.variable</param>
		/// <returns>The upper bound of the encodings.variable</returns>
		public double GetUpperBound(int index)
		{
			Type type = this.type.GetType();
			double result = 0;
			if (type == typeof(RealSolutionType))
			{
				result = ((Real)solution.Variable[index]).UpperBound;
			}
			else if (type == typeof(BinaryRealSolutionType))
			{
				result = ((BinaryReal)solution.Variable[index]).UpperBound;
			}
			else if (type == typeof(ArrayRealSolutionType))
			{
				result = (double)((ArrayReal)solution.Variable[0]).GetUpperBound(index);
			}
			else if (type == typeof(ArrayRealAndBinarySolutionType))
				result = (double)((ArrayReal)solution.Variable[0]).GetUpperBound(index);
			else
			{
				Logger.Log.Error("XReal.GetUpperBound(): Type not valid");
			}
			return result;
		}

		/// <summary>
		/// Returns the number of variables of the solution
		/// </summary>
		/// <returns></returns>
		public int GetNumberOfDecisionVariables()
		{
			int result = 0;
			Type type = this.type.GetType();

			if ((type == typeof(RealSolutionType)) ||
				(type == typeof(BinaryRealSolutionType)))
			{
				result = solution.Variable.Length;
			}
			else if (type == typeof(ArrayRealSolutionType))
			{
				result = ((ArrayReal)solution.Variable[0]).Size;
			}
			else
			{
				Logger.Log.Error("XReal.GetNumberOfDecisionVariables(): Type not valid");
			}
			return result;
		}

		/// <summary>
		/// Returns the number of variables of the solution
		/// </summary>
		/// <returns></returns>
		public int Size()
		{
			int result = 0;
			Type type = this.type.GetType();

			if ((type == typeof(RealSolutionType)) ||
				(type == typeof(BinaryRealSolutionType)))
			{
				result = solution.Variable.Length;
			}
			else if (type == typeof(ArrayRealSolutionType))
			{
				result = ((ArrayReal)solution.Variable[0]).Size;
			}
			else
			{
				Logger.Log.Error("XReal.Size(): Type not valid");
			}
			return result;
		}
	}
}

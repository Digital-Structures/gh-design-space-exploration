using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System.Text;

namespace JMetalCSharp.Encoding.Variable
{
	public class ArrayInt : Core.Variable
	{
		/// <summary>
		/// Problem using the type
		/// </summary>
		private Problem _problem;

		/// <summary>
		/// Stores an array of integer values
		/// </summary>
		public int[] Array { get; set; }

		/// <summary>
		/// Stores the length of the array
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Store the lower bound of each int value of the array in case of
		/// having each one different limits
		/// </summary>
		public new int[] LowerBound { get; set; }

		/// <summary>
		/// Store the upper bound of each int value of the array in case of
		/// having each one different limits
		/// </summary>
		public new int[] UpperBound { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public ArrayInt()
		{
			LowerBound = null;
			UpperBound = null;
			Size = 0;
			Array = null;
			_problem = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="size">Size of the array</param>
		public ArrayInt(int size)
		{
			this.Size = size;
			Array = new int[size];
			LowerBound = new int[size];
			UpperBound = new int[size];
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="size">Size of the array</param>
		public ArrayInt(int size, Problem problem)
		{
			_problem = problem;
			Size = size;
			Array = new int[size];
			LowerBound = new int[size];
			UpperBound = new int[size];

			for (int i = 0; i < Size; i++)
			{
				LowerBound[i] = (int)_problem.LowerLimit[i];
				UpperBound[i] = (int)_problem.UpperLimit[i];
				Array[i] = JMetalRandom.Next(LowerBound[i], UpperBound[i]);
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="size">The size of the array</param>
		/// <param name="lowerBounds">Lower bounds</param>
		/// <param name="upperBounds">Upper bounds</param>
		public ArrayInt(int size, double[] lowerBounds, double[] upperBounds)
		{
			Size = size;
			Array = new int[Size];
			LowerBound = new int[Size];
			UpperBound = new int[Size];

			for (int i = 0; i < Size; i++)
			{
				LowerBound[i] = (int)lowerBounds[i];
				UpperBound[i] = (int)upperBounds[i];
				Array[i] = JMetalRandom.Next(LowerBound[i], UpperBound[i]);
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="arrayInt">The ArrayInt to copy</param>
		private ArrayInt(ArrayInt arrayInt)
		{
			Size = arrayInt.Size;
			Array = new int[Size];
			LowerBound = new int[Size];
			UpperBound = new int[Size];

			for (int i = 0; i < Size; i++)
			{
				Array[i] = arrayInt.Array[i];
				LowerBound[i] = arrayInt.LowerBound[i];
				UpperBound[i] = arrayInt.UpperBound[i];
			}
		}

		public override Core.Variable DeepCopy()
		{
			return new ArrayInt(this);
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

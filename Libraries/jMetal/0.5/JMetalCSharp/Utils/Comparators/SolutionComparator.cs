using JMetalCSharp.Core;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	class SolutionComparator : IComparer<Solution>
	{
		#region Private Attribute

		/// <summary>
		/// Establishes a value of allowed dissimilarity
		/// </summary>
		private static readonly double EPSILON = 1e-10;

		#endregion

		#region Implement Interface

		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="solution1">The first <code>Solution</code>.</param>
		/// <param name="solution2">The second <code>Solution</code>.</param>
		/// <returns>0, if both solutions are equals with a certain dissimilarity, -1 otherwise.</returns>
		public int Compare(Solution solution1, Solution solution2)
		{
			if ((solution1.Variable != null) && (solution2.Variable != null))
			{
				if (solution1.NumberOfVariables() != solution2.NumberOfVariables())
				{
					return -1;
				}
			}

			try
			{
				if ((new Distance()).DistanceBetweenSolutions(solution1, solution2) < EPSILON)
				{
					return 0;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in " + this.GetType().FullName + ".Compare()");
				Logger.Log.Error("Error in " + this.GetType().FullName + ".Compare()", ex);
			}

			return -1;
		}

		#endregion
	}
}

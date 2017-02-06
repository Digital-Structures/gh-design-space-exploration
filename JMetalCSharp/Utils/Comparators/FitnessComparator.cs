using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	/// <summary>
	/// This class implements a <code>Comparator</code> (a method for comparing
	/// <code>Solution</code> objects) based on the fitness value returned by the
	/// method <code>getFitness</code>.
	/// </summary>
	public class FitnessComparator : IComparer<Solution>
	{
		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="s1">Solution 1</param>
		/// <param name="s2">Solution 2</param>
		/// <returns>-1, or 0, or 1 if o1 is less than, equal, or greater than o2,
		/// respectively.</returns>
		public int Compare(Solution s1, Solution s2)
		{
			if (s1 == null)
				return 1;
			else if (s2 == null)
				return -1;

			double fitness1 = s1.Fitness;
			double fitness2 = s2.Fitness;

			if (fitness1 < fitness2)
			{
				return -1;
			}

			if (fitness1 > fitness2)
			{
				return 1;
			}

			return 0;
		}
	}
}

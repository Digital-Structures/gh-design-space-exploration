using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	/// <summary>
	/// This class implements a <code>Comparator</code> (a method for comparing
	/// <code>Solution</code> objects) based on the rank of the solutions.
	/// </summary>
	public class RankComparator : IComparer<Solution>
	{
		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="s1">Representing the first <code>Solution</code>.</param>
		/// <param name="s2">Representing the second <code>Solution</code>.</param>
		/// <returns>-1, or 0, or 1 if s1 is less than, equal, or greater than s2, respectively.</returns>
		public int Compare(Solution s1, Solution s2)
		{
			if (s1 == null)
			{
				return 1;
			}
			else if (s2 == null)
			{
				return -1;
			}


			if (s1.Rank < s2.Rank)
			{
				return -1;
			}

			if (s1.Rank > s2.Rank)
			{
				return 1;
			}

			return 0;
		}
	}
}

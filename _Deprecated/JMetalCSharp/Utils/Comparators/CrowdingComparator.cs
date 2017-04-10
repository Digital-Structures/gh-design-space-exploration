using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	public class CrowdingComparator : IComparer<Solution>
	{
		/// <summary>
		/// stores a comparator for check the rank of solutions
		/// </summary>
		private static readonly IComparer<Solution> comparator = new RankComparator();

		/// <summary>
		/// Compare two solutions.
		/// </summary>
		/// <param name="s1">First <code>Solution</code></param>
		/// <param name="s2">Second <code>Solution</code></param>
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

			int flagComparatorRank = comparator.Compare(s1, s2);
			if (flagComparatorRank != 0)
			{
				return flagComparatorRank;
			}

			/* His rank is equal, then distance crowding comparator */
			double distance1 = s1.CrowdingDistance;
			double distance2 = s2.CrowdingDistance;
			if (distance1 > distance2)
			{
				return -1;
			}

			if (distance1 < distance2)
			{
				return 1;
			}

			return 0;
		}
	}
}

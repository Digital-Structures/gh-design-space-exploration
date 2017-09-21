using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	/// <summary>
	/// This class implements a <code>Comparator</code> (a method for comparing
	/// <code>Solution</code> objects) based on the crowding distance, as in NSGA-II.
	/// </summary>
	public class CrowdingDistanceComparator : IComparer<Solution>
	{
		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="s1">Solution 1</param>
		/// <param name="s2">Solution 2</param>
		/// <returns></returns>
		public int Compare(Solution s1, Solution s2)
		{
			if (s1 == null)
				return 1;
			else if (s2 == null)
				return -1;

			double distance1 = s1.CrowdingDistance;
			double distance2 = s2.CrowdingDistance;
			if (distance1 > distance2)
				return -1;
			if (distance1 < distance2)
				return 1;
			return 0;
		}
	}
}

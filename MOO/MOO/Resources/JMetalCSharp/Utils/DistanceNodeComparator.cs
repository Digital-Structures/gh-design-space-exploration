using System.Collections.Generic;

namespace JMetalCSharp.Utils
{
	/// <summary>
	/// This class implements a <code>Comparator</code> to compare instances of
	/// <code>DistanceNode</code>.
	/// </summary>
	public class DistanceNodeComparator : IComparer<DistanceNode>
	{

		/// <summary>
		/// Compares two <code>DistanceNode</code>.
		/// </summary>
		/// <param name="node1">DistanceNode 1</param>
		/// <param name="node2">DistanceNode 2</param>
		/// <returns>-1 if the distance of o1 is smaller than the distance of o2,
		///           0 if the distance of both are equals, and
		///           1 if the distance of o1 is bigger than the distance of o2</returns>
		public int Compare(DistanceNode node1, DistanceNode node2)
		{
			double distance1, distance2;
			distance1 = node1.Distance;
			distance2 = node2.Distance;

			if (distance1 < distance2)
				return -1;
			else if (distance1 > distance2)
				return 1;
			else
				return 0;
		}
	}
}

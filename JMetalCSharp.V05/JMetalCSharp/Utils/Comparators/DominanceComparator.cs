using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	public class DominanceComparator : IComparer<Solution>
	{
		#region Private Attributes

		IConstraintViolationComparator violationConstraintComparator;

		#endregion
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public DominanceComparator()
		{
			violationConstraintComparator = new OverallConstraintViolationComparator();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="comparator"></param>
		public DominanceComparator(IConstraintViolationComparator comparator)
		{
			violationConstraintComparator = comparator;
		}

		#endregion

		#region Implement Interface

		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="s1">The first <code>Solution</code></param>
		/// <param name="s2">The second <code>Solution</code></param>
		/// <returns>-1, or 0, or 1 if solution1 dominates solution2, both are non-dominated, or solution1 is dominated by solution22, respectively.</returns>
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


			int dominate1; // dominate1 indicates if some objective of solution1 
			// dominates the same objective in solution2. dominate2
			int dominate2; // is the complementary of dominate1.

			dominate1 = 0;
			dominate2 = 0;

			int flag; //stores the result of the comparison

			// Test to determine whether at least a solution violates some constraint
			if (violationConstraintComparator.NeedToCompare(s1, s2))
			{
				return violationConstraintComparator.Compare(s1, s2);
			}

			// Equal number of violated constraints. Applying a dominance Test then
			double value1, value2;
			for (int i = 0; i < s1.NumberOfObjectives; i++)
			{
				value1 = s1.Objective[i];
				value2 = s2.Objective[i];
				if (value1 < value2)
				{
					flag = -1;
				}
				else if (value1 > value2)
				{
					flag = 1;
				}
				else
				{
					flag = 0;
				}

				if (flag == -1)
				{
					dominate1 = 1;
				}

				if (flag == 1)
				{
					dominate2 = 1;
				}
			}

			if (dominate1 == dominate2)
			{
				return 0; //No one dominate the other
			}
			if (dominate1 == 1)
			{
				return -1; // solution1 dominate
			}
			return 1;    // solution2 dominate   
		}

		#endregion
	}
}

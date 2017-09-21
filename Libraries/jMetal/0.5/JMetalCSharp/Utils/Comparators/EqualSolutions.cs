using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	/// <summary>
	/// This class implements a <code>Comparator</code> (a method for comparing
	/// <code>Solution</code> objects) based whether all the objective values are
	/// equal or not. A dominance test is applied to decide about what solution
	/// is the best.
	/// </summary>
	public class EqualSolutions : IComparer<Solution>
	{
		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="solution1">Solution 1.</param>
		/// <param name="solution2">Solution 2.</param>
		/// <returns>-1, or 0, or 1, or 2 if solution1 is dominates solution2, solution1 
		/// and solution2 are equals, or solution1 is greater than solution2, 
		/// respectively. </returns>
		public int Compare(Solution solution1, Solution solution2)
		{
			if (solution1 == null)
				return 1;
			else if (solution2 == null)
				return -1;

			int dominate1; // dominate1 indicates if some objective of solution1 
			// dominates the same objective in solution2. 
			int dominate2; // dominate2 is the complementary of dominate1.

			dominate1 = 0;
			dominate2 = 0;

			int flag;
			double value1, value2;
			for (int i = 0; i < solution1.NumberOfObjectives; i++)
			{
				flag = (new ObjectiveComparator(i)).Compare(solution1, solution2);
				value1 = solution1.Objective[i];
				value2 = solution2.Objective[i];

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

			if (dominate1 == 0 && dominate2 == 0)
			{
				return 0; //No one dominate the other
			}

			if (dominate1 == 1)
			{
				return -1; // solution1 dominate
			}
			else if (dominate2 == 1)
			{
				return 1;    // solution2 dominate
			}
			return 2;
		}
	}
}

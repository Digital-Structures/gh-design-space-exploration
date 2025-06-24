using JMetalCSharp.Core;

namespace JMetalCSharp.Utils.Comparators
{
	public class OverallConstraintViolationComparator : IConstraintViolationComparator
	{
		#region Implement Interface

		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="s1">The first <code>Solution</code></param>
		/// <param name="s2">The second <code>Solution</code></param>
		/// <returns>-1, or 0, or 1 if o1 is less than, equal, or greater than o2, respectively.</returns>
		public int Compare(Solution s1, Solution s2)
		{
			int result;
			double overall1, overall2;
			overall1 = s1.OverallConstraintViolation;
			overall2 = s2.OverallConstraintViolation;

			if ((overall1 < 0) && (overall2 < 0))
			{
				if (overall1 > overall2)
				{
					result = -1;
				}
				else if (overall2 > overall1)
				{
					result = 1;
				}
				else
				{
					result = 0;
				}
			}
			else if ((overall1 == 0) && (overall2 < 0))
			{
				result = -1;
			}
			else if ((overall1 < 0) && (overall2 == 0))
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns>Returns true if solutions s1 and/or s2 have an overall constraint violation < 0</returns>
		public bool NeedToCompare(Solution s1, Solution s2)
		{
			bool needToCompare;
			needToCompare = (s1.OverallConstraintViolation < 0) || (s2.OverallConstraintViolation < 0);

			return needToCompare;
		}

		#endregion
	}
}

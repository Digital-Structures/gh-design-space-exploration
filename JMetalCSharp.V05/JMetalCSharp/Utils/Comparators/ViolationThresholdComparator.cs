using JMetalCSharp.Core;
using System;

namespace JMetalCSharp.Utils.Comparators
{
	public class ViolationThresholdComparator : IConstraintViolationComparator
	{
		#region Private Attributes

		/// <summary>
		/// threshold used for the comparations
		/// </summary>
		private double threshold = 0.0;

		#endregion

		#region Implement Interface

		/// <summary>
		/// Returns true if solutions s1 and/or s2 have an overall constraint violation < 0
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public bool NeedToCompare(Solution s1, Solution s2)
		{
			bool needToCompare;
			double overall1, overall2;

			overall1 = Math.Abs(s1.NumberOfViolatedConstraints * s1.OverallConstraintViolation);
			overall2 = Math.Abs(s2.NumberOfViolatedConstraints * s2.OverallConstraintViolation);

			needToCompare = (overall1 > this.threshold) || (overall2 > this.threshold);

			return needToCompare;
		}

		/// <summary>
		/// Compares two solutions.
		/// </summary>
		/// <param name="s1">The first <code>Solution</code>.</param>
		/// <param name="s2">The second <code>Solution</code>.</param>
		/// <returns>-1, or 0, or 1 if o1 is less than, equal, or greater than o2, respectively.</returns>
		public int Compare(Solution s1, Solution s2)
		{
			double overall1, overall2;
			int result;
			overall1 = s1.NumberOfViolatedConstraints * s1.OverallConstraintViolation;
			overall2 = s2.NumberOfViolatedConstraints * s2.OverallConstraintViolation;

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

		#endregion

		#region Public Methods

		/// <summary>
		/// Updates the threshold value using the population
		/// </summary>
		/// <param name="set"></param>
		public void UpdateThreshold(SolutionSet set)
		{
			threshold = FeasibilityRatio(set) * MeanOveralViolation(set);

		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Computes the feasibility ratio
		/// </summary>
		/// <param name="solutionSet"></param>
		/// <returns>The ratio of feasible solutions</returns>
		private double FeasibilityRatio(SolutionSet solutionSet)
		{
			double aux = 0.0;
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				if (solutionSet.Get(i).OverallConstraintViolation < 0)
				{
					aux = aux + 1.0;
				}
			}
			return aux / (double)solutionSet.Size();
		}

		/// <summary>
		/// Computes the feasibility ratio
		/// </summary>
		/// <param name="solutionSet"></param>
		/// <returns>The ratio of feasible solutions</returns>
		private double MeanOveralViolation(SolutionSet solutionSet)
		{
			double aux = 0.0;
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				aux += Math.Abs(solutionSet.Get(i).NumberOfViolatedConstraints * solutionSet.Get(i).OverallConstraintViolation);
			}
			return aux / (double)solutionSet.Size();
		}

		#endregion
	}
}

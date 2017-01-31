using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator.FastHypervolume.WFG;
using JMetalCSharp.Utils.Comparators;
using System;

namespace JMetalCSharp.QualityIndicator.FastHypervolume
{
	public class FastHypervolume
	{
		#region Private attributes
		Solution referencePoint;
		int numberOfObjectives;
		double offset = 20.0;
		#endregion

		#region Constructors
		public FastHypervolume()
		{
			referencePoint = null;
			numberOfObjectives = 0;
		}

		public FastHypervolume(double offset)
		{
			this.referencePoint = null;
			this.numberOfObjectives = 0;
			this.offset = offset;
		}

		#endregion

		#region Public Methods
		public double ComputeHypervolume(SolutionSet solutionSet)
		{
			double hv;
			if (solutionSet.Size() == 0)
				hv = 0.0;
			else
			{
				numberOfObjectives = solutionSet.Get(0).NumberOfObjectives;
				referencePoint = new Solution(numberOfObjectives);
				UpdateReferencePoint(solutionSet);
				if (numberOfObjectives == 2)
				{
					solutionSet.Sort(new ObjectiveComparator(numberOfObjectives - 1, true));
					hv = Get2DHV(solutionSet);
				}
				else
				{
					UpdateReferencePoint(solutionSet);
					Front front = new Front(solutionSet.Size(), numberOfObjectives, solutionSet);
					hv = new WFGHV(numberOfObjectives, solutionSet.Size(), referencePoint).GetHV(front);
				}
			}

			return hv;
		}

		public double ComputeHypervolume(SolutionSet solutionSet, Solution referencePoint)
		{
			double hv = 0.0;
			if (solutionSet.Size() == 0)
				hv = 0.0;
			else
			{
				this.numberOfObjectives = solutionSet.Get(0).NumberOfObjectives;
				this.referencePoint = referencePoint;

				if (this.numberOfObjectives == 2)
				{
					solutionSet.Sort(new ObjectiveComparator(this.numberOfObjectives - 1, true));

					hv = Get2DHV(solutionSet);
				}
				else
				{
					WFGHV wfg = new WFGHV(this.numberOfObjectives, solutionSet.Size());
					Front front = new Front(solutionSet.Size(), numberOfObjectives, solutionSet);
					hv = wfg.GetHV(front, referencePoint);
				}
			}

			return hv;
		}

		/// <summary>
		/// Computes the HV of a solution set.
		/// REQUIRES: The problem is bi-objective
		/// REQUIRES: The archive is ordered in descending order by the second objective
		/// </summary>
		/// <param name="solutionSet"></param>
		/// <returns></returns>
		public double Get2DHV(SolutionSet solutionSet)
		{
			double hv = 0.0;
			if (solutionSet.Size() > 0)
			{
				hv = Math.Abs((solutionSet.Get(0).Objective[0] - referencePoint.Objective[0]) * (solutionSet.Get(0).Objective[1] - referencePoint.Objective[1]));

				for (int i = 1; i < solutionSet.Size(); i++)
				{
					double tmp = Math.Abs((solutionSet.Get(i).Objective[0] - referencePoint.Objective[0]) * (solutionSet.Get(i).Objective[1] - solutionSet.Get(i - 1).Objective[1]));
					hv += tmp;
				}
			}
			return hv;
		}

		/// <summary>
		/// Computes the HV contribution of the solutions
		/// </summary>
		/// <param name="solutionSet"></param>
		public void ComputeHVContributions(SolutionSet solutionSet)
		{
			double[] contributions = new double[solutionSet.Size()];
			double solutionSetHV = 0;

			solutionSetHV = ComputeHypervolume(solutionSet);

			for (int i = 0; i < solutionSet.Size(); i++)
			{
				Solution currentPoint = solutionSet.Get(i);
				solutionSet.Remove(i);

				if (numberOfObjectives == 2)
				{
					contributions[i] = solutionSetHV - Get2DHV(solutionSet);
				}
				else
				{
					Front front = new Front(solutionSet.Size(), numberOfObjectives, solutionSet);
					double hv = new WFGHV(numberOfObjectives, solutionSet.Size(), referencePoint).GetHV(front);
					contributions[i] = solutionSetHV - hv;
				}
				solutionSet.Add(i, currentPoint);
			}

			for (int i = 0; i < solutionSet.Size(); i++)
			{
				solutionSet.Get(i).CrowdingDistance = contributions[i];
			}
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// Updates the reference point
		/// </summary>
		/// <param name="solutionSet"></param>
		private void UpdateReferencePoint(SolutionSet solutionSet)
		{
			double[] maxObjectives = new double[numberOfObjectives];
			for (int i = 0; i < numberOfObjectives; i++)
				maxObjectives[i] = 0;

			for (int i = 0; i < solutionSet.Size(); i++)
				for (int j = 0; j < numberOfObjectives; j++)
					if (maxObjectives[j] < solutionSet.Get(i).Objective[j])
						maxObjectives[j] = solutionSet.Get(i).Objective[j];

			for (int i = 0; i < referencePoint.NumberOfObjectives; i++)
			{
				referencePoint.Objective[i] = maxObjectives[i] + offset;
			}
		}
		#endregion
	}
}

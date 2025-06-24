using JMetalCSharp.Core;
using JMetalCSharp.Utils.Archive;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.QualityIndicator.FastHypervolume
{
	/// <summary>
	/// This class implements a bounded archive based on the hypervolume quality indicator
	/// </summary>
	public class FastHypervolumeArchive : Archive
	{
		#region Private Methods
		/// <summary>
		/// Stores the maximum size of the archive.
		/// </summary>
		private int maxSize;

		/// <summary>
		/// Stores the number of the objectives.
		/// </summary>
		private int objectives;

		/// <summary>
		/// Stores a Comparator for dominance checking.
		/// </summary>
		private IComparer<Solution> dominance;

		/// <summary>
		/// Stores a Comparator for equality checking (in the objective space).
		/// </summary>
		private IComparer<Solution> equals;

		private IComparer<Solution> crowdingDistance;
		#endregion

		#region Public Properties
		public Solution ReferencePoint { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="maxSize">The maximum size of the archive.</param>
		/// <param name="numberOfObjectives">The number of objectives.</param>
		public FastHypervolumeArchive(int maxSize, int numberOfObjectives)
			: base(maxSize)
		{
			this.maxSize = maxSize;
			this.objectives = numberOfObjectives;
			this.dominance = new DominanceComparator();
			this.equals = new EqualSolutions();
			this.ReferencePoint = new Solution(objectives);
			for (int i = 0; i < objectives; i++)
			{
				ReferencePoint.Objective[i] = double.MaxValue;
			}

			crowdingDistance = new CrowdingComparator();
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Adds a <code>Solution</code> to the archive. If the <code>Solution</code>
		/// is dominated by any member of the archive, then it is discarded. If the 
		/// <code>Solution</code> dominates some members of the archive, these are
		/// removed. If the archive is full and the <code>Solution</code> has to be
		/// inserted, the solution contributing the least to the HV of the solution set
		/// is discarded.
		/// </summary>
		/// <param name="solution">The <code>Solution</code></param>
		/// <returns>true if the <code>Solution</code> has been inserted, false otherwise.</returns>
		public override bool Add(Solution solution)
		{
			int flag = 0;
			int i = 0;
			Solution aux; //Store an solution temporally

			while (i < this.SolutionsList.Count)
			{
				aux = this.SolutionsList[i];

				flag = dominance.Compare(solution, aux);
				if (flag == 1)
				{               // The solution to add is dominated
					return false;                // Discard the new solution
				}
				else if (flag == -1)
				{       // A solution in the archive is dominated
					this.SolutionsList.RemoveAt(i);    // Remove it from the population            
				}
				else
				{
					if (equals.Compare(aux, solution) == 0)
					{ // There is an equal solution 
						// in the population
						return false; // Discard the new solution
					}
					i++;
				}
			}
			// Insert the solution into the archive
			this.SolutionsList.Add(solution);
			if (Size() > maxSize)
			{ // The archive is full
				ComputeHVContribution();

				Remove(IndexWorst(crowdingDistance));
			}
			return true;
		}

		/// <summary>
		/// This method forces to compute the contribution of each solution (required for PAEShv)
		/// </summary>
		public void ComputeHVContribution()
		{
			if (Size() > 2)
			{ // The contribution can be updated

				FastHypervolume fastHV = new FastHypervolume();
				fastHV.ComputeHVContributions(this);
			}
		}

		#endregion
	}
}

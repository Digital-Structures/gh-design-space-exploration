using JMetalCSharp.Core;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Archive
{
	/// <summary>
	/// This class implements a bounded archive based on crowding distances (as
	/// defined in NSGA-II).
	/// </summary>
	public class CrowdingArchive : Archive
	{
		/// <summary>
		/// Stores the maximum size of the archive.
		/// </summary>
		private int maxSize;

		/// <summary>
		/// Stores the number of the objectives.
		/// </summary>
		private int objectives;

		/// <summary>
		/// Stores a <code>Comparator</code> for dominance checking.
		/// </summary>
		private IComparer<Solution> dominance;

		/// <summary>
		/// Stores a <code>Comparator</code> for equality checking (in the objective
		/// space).
		/// </summary>
		private IComparer<Solution> equals;

		/// <summary>
		/// Stores a <code>Comparator</code> for checking crowding distances.
		/// </summary>
		private IComparer<Solution> crowdingDistance;

		/// <summary>
		/// Stores a <code>Distance</code> object, for distances utilities
		/// </summary>
		private Distance distance;

		/// <summary>
		/// Constructor. 
		/// </summary>
		/// <param name="maxSize">The maximum size of the archive.</param>
		/// <param name="numberOfObjectives">The number of objectives.</param>
		public CrowdingArchive(int maxSize, int numberOfObjectives)
			: base(maxSize)
		{
			this.maxSize = maxSize;
			this.objectives = numberOfObjectives;
			this.dominance = new DominanceComparator();
			this.equals = new EqualSolutions();
			this.crowdingDistance = new CrowdingDistanceComparator();
			this.distance = new Distance();

		}

		/// <summary>
		/// Adds a <code>Solution</code> to the archive. If the <code>Solution</code>
		/// is dominated by any member of the archive, then it is discarded. If the 
		/// <code>Solution</code> dominates some members of the archive, these are
		/// removed. If the archive is full and the <code>Solution</code> has to be
		/// inserted, the solutions are sorted by crowding distance and the one having
		/// the minimum crowding distance value.
		/// </summary>
		/// <param name="solution">The <code>Solution</param>
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
					}  // if
					i++;
				}
			}
			// Insert the solution into the archive
			this.SolutionsList.Add(solution);
			if (Size() > maxSize)
			{ // The archive is full
				distance.CrowdingDistanceAssignment(this, objectives);
				Remove(IndexWorst(crowdingDistance));
			}
			return true;
		}
	}
}

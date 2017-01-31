using JMetalCSharp.Core;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;
using System.Linq;

namespace JMetalCSharp.Utils.Archive
{
	public class AdaptiveGridArchive : Archive
	{
		/// <summary>
		/// Stores the adaptive grid
		/// </summary>
		public AdaptiveGrid Grid { get; private set; }

		/// <summary>
		/// Stores the maximum size of the archive
		/// </summary>
		private int maxSize;

		/// <summary>
		/// Stores a <code>Comparator</code> for dominance checking
		/// </summary>
		private IComparer<Solution> dominance;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="maxSize">The maximum size of the archive</param>
		/// <param name="bisections">The maximum number of bi-divisions for the adaptive grid.</param>
		/// <param name="objectives">The number of objectives.</param>
		public AdaptiveGridArchive(int maxSize, int bisections, int objectives)
			: base(maxSize)
		{
			this.maxSize = maxSize;
			dominance = new DominanceComparator();
			Grid = new AdaptiveGrid(bisections, objectives);
		}

		/// <summary>
		/// Adds a <code>Solution</code> to the archive. If the <code>Solution</code>
		/// is dominated by any member of the archive then it is discarded. If the
		/// <code>Solution</code> dominates some members of the archive, these are
		/// removed. If the archive is full and the <code>Solution</code> has to be
		/// inserted, one <code>Solution</code> of the most populated hypercube of
		/// the adaptive grid is removed.
		/// </summary>
		/// <param name="solution">The <code>Solution</code></param>
		/// <returns>true if the <code>Solution</code> has been inserted, false otherwise.</returns>
		public override bool Add(Solution solution)
		{
			var iterator = SolutionsList.ToList();

			foreach (var element in iterator)
			{
				int flag = dominance.Compare(solution, element);
				if (flag == -1)
				{ // The Individual to insert dominates other 
					// individuals in  the archive
					SolutionsList.Remove(element);
					int location = Grid.Location(element);
					if (Grid.GetLocationDensity(location) > 1)
					{//The hypercube contains 
						Grid.RemoveSolution(location);            //more than one individual
					}
					else
					{
						Grid.UpdateGrid(this);
					}
				}
				else if (flag == 1)
				{ // An Individual into the file dominates the 
					// solution to insert
					return false; // The solution will not be inserted
				}
			}

			// At this point, the solution may be inserted
			if (Size() == 0)
			{ //The archive is empty
				SolutionsList.Add(solution);
				Grid.UpdateGrid(this);
				return true;
			}

			if (Size() < maxSize)
			{ //The archive is not full              
				Grid.UpdateGrid(solution, this); // Update the grid if applicable
				int loc;
				loc = Grid.Location(solution); // Get the location of the solution
				Grid.AddSolution(loc); // Increment the density of the hypercube
				SolutionsList.Add(solution); // Add the solution to the list
				return true;
			}

			// At this point, the solution has to be inserted and the archive is full
			Grid.UpdateGrid(solution, this);
			int loca = Grid.Location(solution);
			if (loca == Grid.MostPopulated)
			{ // The solution is in the 
				// most populated hypercube
				return false; // Not inserted
			}
			else
			{
				// Remove an solution from most populated area
				iterator = SolutionsList.ToList();
				bool removed = false;

				foreach (var element in iterator)
				{
					if (!removed)
					{
						int location2 = Grid.Location(element);
						if (location2 == Grid.MostPopulated)
						{
							SolutionsList.Remove(element);
							Grid.RemoveSolution(location2);
						}
					}
				}
				// A solution from most populated hypercube has been removed, 
				// insert now the solution
				Grid.AddSolution(loca);
				SolutionsList.Add(solution);
			}
			return true;
		}
	}
}

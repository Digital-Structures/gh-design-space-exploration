using JMetalCSharp.Core;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;
using System.Linq;

namespace JMetalCSharp.Utils
{
	public class NonDominatedSolutionList : SolutionSet
	{
		#region Private Attributes
		/// <summary>
		/// Stores a <code>Comparator</code> for dominance checking
		/// </summary>
		private IComparer<Solution> dominance = new DominanceComparator();

		/// <summary>
		/// Stores a <code>Comparator</code> for checking if two solutions are equal
		/// </summary>
		private static readonly IComparer<Solution> equal = new SolutionComparator();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// The objects of this class are lists of non-dominated solutions according to
		/// a Pareto dominance comparator. 
		/// </summary>
		public NonDominatedSolutionList()
			: base()
		{
		}

		/// <summary>
		/// Constructor
		/// This constructor creates a list of non-dominated individuals using a comparator object.
		/// </summary>
		/// <param name="dominance">The comparator for dominance checking.</param>
		public NonDominatedSolutionList(IComparer<Solution> dominance)
			: base()
		{
			this.dominance = dominance;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Inserts a solution in the list
		/// The decision variables can be null if the solution is read from a file; in
		/// that case, the domination tests are omitted
		/// </summary>
		/// <param name="solution">The solution to be inserted.</param>
		/// <returns>true if the operation success, and false if the solution is dominated or if an identical individual exists.</returns>
		public override bool Add(Solution solution)
		{
			if (SolutionsList.Count == 0)
			{
				SolutionsList.Add(solution);
				return true;
			}
			else
			{
				List<Solution> copyList = SolutionsList.ToList();
				foreach (Solution s in copyList)
				{
					int flag = dominance.Compare(solution, s);

					if (flag == -1)
					{
						// A solution in the list is dominated by the new one
						SolutionsList.Remove(s);
					}
					else if (flag == 0)
					{
						// Non-dominated solutions  
					}
					else if (flag == 1)
					{ // The new solution is dominated
						return false;
					}
				}

				//At this point, the solution is inserted into the list
				SolutionsList.Add(solution);

				return true;
			}
		}

		#endregion
	}
}

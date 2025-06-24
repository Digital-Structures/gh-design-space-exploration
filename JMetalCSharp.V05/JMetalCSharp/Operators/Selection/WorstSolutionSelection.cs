using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// This class implements a selection operator used for selecting the worst
	/// solution in a SolutionSet according to a given comparator
	/// </summary>
	public class WorstSolutionSelection : Selection
	{
		#region Private Attributes

		/// <summary>
		/// Comparator
		/// </summary>
		private IComparer<Solution> comparator;

		#endregion

		#region Constructors

		public WorstSolutionSelection(Dictionary<string, object> parameters)
			: base(parameters)
		{

			comparator = (IComparer<Solution>)parameters["comparator"];
		}

		#endregion

		#region Public Override
		/// <summary>
		/// Performs the operation
		/// </summary>
		/// <param name="obj">Object representing a SolutionSet.</param>
		/// <returns>The best solution found</returns>
		public override object Execute(object obj)
		{
			SolutionSet solutionSet = (SolutionSet)obj;

			if (solutionSet.Size() == 0)
			{
				return null;
			}
			int worstSolution;

			worstSolution = 0;

			for (int i = 1; i < solutionSet.Size(); i++)
			{
				if (comparator.Compare(solutionSet.Get(i), solutionSet.Get(worstSolution)) > 0)
				{
					worstSolution = i;
				}
			}

			return worstSolution;
		}

		#endregion
	}
}

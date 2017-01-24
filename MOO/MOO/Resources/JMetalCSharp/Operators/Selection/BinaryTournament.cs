using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// This class implements an binary tournament selection operator
	/// </summary>
	public class BinaryTournament : Selection
	{
		/// <summary>
		/// Stores the <code>Comparator</code> used to compare two solutions
		/// </summary>
		private IComparer<Solution> comparator;

		/// <summary>
		/// Constructor
		/// Creates a new Binary tournament operator using a BinaryTournamentComparator
		/// </summary>
		/// <param name="parameters"></param>
		public BinaryTournament(Dictionary<string, object> parameters)
			: base(parameters)
		{
			object value;

			if ((parameters != null) && (parameters.TryGetValue("comparator", out value)))
			{
				comparator = (IComparer<Solution>)value;
			}
			else
			{
				comparator = new DominanceComparator();
			}
		}

		/// <summary>
		/// Performs the operation
		/// </summary>
		/// <param name="obj">Object representing a SolutionSet</param>
		/// <returns>The selected solution</returns>
		public override object Execute(object obj)
		{
			SolutionSet solutionSet = (SolutionSet)obj;
			Solution solution1, solution2;
			solution1 = solutionSet.Get(JMetalRandom.Next(0, solutionSet.Size() - 1));
			solution2 = solutionSet.Get(JMetalRandom.Next(0, solutionSet.Size() - 1));

			if (solutionSet.Size() >= 2)
				while (solution1 == solution2)
					solution2 = solutionSet.Get(JMetalRandom.Next(0, solutionSet.Size() - 1));

			int flag = comparator.Compare(solution1, solution2);
			if (flag == -1)
				return solution1;
			else if (flag == 1)
				return solution2;
			else
				if (JMetalRandom.NextDouble() < 0.5)
					return solution1;
				else
					return solution2;
		}
	}
}

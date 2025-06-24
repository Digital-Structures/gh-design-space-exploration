using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// Class implementing a factory of selection operators
	/// </summary>
	public static class SelectionFactory
	{
		/// <summary>
		/// Gets a selection operator through its name.
		/// </summary>
		/// <param name="name">Name of the operator</param>
		/// <param name="parameters"></param>
		/// <returns>The operator</returns>
		public static Selection GetSelectionOperator(string name, Dictionary<string, object> parameters)
		{
			if (name.Equals("BinaryTournament", StringComparison.InvariantCultureIgnoreCase))
			{
				return new BinaryTournament(parameters);
			}
			else if (name.Equals("BinaryTournament2", StringComparison.InvariantCultureIgnoreCase))
			{
				return new BinaryTournament2(parameters);
			}
			else if (name.Equals("PESA2Selection", StringComparison.InvariantCultureIgnoreCase))
			{
				return new PESA2Selection(parameters);
			}
			else if (name.Equals("RandomSelection", StringComparison.InvariantCultureIgnoreCase))
			{
				return new RandomSelection(parameters);
			}
			else if (name.Equals("RankingAndCrowdingSelection", StringComparison.InvariantCultureIgnoreCase))
			{
				return new RankingAndCrowdingSelection(parameters);
			}
			else if (name.Equals("DifferentialEvolutionSelection", StringComparison.InvariantCultureIgnoreCase))
			{
				return new DifferentialEvolutionSelection(parameters);
			}
			else if (name.Equals("BestSolutionSelection", StringComparison.InvariantCultureIgnoreCase))
			{
				return new BestSolutionSelection(parameters);
			}
			else if (name.Equals("WorstSolutionSelection", StringComparison.InvariantCultureIgnoreCase))
			{
				return new WorstSolutionSelection(parameters);
			}
			else
			{
				Console.WriteLine("Exception in " + typeof(SelectionFactory).FullName + ".GetSelectionOperator()");
				throw new Exception("Exception in " + typeof(SelectionFactory).FullName + ".GetSelectionOperator()");
			}
		}
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// Class implementing the selection operator used in DE: three different solutions are returned from a population.
	/// </summary>
	public class DifferentialEvolutionSelection : Selection
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parameters"></param>
		public DifferentialEvolutionSelection(Dictionary<string, object> parameters)
			: base(parameters)
		{
		}

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing the population and the position (index) of the current individual</param>
		/// <returns>An object containing the three selected parents</returns>
		public override object Execute(object obj)
		{
			object[] parameters = (object[])obj;
			SolutionSet population = (SolutionSet)parameters[0];
			int index = (int)parameters[1];

			Solution[] parents = new Solution[3];
			int r1, r2, r3;

			if (population.Size() < 4)
			{
				throw new Exception("DifferentialEvolutionSelection: the population has less than four solutions");
			}

			do
			{
				r1 = (int)(JMetalRandom.Next(0, population.Size() - 1));
			} while (r1 == index);
			do
			{
				r2 = (int)(JMetalRandom.Next(0, population.Size() - 1));
			} while (r2 == index || r2 == r1);
			do
			{
				r3 = (int)(JMetalRandom.Next(0, population.Size() - 1));
			} while (r3 == index || r3 == r1 || r3 == r2);

			parents[0] = population.Get(r1);
			parents[1] = population.Get(r2);
			parents[2] = population.Get(r3);

			return parents;
		}
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// This class implements a random selection operator used for selecting two random parents
	/// </summary>
	public class RandomSelection : Selection
	{
		#region Constructors

		public RandomSelection(Dictionary<string, object> parameters)
			: base(parameters)
		{
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Performs the operation
		/// </summary>
		/// <param name="obj">Object representing a SolutionSet.</param>
		/// <returns>An object representing an array with the selected parents</returns>
		public override object Execute(object obj)
		{
			SolutionSet population = (SolutionSet)obj;
			int pos1, pos2;
			pos1 = JMetalRandom.Next(0, population.Size() - 1);
			pos2 = JMetalRandom.Next(0, population.Size() - 1);
			while ((pos1 == pos2) && (population.Size() > 1))
			{
				pos2 = JMetalRandom.Next(0, population.Size() - 1);
			}

			Solution[] parents = new Solution[2];
			parents[0] = population.Get(pos1);
			parents[1] = population.Get(pos2);

			return parents;
		}

		#endregion
	}
}

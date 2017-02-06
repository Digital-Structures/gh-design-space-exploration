using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Crossover
{
	/// <summary>
	/// Class implementing a factory for crossover operators.
	/// </summary>
	public static class CrossoverFactory
	{
		/// <summary>
		/// Gets a crossover operator through its name.
		/// </summary>
		/// <param name="name">Name of the operator</param>
		/// <param name="parameters"></param>
		/// <returns>The operator</returns>
		public static Crossover GetCrossoverOperator(string name, Dictionary<string, object> parameters)
		{
			if (name.Equals("SBXCrossover", StringComparison.InvariantCultureIgnoreCase))
				return new SBXCrossover(parameters);
			else if (name.Equals("SinglePointCrossover", StringComparison.InvariantCultureIgnoreCase))
				return new SinglePointCrossover(parameters);
			else if (name.Equals("PMXCrossover", StringComparison.InvariantCultureIgnoreCase))
				return new PMXCrossover(parameters);
			else if (name.Equals("TwoPointsCrossover", StringComparison.InvariantCultureIgnoreCase))
				return new TwoPointsCrossover(parameters);
			else if (name.Equals("HUXCrossover", StringComparison.InvariantCultureIgnoreCase))
				return new HUXCrossover(parameters);
			else if (name.Equals("DifferentialEvolutionCrossover", StringComparison.InvariantCultureIgnoreCase))
				return new DifferentialEvolutionCrossover(parameters);
			else if (name.Equals("BLXAlphaCrossover", StringComparison.InvariantCultureIgnoreCase))
				return new BLXAlphaCrossover(parameters);
			else
			{
				Logger.Log.Error("Exception in CrossoverFactory.GetCrossoverOperator(): 'Operator " + name + "' not found");
				throw new Exception("Exception in CrossoverFactory.GetCrossoverOperator(): 'Operator " + name + "' not found");
			}
		}
	}
}

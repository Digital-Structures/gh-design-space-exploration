using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.LocalSearch
{
	public static class LocalSearchFactory
	{
		public static LocalSearch GetLocalSearchOperator(string name, Dictionary<string, object> parameters)
		{
			if (name.Equals("MutationLocalSearch", StringComparison.InvariantCultureIgnoreCase))
				return new MutationLocalSearch(parameters);
			else
			{
				Logger.Log.Error("Exception in LocalSearchFactory.GetLocalSearchOperator(): 'Operator " + name + "' not found");
				throw new Exception("Exception in LocalSearchFactory.GetLocalSearchOperator(): 'Operator " + name + "' not found");
			}
		}
	}
}

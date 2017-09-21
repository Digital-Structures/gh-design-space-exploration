using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{
	/// <summary>
	/// Class implementing a factory for Mutation objects.
	/// </summary>
	public class MutationFactory
	{
		public static Mutation GetMutationOperator(string name, Dictionary<string, object> parameters)
		{
			if (name.Equals("PolynomialMutation", StringComparison.InvariantCultureIgnoreCase))
				return new PolynomialMutation(parameters);
			else if (name.Equals("BitFlipMutation", StringComparison.InvariantCultureIgnoreCase))
				return new BitFlipMutation(parameters);
			else if (name.Equals("NonUniformMutation", StringComparison.InvariantCultureIgnoreCase))
				return new NonUniformMutation(parameters);
			else if (name.Equals("SwapMutation", StringComparison.InvariantCultureIgnoreCase))
				return new SwapMutation(parameters);
			else if (name.Equals("PolynomialBitFlipMutation", StringComparison.InvariantCultureIgnoreCase))
				return new PolynomialBitFlipMutation(parameters);
			else if (name.Equals("UniformMutation", StringComparison.InvariantCultureIgnoreCase))
				return new UniformMutation(parameters);
			else
			{
				Logger.Log.Error("Exception in " + typeof(MutationFactory).FullName + ".GetMutationOperator()");
				Console.WriteLine("Exception in " + typeof(MutationFactory).FullName + ".GetMutationOperator()");
				throw new Exception("Exception in " + typeof(MutationFactory).FullName + ".GetMutationOperator()");
			}
		}
	}
}

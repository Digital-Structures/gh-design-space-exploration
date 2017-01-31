using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{
	public abstract class Mutation : Operator
	{
		public Mutation(Dictionary<string, object> parameters)
			: base(parameters)
		{

		}
	}
}

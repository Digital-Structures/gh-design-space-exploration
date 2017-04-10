using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// This class represents the super class of all the selection operators
	/// </summary>
	public abstract class Selection : Operator
	{
		public Selection(Dictionary<string, object> parameters)
			: base(parameters)
		{

		}
	}
}

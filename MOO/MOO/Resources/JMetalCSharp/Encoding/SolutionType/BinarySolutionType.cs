using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace JMetalCSharp.Encoding.SolutionType
{
	/// <summary>
	/// Class representing the solution type of solutions composed of Binary
	/// variables
	/// </summary>
	public class BinarySolutionType : Core.SolutionType
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public BinarySolutionType(Problem problem)
			: base(problem)
		{

		}

		/// <summary>
		/// Create the variables of the solution
		/// </summary>
		/// <returns></returns>
		public override Core.Variable[] CreateVariables()
		{
			Core.Variable[] variables = new Core.Variable[Problem.NumberOfVariables];

			for (int i = 0, li = Problem.NumberOfVariables; i < li; i++)
			{
				variables[i] = new Binary(Problem.GetLength(i));
			}

			return variables;
		}

	}
}

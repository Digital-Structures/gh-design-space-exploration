using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace JMetalCSharp.Encoding.SolutionType
{
	/// <summary>
	/// Class representing the solution type of solutions composed of an ArrayReal 
	/// encodings.variable
	/// </summary>
	public class ArrayRealSolutionType : Core.SolutionType
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public ArrayRealSolutionType(Problem problem)
			: base(problem)
		{

		}

		public override Core.Variable[] CreateVariables()
		{
			Core.Variable[] variables = new Core.Variable[1];

			variables[0] = new ArrayReal(Problem.NumberOfVariables, Problem);
			return variables;
		}
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace JMetalCSharp.Encoding.SolutionType
{
	/// <summary>
	/// Class representing a solution type composed of real variables
	/// </summary>
	public class RealSolutionType : Core.SolutionType
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public RealSolutionType(Problem problem)
			: base(problem)
		{

		}

		public override Core.Variable[] CreateVariables()
		{
			Core.Variable[] variables = new Core.Variable[Problem.NumberOfVariables];

			for (int i = 0, li = Problem.NumberOfVariables; i < li; i++)
			{
				variables[i] = new Real(Problem.LowerLimit[i], Problem.UpperLimit[i]);
			}

			return variables;
		}
	}
}

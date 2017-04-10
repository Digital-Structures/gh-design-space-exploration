using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace JMetalCSharp.Encoding.SolutionType
{
	/// <summary>
	/// Class representing  a solution type including two variables: an integer 
	/// and a real.
	/// </summary>
	public class IntRealSolutionType : Core.SolutionType
	{
		private readonly int intVariables;
		private readonly int realVariables;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		/// <param name="intVariables">Number of integer variables</param>
		/// <param name="realVariables">Number of real variables</param>
		public IntRealSolutionType(Problem problem, int intVariables, int realVariables)
			: base(problem)
		{
			this.intVariables = intVariables;
			this.realVariables = realVariables;
		}

		/// <summary>
		/// Create the variables of the solution
		/// </summary>
		/// <returns></returns>
		public override Core.Variable[] CreateVariables()
		{
			Core.Variable[] variables = new Core.Variable[Problem.NumberOfVariables];

			for (int i = 0; i < intVariables; i++)
			{
				variables[i] = new Int((int)Problem.LowerLimit[i], (int)Problem.UpperLimit[i]);
			}

			for (int i = intVariables; i < (intVariables + realVariables); i++)
			{
				variables[i] = new Real(Problem.LowerLimit[i], Problem.UpperLimit[i]);
			}

			return variables;
		}
	}
}

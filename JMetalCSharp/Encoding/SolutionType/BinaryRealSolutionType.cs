using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace JMetalCSharp.Encoding.SolutionType
{
	/// <summary>
	/// Class representing the solution type of solutions composed of BinaryReal 
	/// variables
	/// </summary>
	public class BinaryRealSolutionType : Core.SolutionType
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public BinaryRealSolutionType(Problem problem)
			: base(problem)
		{

		}

		/// <summary>
		/// Creates the variables of the solution
		/// </summary>
		/// <returns></returns>
		public override Core.Variable[] CreateVariables()
		{
			Core.Variable[] variables = new Core.Variable[Problem.NumberOfVariables];

			for (int i = 0, li = Problem.NumberOfVariables; i < li; i++)
			{
				if (Problem.Precision == null)
				{
					int[] precision = new int[Problem.NumberOfVariables];
					for (int j = 0, lj = Problem.NumberOfVariables; j < lj; j++)
					{
						precision[j] = BinaryReal.DEFAULT_PRECISION;
					}
					Problem.Precision = precision;
				}
				variables[i] = new BinaryReal(Problem.Precision[i], Problem.LowerLimit[i], Problem.UpperLimit[i]);
			}
			return variables;
		}
	}
}

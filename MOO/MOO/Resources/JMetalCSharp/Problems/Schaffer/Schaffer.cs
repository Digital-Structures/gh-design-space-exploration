using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.Schaffer
{
	/// <summary>
	/// Class representing problem Schaffer
	/// </summary>
	public class Schaffer : Problem
	{
		#region Constructor
		/// <summary>
		/// Constructor.
		/// Creates a default instance of problem Schaffer
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		public Schaffer(string solutionType)
		{
			NumberOfVariables = 1;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "Schaffer";

			LowerLimit = new double[NumberOfVariables];
			UpperLimit = new double[NumberOfVariables];
			LowerLimit[0] = -100000;
			UpperLimit[0] = 100000;

			if (solutionType == "BinaryReal")
				SolutionType = new BinaryRealSolutionType(this);
			else if (solutionType == "Real")
				SolutionType = new RealSolutionType(this);
			else
			{
				Console.WriteLine("Error: solution type " + solutionType + " is invalid");
				Logger.Log.Error("Error: solution type " + solutionType + " is invalid");
				Environment.Exit(-1);
			}
		}

		#endregion

		#region Public Overrides
		/// <summary>
		/// Evaluates a solution
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			double variable = GetVariableValue(solution.Variable);

			double[] f = new double[NumberOfObjectives];
			f[0] = variable * variable;

			f[1] = (variable - 2.0) * (variable - 2.0);

			solution.Objective[0] = f[0];
			solution.Objective[1] = f[1];
		}

		#endregion

		#region Private Methods

		private double GetVariableValue(Variable variable)
		{
			double result;

			if (SolutionType.GetType() == typeof(BinaryRealSolutionType))
			{
				result = ((BinaryReal)variable).Value;
			}
			else
			{
				result = ((Real)variable).Value;
			}
			return result;
		}

		private double GetVariableValue(Variable[] gen)
		{
			double x = GetVariableValue(gen[0]);

			return x;
		}

		#endregion
	}

}

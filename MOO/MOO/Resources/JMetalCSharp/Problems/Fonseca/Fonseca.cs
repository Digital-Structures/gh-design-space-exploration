using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;

namespace JMetalCSharp.Problems.Fonseca
{

	/// <summary>
	/// Class representing problem Fonseca
	/// </summary>
	public class Fonseca : Problem
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// Creates a default instance of the Fonseca problem
		/// </summary>
		/// <param name="solutionType">The solution type must "Real", "BinaryReal, ArrayReal, or ArrayRealC".</param>
		public Fonseca(string solutionType)
		{
			NumberOfVariables = 3;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "Fonseca";

			UpperLimit = new double[NumberOfVariables];
			LowerLimit = new double[NumberOfVariables];
			for (int var = 0; var < NumberOfVariables; var++)
			{
				LowerLimit[var] = -4.0;
				UpperLimit[var] = 4.0;
			}

			if (solutionType == "BinaryReal")
				SolutionType = new BinaryRealSolutionType(this);
			else if (solutionType == "Real")
				SolutionType = new RealSolutionType(this);
			else if (solutionType == "ArrayReal")
				SolutionType = new ArrayRealSolutionType(this);
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
			XReal x = new XReal(solution);

			double[] f = new double[NumberOfObjectives];
			double sum1 = 0.0;
			for (int var = 0; var < NumberOfVariables; var++)
			{
				sum1 += Math.Pow(x.GetValue(var) - (1.0 / Math.Sqrt((double)NumberOfVariables)), 2.0);
			}
			double exp1 = Math.Exp((-1.0) * sum1);
			f[0] = 1 - exp1;

			double sum2 = 0.0;
			for (int var = 0; var < NumberOfVariables; var++)
			{
				sum2 += Math.Pow(x.GetValue(var) + (1.0 / Math.Sqrt((double)NumberOfVariables)), 2.0);
			}
			double exp2 = Math.Exp((-1.0) * sum2);
			f[1] = 1 - exp2;

			solution.Objective[0] = f[0];
			solution.Objective[1] = f[1];
		}

		#endregion
	}
}

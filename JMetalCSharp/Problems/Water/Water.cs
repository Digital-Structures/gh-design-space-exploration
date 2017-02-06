using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.Water
{
	public class Water : Problem
	{
		#region Private Attributes

		/// <summary>
		/// Defining the lower limits
		/// </summary>
		private static readonly double[] LOWERLIMIT = { 0.01, 0.01, 0.01 };

		/// <summary>
		/// Defining the upper limits
		/// </summary>
		private static readonly double[] UPPERLIMIT = { 0.45, 0.10, 0.10 };

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor. Creates a default instance of the Water problem.
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		public Water(string solutionType)
		{
			NumberOfVariables = 3;
			NumberOfObjectives = 5;
			NumberOfConstraints = 7;
			ProblemName = "Water";



			UpperLimit = new double[NumberOfVariables];
			LowerLimit = new double[NumberOfVariables];

			for (int var = 0; var < NumberOfVariables; var++)
			{
				LowerLimit[var] = LOWERLIMIT[var];
				UpperLimit[var] = UPPERLIMIT[var];
			}

			if (solutionType == "BinaryReal")
			{
				SolutionType = new BinaryRealSolutionType(this);
			}
			else if (solutionType == "Real")
			{
				SolutionType = new RealSolutionType(this);
			}
			else
			{
				Console.WriteLine("Error: solution type " + solutionType + " is invalid");
				Logger.Log.Error("Solution type " + solutionType + " is invalid");
				Environment.Exit(-1);
			}
		}

		#endregion

		#region Public Override

		/// <summary>
		/// Evaluates a solution
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			double[] f = new double[5]; // 5 functions
			double[] x = GetVariableValues(solution);
			// First function
			f[0] = 106780.37 * (x[1] + x[2]) + 61704.67;
			// Second function
			f[1] = 3000 * x[0];
			// Third function
			f[2] = 305700 * 2289 * x[1] / Math.Pow(0.06 * 2289, 0.65);
			// Fourth function
			f[3] = 250 * 2289 * Math.Exp(-39.75 * x[1] + 9.9 * x[2] + 2.74);
			// Third function
			f[4] = 25 * (1.39 / (x[0] * x[1]) + 4940 * x[2] - 80);

			solution.Objective[0] = f[0];
			solution.Objective[1] = f[1];
			solution.Objective[2] = f[2];
			solution.Objective[3] = f[3];
			solution.Objective[4] = f[4];
		}

		/// <summary>
		///Evaluates the constraint overhead of a solution 
		/// </summary>
		/// <param name="solution">The solution</param>
		public override void EvaluateConstraints(Solution solution)
		{
			double[] constraint = new double[7]; // 7 constraints
			double[] x = GetVariableValues(solution);

			constraint[0] = 1 - (0.00139 / (x[0] * x[1]) + 4.94 * x[2] - 0.08);
			constraint[1] = 1 - (0.000306 / (x[0] * x[1]) + 1.082 * x[2] - 0.0986);
			constraint[2] = 50000 - (12.307 / (x[0] * x[1]) + 49408.24 * x[2] + 4051.02);
			constraint[3] = 16000 - (2.098 / (x[0] * x[1]) + 8046.33 * x[2] - 696.71);
			constraint[4] = 10000 - (2.138 / (x[0] * x[1]) + 7883.39 * x[2] - 705.04);
			constraint[5] = 2000 - (0.417 * x[0] * x[1] + 1721.26 * x[2] - 136.54);
			constraint[6] = 550 - (0.164 / (x[0] * x[1]) + 631.13 * x[2] - 54.48);

			double total = 0.0;
			int number = 0;
			for (int i = 0; i < NumberOfConstraints; i++)
			{
				if (constraint[i] < 0.0)
				{
					total += constraint[i];
					number++;
				}
			}

			solution.OverallConstraintViolation = total;
			solution.NumberOfViolatedConstraints = number;
		}

		#endregion

		#region Private Methods

		private double[] GetVariableValues(Solution solution)
		{
			double[] x = new double[3];

			if (SolutionType.GetType() == typeof(BinaryRealSolutionType))
			{
				x[0] = ((BinaryReal)solution.Variable[0]).Value;
				x[1] = ((BinaryReal)solution.Variable[1]).Value;
				x[2] = ((BinaryReal)solution.Variable[2]).Value;
			}
			else
			{
				x[0] = ((Real)solution.Variable[0]).Value;
				x[1] = ((Real)solution.Variable[1]).Value;
				x[2] = ((Real)solution.Variable[2]).Value;
			}
			return x;
		}

		#endregion
	}
}

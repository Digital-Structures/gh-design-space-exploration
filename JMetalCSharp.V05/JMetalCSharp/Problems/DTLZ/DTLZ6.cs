using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.DTLZ
{
	/// <summary>
	/// Class representing problem DTLZ6
	/// </summary>
	public class DTLZ6 : Problem
	{
		#region Constructors

		/// <summary>
		/// Creates a default DTLZ6 problem instance (12 variables and 3 objectives)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		public DTLZ6(string solutionType)
			: this(solutionType, 12, 3)
		{
		}

		/// <summary>
		/// Creates a new DTLZ6 problem instance
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal"</param>
		/// <param name="numberOfVariables">Number of variables</param>
		/// <param name="numberOfObjectives">Number of objective functions</param>
		public DTLZ6(string solutionType, int numberOfVariables, int numberOfObjectives)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = numberOfObjectives;
			NumberOfConstraints = 0;
			ProblemName = "DTLZ6";

			LowerLimit = new double[NumberOfVariables];
			UpperLimit = new double[NumberOfVariables];
			for (int var = 0; var < NumberOfVariables; var++)
			{
				LowerLimit[var] = 0.0;
				UpperLimit[var] = 1.0;
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
			Variable[] gen = solution.Variable;

			double[] x = new double[NumberOfVariables];
			double[] f = new double[NumberOfObjectives];
			double[] theta = new double[NumberOfObjectives - 1];
			int k = NumberOfVariables - NumberOfObjectives + 1;

			for (int i = 0; i < NumberOfVariables; i++)
			{
				x[i] = GetVariableValue(gen[i]);
			}

			double g = 0.0;
			for (int i = NumberOfVariables - k; i < NumberOfVariables; i++)
			{
				g += Math.Pow(x[i], 0.1);
			}

			double t = Math.PI / (4.0 * (1.0 + g));
			theta[0] = x[0] * Math.PI / 2;
			for (int i = 1; i < (NumberOfObjectives - 1); i++)
			{
				theta[i] = t * (1.0 + 2.0 * g * x[i]);
			}

			for (int i = 0; i < NumberOfObjectives; i++)
			{
				f[i] = 1.0 + g;
			}

			for (int i = 0; i < NumberOfObjectives; i++)
			{
				for (int j = 0; j < NumberOfObjectives - (i + 1); j++)
				{
					f[i] *= Math.Cos(theta[j]);
				}
				if (i != 0)
				{
					int aux = NumberOfObjectives - (i + 1);
					f[i] *= Math.Sin(theta[aux]);
				}
			}

			for (int i = 0; i < NumberOfObjectives; i++)
			{
				solution.Objective[i] = f[i];
			}
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

		#endregion
	}
}

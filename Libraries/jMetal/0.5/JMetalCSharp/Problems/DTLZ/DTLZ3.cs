using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.DTLZ
{
	/// <summary>
	/// Class representing problem DTLZ3
	/// </summary>
	public class DTLZ3 : Problem
	{
		#region Constructors

		/// <summary>
		/// Creates a default DTLZ3 problem (12 variables and 3 objectives)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		public DTLZ3(string solutionType)
			: this(solutionType, 12, 3)
		{

		}

		/// <summary>
		/// Creates a new DTLZ3 problem instance
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		/// <param name="numberOfVariables">Number of variables</param>
		/// <param name="numberOfObjectives">Number of objective functions</param>
		public DTLZ3(string solutionType, int numberOfVariables, int numberOfObjectives)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = numberOfObjectives;
			NumberOfConstraints = 0;
			ProblemName = "DTLZ3";

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
			int k = NumberOfVariables - NumberOfObjectives + 1;

			for (int i = 0; i < NumberOfVariables; i++)
			{
				x[i] = GetVariableValue(gen[i]);
			}

			double g = 0.0;
			for (int i = NumberOfVariables - k; i < NumberOfVariables; i++)
			{
				g += (x[i] - 0.5) * (x[i] - 0.5) - Math.Cos(20.0 * Math.PI * (x[i] - 0.5));
			}

			g = 100.0 * (k + g);
			for (int i = 0; i < NumberOfObjectives; i++)
			{
				f[i] = 1.0 + g;
			}

			for (int i = 0; i < NumberOfObjectives; i++)
			{
				for (int j = 0; j < NumberOfObjectives - (i + 1); j++)
				{
					f[i] *= Math.Cos(x[j] * 0.5 * Math.PI);
				}

				if (i != 0)
				{
					int aux = NumberOfObjectives - (i + 1);
					f[i] *= Math.Sin(x[aux] * 0.5 * Math.PI);
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

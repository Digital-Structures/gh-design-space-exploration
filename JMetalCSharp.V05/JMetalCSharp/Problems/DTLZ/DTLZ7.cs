using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.DTLZ
{
	/// <summary>
	/// Class representing problem DTLZ7
	/// </summary>
	public class DTLZ7 : Problem
	{
		#region Constructors

		/// <summary>
		/// Creates a default DTLZ7 problem instance (22 variables and 3 objectives)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		public DTLZ7(string solutionType)
			: this(solutionType, 22, 3)
		{

		}

		/// <summary>
		/// Creates a new DTLZ7 problem instance
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		/// <param name="numberOfVariables">Number of variables</param>
		/// <param name="numberOfObjectives">Number of objective functions</param>
		public DTLZ7(string solutionType, int numberOfVariables, int numberOfObjectives)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = numberOfObjectives;
			NumberOfConstraints = 0;
			ProblemName = "DTLZ7";

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

			//Calculate g
			double g = 0.0;
			for (int i = this.NumberOfVariables - k; i < NumberOfVariables; i++)
			{
				g += x[i];
			}

			g = 1 + (9.0 * g) / k;
			//<-

			//Calculate the value of f1,f2,f3,...,fM-1 (take acount of vectors start at 0)
			Array.Copy(x, 0, f, 0, NumberOfObjectives - 1);
			//<-

			//->Calculate fM
			double h = 0.0;
			for (int i = 0; i < NumberOfObjectives - 1; i++)
			{
				h += (f[i] / (1.0 + g)) * (1 + Math.Sin(3.0 * Math.PI * f[i]));
			}

			h = NumberOfObjectives - h;

			f[NumberOfObjectives - 1] = (1 + g) * h;
			//<-

			//-> Setting up the value of the objetives
			for (int i = 0; i < NumberOfObjectives; i++)
			{
				solution.Objective[i] = f[i];
			}
			//<-
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

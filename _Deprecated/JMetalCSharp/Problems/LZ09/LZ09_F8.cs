using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Problems.LZ09
{
	/// <summary>
	/// Class representing problem LZ09_F8
	/// </summary>
	public class LZ09_F8 : Problem
	{
		#region Private Attributes

		LZ09 LZ09;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a default LZ09_F8 problem (10 variables and 2 objectives)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		public LZ09_F8(string solutionType)
			: this(solutionType, 21, 4, 21)
		{
		}

		/// <summary>
		/// Creates a LZ09_F8 problem instance
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		/// <param name="ptype"></param>
		/// <param name="dtype"></param>
		/// <param name="ltype"></param>
		public LZ09_F8(string solutionType, int ptype, int dtype, int ltype)
		{
			NumberOfVariables = 10;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "LZ09_F8";

			LZ09 = new LZ09(NumberOfVariables, NumberOfObjectives, ptype, dtype, ltype);

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

			List<double> x = new List<double>(NumberOfVariables);
			List<double> y = new List<double>(NumberOfObjectives);

			for (int i = 0; i < NumberOfVariables; i++)
			{
				x.Add(LZ09.GetVariableValue(gen[i], SolutionType));
				y.Add(0.0);
			}

			LZ09.Objective(x, y);

			for (int i = 0; i < NumberOfObjectives; i++)
			{
				solution.Objective[i] = y[i];
			}
		}

		#endregion
	}
}

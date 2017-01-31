using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;

namespace JMetalCSharp.Problems.ZDT
{
	/// <summary>
	/// Class representing problem ZDT2
	/// </summary>
	public class ZDT2 : Problem
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// Creates a default instance of  problem ZDT2 (30 decision variables)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real", "BinaryReal, and "ArrayReal".</param>
		public ZDT2(string solutionType)
			: this(solutionType, 30)
		{

		}

		/// <summary>
		/// Constructor.
		/// Creates a new ZDT2 problem instance.
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal", and "ArrayReal".</param>
		/// <param name="numberOfVariables">Number of variables</param>
		public ZDT2(string solutionType, int numberOfVariables)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "ZDT2";

			UpperLimit = new double[NumberOfVariables];
			LowerLimit = new double[NumberOfVariables];

			for (int i = 0; i < NumberOfVariables; i++)
			{
				LowerLimit[i] = 0;
				UpperLimit[i] = 1;
			}

			if (solutionType == "BinaryReal")
			{
				SolutionType = new BinaryRealSolutionType(this);
			}
			else if (solutionType == "Real")
			{
				SolutionType = new RealSolutionType(this);
			}
			else if (solutionType == "ArrayReal")
			{
				SolutionType = new ArrayRealSolutionType(this);
			}
			else
			{
				Console.WriteLine("Error: solution type " + solutionType + " is invalid");
				Logger.Log.Error("Solution type " + solutionType + " is invalid");
				return;
			}
		}

		#endregion

		#region Public overrides

		/// <summary>
		/// Evaluates a solution 
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			XReal x = new XReal(solution);
			double[] fx = new double[NumberOfObjectives];
			fx[0] = x.GetValue(0);
			double g = this.EvalG(x);
			double h = this.EvalH(fx[0], g);
			fx[1] = h * g;

			solution.Objective[0] = fx[0];
			solution.Objective[1] = fx[1];

		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the value of the ZDT2 function G.
		/// </summary>
		/// <param name="x">Solution</param>
		/// <returns></returns>
		private double EvalG(XReal x)
		{
			double g = 0;
			for (int i = 1; i < x.GetNumberOfDecisionVariables(); i++)
			{
				g += x.GetValue(i);
			}
			double constant = (9.0 / (NumberOfVariables - 1));
			g = constant * g;
			g += 1.0;

			return g;
		}

		/// <summary>
		/// Returns the value of the ZDT2 function H
		/// </summary>
		/// <param name="f">First argument of the function H.</param>
		/// <param name="g">Second argument of the function H.</param>
		/// <returns></returns>
		private double EvalH(double f, double g)
		{
			double h = 0.0;
			h = 1.0 - Math.Pow(f / g, 2.0);
			return h;
		}

		#endregion
	}
}

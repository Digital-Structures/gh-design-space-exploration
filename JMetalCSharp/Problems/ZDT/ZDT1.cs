using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;

namespace JMetalCSharp.Problems.ZDT
{
	/// <summary>
	/// Class representing problem ZDT1
	/// </summary>
	public class ZDT1 : Problem
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// Creates a default instance of problem ZDT1 (30 decision variables)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real", "BinaryReal, and "ArrayReal".</param>
		public ZDT1(string solutionType)
			: this(solutionType, 30)
		{

		}

		/// <summary>
		/// Creates a new instance of problem ZDT1.
		/// </summary>
		/// <param name="solutionType">Number of variables.</param>
		/// <param name="numberOfVariables">The solution type must "Real", "BinaryReal, and "ArrayReal".</param>
		public ZDT1(string solutionType, int numberOfVariables)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "ZDT1";

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

		#region Public override

		/// <summary>
		/// Evaluates a solution 
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			XReal x = new XReal(solution);

			double[] f = new double[NumberOfObjectives];
			f[0] = x.GetValue(0);
			double g = this.EvalG(x);
			double h = this.EvalH(f[0], g);
			f[1] = h * g;

			solution.Objective[0] = f[0];
			solution.Objective[1] = f[1];
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the value of the ZDT1 function G.
		/// </summary>
		/// <param name="x">Solution</param>
		/// <returns></returns>
		private double EvalG(XReal x)
		{
			double g = 0;
			double tmp;
			for (int i = 1; i < x.GetNumberOfDecisionVariables(); i++)
			{
				tmp = x.GetValue(i);
				g += tmp;
			}
			double constant = (9.0 / (NumberOfVariables - 1));
			g = constant * g;
			g = g + 1;
			return g;
		}

		/// <summary>
		/// Returns the value of the ZDT1 function H.
		/// </summary>
		/// <param name="f">First argument of the function H.</param>
		/// <param name="g">Second argument of the function H.</param>
		/// <returns></returns>
		private double EvalH(double f, double g)
		{
			double h = 0;
			h = 1 - Math.Sqrt(f / g);
			return h;
		}

		#endregion
	}
}

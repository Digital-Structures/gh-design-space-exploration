using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;

namespace JMetalCSharp.Problems.ZDT
{
	/// <summary>
	/// Class representing problem ZDT6
	/// </summary>
	public class ZDT6 : Problem
	{
		#region Constructors

		/// <summary>
		/// Creates a default instance of problem ZDT6 (10 decision variables)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real", "BinaryReal, and "ArrayReal".</param>
		public ZDT6(string solutionType)
			: this(solutionType, 10)
		{

		}

		/// <summary>
		/// Creates a instance of problem ZDT6
		/// </summary>
		/// <param name="solutionType">The solution type must "Real", "BinaryReal, and "ArrayReal".</param>
		/// <param name="numberOfVariables">Number of variables</param>
		public ZDT6(string solutionType, int numberOfVariables)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "ZDT6";

			LowerLimit = new double[NumberOfVariables];
			UpperLimit = new double[NumberOfVariables];

			for (int i = 0; i < NumberOfVariables; i++)
			{
				LowerLimit[i] = 0.0;
				UpperLimit[i] = 1.0;
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

		#region Public Overrides

		public override void Evaluate(Solution solution)
		{
			XReal x = new XReal(solution);

			double x1 = x.GetValue(0);
			double[] f = new double[NumberOfObjectives];
			f[0] = 1.0 - Math.Exp((-4.0) * x1) * Math.Pow(Math.Sin(6.0 * Math.PI * x1), 6.0);
			double g = this.EvalG(x);
			double h = this.EvalH(f[0], g);
			f[1] = h * g;

			solution.Objective[0] = f[0];
			solution.Objective[1] = f[1];
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the value of the ZDT6 function G.
		/// </summary>
		/// <param name="x">Solution</param>
		/// <returns></returns>
		private double EvalG(XReal x)
		{
			double g = 0.0;
			for (int i = 1; i < NumberOfVariables; i++)
			{
				g += x.GetValue(i);
			}
			g = g / (NumberOfVariables - 1);
			g = Math.Pow(g, 0.25);
			g = 9.0 * g;
			g = 1.0 + g;
			return g;
		}

		/// <summary>
		/// Returns the value of the ZDT6 function H.
		/// </summary>
		/// <param name="f">First argument of the function H.</param>
		/// <param name="g">Second argument of the function H.</param>
		/// <returns></returns>
		private double EvalH(double f, double g)
		{
			return 1.0 - Math.Pow((f / g), 2.0);
		}

		#endregion
	}
}

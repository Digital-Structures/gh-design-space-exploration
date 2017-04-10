using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;

namespace JMetalCSharp.Problems.ZDT
{
	/// <summary>
	/// Class representing problem ZDT4
	/// </summary>
	public class ZDT4 : Problem
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// Creates a default instance of  problem ZDT4 (10 decision variables)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real", "BinaryReal, and "ArrayReal".</param>
		public ZDT4(string solutionType)
			: this(solutionType, 10)
		{

		}

		/// <summary>
		/// Constructor.
		/// Creates a new ZDT4 problem instance.
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal", and "ArrayReal".</param>
		/// <param name="numberOfVariables">Number of variables</param>
		public ZDT4(string solutionType, int numberOfVariables)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "ZDT4";

			UpperLimit = new double[NumberOfVariables];
			LowerLimit = new double[NumberOfVariables];

			LowerLimit[0] = 0.0;
			UpperLimit[0] = 1.0;

			for (int i = 1; i < NumberOfVariables; i++)
			{
				LowerLimit[i] = -5.0;
				UpperLimit[i] = 5.0;
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
		/// Returns the value of the ZDT4 function G.
		/// </summary>
		/// <param name="x">Solution</param>
		/// <returns></returns>
		public double EvalG(XReal x)
		{
			double g = 0.0;
			for (int i = 1; i < NumberOfVariables; i++)
				g += Math.Pow(x.GetValue(i), 2.0) +
					-10.0 * Math.Cos(4.0 * Math.PI * x.GetValue(i));

			double constante = 1.0 + 10.0 * (NumberOfVariables - 1);
			return g + constante;
		}

		/// <summary>
		/// Returns the value of the ZDT4 function H.
		/// </summary>
		/// <param name="f">First argument of the function H.</param>
		/// <param name="g">Second argument of the function H.</param>
		/// <returns></returns>
		public double EvalH(double f, double g)
		{
			return 1.0 - Math.Sqrt(f / g);
		}

		#endregion
	}
}

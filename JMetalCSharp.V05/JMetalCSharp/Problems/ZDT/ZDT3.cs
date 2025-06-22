using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;

namespace JMetalCSharp.Problems.ZDT
{
	/// <summary>
	/// 
	/// </summary>
	public class ZDT3 : Problem
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// Creates a default instance of  problem ZDT3 (30 decision variables)
		/// </summary>
		/// <param name="solutionType">The solution type must "Real", "BinaryReal, and "ArrayReal".</param>
		public ZDT3(string solutionType)
			: this(solutionType, 30)
		{

		}

		/// <summary>
		/// Constructor.
		/// Creates a new ZDT3 problem instance.
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal", and "ArrayReal".</param>
		/// <param name="numberOfVariables">Number of variables</param>
		public ZDT3(string solutionType, int numberOfVariables)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "ZDT3";

			UpperLimit = new double[NumberOfVariables];
			LowerLimit = new double[NumberOfVariables];

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

		#region Public overrides

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

		#region Private Region

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public double EvalG(XReal x)
		{
			double g = 0.0;
			for (int i = 1; i < x.GetNumberOfDecisionVariables(); i++)
			{
				g += x.GetValue(i);
			}
			double constant = (9.0 / (NumberOfVariables - 1));
			g = constant * g;
			g = g + 1.0;
			return g;
		}

		public double EvalH(double f, double g)
		{
			double h = 0.0;
			h = 1.0 - Math.Sqrt(f / g) - (f / g) * Math.Sin(10.0 * Math.PI * f);
			return h;
		}

		#endregion
	}
}

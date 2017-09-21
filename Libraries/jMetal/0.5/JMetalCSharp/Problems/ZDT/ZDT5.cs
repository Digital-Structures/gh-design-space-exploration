using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.ZDT
{
	/// <summary>
	/// Class representing problem ZDT5
	/// </summary>
	public class ZDT5 : Problem
	{
		#region Constructors

		/// <summary>
		/// Creates a default instance of problem ZDT5 (11 decision variables).
		/// This problem allows only "Binary" representations.
		/// </summary>
		/// <param name="solutionType">The solution</param>
		public ZDT5(string solutionType)
			: this(solutionType, 11)
		{

		}

		/// <summary>
		/// Creates a instance of problem ZDT5
		/// This problem allows only "Binary" representations.
		/// </summary>
		/// <param name="solutionType">Number of variables.</param>
		/// <param name="numberOfVariables"></param>
		public ZDT5(string solutionType, int numberOfVariables)
		{
			NumberOfVariables = numberOfVariables;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "ZDT5";

			Length = new int[NumberOfVariables];
			Length[0] = 30;
			for (int i = 1; i < NumberOfVariables; i++)
			{
				Length[i] = 5;
			}

			if (solutionType == "Binary")
			{
				SolutionType = new BinarySolutionType(this);
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

		/// <summary>
		/// Evaluates a solution
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			double[] f = new double[NumberOfObjectives];
			f[0] = 1 + U((Binary)solution.Variable[0]);
			double g = EvalG(solution.Variable);
			double h = EvalH(f[0], g);
			f[1] = h * g;

			solution.Objective[0] = f[0];
			solution.Objective[1] = f[1];
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the value of the ZDT5 function G.
		/// </summary>
		/// <param name="decisionVariables">The decision variables of the solution to evaluate.</param>
		/// <returns></returns>
		private double EvalG(Variable[] decisionVariables)
		{
			double res = 0.0;

			for (int i = 1; i < NumberOfVariables; i++)
			{
				res += EvalV(U((Binary)decisionVariables[i]));
			}

			return res;
		}

		/// <summary>
		/// Returns the value of the ZDT5 function V.
		/// </summary>
		/// <param name="value">The parameter of V function.</param>
		/// <returns></returns>
		private double EvalV(double value)
		{
			double res;
			if (value < 5.0)
			{
				res = 2.0 + value;
			}
			else
			{
				res = 1.0;
			}

			return res;
		}

		/// <summary>
		/// Returns the value of the ZDT5 function H.
		/// </summary>
		/// <param name="f">First argument of the function H.</param>
		/// <param name="g">Second argument of the function H.</param>
		/// <returns></returns>
		private double EvalH(double f, double g)
		{
			return 1 / f;
		}

		/// <summary>
		/// Returns the u value defined in ZDT5 for a encodings.variable.
		/// </summary>
		/// <param name="variable"></param>
		/// <returns></returns>
		private double U(Binary variable)
		{
			return variable.Bits.Cardinality();
		}

		#endregion
	}
}

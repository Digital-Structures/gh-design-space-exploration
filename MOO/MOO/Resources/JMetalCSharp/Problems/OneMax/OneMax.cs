using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.OneMax
{
	/// <summary>
	/// Class representing problem OneMax. The problem consist of maximizing the
	/// number of '1's in a binary string.
	/// </summary>
	public class OneMax : Problem
	{
		/// <summary>
		/// Creates a new OneZeroMax problem instance
		/// </summary>
		/// <param name="solutionType">Solution type</param>
		public OneMax(string solutionType)
			: this(solutionType, 512)
		{

		}

		/// <summary>
		/// Creates a new OneMax problem instance
		/// </summary>
		/// <param name="solutionType"></param>
		/// <param name="numberOfBits">Length of the problem</param>
		public OneMax(string solutionType, int numberOfBits)
		{
			NumberOfVariables = 1;
			NumberOfObjectives = 1;
			NumberOfConstraints = 0;
			ProblemName = "ONEMAX";

			this.Length = new int[NumberOfVariables];
			Length[0] = numberOfBits;

			if (solutionType == "Binary")
				this.SolutionType = new BinarySolutionType(this);
			else
			{
				Console.WriteLine("Error: solution type " + solutionType + " is invalid");
				Logger.Log.Error("Solution type " + solutionType + " is invalid");
				return;
			}

		}

		/// <summary>
		/// Evaluates a solution 
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			Binary variable;
			int counter;

			variable = ((Binary)solution.Variable[0]);

			counter = 0;

			for (int i = 0; i < variable.NumberOfBits; i++)
			{
				if (variable.Bits[i])
				{
					counter++;
				}
			}

			// OneMax is a maximization problem: multiply by -1 to minimize
			solution.Objective[0] = -1.0 * counter;
		}
	}
}

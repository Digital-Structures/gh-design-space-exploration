using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.OneZeroMax
{
	/// <summary>
	/// Class representing problem OneZeroMax. The problem consist of maximizing the
	/// number of '1's and '0's in a binary string.
	/// </summary>
	public class OneZeroMax : Problem
	{
		/// <summary>
		/// Creates a new OneZeroMax problem instance
		/// </summary>
		/// <param name="solutionType">Solution type</param>
		public OneZeroMax(string solutionType)
			: this(solutionType, 512)
		{
		}

		/// <summary>
		/// Creates a new OneZeroMax problem instance
		/// </summary>
		/// <param name="solutionType">Solution type</param>
		/// <param name="numberOfBits">Length of the problem</param>
		public OneZeroMax(string solutionType, int numberOfBits)
		{
			NumberOfVariables = 1;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "OneZeroMax";

			Length = new int[NumberOfVariables];
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
			int counterOnes;
			int counterZeroes;

			variable = ((Binary)solution.Variable[0]);

			counterOnes = 0;
			counterZeroes = 0;

			for (int i = 0; i < variable.NumberOfBits; i++)
			{
				if (variable.Bits[i])
				{
					counterOnes++;
				}
				else
				{
					counterZeroes++;
				}
			}

			// OneZeroMax is a maximization problem: multiply by -1 to minimize
			solution.Objective[0] = -1.0 * counterOnes;
			solution.Objective[1] = -1.0 * counterZeroes;
		}
	}
}

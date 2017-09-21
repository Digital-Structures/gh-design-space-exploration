using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{
	/// <summary>
	/// This class implements a swap mutation. The solution type of the solution
	/// must be Permutation.
	/// </summary>
	public class SwapMutation : Mutation
	{
		#region Private Attributes

		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>()
        {
            typeof(PermutationSolutionType)
        };

		private double? mutationProbability = null;

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parameters"></param>
		public SwapMutation(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref mutationProbability);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Performs the operation
		/// </summary>
		/// <param name="probability">Mutation probability</param>
		/// <param name="solution">The solution to mutate</param>
		private void DoMutation(double probability, Solution solution)
		{
			int[] permutation;
			int permutationLength;

			if (solution.Type.GetType() == typeof(PermutationSolutionType))
			{
				permutationLength = ((Permutation)solution.Variable[0]).Size;
				permutation = ((Permutation)solution.Variable[0]).Vector;

				if (JMetalRandom.NextDouble() < probability)
				{
					int pos1;
					int pos2;

					pos1 = JMetalRandom.Next(0, permutationLength - 1);
					pos2 = JMetalRandom.Next(0, permutationLength - 1);

					while (pos1 == pos2)
					{
						if (pos1 == (permutationLength - 1))
						{
							pos2 = JMetalRandom.Next(0, permutationLength - 2);
						}
						else
						{
							pos2 = JMetalRandom.Next(pos1, permutationLength - 1);
						}
					}
					// swap
					int temp = permutation[pos1];
					permutation[pos1] = permutation[pos2];
					permutation[pos2] = temp;
				}
			}
			else
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".DoMutation()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".DoMutation()");
				throw new Exception("Exception in " + this.GetType().FullName + ".DoMutation()");
			}
		}

		#endregion

		#region Public Override

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing the solution to mutate</param>
		/// <returns>An object containing the mutated solution</returns>
		public override object Execute(object obj)
		{
			Solution solution = (Solution)obj;

			if (!VALID_TYPES.Contains(solution.Type.GetType()))
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			this.DoMutation(mutationProbability.Value, solution);

			return solution;
		}

		#endregion
	}
}

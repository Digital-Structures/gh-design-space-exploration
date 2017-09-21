using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Crossover
{

	public class TwoPointsCrossover : Crossover
	{
		#region Private Attributes

		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>()
        {
            typeof(PermutationSolutionType)
        };

		private double? crossoverProbability = null;

		#endregion

		#region Constructors
		/// <summary>
		/// Constructor Creates a new intance of the two point crossover operator
		/// </summary>
		/// <param name="parameters"></param>
		public TwoPointsCrossover(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref crossoverProbability);

		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Perform the crossover operation
		/// </summary>
		/// <param name="probability">Crossover probability</param>
		/// <param name="parent1">The first parent</param>
		/// <param name="parent2">The second parent</param>
		/// <returns>Two offspring solutions</returns>
		private Solution[] DoCrossover(double probability, Solution parent1, Solution parent2)
		{

			Solution[] offspring = new Solution[2];

			offspring[0] = new Solution(parent1);
			offspring[1] = new Solution(parent2);

			if (parent1.Type.GetType() == typeof(PermutationSolutionType))
			{
				if (JMetalRandom.NextDouble() < probability)
				{
					int crosspoint1;
					int crosspoint2;
					int permutationLength;
					int[] parent1Vector;
					int[] parent2Vector;
					int[] offspring1Vector;
					int[] offspring2Vector;

					permutationLength = ((Permutation)parent1.Variable[0]).Size;
					parent1Vector = ((Permutation)parent1.Variable[0]).Vector;
					parent2Vector = ((Permutation)parent2.Variable[0]).Vector;
					offspring1Vector = ((Permutation)offspring[0].Variable[0]).Vector;
					offspring2Vector = ((Permutation)offspring[1].Variable[0]).Vector;

					// STEP 1: Get two cutting points
					crosspoint1 = JMetalRandom.Next(0, permutationLength - 1);
					crosspoint2 = JMetalRandom.Next(0, permutationLength - 1);

					while (crosspoint2 == crosspoint1)
					{
						crosspoint2 = JMetalRandom.Next(0, permutationLength - 1);
					}

					if (crosspoint1 > crosspoint2)
					{
						int swap;
						swap = crosspoint1;
						crosspoint1 = crosspoint2;
						crosspoint2 = swap;
					}

					// STEP 2: Obtain the first child
					int m = 0;
					for (int j = 0; j < permutationLength; j++)
					{
						bool exist = false;
						int temp = parent2Vector[j];
						for (int k = crosspoint1; k <= crosspoint2; k++)
						{
							if (temp == offspring1Vector[k])
							{
								exist = true;
								break;
							}
						}

						if (!exist)
						{
							if (m == crosspoint1)
							{
								m = crosspoint2 + 1;
							}
							offspring1Vector[m++] = temp;
						}
					}

					// STEP 3: Obtain the second child
					m = 0;
					for (int j = 0; j < permutationLength; j++)
					{
						bool exist = false;
						int temp = parent1Vector[j];
						for (int k = crosspoint1; k <= crosspoint2; k++)
						{
							if (temp == offspring2Vector[k])
							{
								exist = true;
								break;
							}
						}
						if (!exist)
						{
							if (m == crosspoint1)
							{
								m = crosspoint2 + 1;
							}
							offspring2Vector[m++] = temp;
						}
					}
				}
			}
			else
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".DoCrossover()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".DoCrossover()");
				throw new Exception("Exception in " + this.GetType().FullName + ".DoCrossover()");
			}

			return offspring;
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing an array of two solutions</param>
		/// <returns>An object containing an array with the offSprings</returns>
		public override object Execute(object obj)
		{
			Solution[] parents = (Solution[])obj;
			double crossoverProbability;

			if (!(VALID_TYPES.Contains(parents[0].Type.GetType())
				&& VALID_TYPES.Contains(parents[1].Type.GetType())))
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			crossoverProbability = (double)GetParameter("probability");

			if (parents.Length < 2)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			Solution[] offspring = DoCrossover(crossoverProbability, parents[0], parents[1]);

			return offspring;
		}

		#endregion
	}
}

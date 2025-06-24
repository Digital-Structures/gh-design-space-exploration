using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Crossover
{
	/// <summary>
	/// This class allows to apply a PMX crossover operator using two parent
	/// solutions. NOTE: the operator is applied to the first encodings.variable of
	/// the solutions, and the type of those variables must be
	/// VariableType_.Permutation.
	/// </summary>
	public class PMXCrossover : Crossover
	{
		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>(){
            typeof (PermutationSolutionType)
        };

		private double? crossoverProbability = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public PMXCrossover(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref crossoverProbability);
		}

		public Solution[] DoCrossover(double probability, Solution parent1, Solution parent2)
		{
			Solution[] offspring = new Solution[2];

			offspring[0] = new Solution(parent1);
			offspring[1] = new Solution(parent2);

			int permutationLength = ((Permutation)parent1.Variable[0]).Size;

			int[] parent1Vector = ((Permutation)parent1.Variable[0]).Vector;
			int[] parent2Vector = ((Permutation)parent2.Variable[0]).Vector;
			int[] offspring1Vector = ((Permutation)offspring[0].Variable[0]).Vector;
			int[] offspring2Vector = ((Permutation)offspring[1].Variable[0]).Vector;

			if (JMetalRandom.NextDouble() < probability)
			{
				int cuttingPoint1;
				int cuttingPoint2;

				// STEP 1: Get two cutting points
				cuttingPoint1 = JMetalRandom.Next(0, permutationLength - 1);
				cuttingPoint2 = JMetalRandom.Next(0, permutationLength - 1);

				while (cuttingPoint1 == cuttingPoint2)
				{
					cuttingPoint2 = JMetalRandom.Next(0, permutationLength - 1);
				}

				if (cuttingPoint1 > cuttingPoint2)
				{
					int swap;
					swap = cuttingPoint1;
					cuttingPoint1 = cuttingPoint2;
					cuttingPoint2 = swap;
				}

				// STEP 2: Get the subchains to interchange
				int[] replacement1 = new int[permutationLength];
				int[] replacement2 = new int[permutationLength];
				for (int i = 0; i < permutationLength; i++)
				{
					replacement1[i] = replacement2[i] = -1;
				}

				// STEP 3: Interchange

				for (int i = cuttingPoint1; i <= cuttingPoint2; i++)
				{
					offspring1Vector[i] = parent2Vector[i];
					offspring2Vector[i] = parent1Vector[i];

					replacement1[parent2Vector[i]] = parent1Vector[i];
					replacement2[parent1Vector[i]] = parent2Vector[i];
				}

				// STEP 4: Repair offsprings

				for (int i = 0; i < permutationLength; i++)
				{
					if ((i >= cuttingPoint1) && (i <= cuttingPoint2))
					{
						continue;
					}

					int n1 = parent1Vector[i];
					int m1 = replacement1[n1];

					int n2 = parent2Vector[i];
					int m2 = replacement2[n2];

					while (m1 != -1)
					{
						n1 = m1;
						m1 = replacement1[m1];
					}
					while (m2 != -1)
					{
						n2 = m2;
						m2 = replacement2[m2];
					}

					offspring1Vector[i] = n1;
					offspring2Vector[i] = n2;
				}
			}

			return offspring;
		}

		public override object Execute(object obj)
		{
			Solution[] parents = (Solution[])obj;
			double? crossoverProb = null;

			if (!(VALID_TYPES.Contains(parents[0].Type.GetType()) &&
				VALID_TYPES.Contains(parents[1].Type.GetType())))
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
			}

			crossoverProb = (double)GetParameter("probability");

			if (parents.Length < 2)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			Solution[] offspring = DoCrossover(crossoverProb.Value,
				parents[0],
				parents[1]);

			return offspring;
		}
	}
}

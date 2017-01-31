using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Crossover
{
	/// <summary>
	/// This class allows to apply a Single Point crossover operator using two parent
	/// solutions.
	/// </summary>
	public class SinglePointCrossover : Crossover
	{

		#region Private Attributes
		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>()
        {
            typeof(BinarySolutionType),
            typeof(BinaryRealSolutionType),
            typeof(IntSolutionType)
        };

		private double? crossoverProbability = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// Creates a new instance of the single point crossover operator
		/// </summary>
		/// <param name="parameters"></param>
		public SinglePointCrossover(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref crossoverProbability);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Perform the crossover operation.
		/// </summary>
		/// <param name="probability">Crossover probability</param>
		/// <param name="parent1">The first parent</param>
		/// <param name="parent2">The second parent</param>
		/// <returns>An array containig the two offsprings</returns>
		private Solution[] DoCrossover(double probability, Solution parent1, Solution parent2)
		{
			Solution[] offSpring = new Solution[2];

			offSpring[0] = new Solution(parent1);
			offSpring[1] = new Solution(parent2);

			try
			{
				if (JMetalRandom.NextDouble() < probability)
				{
					if ((parent1.Type.GetType() == typeof(BinarySolutionType))
						|| (parent1.Type.GetType() == typeof(BinaryRealSolutionType)))
					{
						//1. Compute the total number of bits
						int totalNumberOfBits = 0;
						for (int i = 0; i < parent1.Variable.Length; i++)
						{
							totalNumberOfBits += ((Binary)parent1.Variable[i]).NumberOfBits;
						}

						//2. Calculate the point to make the crossover
						int crossoverPoint = JMetalRandom.Next(0, totalNumberOfBits - 1);

						//3. Compute the encodings.variable containing the crossoverPoint bit
						int variable = 0;
						int acountBits = ((Binary)parent1.Variable[variable]).NumberOfBits;

						while (acountBits < (crossoverPoint + 1))
						{
							variable++;
							acountBits += ((Binary)parent1.Variable[variable]).NumberOfBits;
						}

						//4. Compute the bit into the selected encodings.variable
						int diff = acountBits - crossoverPoint;
						int intoVariableCrossoverPoint = ((Binary)parent1.Variable[variable]).NumberOfBits - diff;

						//5. Make the crossover into the gene;
						Binary offSpring1, offSpring2;
						offSpring1 = (Binary)parent1.Variable[variable].DeepCopy();
						offSpring2 = (Binary)parent2.Variable[variable].DeepCopy();

						for (int i = intoVariableCrossoverPoint; i < offSpring1.NumberOfBits; i++)
						{
							bool swap = offSpring1.Bits[i];
							offSpring1.Bits[i] = offSpring2.Bits[i];
							offSpring2.Bits[i] = swap;
						}

						offSpring[0].Variable[variable] = offSpring1;
						offSpring[1].Variable[variable] = offSpring2;

						//6. Apply the crossover to the other variables
						for (int i = 0; i < variable; i++)
						{
							offSpring[0].Variable[i] = parent2.Variable[i].DeepCopy();

							offSpring[1].Variable[i] = parent1.Variable[i].DeepCopy();

						}

						//7. Decode the results
						for (int i = 0; i < offSpring[0].Variable.Length; i++)
						{
							((Binary)offSpring[0].Variable[i]).Decode();
							((Binary)offSpring[1].Variable[i]).Decode();
						}
					} // Binary or BinaryReal
					else
					{ // Integer representation
						int crossoverPoint = JMetalRandom.Next(0, parent1.NumberOfVariables() - 1);
						int valueX1;
						int valueX2;
						for (int i = crossoverPoint; i < parent1.NumberOfVariables(); i++)
						{
							valueX1 = ((Int)parent1.Variable[i]).Value;
							valueX2 = ((Int)parent2.Variable[i]).Value;
							((Int)offSpring[0].Variable[i]).Value = valueX2;
							((Int)offSpring[1].Variable[i]).Value = valueX1;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Error in: " + this.GetType().FullName + ".DoCrossover()", ex);
				Console.WriteLine("Error in " + this.GetType().FullName + ".DoCrossover()");
				throw new Exception("Exception in " + this.GetType().FullName + ".DoCrossover()");
			}
			return offSpring;
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

			if (!(VALID_TYPES.Contains(parents[0].Type.GetType())
				&& VALID_TYPES.Contains(parents[1].Type.GetType())))
			{
				Logger.Log.Error("Error in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Error in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			if (parents.Length < 2)
			{
				Logger.Log.Error("Error in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Error in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			Solution[] offSpring;
			offSpring = DoCrossover(crossoverProbability.Value, parents[0], parents[1]);

			//-> Update the offSpring solutions
			for (int i = 0; i < offSpring.Length; i++)
			{
				offSpring[i].CrowdingDistance = 0.0;
				offSpring[i].Rank = 0;
			}
			return offSpring;
		}

		#endregion
	}
}

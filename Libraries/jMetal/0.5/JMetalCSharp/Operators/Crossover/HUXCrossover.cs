using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Crossover
{
	/// <summary>
	/// This class allows to apply a HUX crossover operator using two parent
	/// solutions.
	/// NOTE: the operator is applied to the first encodings.variable of the solutions, and
	/// the type of the solutions must be Binary or BinaryReal
	/// </summary>
	public class HUXCrossover : Crossover
	{
		/// <summary>
		/// Valid solution types to apply this operator 
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>(){
            typeof(BinarySolutionType),
            typeof(BinaryRealSolutionType)
        };

		private double? probability = null;

		/// <summary>
		/// Constructor
		/// Create a new instance of the HUX crossover operator.
		/// </summary>
		/// <param name="parameters"></param>
		public HUXCrossover(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref probability);
		}

		/// <summary>
		/// Perform the crossover operation
		/// </summary>
		/// <param name="probability">Crossover probability</param>
		/// <param name="parent1">The first parent</param>
		/// <param name="parent2">The second parent</param>
		/// <returns>An array containing the two offsprings</returns>
		public Solution[] DoCrossover(double? probability, Solution parent1, Solution parent2)
		{
			Solution[] offSpring = new Solution[2];

			offSpring[0] = new Solution(parent1);
			offSpring[1] = new Solution(parent2);

			try
			{
				if (JMetalRandom.NextDouble() < probability)
				{
					for (int i = 0; i < parent1.Variable.Length; i++)
					{
						Binary p1 = (Binary)parent1.Variable[i];
						Binary p2 = (Binary)parent2.Variable[i];

						for (int bit = 0; bit < p1.NumberOfBits; bit++)
						{
							if (p1.Bits[bit] != p2.Bits[bit])
							{
								if (JMetalRandom.NextDouble() < 0.5)
								{
									((Binary)offSpring[0].Variable[i])
									.Bits[bit] = p2.Bits[bit];
									((Binary)offSpring[1].Variable[i])
									.Bits[bit] = p1.Bits[bit];
								}
							}
						}
					}
					//7. Decode the results
					for (int i = 0; i < offSpring[0].Variable.Length; i++)
					{
						((Binary)offSpring[0].Variable[i]).Decode();
						((Binary)offSpring[1].Variable[i]).Decode();
					}
				}
			}
			catch (InvalidCastException ex)
			{
				Logger.Log.Error("Error in " + this.GetType().FullName + ".DoCrossover()", ex);
				Console.WriteLine("Error in " + this.GetType().FullName + ".DoCrossover()");
				throw new Exception("Exception in " + this.GetType().FullName + ".DoCrossover()");
			}
			return offSpring;
		}

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing an array of two solutions</param>
		/// <returns>An object containing the offSprings</returns>
		public override object Execute(object obj)
		{
			Solution[] parents = (Solution[])obj;

			if (parents.Length < 2)
			{
				Logger.Log.Error("Error in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Error in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			if (!(VALID_TYPES.Contains(parents[0].Type.GetType()) &&
				VALID_TYPES.Contains(parents[1].Type.GetType())))
			{

				Logger.Log.Error("Error in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Error in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			Solution[] offSpring = DoCrossover(probability, parents[0], parents[1]);

			for (int i = 0; i < offSpring.Length; i++)
			{
				offSpring[i].CrowdingDistance = 0.0;
				offSpring[i].Rank = 0;
			}

			return offSpring;
		}
	}
}

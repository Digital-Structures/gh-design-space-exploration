using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{
	/// <summary>
	/// This class implements a bit flip mutation operator.
	/// NOTE: the operator is applied to binary or integer solutions, considering the
	/// whole solution as a single encodings.variable.
	/// </summary>
	public class BitFlipMutation : Mutation
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

		private double? mutationProbability = null;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// Creates a new instance of the Bit Flip mutation operator
		/// </summary>
		/// <param name="parameters"></param>
		public BitFlipMutation(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref mutationProbability);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Perform the mutation operation
		/// </summary>
		/// <param name="probability">Mutation probability</param>
		/// <param name="solution">The solution to mutate</param>
		private void DoMutation(double probability, Solution solution)
		{
			try
			{
				if ((solution.Type.GetType() == typeof(BinarySolutionType))
					|| (solution.Type.GetType() == typeof(BinaryRealSolutionType)))
				{
					for (int i = 0; i < solution.Variable.Length; i++)
					{
						for (int j = 0; j < ((Binary)solution.Variable[i]).NumberOfBits; j++)
						{
							if (JMetalRandom.NextDouble() < probability)
							{
								((Binary)solution.Variable[i]).Bits.Flip(j);
							}
						}
					}

					for (int i = 0; i < solution.Variable.Length; i++)
					{
						((Binary)solution.Variable[i]).Decode();
					}
				}
				else
				{ // Integer representation
					for (int i = 0; i < solution.Variable.Length; i++)
					{
						if (JMetalRandom.NextDouble() < probability)
						{
							int value = JMetalRandom.Next(
								(int)solution.Variable[i].LowerBound,
								(int)solution.Variable[i].UpperBound);
							solution.Variable[i].Value = value;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Error in " + this.GetType().FullName + ".DoMutation()", ex);
				Console.WriteLine("Error in " + this.GetType().FullName + ".DoMutation()");
				throw new Exception("Exception in " + this.GetType().FullName + ".DoMutation()");
			}
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing a solution to mutate</param>
		/// <returns>An object containing the mutated solution</returns>
		public override object Execute(object obj)
		{
			Solution solution = (Solution)obj;

			if (!VALID_TYPES.Contains(solution.Type.GetType()))
			{
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			DoMutation(mutationProbability.Value, solution);
			return solution;
		}

		#endregion
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{
	/// <summary>
	/// This class implements a uniform mutation operator.
	/// </summary>
	public class UniformMutation : Mutation
	{
		#region Private Attributes
		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>()
        {
            typeof(RealSolutionType),
            typeof(ArrayRealSolutionType)
        };

		/// <summary>
		/// Stores the value used in a uniform mutation operator
		/// </summary>
		private double? perturbation;

		private double? mutationProbability = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor 
		/// Creates a new uniform mutation operator instance
		/// </summary>
		/// <param name="parameters"></param>
		public UniformMutation(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref mutationProbability);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "perturbation", ref perturbation);
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
			XReal x = new XReal(solution);

			for (int var = 0; var < solution.Variable.Length; var++)
			{
				if (JMetalRandom.NextDouble() < probability)
				{
					double rand = JMetalRandom.NextDouble();
					double tmp = (rand - 0.5) * perturbation.Value;

					tmp += x.GetValue(var);

					if (tmp < x.GetLowerBound(var))
					{
						tmp = x.GetLowerBound(var);
					}
					else if (tmp > x.GetUpperBound(var))
					{
						tmp = x.GetUpperBound(var);
					}

					x.SetValue(var, tmp);
				}
			}
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing the solution to mutate</param>
		/// <returns></returns>
		public override object Execute(object obj)
		{
			Solution solution = (Solution)obj;

			if (!VALID_TYPES.Contains(solution.Type.GetType()))
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			DoMutation(mutationProbability.Value, solution);

			return solution;
		}

		#endregion
	}
}

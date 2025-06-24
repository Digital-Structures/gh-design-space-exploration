using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{

	/// <summary>
	/// This class implements a non-uniform mutation operator.
	/// </summary>
	public class NonUniformMutation : Mutation
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
		/// Stores the perturbation value used in the Non Uniform mutation operator
		/// </summary>
		private double? perturbation = null;

		/// <summary>
		/// Stores the maximun number of iterations.
		/// </summary>
		private int maxIterations;

		/// <summary>
		/// Stores the iteration in which the operator is going to be applied
		/// </summary>
		private int? currentIteration = null;

		private double? mutationProbability = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// Creates a new instance of the non uniform mutation
		/// </summary>
		/// <param name="parameters"></param>
		public NonUniformMutation(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref mutationProbability);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "perturbation", ref perturbation);
			Utils.Utils.GetIntValueFromParameter(parameters, "maxIterations", ref maxIterations);
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
			XReal x = new XReal(solution);

			for (int var = 0; var < solution.Variable.Length; var++)
			{
				if (JMetalRandom.NextDouble() < probability)
				{
					double rand = JMetalRandom.NextDouble();
					double tmp;

					if (rand <= 0.5)
					{
						tmp = Delta(x.GetUpperBound(var) - x.GetValue(var), perturbation.Value);
						tmp += x.GetValue(var);
					}
					else
					{
						tmp = Delta(x.GetLowerBound(var) - x.GetValue(var), perturbation.Value);
						tmp += x.GetValue(var);
					}

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

		/// <summary>
		/// Calculates the delta value used in NonUniform mutation operator
		/// </summary>
		/// <param name="y"></param>
		/// <param name="bMutationParameter"></param>
		/// <returns></returns>
		private double Delta(double y, double bMutationParameter)
		{
			double rand = JMetalRandom.NextDouble();
			int it, maxIt;
			it = currentIteration.Value;
			maxIt = maxIterations;

			return (y * (1.0 - Math.Pow(rand, Math.Pow((1.0 - it / (double)maxIt), bMutationParameter))));
		}

		#endregion

		#region Public Override

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing a solution</param>
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

			if (GetParameter("currentIteration") != null)
			{
				currentIteration = (int)GetParameter("currentIteration");
			}

			DoMutation(mutationProbability.Value, solution);

			return solution;
		}

		#endregion
	}
}

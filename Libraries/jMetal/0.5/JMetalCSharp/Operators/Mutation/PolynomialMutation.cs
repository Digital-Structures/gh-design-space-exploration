using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{
	/// <summary>
	/// This class implements a polynomial mutation operator. 
	/// </summary>
	public class PolynomialMutation : Mutation
	{
		#region Private Attributes

		private static readonly double ETA_M_DEFAULT = 20.0;
		private static readonly double eta_m = ETA_M_DEFAULT;

		private double? mutationProbability = null;
		private double distributionIndex = eta_m;

		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>() { typeof(RealSolutionType), typeof(ArrayRealSolutionType) };

		#endregion

		#region Constructors
		/// <summary>
		/// Constructor.
		/// Creates a new instance of the polynomial mutation operator
		/// </summary>
		/// <param name="parameters"></param>
		public PolynomialMutation(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref mutationProbability);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "distributionIndex", ref distributionIndex);
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
			double rnd, delta1, delta2, mut_pow, deltaq;
			double y, yl, yu, val, xy;

			XReal x = new XReal(solution);
			for (int var = 0; var < solution.NumberOfVariables(); var++)
			{
				if (JMetalRandom.NextDouble() <= probability)
				{
					y = x.GetValue(var);
					yl = x.GetLowerBound(var);
					yu = x.GetUpperBound(var);
					delta1 = (y - yl) / (yu - yl);
					delta2 = (yu - y) / (yu - yl);
					rnd = JMetalRandom.NextDouble();
					mut_pow = 1.0 / (eta_m + 1.0);
					if (rnd <= 0.5)
					{
						xy = 1.0 - delta1;
						val = 2.0 * rnd + (1.0 - 2.0 * rnd) * (Math.Pow(xy, (distributionIndex + 1.0)));
						deltaq = Math.Pow(val, mut_pow) - 1.0;
					}
					else
					{
						xy = 1.0 - delta2;
						val = 2.0 * (1.0 - rnd) + 2.0 * (rnd - 0.5) * (Math.Pow(xy, (distributionIndex + 1.0)));
						deltaq = 1.0 - (Math.Pow(val, mut_pow));
					}
					y = y + deltaq * (yu - yl);
					if (y < yl)
					{
						y = yl;
					}
					if (y > yu)
					{
						y = yu;
					}
					x.SetValue(var, y);
				}
			}
		}

		#endregion

		#region Public Methods

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
				throw new Exception("Exception in " + this.GetType().FullName + ".execute()");
			}

			DoMutation(mutationProbability.Value, solution);

			return solution;
		}

		#endregion
	}
}

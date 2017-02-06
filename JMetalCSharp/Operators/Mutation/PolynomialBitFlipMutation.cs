using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Mutation
{
	public class PolynomialBitFlipMutation : Mutation
	{
		#region Private Attributes
		private static readonly double ETA_M_DEFAULT = 20.0;
		private static double eta_m = ETA_M_DEFAULT;

		private double? realMutationProbability = null;
		private double? binaryMutationProbability = null;
		private double distributionIndex = eta_m;

		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private readonly List<Type> VALID_TYPES = new List<Type>()
        {
            typeof(ArrayRealAndBinarySolutionType)
        };

		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parameters"></param>
		public PolynomialBitFlipMutation(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "realMutationProbability", ref realMutationProbability);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "binaryMutationProbability", ref binaryMutationProbability);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "distributionIndex", ref distributionIndex);
		}

		#endregion

		#region Public Overrides

		public override object Execute(object obj)
		{
			Solution solution = (Solution)obj;

			if (!VALID_TYPES.Contains(solution.Type.GetType()))
			{
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			DoMutation(realMutationProbability.Value, binaryMutationProbability.Value, solution);
			return solution;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// DoMutation method
		/// </summary>
		/// <param name="realProbability"></param>
		/// <param name="binaryProbability"></param>
		/// <param name="solution"></param>
		private void DoMutation(double realProbability, double binaryProbability, Solution solution)
		{
			double rnd, delta1, delta2, mut_pow, deltaq;
			double y, yl, yu, val, xy;

			XReal x = new XReal(solution);

			Binary binaryVariable = (Binary)solution.Variable[1];

			// Polynomial mutation applied to the array real
			for (int var = 0; var < x.Size(); var++)
			{
				if (JMetalRandom.NextDouble() <= realProbability)
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

			// BitFlip mutation applied to the binary part
			for (int i = 0; i < binaryVariable.NumberOfBits; i++)
			{
				if (JMetalRandom.NextDouble() < binaryProbability)
				{
					binaryVariable.Bits.Flip(i);
				}
			}
		}

		#endregion
	}
}

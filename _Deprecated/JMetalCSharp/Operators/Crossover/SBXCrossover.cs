using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Crossover
{
	/// <summary>
	/// This class allows to apply a SBX crossover operator using two parent
	/// solutions.
	/// </summary>
	public class SBXCrossover : Crossover
	{
		#region Private Attributes

		/// <summary>
		/// EPS defines the minimum difference allowed between real values
		/// </summary>
		private static readonly double EPS = 1.0e-14;

		private static readonly double ETA_C_DEFAULT = 20.0;
		private double crossoverProbability = 0.9;
		private double distributionIndex = ETA_C_DEFAULT;

		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>()
        {
            typeof(RealSolutionType),
            typeof(ArrayRealSolutionType)
        };

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor Create a new SBX crossover operator whit a default index
		/// given by <code>DEFAULT_INDEX_CROSSOVER</code>
		/// </summary>
		/// <param name="parameters"></param>
		public SBXCrossover(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref crossoverProbability);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "distributionIndex", ref distributionIndex);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Perform the crossover operation.
		/// </summary>
		/// <param name="probability">Crossover probability</param>
		/// <param name="parent1">The first parent</param>
		/// <param name="parent2">The second parent</param>
		/// <returns>An array containing the two offsprings</returns>
		private Solution[] DoCrossover(double probability, Solution parent1, Solution parent2)
		{

			Solution[] offSpring = new Solution[2];

			offSpring[0] = new Solution(parent1);
			offSpring[1] = new Solution(parent2);

			int i;
			double rand;
			double y1, y2, yL, yu;
			double c1, c2;
			double alpha, beta, betaq;
			double valueX1, valueX2;
			XReal x1 = new XReal(parent1);
			XReal x2 = new XReal(parent2);
			XReal offs1 = new XReal(offSpring[0]);
			XReal offs2 = new XReal(offSpring[1]);

			int numberOfVariables = x1.GetNumberOfDecisionVariables();

			if (JMetalRandom.NextDouble() <= probability)
			{
				for (i = 0; i < numberOfVariables; i++)
				{
					valueX1 = x1.GetValue(i);
					valueX2 = x2.GetValue(i);
					if (JMetalRandom.NextDouble() <= 0.5)
					{
						if (Math.Abs(valueX1 - valueX2) > EPS)
						{

							if (valueX1 < valueX2)
							{
								y1 = valueX1;
								y2 = valueX2;
							}
							else
							{
								y1 = valueX2;
								y2 = valueX1;
							}

							yL = x1.GetLowerBound(i);
							yu = x1.GetUpperBound(i);
							rand = JMetalRandom.NextDouble();
							beta = 1.0 + (2.0 * (y1 - yL) / (y2 - y1));
							alpha = 2.0 - Math.Pow(beta, -(distributionIndex + 1.0));

							if (rand <= (1.0 / alpha))
							{
								betaq = Math.Pow((rand * alpha), (1.0 / (distributionIndex + 1.0)));
							}
							else
							{
								betaq = Math.Pow((1.0 / (2.0 - rand * alpha)), (1.0 / (distributionIndex + 1.0)));
							}

							c1 = 0.5 * ((y1 + y2) - betaq * (y2 - y1));
							beta = 1.0 + (2.0 * (yu - y2) / (y2 - y1));
							alpha = 2.0 - Math.Pow(beta, -(distributionIndex + 1.0));

							if (rand <= (1.0 / alpha))
							{
								betaq = Math.Pow((rand * alpha), (1.0 / (distributionIndex + 1.0)));
							}
							else
							{
								betaq = Math.Pow((1.0 / (2.0 - rand * alpha)), (1.0 / (distributionIndex + 1.0)));
							}

							c2 = 0.5 * ((y1 + y2) + betaq * (y2 - y1));

							if (c1 < yL)
							{
								c1 = yL;
							}

							if (c2 < yL)
							{
								c2 = yL;
							}

							if (c1 > yu)
							{
								c1 = yu;
							}

							if (c2 > yu)
							{
								c2 = yu;
							}

							if (JMetalRandom.NextDouble() <= 0.5)
							{
								offs1.SetValue(i, c2);
								offs2.SetValue(i, c1);
							}
							else
							{
								offs1.SetValue(i, c1);
								offs2.SetValue(i, c2);
							}
						}
						else
						{
							offs1.SetValue(i, valueX1);
							offs2.SetValue(i, valueX2);
						}
					}
					else
					{
						offs1.SetValue(i, valueX2);
						offs2.SetValue(i, valueX1);
					}
				}
			}

			return offSpring;
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing an array of two parents</param>
		/// <returns>An object containing the offSprings</returns>
		public override object Execute(object obj)
		{
			Solution[] parents = (Solution[])obj;

			if (parents.Length != 2)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			if (!(VALID_TYPES.Contains(parents[0].Type.GetType())
					&& VALID_TYPES.Contains(parents[1].Type.GetType())))
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			Solution[] offSpring;
			offSpring = DoCrossover(crossoverProbability, parents[0], parents[1]);

			return offSpring;
		}

		#endregion
	}
}

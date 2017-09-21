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
	public class BLXAlphaCrossover : Crossover
	{
		/// <summary>
		/// EPS defines the minimum difference allowed between real values
		/// </summary>
		private static readonly double DEFAULT_ALPHA = 0.5;

		private double alpha = DEFAULT_ALPHA;
		private double? crossoverProbability = null;

		/// <summary>
		/// Valid solution types to apply this operator
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>(){
            typeof(RealSolutionType),
            typeof(ArrayRealSolutionType)
        };

		/// <summary>
		/// Constructor Create a new SBX crossover operator with a default index 
		/// given by <code>DEFAULT_INDEX_CROSSOVER</code>
		/// </summary>
		/// <param name="parameters"></param>
		public BLXAlphaCrossover(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetDoubleValueFromParameter(parameters, "probability", ref crossoverProbability);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "alpha", ref alpha);
		}

		/// <summary>
		/// Perform the crossover operation.
		/// </summary>
		/// <param name="probability">Crossover probability</param>
		/// <param name="parent1">The first parent</param>
		/// <param name="parent2">The second parent</param>
		/// <returns></returns>
		private Solution[] DoCrossover(double? probability, Solution parent1, Solution parent2)
		{
			Solution[] offSpring = new Solution[2];

			offSpring[0] = new Solution(parent1);
			offSpring[1] = new Solution(parent2);

			double rand;
			double valueY1;
			double valueY2;
			double valueX1;
			double valueX2;
			double upperValue;
			double lowerValue;

			XReal x1 = new XReal(parent1);
			XReal x2 = new XReal(parent2);
			XReal offs1 = new XReal(offSpring[0]);
			XReal offs2 = new XReal(offSpring[1]);

			int numberOfVaribales = x1.GetNumberOfDecisionVariables();
			if (JMetalRandom.NextDouble() <= probability)
			{
				for (int i = 0; i < numberOfVaribales; i++)
				{
					double max;
					double min;
					double range;
					double minRange;
					double maxRange;

					upperValue = x1.GetUpperBound(i);
					lowerValue = x1.GetLowerBound(i);
					valueX1 = x1.GetValue(i);
					valueX2 = x2.GetValue(i);

					if (valueX2 > valueX1)
					{
						max = valueX2;
						min = valueX1;
					}
					else
					{
						max = valueX1;
						min = valueX2;
					}

					range = max - min;

					minRange = min - range * alpha;
					maxRange = max + range * alpha;

					rand = JMetalRandom.NextDouble();
					valueY1 = minRange + rand * (maxRange - minRange);
					rand = JMetalRandom.NextDouble();
					valueY2 = minRange + rand * (maxRange - minRange);

					if (valueY1 < lowerValue)
					{
						offs1.SetValue(i, lowerValue);
					}
					else if (valueY1 > upperValue)
					{
						offs1.SetValue(i, upperValue);
					}
					else
					{
						offs1.SetValue(i, valueY1);
					}

					if (valueY2 < lowerValue)
					{
						offs2.SetValue(i, lowerValue);
					}
					else if (valueY2 > upperValue)
					{
						offs2.SetValue(i, upperValue);
					}
					else
					{
						offs2.SetValue(i, valueY2);
					}
				}
			}

			return offSpring;
		}

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing an array of two parents</param>
		/// <returns></returns>
		public override object Execute(object obj)
		{
			Solution[] parents = (Solution[])obj;

			if (parents.Length != 2)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			if (!(VALID_TYPES.Contains(parents[0].Type.GetType()) &&
				VALID_TYPES.Contains(parents[1].Type.GetType())))
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			Solution[] offSpring;
			offSpring = DoCrossover(crossoverProbability, parents[0], parents[1]);

			return offSpring;
		}
	}
}

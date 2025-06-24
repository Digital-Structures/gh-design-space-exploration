using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Crossover
{
	/// <summary>
	/// Differential evolution crossover operators
	/// Comments:
	/// - The operator receives two parameters: the current individual and an array
	///   of three parent individuals
	/// - The best and rand variants depends on the third parent, according whether
	///   it represents the current of the "best" individual or a randon one. 
	///   The implementation of both variants are the same, due to that the parent 
	///   selection is external to the crossover operator. 
	/// - Implemented variants:
	///   - rand/1/bin (best/1/bin)
	///   - rand/1/exp (best/1/exp)
	///   - current-to-rand/1 (current-to-best/1)
	///   - current-to-rand/1/bin (current-to-best/1/bin)
	///   - current-to-rand/1/exp (current-to-best/1/exp)
	/// </summary>
	public class DifferentialEvolutionCrossover : Crossover
	{
		/// <summary>
		/// DEFAULT_CR defines a default CR (crossover operation control) value
		/// </summary>
		private static readonly double DEFAULT_CR = 0.5;

		/// <summary>
		/// DEFAULT_F defines the default F (Scaling factor for mutation) value
		/// </summary>
		private static readonly double DEFAULT_F = 0.5;

		/// <summary>
		/// DEFAULT_K defines a default K value used in variants current-to-rand/1
		/// and current-to-best/1
		/// </summary>
		private static readonly double DEFAULT_K = 0.5;

		/// <summary>
		/// DEFAULT_DE_VARIANT defines the default DE variant
		/// </summary>
		private static readonly string DEFAULT_DE_VARIANT = "rand/1/bin";

		/// <summary>
		/// Valid solution types to apply this operator 
		/// </summary>
		private static readonly List<Type> VALID_TYPES = new List<Type>(){
            typeof(RealSolutionType),
            typeof(ArrayRealSolutionType)
        };

		private double cr;
		private double f;
		private double k;
		private string deVariant; // DE variant (rand/1/bin, rand/1/exp, etc.)

		public DifferentialEvolutionCrossover(Dictionary<string, object> parameters)
			: base(parameters)
		{
			cr = DEFAULT_CR;
			f = DEFAULT_F;
			k = DEFAULT_K;
			deVariant = DEFAULT_DE_VARIANT;


			Utils.Utils.GetDoubleValueFromParameter(parameters, "CR", ref cr);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "F", ref f);
			Utils.Utils.GetDoubleValueFromParameter(parameters, "K", ref k);
			Utils.Utils.GetStringValueFromParameter(parameters, "DE_VARIANT", ref deVariant);
		}

		/// <summary>
		/// Executes the operation
		/// </summary>
		/// <param name="obj">An object containing an array of three parents</param>
		/// <returns>An object containing the offSprings</returns>
		public override object Execute(object obj)
		{
			object[] parameters = (object[])obj;
			Solution current = (Solution)parameters[0];
			Solution[] parent = (Solution[])parameters[1];

			Solution child;

			if (!(VALID_TYPES.Contains(parent[0].Type.GetType()) &&
				VALID_TYPES.Contains(parent[1].Type.GetType()) &&
				VALID_TYPES.Contains(parent[2].Type.GetType())))
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}

			int jrand;

			child = new Solution(current);

			XReal xParent0 = new XReal(parent[0]);
			XReal xParent1 = new XReal(parent[1]);
			XReal xParent2 = new XReal(parent[2]);
			XReal xCurrent = new XReal(current);
			XReal xChild = new XReal(child);

			int numberOfVariables = xParent0.GetNumberOfDecisionVariables();
			jrand = JMetalRandom.Next(0, numberOfVariables - 1);

			// STEP 4. Checking the DE variant
			if ((deVariant == "rand/1/bin") || (deVariant == "best/1/bin"))
			{
				for (int j = 0; j < numberOfVariables; j++)
				{
					if (JMetalRandom.NextDouble(0, 1) < cr || j == jrand)
					{
						double value;
						value = xParent2.GetValue(j) + f * (xParent0.GetValue(j) - xParent1.GetValue(j));

						if (value < xChild.GetLowerBound(j))
						{
							value = xChild.GetLowerBound(j);
						}
						if (value > xChild.GetUpperBound(j))
						{
							value = xChild.GetUpperBound(j);
						}

						xChild.SetValue(j, value);
					}
					else
					{
						double value;
						value = xCurrent.GetValue(j);
						xChild.SetValue(j, value);
					}
				}
			}
			else if ((deVariant == "rand/1/exp") || (deVariant == "best/1/exp"))
			{
				for (int j = 0; j < numberOfVariables; j++)
				{
					if (JMetalRandom.NextDouble(0, 1) < cr || j == jrand)
					{
						double value;
						value = xParent2.GetValue(j) + f * (xParent0.GetValue(j) - xParent1.GetValue(j));

						if (value < xChild.GetLowerBound(j))
						{
							value = xChild.GetLowerBound(j);
						}
						if (value > xChild.GetUpperBound(j))
						{
							value = xChild.GetUpperBound(j);
						}

						xChild.SetValue(j, value);
					}
					else
					{
						cr = 0;
						double value;
						value = xCurrent.GetValue(j);
						xChild.SetValue(j, value);
					}
				}
			}
			else if ((deVariant == "current-to-rand/1") || (deVariant == "current-to-best/1"))
			{
				for (int j = 0; j < numberOfVariables; j++)
				{
					double value;
					value = xCurrent.GetValue(j) + k * (xParent2.GetValue(j)
							- xCurrent.GetValue(j))
							+ f * (xParent0.GetValue(j) - xParent1.GetValue(j));

					if (value < xChild.GetLowerBound(j))
					{
						value = xChild.GetLowerBound(j);
					}
					if (value > xChild.GetUpperBound(j))
					{
						value = xChild.GetUpperBound(j);
					}

					xChild.SetValue(j, value);
				}
			}
			else if ((deVariant == "current-to-rand/1/bin") || (deVariant == "current-to-best/1/bin"))
			{
				for (int j = 0; j < numberOfVariables; j++)
				{
					if (JMetalRandom.NextDouble(0, 1) < cr || j == jrand)
					{
						double value;
						value = xCurrent.GetValue(j) + k * (xParent2.GetValue(j)
								- xCurrent.GetValue(j))
								+ f * (xParent0.GetValue(j) - xParent1.GetValue(j));

						if (value < xChild.GetLowerBound(j))
						{
							value = xChild.GetLowerBound(j);
						}
						if (value > xChild.GetUpperBound(j))
						{
							value = xChild.GetUpperBound(j);
						}

						xChild.SetValue(j, value);
					}
					else
					{
						double value;
						value = xCurrent.GetValue(j);
						xChild.SetValue(j, value);
					}
				}
			}
			else if ((deVariant == "current-to-rand/1/exp") || (deVariant == "current-to-best/1/exp"))
			{
				for (int j = 0; j < numberOfVariables; j++)
				{
					if (JMetalRandom.NextDouble(0, 1) < cr || j == jrand)
					{
						double value;
						value = xCurrent.GetValue(j) + k * (xParent2.GetValue(j)
								- xCurrent.GetValue(j))
								+ f * (xParent0.GetValue(j) - xParent1.GetValue(j));

						if (value < xChild.GetLowerBound(j))
						{
							value = xChild.GetLowerBound(j);
						}
						if (value > xChild.GetUpperBound(j))
						{
							value = xChild.GetUpperBound(j);
						}

						xChild.SetValue(j, value);
					}
					else
					{
						cr = 0.0;
						double value;
						value = xCurrent.GetValue(j);
						xChild.SetValue(j, value);
					}
				}
			}
			else
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()");
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}
			return child;
		}
	}
}

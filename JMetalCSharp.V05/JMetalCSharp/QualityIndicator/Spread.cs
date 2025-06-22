using JMetalCSharp.QualityIndicator.Utils;
using System;

namespace JMetalCSharp.QualityIndicator
{
	/// <summary>
	/// This class implements the spread quality indicator.
	/// This metric is only applicable to two bi-objective problems.
	/// Reference: Deb, K., Pratap, A., Agarwal, S., Meyarivan, T.: A fast and 
	///            elitist multiobjective genetic algorithm: NSGA-II. IEEE Trans. 
	///            on Evol. Computation 6 (2002) 182-197
	/// </summary>
	public class Spread
	{
		#region Private Attributes

		/// <summary>
		/// It is used to access to the MetricsUtil funcionalities
		/// </summary>
		private MetricsUtil utils;

		#endregion

		#region Constructors

		public Spread()
		{
			utils = new MetricsUtil();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Calculates the Spread metric. Given the front, the true pareto front as
		/// <code>double []</code>, and the number of objectives, the method returns
		/// the value of the metric.
		/// </summary>
		/// <param name="front">The front.</param>
		/// <param name="trueParetoFront">The true pareto front.</param>
		/// <param name="numberOfObjectives">The number of objectives.</param>
		/// <returns></returns>
		public double CalculateSpread(double[][] front, double[][] trueParetoFront, int numberOfObjectives)
		{
			// Stores the maximum values of true pareto front.
			double[] maximumValue;

			// Stores the minimum values of the true pareto front.
			double[] minimumValue;

			// Stores the normalized front.
			double[][] normalizedFront;

			// Stores the normalized true Pareto front.
			double[][] normalizedParetoFront;

			// STEP 1. Obtain the maximum and minimum values of the Pareto front
			maximumValue = utils.GetMaximumValues(trueParetoFront, numberOfObjectives);
			minimumValue = utils.GetMinimumValues(trueParetoFront, numberOfObjectives);

			// STEP 2. Get the normalized front and true Pareto fronts
			normalizedFront = utils.GetNormalizedFront(front, maximumValue, minimumValue);
			normalizedParetoFront = utils.GetNormalizedFront(trueParetoFront, maximumValue, minimumValue);

			// STEP 3. Sort normalizedFront and normalizedParetoFront;

			Array.Sort(normalizedFront, new LexicoGraphicalComparator());
			Array.Sort(normalizedParetoFront, new LexicoGraphicalComparator());

			int numberOfPoints = normalizedFront.Length;

			// STEP 4. Compute df and dl (See specifications in Deb's description of 
			// the metric)
			double df = utils.Distance(normalizedFront[0], normalizedParetoFront[0]);
			double dl = utils.Distance(normalizedFront[normalizedFront.Length - 1],
					normalizedParetoFront[normalizedParetoFront.Length - 1]);

			double mean = 0.0;
			double diversitySum = df + dl;

			// STEP 5. Calculate the mean of distances between points i and (i - 1). 
			// (the poins are in lexicografical order)
			for (int i = 0; i < (normalizedFront.Length - 1); i++)
			{
				mean += utils.Distance(normalizedFront[i], normalizedFront[i + 1]);
			}

			mean = mean / (double)(numberOfPoints - 1);

			// STEP 6. If there are more than a single point, continue computing the 
			// metric. In other case, return the worse value (1.0, see metric's 
			// description).
			if (numberOfPoints > 1)
			{
				for (int i = 0; i < (numberOfPoints - 1); i++)
				{
					diversitySum += Math.Abs(utils.Distance(normalizedFront[i], normalizedFront[i + 1]) - mean);
				}
				return diversitySum / (df + dl + (numberOfPoints - 1) * mean);
			}
			else
			{
				return 1.0;
			}
		}

		#endregion
	}
}

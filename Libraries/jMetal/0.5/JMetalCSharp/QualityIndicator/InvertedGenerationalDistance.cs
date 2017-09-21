using JMetalCSharp.QualityIndicator.Utils;
using System;

namespace JMetalCSharp.QualityIndicator
{
	/// <summary>
	/// This class implements the inverted generational distance metric. 
	/// Reference: Van Veldhuizen, D.A., Lamont, G.B.:
	/// Multiobjective Evolutionary Algorithm Research: A History and Analysis.
	/// Technical Report TR-98-03, Dept. Elec. Comput. Eng., Air Force Inst. Technol.
	/// (1998)
	/// </summary>
	public class InvertedGenerationalDistance
	{
		#region Private Attributes
		/// <summary>
		/// Is used to access to the MetricsUtil funcionalities
		/// </summary>
		private MetricsUtil utils;  //

		/// <summary>
		/// This is the pow used for the distances
		/// </summary>
		private static readonly double pow = 2.0;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor. Creates a new instance of the generational distance metric.
		/// </summary>
		public InvertedGenerationalDistance()
		{
			utils = new MetricsUtil();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns the inverted generational distance value for a given front
		/// </summary>
		/// <param name="front">The front</param>
		/// <param name="trueParetoFront">The true pareto front</param>
		/// <param name="numberOfObjectives"></param>
		/// <returns></returns>
		public double CalculateInvertedGenerationalDistance(double[][] front, double[][] trueParetoFront, int numberOfObjectives)
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
			normalizedFront = utils.GetNormalizedFront(front,
					maximumValue,
					minimumValue);
			normalizedParetoFront = utils.GetNormalizedFront(trueParetoFront,
					maximumValue,
					minimumValue);

			// STEP 3. Sum the distances between each point of the true Pareto front and
			// the nearest point in the true Pareto front
			double sum = 0.0;
			foreach (double[] aNormalizedParetoFront in normalizedParetoFront)
			{
				sum += Math.Pow(utils.DistanceToClosedPoint(aNormalizedParetoFront, normalizedFront), pow);
			}

			// STEP 4. Obtain the sqrt of the sum
			sum = Math.Pow(sum, 1.0 / pow);

			// STEP 5. Divide the sum by the maximum number of points of the front
			double generationalDistance = sum / normalizedParetoFront.Length;

			return generationalDistance;
		}

		#endregion
	}
}

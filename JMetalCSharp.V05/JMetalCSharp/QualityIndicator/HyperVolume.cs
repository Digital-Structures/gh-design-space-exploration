using JMetalCSharp.QualityIndicator.Utils;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.QualityIndicator
{
	/// <summary>
	/// This class implements the hypervolume indicator. The code is the a Java version
	/// of the original metric implementation by Eckart Zitzler.
	/// Reference: E. Zitzler and L. Thiele
	///           Multiobjective Evolutionary Algorithms: A Comparative Case Study 
	///           and the Strength Pareto Approach,
	///           IEEE Transactions on Evolutionary Computation, vol. 3, no. 4, 
	///           pp. 257-271, 1999.
	/// </summary>
	public class HyperVolume
	{
		#region Private Attributes

		private MetricsUtil utils;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// Creates a new instance of MultiDelta
		/// </summary>
		public HyperVolume()
		{
			utils = new MetricsUtil();
		}

		#endregion
		#region Public Methods

		public double CalculateHypervolume(double[][] front, int noPoints, int noObjectives)
		{
			int n;
			double volume, distance;

			volume = 0;
			distance = 0;
			n = noPoints;
			while (n > 0)
			{
				int noNondominatedPoints;
				double tempVolume, tempDistance;

				noNondominatedPoints = FilterNondominatedSet(front, n, noObjectives - 1);
				if (noObjectives < 3)
				{
					if (noNondominatedPoints < 1)
					{
						Logger.Log.Error("HyperVolume.CalculateHypervolume(): Run-Time Error");
						Console.Error.WriteLine("run-time Error");
					}

					tempVolume = front[0][0];
				}
				else
				{
					tempVolume = CalculateHypervolume(front, noNondominatedPoints,
							noObjectives - 1);
				}
				try
				{
					tempDistance = SurfaceUnchangedTo(front, n, noObjectives - 1);
				}
				catch (Exception ex)
				{
					throw ex;
				}
				volume += tempVolume * (tempDistance - distance);
				distance = tempDistance;
				n = ReduceNondominatedSet(front, n, noObjectives - 1, distance);
			}
			return volume;
		}

		/// <summary>
		/// Returns the hypevolume value of the paretoFront. This method call to the
		/// calculate hipervolume one
		/// </summary>
		/// <param name="paretoFront">The pareto front</param>
		/// <param name="paretoTrueFront">The true pareto front</param>
		/// <param name="numberOfObjectives">Number of objectives of the pareto front</param>
		/// <returns></returns>
		public double Hypervolume(double[][] paretoFront, double[][] paretoTrueFront, int numberOfObjectives)
		{
			// Stores the maximum values of true pareto front.
			double[] maximumValues;

			// Stores the minimum values of the true pareto front.
			double[] minimumValues;

			// Stores the normalized front.
			double[][] normalizedFront;

			// Stores the inverted front. Needed for minimization problems
			double[][] invertedFront;

			// STEP 1. Obtain the maximum and minimum values of the Pareto front
			maximumValues = utils.GetMaximumValues(paretoTrueFront, numberOfObjectives);
			minimumValues = utils.GetMinimumValues(paretoTrueFront, numberOfObjectives);

			// STEP 2. Get the normalized front
			normalizedFront = utils.GetNormalizedFront(paretoFront,
					maximumValues,
					minimumValues);

			// STEP 3. Inverse the pareto front. This is needed because of the original
			//metric by Zitzler is for maximization problems
			invertedFront = utils.InvertedFront(normalizedFront);

			// STEP4. The hypervolumen (control is passed to java version of Zitzler code)
			return this.CalculateHypervolume(invertedFront, invertedFront.Length, numberOfObjectives);
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="point1"></param>
		/// <param name="point2"></param>
		/// <param name="noObjectives"></param>
		/// <returns>true if 'point1' dominates 'points2' with respect to the to the first 'noObjectives' objectives </returns>
		private bool Dominates(double[] point1, double[] point2, int noObjectives)
		{
			int i;
			int betterInAnyObjective;

			betterInAnyObjective = 0;
			for (i = 0; i < noObjectives && point1[i] >= point2[i]; i++)
			{
				if (point1[i] > point2[i])
				{
					betterInAnyObjective = 1;
				}
			}

			return ((i >= noObjectives) && (betterInAnyObjective > 0));
		}

		private void Swap(double[][] front, int i, int j)
		{
			double[] temp;

			temp = front[i];
			front[i] = front[j];
			front[j] = temp;
		}

		/// <summary>
		/// all nondominated points regarding the first 'noObjectives' dimensions
		/// are collected; the points referenced by 'front[0..noPoints-1]' are
		/// considered; 'front' is resorted, such that 'front[0..n-1]' contains
		/// the nondominated points; n is returned 
		/// </summary>
		/// <param name="front"></param>
		/// <param name="noPoints"></param>
		/// <param name="noObjectives"></param>
		/// <returns></returns>
		private int FilterNondominatedSet(double[][] front, int noPoints, int noObjectives)
		{
			int i, j;
			int n;

			n = noPoints;
			i = 0;
			while (i < n)
			{
				j = i + 1;
				while (j < n)
				{
					if (Dominates(front[i], front[j], noObjectives))
					{
						/* remove point 'j' */
						n--;
						Swap(front, j, n);
					}
					else if (Dominates(front[j], front[i], noObjectives))
					{
						/* remove point 'i'; ensure that the point copied to index 'i'
						 is considered in the next outer loop (thus, decrement i) */
						n--;
						Swap(front, i, n);
						i--;
						break;
					}
					else
					{
						j++;
					}
				}
				i++;
			}
			return n;
		}

		/// <summary>
		/// calculate next value regarding dimension 'objective'; consider
		/// points referenced in 'front[0..noPoints-1]'
		/// </summary>
		/// <param name="front"></param>
		/// <param name="noPoints"></param>
		/// <param name="objective"></param>
		/// <returns></returns>
		private double SurfaceUnchangedTo(double[][] front, int noPoints, int objective)
		{
			int i;
			double minValue, value;

			if (noPoints < 1)
			{
				Logger.Log.Error("HyperVolume.SurfaceUnchangedTo(): Run-Time Error");
				Console.Error.WriteLine("run-time error");
			}

			minValue = front[0][objective];
			for (i = 1; i < noPoints; i++)
			{
				value = front[i][objective];
				if (value < minValue)
				{
					minValue = value;
				}
			}
			return minValue;
		}

		/// <summary>
		/// remove all points which have a value <= 'threshold' regarding the
		/// dimension 'objective'; the points referenced by
		/// 'front[0..noPoints-1]' are considered; 'front' is resorted, such that
		/// 'front[0..n-1]' contains the remaining points; 'n' is returned
		/// </summary>
		/// <param name="front"></param>
		/// <param name="noPoints"></param>
		/// <param name="objective"></param>
		/// <param name="threshold"></param>
		/// <returns></returns>
		private int ReduceNondominatedSet(double[][] front, int noPoints, int objective, double threshold)
		{
			int n;
			int i;

			n = noPoints;
			for (i = 0; i < n; i++)
			{
				if (front[i][objective] <= threshold)
				{
					n--;
					Swap(front, i, n);
				}
			}

			return n;
		}

		/// <summary>
		/// merge two fronts
		/// </summary>
		/// <param name="front1"></param>
		/// <param name="sizeFront1"></param>
		/// <param name="front2"></param>
		/// <param name="sizeFront2"></param>
		/// <param name="noObjectives"></param>
		/// <returns></returns>
		private double[][] MergeFronts(double[][] front1, int sizeFront1,
				double[][] front2, int sizeFront2, int noObjectives)
		{
			int i, j;
			int noPoints;
			double[][] frontPtr;

			/* allocate memory */
			noPoints = sizeFront1 + sizeFront2;
			frontPtr = new double[noPoints][];
			for (int k = 0; k < noPoints; k++)
			{
				frontPtr[k] = new double[noObjectives];
			}
			/* copy points */
			noPoints = 0;
			for (i = 0; i < sizeFront1; i++)
			{
				for (j = 0; j < noObjectives; j++)
				{
					frontPtr[noPoints][j] = front1[i][j];
				}
				noPoints++;
			}
			for (i = 0; i < sizeFront2; i++)
			{
				for (j = 0; j < noObjectives; j++)
				{
					frontPtr[noPoints][j] = front2[i][j];
				}
				noPoints++;
			}

			return frontPtr;
		}

		#endregion
	}
}

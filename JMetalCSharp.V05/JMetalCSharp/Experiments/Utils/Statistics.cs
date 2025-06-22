using System.Collections.Generic;
using System.Linq;

namespace JMetalCSharp.Experiments.Utils
{
	/// <summary>
	/// This class provides methods for computing some statistics
	/// </summary>
	public class Statistics
	{
		#region Public Methods
		/// <summary>
		///  Calculates the median of a vector considering the positions indicated by the parameters first and last
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="needOrder">indicate if is necessary order the vector</param>
		/// <returns>The median</returns>
		public static double CalculateMedian(List<double> vector, bool needOrder = true)
		{
			List<double> orderedVector = OrderVectorIfNeeded(vector, needOrder);

			double median = 0.0;

			int length = orderedVector.Count;

			if (length % 2 != 0)
			{
				median = orderedVector[length / 2];
			}
			else
			{
				median = (orderedVector[length / 2 - 1] + orderedVector[length / 2]) / 2.0;
			}

			return median;
		}

		/// <summary>
		/// Calculates the interquartile range (IQR) of a vector of Doubles
		/// </summary>
		/// <param name="vector">vector</param>
		/// <returns>The IQR</returns>
		public static double CalculateIQR(List<double> vector)
		{
			double q3 = 0.0;
			double q1 = 0.0;

			if (vector.Count > 1)
			{ // == 1 implies IQR = 0
				List<double> orderedVector = new List<double>(vector);
				orderedVector.Sort();

				q1 = CalculateQ1(orderedVector, false);
				q3 = CalculateQ3(orderedVector, false);
			}

			return q3 - q1;
		}

		public static double CalculateQ1(List<double> vector, bool needOrder = true)
		{
			List<double> orderedVector = OrderVectorIfNeeded(vector, needOrder);

			double q1 = 0.0;

			int length = orderedVector.Count;

			q1 = CalculateMedian(orderedVector.GetRange(0, length / 2), false);

			return q1;
		}

		public static double CalculateQ3(List<double> vector, bool needOrder = true)
		{
			List<double> orderedVector = OrderVectorIfNeeded(vector, needOrder);

			double q3 = 0.0;

			int length = orderedVector.Count;

			if (length % 2 != 0)
			{
				q3 = CalculateMedian(orderedVector.GetRange(length / 2 + 1, length - (length / 2 + 1)), false);
			}
			else
			{
				q3 = CalculateMedian(orderedVector.GetRange(length / 2, length - (length / 2)), false);
			}

			return q3;
		}

		public static double GetBoxPlotLowValue(double q1, double q3, List<double> vector, bool needOrder = true)
		{
			List<double> orderedVector = OrderVectorIfNeeded(vector, needOrder);
			double value;
			double iqr = q3 - q1;

			double outlierLimitBottom = q1 - (1.5 * iqr);

			value = orderedVector.First(v => v >= outlierLimitBottom);

			return value;
		}

		public static double GetBoxPlotTopValue(double q1, double q3, List<double> vector, bool needOrder = true)
		{
			List<double> orderedVector = OrderVectorIfNeeded(vector, needOrder);
			double value;
			double iqr = q3 - q1;

			double outlierLimitTop = q3 + (1.5 * iqr);

			value = orderedVector.Last(v => v <= outlierLimitTop);

			return value;
		}

		public static IEnumerable<double> GetOutlidersValues(double q1, double q3, List<double> vector, bool needOrder = true)
		{
			List<double> orderedVector = OrderVectorIfNeeded(vector, needOrder);
			IEnumerable<double> result;
			double low = GetBoxPlotLowValue(q1, q3, orderedVector, false);
			double top = GetBoxPlotTopValue(q1, q3, orderedVector, false);

			result = orderedVector.Where(x => x < low || x > top);

			return result;
		}

		#endregion

		#region Private Methods

		private static List<double> OrderVectorIfNeeded(List<double> vector, bool needOrder)
		{
			List<double> orderedVector;
			if (needOrder)
			{
				orderedVector = new List<double>(vector);
				orderedVector.Sort();
			}
			else
			{
				orderedVector = vector;
			}
			return orderedVector;
		}

		#endregion
	}
}

using System.Collections.Generic;

namespace JMetalCSharp.QualityIndicator.Utils
{
	/// <summary>
	/// This class  is used to compare points given as <code>double</code>.
	/// The order used is the lexicograhphical.
	/// </summary>
	class LexicoGraphicalComparator : IComparer<double[]>
	{
		#region Implement Interface

		/// <summary>
		/// The compare method compare the objects o1 and o2.
		/// </summary>
		/// <param name="pointOne"></param>
		/// <param name="pointTwo"></param>
		/// <returns></returns>
		public int Compare(double[] pointOne, double[] pointTwo)
		{
			//To determine the first i, that pointOne[i] != pointTwo[i];
			int index = 0;
			while ((index < pointOne.Length) && (index < pointTwo.Length)
					&& pointOne[index] == pointTwo[index])
			{
				index++;
			}
			if ((index >= pointOne.Length) || (index >= pointTwo.Length))
			{
				return 0;
			}
			else if (pointOne[index] < pointTwo[index])
			{
				return -1;
			}
			else if (pointOne[index] > pointTwo[index])
			{
				return 1;
			}
			return 0;
		}

		#endregion
	}
}

using System.Collections.Generic;

namespace JMetalCSharp.QualityIndicator.FastHypervolume.WFG
{

	public class PointComparator : IComparer<Point>
	{
		bool maximizing;

		public PointComparator(bool maximizing)
		{
			this.maximizing = maximizing;
		}

		/// <summary>
		/// Compares two POINT objects according to the last objectives
		/// </summary>
		/// <param name="pointOne"></param>
		/// <param name="pointTwo"></param>
		/// <returns>-1 if pointOne < pointTwo, 1 if pointOne > pointTwo or 0 in other case.</returns>
		public int Compare(Point pointOne, Point pointTwo)
		{
			for (int i = pointOne.GetNumberOfObjectives() - 1; i >= 0; i--)
			{
				if (IsBetter(pointOne.Objectives[i], pointTwo.Objectives[i]))
					return -1;
				else if (IsBetter(pointTwo.Objectives[i], pointOne.Objectives[i]))
					return 1;
			}
			return 0;
		}

		private bool IsBetter(double v1, double v2)
		{
			if (maximizing)
				return (v1 > v2);
			else
				return (v2 > v1);
		}
	}

}

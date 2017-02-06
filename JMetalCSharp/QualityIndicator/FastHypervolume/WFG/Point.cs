using JMetalCSharp.Core;
using System;

namespace JMetalCSharp.QualityIndicator.FastHypervolume.WFG
{
	public class Point
	{
		public double[] Objectives { get; private set; }

		public Point(int dimension)
		{
			Objectives = new double[dimension];

			for (int i = 0; i < dimension; i++)
			{
				Objectives[i] = 0.0;
			}
		}

		public Point(Solution solution)
		{
			int dimension = solution.NumberOfObjectives;
			Objectives = new double[dimension];

			for (int i = 0; i < dimension; i++)
			{
				Objectives[i] = solution.Objective[i];
			}
		}

		public Point(double[] points)
		{
			Objectives = new double[points.Length];
			Array.Copy(points, Objectives, points.Length);
		}

		public int GetNumberOfObjectives()
		{
			return Objectives.Length;
		}

		public override string ToString()
		{
			string result = "";
			for (int i = 0; i < Objectives.Length; i++)
				result += Objectives[i] + " ";

			return result;
		}
	}

}

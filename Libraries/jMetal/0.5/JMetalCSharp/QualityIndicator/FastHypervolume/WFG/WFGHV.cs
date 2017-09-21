using JMetalCSharp.Core;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.QualityIndicator.FastHypervolume.WFG
{
	public class WFGHV
	{
		#region Private Attributes
		Front[] fs;
		Point referencePoint;
		int currentDeep;
		int currentDimension;
		int maxNumberOfPoints;
		int maxNumberOfObjectives;
		readonly int OPT = 2;
		IComparer<Point> pointComparator;
		#endregion

		#region Constructors
		public WFGHV(int dimension, int maxNumberOfPoints)
		{
			this.referencePoint = null;
			this.currentDeep = 0;
			this.currentDimension = dimension;
			this.maxNumberOfPoints = maxNumberOfPoints;
			this.maxNumberOfObjectives = dimension;
			this.pointComparator = new PointComparator(true);

			int maxd = maxNumberOfPoints - (OPT / 2 + 1);
			this.fs = new Front[maxd];
			for (int i = 0; i < maxd; i++)
			{
				this.fs[i] = new Front(maxNumberOfPoints, dimension);
			}
		}

		public WFGHV(int dimension, int maxNumberOfPoints, Solution referencePoint)
		{
			this.referencePoint = new Point(referencePoint);
			this.currentDeep = 0;
			this.currentDimension = dimension;
			this.maxNumberOfPoints = maxNumberOfPoints;
			this.maxNumberOfObjectives = dimension;
			this.pointComparator = new PointComparator(true);

			int maxd = maxNumberOfPoints - (OPT / 2 + 1);
			fs = new Front[maxd];
			for (int i = 0; i < maxd; i++)
			{
				fs[i] = new Front(maxNumberOfPoints, dimension);
			}
		}

		public WFGHV(int dimension, int maxNumberOfPoints, Point referencePoint)
		{
			this.referencePoint = referencePoint;
			this.currentDeep = 0;
			this.currentDimension = dimension;
			this.maxNumberOfPoints = maxNumberOfPoints;
			this.maxNumberOfObjectives = dimension;
			this.pointComparator = new PointComparator(true);

			int maxd = maxNumberOfPoints - (OPT / 2 + 1);
			this.fs = new Front[maxd];
			for (int i = 0; i < maxd; i++)
			{
				this.fs[i] = new Front(maxNumberOfPoints, dimension);
			}
		}
		#endregion

		#region Public Methods
		public int GetLessContributorHV(SolutionSet set)
		{

			Front wholeFront = new Front();

			wholeFront.LoadFront(set, -1);

			int index = 0;
			double contribution = double.PositiveInfinity;

			for (int i = 0; i < set.Size(); i++)
			{
				double[] v = new double[set.Get(i).NumberOfObjectives];
				for (int j = 0; j < v.Length; j++)
				{
					v[j] = set.Get(i).Objective[j];
				}
				Point p = new Point(v);
				double aux = this.GetExclusiveHV(wholeFront, i);
				if ((aux) < contribution)
				{
					index = i;
					contribution = aux;
				}
				set.Get(i).CrowdingDistance = aux;
			}

			return index;
		}

		public double GetHV(Front front, Solution refPoint)
		{
			this.referencePoint = new Point(refPoint);
			double volume = 0.0;
			Sort(front);

			if (currentDimension == 2)
				volume = Get2DHV(front);
			else
			{
				volume = 0.0;

				currentDimension--;
				for (int i = front.NPoints - 1; i >= 0; i--)
				{
					volume += Math.Abs(front.Points[i].Objectives[currentDimension] - this.referencePoint.Objectives[currentDimension]) * this.GetExclusiveHV(front, i);
				}
				currentDimension++;
			}

			return volume;
		}

		public double GetHV(Front front)
		{
			double volume = 0.0;
			Sort(front);

			if (currentDimension == 2)
				volume = Get2DHV(front);
			else
			{
				volume = 0.0;

				currentDimension--;
				for (int i = front.NPoints - 1; i >= 0; i--)
				{
					volume += Math.Abs(front.Points[i].Objectives[currentDimension] - referencePoint.Objectives[currentDimension]) * this.GetExclusiveHV(front, i);
				}
				currentDimension++;
			}

			return volume;
		}

		public double Get2DHV(Front front)
		{
			double hv = 0.0;

			hv = Math.Abs((front.Points[0].Objectives[0] - referencePoint.Objectives[0]) * (front.Points[0].Objectives[1] - referencePoint.Objectives[1]));

			for (int i = 1; i < front.NPoints; i++)
			{
				hv += Math.Abs((front.Points[i].Objectives[0] - referencePoint.Objectives[0]) * (front.Points[i].Objectives[1] - front.Points[i - 1].Objectives[1]));

			}

			return hv;

		}

		public double GetInclusiveHV(Point p)
		{
			double volume = 1;
			for (int i = 0; i < currentDimension; i++)
			{
				volume *= Math.Abs(p.Objectives[i] - referencePoint.Objectives[i]);
			}

			return volume;
		}

		public double GetExclusiveHV(Front front, int point)
		{
			double volume;

			volume = GetInclusiveHV(front.Points[point]);
			if (front.NPoints > point + 1)
			{
				MakeDominatedBit(front, point);
				double v = GetHV(fs[currentDeep - 1]);
				volume -= v;
				currentDeep--;
			}

			return volume;
		}

		public void MakeDominatedBit(Front front, int p)
		{
			int z = front.NPoints - 1 - p;

			for (int i = 0; i < z; i++)
				for (int j = 0; j < currentDimension; j++)
				{
					fs[currentDeep].Points[i].Objectives[j] = Worse(front.Points[p].Objectives[j], front.Points[p + 1 + i].Objectives[j], false);
				}

			Point t;
			fs[currentDeep].NPoints = 1;

			for (int i = 1; i < z; i++)
			{
				int j = 0;
				bool keep = true;
				while (j < fs[currentDeep].NPoints && keep)
				{
					switch (Dominates2way(fs[currentDeep].Points[i], fs[currentDeep].Points[j]))
					{
						case -1:
							t = fs[currentDeep].Points[j];
							fs[currentDeep].NPoints--;
							fs[currentDeep].Points[j] = fs[currentDeep].Points[fs[currentDeep].NPoints];
							fs[currentDeep].Points[fs[currentDeep].NPoints] = t;
							break;
						case 0:
							j++;
							break;
						default:
							keep = false;
							break;
					}
				}
				if (keep)
				{
					t = fs[currentDeep].Points[fs[currentDeep].NPoints];
					fs[currentDeep].Points[fs[currentDeep].NPoints] = fs[currentDeep].Points[i];
					fs[currentDeep].Points[i] = t;
					fs[currentDeep].NPoints++;
				}
			}

			currentDeep++;
		}

		public void Sort(Front front)
		{
			Array.Sort(front.Points, 0, front.NPoints, pointComparator);
		}

		#endregion

		#region Private Methods
		private double Worse(double x, double y, bool maximizing)
		{
			double result;
			if (maximizing)
			{
				if (x > y)
					result = y;
				else
					result = x;
			}
			else
			{
				if (x > y)
					result = x;
				else
					result = y;
			}
			return result;
		}

		/// <summary>
		/// (ASSUMING MINIMIZATION)
		/// </summary>
		/// <param name="p"></param>
		/// <param name="q"></param>
		/// <returns>returns -1 if p dominates q, 1 if q dominates p, 2 if p == q, 0 otherwise</returns>
		int Dominates2way(Point p, Point q)
		{
			// domination could be checked in either order
			for (int i = currentDimension - 1; i >= 0; i--)
				if (p.Objectives[i] < q.Objectives[i])
				{
					for (int j = i - 1; j >= 0; j--)
						if (q.Objectives[j] < p.Objectives[j])
							return 0;
					return -1;
				}
				else
					if (q.Objectives[i] < p.Objectives[i])
					{
						for (int j = i - 1; j >= 0; j--)
							if (p.Objectives[j] < q.Objectives[j])
								return 0;
						return 1;
					}
			return 2;
		}

		#endregion
	}

}

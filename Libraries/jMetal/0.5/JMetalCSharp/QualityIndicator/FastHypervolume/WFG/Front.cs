using JMetalCSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace JMetalCSharp.QualityIndicator.FastHypervolume.WFG
{

	public class Front
	{
		private int dimension;
		private bool maximizing;
		public int NPoints
		{
			get;
			set;
		}

		public int NumberOfObjectives
		{
			get { return dimension; }
		}

		public int NumberOfPoints
		{
			get;
			private set;
		}

		public Point[] Points
		{
			get;
			private set;
		}

		private IComparer<Point> pointComparator;

		public Front()
		{
			maximizing = true;
			pointComparator = new PointComparator(maximizing);

		}

		public Front(int numberOfPoints, int dimension, SolutionSet solutionSet)
		{
			this.maximizing = true;
			this.pointComparator = new PointComparator(maximizing);
			this.NumberOfPoints = numberOfPoints;
			this.dimension = dimension;
			this.NPoints = numberOfPoints;

			this.Points = new Point[numberOfPoints];
			for (int i = 0; i < numberOfPoints; i++)
			{
				double[] p = new double[dimension];
				for (int j = 0; j < dimension; j++)
				{
					p[j] = solutionSet.Get(i).Objective[j];
				}
				Points[i] = new Point(p);
			}
		}

		public Front(int numberOfPoints, int dimension)
		{
			this.maximizing = true;
			this.pointComparator = new PointComparator(maximizing);
			this.NumberOfPoints = numberOfPoints;
			this.dimension = dimension;
			this.NPoints = numberOfPoints;
			this.Points = new Point[numberOfPoints];
			for (int i = 0; i < numberOfPoints; i++)
			{
				double[] p = new double[dimension];
				for (int j = 0; j < dimension; j++)
				{
					p[j] = 0.0;
				}
				Points[i] = new Point(p);
			}
		}

		public Front(int numberOfPoints, int dimension, List<double[]> listOfPoints)
		{
			this.maximizing = true;
			this.pointComparator = new PointComparator(maximizing);
			this.NumberOfPoints = numberOfPoints;
			this.dimension = dimension;
			this.Points = new Point[numberOfPoints];
			for (int i = 0; i < numberOfPoints; i++)
			{
				this.Points[i] = new Point(listOfPoints[i]);
			}
		}


		public void ReadFront(string fileName)
		{
			using (StreamReader reader = new StreamReader(fileName))
			{
				List<double[]> list = new List<double[]>();
				int numberOfObjectives = 0;
				string aux = reader.ReadLine();
				while (aux != null)
				{
					string[] st = aux.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					int i = 0;
					numberOfObjectives = st.Length;

					double[] vector = new double[st.Length];
					foreach (string s in st)
					{
						double value = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(s);
						vector[i] = value;
						i++;
					}
					list.Add(vector);
					aux = reader.ReadLine();
				}
				NumberOfPoints = list.Count;
				dimension = numberOfObjectives;
				Points = new Point[NumberOfPoints];
				NPoints = NumberOfPoints;
				for (int i = 0; i < NumberOfPoints; i++)
				{
					Points[i] = new Point(list[i]);
				}
			}
		}

		public void LoadFront(SolutionSet solutionSet, int notLoadingIndex)
		{

			if (notLoadingIndex >= 0 && notLoadingIndex < solutionSet.Size())
			{
				NumberOfPoints = solutionSet.Size() - 1;
			}
			else
			{
				NumberOfPoints = solutionSet.Size();
			}

			NPoints = NumberOfPoints;
			dimension = solutionSet.Get(0).NumberOfObjectives;

			Points = new Point[NumberOfPoints];

			int index = 0;
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				if (i != notLoadingIndex)
				{
					double[] vector = new double[dimension];
					for (int j = 0; j < dimension; j++)
					{
						vector[j] = solutionSet.Get(i).Objective[j];
					}
					Points[index++] = new Point(vector);
				}
			}
		}

		public void PrintFront()
		{
			Console.WriteLine("Objectives:       " + dimension);
			Console.WriteLine("Number of points: " + NumberOfPoints);

			for (int i = 0, li = Points.Length; i < li; i++)
			{
				Console.WriteLine(Points[i]);
			}
		}

		public void SetToMazimize()
		{
			maximizing = true;
			pointComparator = new PointComparator(maximizing);
		}

		public void SetToMinimize()
		{
			maximizing = false;
			pointComparator = new PointComparator(maximizing);
		}

		public void Sort()
		{
			Array.Sort<Point>(Points, pointComparator);
		}

		public Point getReferencePoint()
		{
			Point referencePoint = new Point(dimension);

			double[] maxObjectives = new double[NumberOfPoints];
			for (int i = 0; i < NumberOfPoints; i++)
				maxObjectives[i] = 0;

			for (int i = 0; i < Points.Length; i++)
			{
				for (int j = 0; j < dimension; j++)
				{
					if (maxObjectives[j] < Points[i].Objectives[j])
					{
						maxObjectives[j] = Points[i].Objectives[j];
					}
				}
			}

			for (int i = 0; i < dimension; i++)
			{
				referencePoint.Objectives[i] = maxObjectives[i];
			}

			return referencePoint;
		}
	}
}

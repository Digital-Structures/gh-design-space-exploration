using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Metaheuristics.MOEAD
{
	public static class Utils
	{
		public static double DistVector(double[] vector1, double[] vector2)
		{
			int dim = vector1.Length;
			double sum = 0;
			for (int n = 0; n < dim; n++)
			{
				sum += (vector1[n] - vector2[n]) * (vector1[n] - vector2[n]);
			}
			return Math.Sqrt(sum);
		}

		public static void MinFastSort(double[] x, int[] idx, int n, int m)
		{
			for (int i = 0; i < m; i++)
			{
				for (int j = i + 1; j < n; j++)
				{
					if (x[i] > x[j])
					{
						double temp = x[i];
						x[i] = x[j];
						x[j] = temp;
						int id = idx[i];
						idx[i] = idx[j];
						idx[j] = id;
					}
				}
			}
		}

		public static void RandomPermutation(int[] perm, int size)
		{
			int[] index = new int[size];
			bool[] flag = new bool[size];

			for (int n = 0; n < size; n++)
			{
				index[n] = n;
				flag[n] = true;
			}

			int num = 0;
			while (num < size)
			{
				int start = JMetalRandom.Next(0, size - 1);

				while (true)
				{
					if (flag[start])
					{
						perm[num] = index[start];
						flag[start] = false;
						num++;
						break;
					}
					if (start == (size - 1))
					{
						start = 0;
					}
					else
					{
						start++;
					}
				}
			}
		}
	}
}

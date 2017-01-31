using System;

namespace JMetalCSharp.Problems.WFG
{

	/// <summary>
	/// Class implementing shape functions for WFG benchmark
	/// Reference: Simon Huband, Luigi Barone, Lyndon While, Phil Hingston
	///            A Scalable Multi-objective Test Problem Toolkit.
	///            Evolutionary Multi-Criterion Optimization: 
	///            Third International Conference, EMO 2005. 
	///            Proceedings, volume 3410 of Lecture Notes in Computer Science
	/// </summary>
	public static class Shapes
	{

		/// <summary>
		/// Calculate a linear shape
		/// </summary>
		/// <param name="x"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		public static float Linear(float[] x, int m)
		{
			float result = (float)1.0;
			int M = x.Length;

			for (int i = 1; i <= M - m; i++)
				result *= x[i - 1];

			if (m != 1)
				result *= (1 - x[M - m]);

			return result;
		}

		/// <summary>
		/// Calculate a convex shape
		/// </summary>
		/// <param name="x"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		public static float Convex(float[] x, int m)
		{
			float result = (float)1.0;
			int M = x.Length;

			for (int i = 1; i <= M - m; i++)
				result *= (float)(1 - Math.Cos(x[i - 1] * Math.PI * 0.5));

			if (m != 1)
				result *= (float)(1 - Math.Sin(x[M - m] * Math.PI * 0.5));

			return result;
		}

		/// <summary>
		/// Calculate a concave shape
		/// </summary>
		/// <param name="x"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		public static float Concave(float[] x, int m)
		{
			float result = (float)1.0;
			int M = x.Length;

			for (int i = 1; i <= M - m; i++)
				result *= (float)Math.Sin(x[i - 1] * Math.PI * 0.5);

			if (m != 1)
				result *= (float)Math.Cos(x[M - m] * Math.PI * 0.5);

			return result;
		}

		/// <summary>
		/// Calculate a mixed shape
		/// </summary>
		/// <param name="x"></param>
		/// <param name="A"></param>
		/// <param name="alpha"></param>
		/// <returns></returns>
		public static float Mixed(float[] x, int A, float alpha)
		{
			float tmp;
			tmp = (float)Math.Cos((float)2.0 * A * (float)Math.PI * x[0] + (float)Math.PI * (float)0.5);
			tmp /= (float)(2.0 * (float)A * Math.PI);

			return (float)Math.Pow(((float)1.0 - x[0] - tmp), alpha);
		}

		/// <summary>
		/// Calculate a disc shape
		/// </summary>
		/// <param name="x"></param>
		/// <param name="A"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		public static float Disc(float[] x, int A, float alpha, float beta)
		{
			float tmp;
			tmp = (float)Math.Cos((float)A * Math.Pow(x[0], beta) * Math.PI);

			return (float)1.0 - (float)Math.Pow(x[0], alpha) * (float)Math.Pow(tmp, 2.0);
		}
	}
}

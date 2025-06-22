using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.WFG
{

	/// <summary>
	/// Class implementing the basics transformations for WFG
	/// </summary>
	public static class Transformations
	{

		/// <summary>
		/// Stores a default epsilon value 
		/// </summary>
		private static readonly float epsilon = (float)1.0e-10;

		/// <summary>
		///  b_poly transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="alpha"></param>
		/// <returns></returns>
		public static float B_poly(float y, float alpha)
		{

			if (!(alpha > 0))
			{

				Logger.Log.Error("WFG.Transformations.B_poly: Param alpha must be > 0");
				throw new Exception("Exception in " + typeof(Transformations).FullName + ".B_poly()");
			}

			return Correct_to_01((float)Math.Pow(y, alpha));
		}

		/// <summary>
		/// b_flat transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="C"></param>
		/// <returns></returns>
		public static float B_flat(float y, float A, float B, float C)
		{
			float tmp1 = Math.Min((float)0, (float)Math.Floor(y - B)) * A * (B - y) / B;
			float tmp2 = Math.Min((float)0, (float)Math.Floor(C - y)) * (1 - A) * (y - C) / (1 - C);

			return Correct_to_01(A + tmp1 - tmp2);
		}

		/// <summary>
		/// s_linear transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="A"></param>
		/// <returns></returns>
		public static float S_linear(float y, float A)
		{
			return Correct_to_01(Math.Abs(y - A) / (float)Math.Abs(Math.Floor(A - y) + A));
		}

		/// <summary>
		/// s_decept transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="C"></param>
		/// <returns></returns>
		public static float S_decept(float y, float A, float B, float C)
		{
			float tmp, tmp1, tmp2;

			tmp1 = (float)Math.Floor(y - A + B) * ((float)1.0 - C + (A - B) / B) / (A - B);
			tmp2 = (float)Math.Floor(A + B - y) * ((float)1.0 - C + ((float)1.0 - A - B) / B) / ((float)1.0 - A - B);

			tmp = Math.Abs(y - A) - B;

			return Correct_to_01((float)1 + tmp * (tmp1 + tmp2 + (float)1.0 / B));
		}

		/// <summary>
		/// s_multi transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="C"></param>
		/// <returns></returns>
		public static float S_multi(float y, int A, int B, float C)
		{
			float tmp1, tmp2;

			tmp1 = ((float)4.0 * A + (float)2.0) *
				   (float)Math.PI *
				   ((float)0.5 - Math.Abs(y - C) / ((float)2.0 * ((float)Math.Floor(C - y) + C)));
			tmp2 = (float)4.0 * B *
				   (float)Math.Pow(Math.Abs(y - C) / ((float)2.0 * ((float)Math.Floor(C - y) + C))
										 , (float)2.0);

			return Correct_to_01(((float)1.0 + (float)Math.Cos(tmp1) + tmp2) / (B + (float)2.0));
		}

		/// <summary>
		/// r_sum transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <returns></returns>
		public static float R_sum(float[] y, float[] w)
		{
			float tmp1 = (float)0.0, tmp2 = (float)0.0;
			for (int i = 0; i < y.Length; i++)
			{
				tmp1 += y[i] * w[i];
				tmp2 += w[i];
			}

			return Correct_to_01(tmp1 / tmp2);
		}

		/// <summary>
		/// r_nonsep transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="A"></param>
		/// <returns></returns>
		public static float R_nonsep(float[] y, int A)
		{
			float tmp, denominator, numerator;

			tmp = (float)Math.Ceiling(A / (float)2.0);
			denominator = y.Length * tmp * ((float)1.0 + (float)2.0 * A - (float)2.0 * tmp) / A;
			numerator = (float)0.0;
			for (int j = 0; j < y.Length; j++)
			{
				numerator += y[j];
				for (int k = 0; k <= A - 2; k++)
				{
					numerator += Math.Abs(y[j] - y[(j + k + 1) % y.Length]);
				}
			}

			return Correct_to_01(numerator / denominator);
		}

		/// <summary>
		/// b_param transformation
		/// </summary>
		/// <param name="y"></param>
		/// <param name="u"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="C"></param>
		/// <returns></returns>
		public static float B_param(float y, float u, float A, float B, float C)
		{
			float result, v, exp;

			v = A - ((float)1.0 - (float)2.0 * u) *
					Math.Abs((float)Math.Floor((float)0.5 - u) + A);
			exp = B + (C - B) * v;
			result = (float)Math.Pow(y, exp);

			return Correct_to_01(result);
		}

		private static float Correct_to_01(float a)
		{
			float min = (float)0.0;
			float max = (float)1.0;
			float min_epsilon = min - epsilon;
			float max_epsilon = max + epsilon;

			if ((a <= min && a >= min_epsilon) || (a >= min && a <= min_epsilon))
			{
				return min;
			}
			else if ((a >= max && a <= max_epsilon) || (a <= max && a >= max_epsilon))
			{
				return max;
			}
			else
			{
				return a;
			}
		}
	}
}

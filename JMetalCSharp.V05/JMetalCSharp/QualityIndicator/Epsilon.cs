using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.QualityIndicator
{
	/// <summary>
	/// This class implements the unary epsilon additive indicator as proposed in
	/// E. Zitzler, E. Thiele, L. Laummanns, M., Fonseca, C., and Grunert da Fonseca.
	/// V (2003): Performance Assesment of Multiobjective Optimizers: An Analysis and
	/// Review. The code is the a Java version of the original metric implementation
	/// by Eckart Zitzler.
	/// </summary>
	public class Epsilon
	{
		#region Private Attributes

		/// <summary>
		/// stores the number of objectives
		/// </summary>
		private int dim;

		/// <summary>
		/// obj_[i]=0 means objective i is to be minimized. This code always assume the minimization of all the objectives
		/// </summary>
		private int[] obj;

		/// <summary>
		/// method_ = 0 means apply additive epsilon and method_ = 1 means multiplicative
		/// epsilon. This code always apply additive epsilon
		/// </summary>
		private int method;

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns the epsilon indicator.
		/// </summary>
		/// <param name="b">True Pareto front</param>
		/// <param name="a">Solution front</param>
		/// <param name="dim"></param>
		/// <returns>the value of the epsilon indicator</returns>
		public double CalcualteEpsilon(double[][] b, double[][] a, int dim)
		{
			int i, j, k;
			double eps, eps_j = 0.0, eps_k = 0.0, eps_temp;

			this.dim = dim;
			SetParams();

			if (method == 0)
			{
				eps = double.MinValue;
			}
			else
			{
				eps = 0;
			}

			for (i = 0; i < a.Length; i++)
			{
				for (j = 0; j < b.Length; j++)
				{
					for (k = 0; k < dim; k++)
					{
						switch (method)
						{
							case 0:
								if (obj[k] == 0)
								{
									eps_temp = b[j][k] - a[i][k];
								}
								else
								{
									eps_temp = a[i][k] - b[j][k];
								}

								break;
							default:
								if ((a[i][k] < 0 && b[j][k] > 0)
									|| (a[i][k] > 0 && b[j][k] < 0)
									|| (a[i][k] == 0 || b[j][k] == 0))
								{
									Logger.Log.Error("Error in CalcualteEpsilon in data file");
									Console.Error.WriteLine("error in data file");
									Environment.Exit(0);
								}
								if (obj[k] == 0)
								{
									eps_temp = b[j][k] / a[i][k];
								}
								else
								{
									eps_temp = a[i][k] / b[j][k];
								}

								break;
						}
						if (k == 0)
						{
							eps_k = eps_temp;
						}
						else if (eps_k < eps_temp)
						{
							eps_k = eps_temp;
						}
					}
					if (j == 0)
					{
						eps_j = eps_k;
					}
					else if (eps_j > eps_k)
					{
						eps_j = eps_k;
					}
				}
				if (i == 0)
				{
					eps = eps_j;
				}
				else if (eps < eps_j)
				{
					eps = eps_j;
				}
			}
			return eps;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Established the params by default
		/// </summary>
		private void SetParams()
		{
			int i;
			obj = new int[dim];
			for (i = 0; i < dim; i++)
			{
				obj[i] = 0;
			}
			method = 0;
		}

		#endregion
	}
}

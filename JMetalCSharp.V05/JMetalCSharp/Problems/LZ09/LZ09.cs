using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Problems.LZ09
{
	/// <summary>
	///  Base class to implement the problems of the LZ09 benchmark, which is defined
	/// in: H. Li and Q. Zhang. Multiobjective optimization problems with complicated
	/// pareto sets, MOEA/D and NSGA-II. IEEE Transactions on Evolutionary
	/// Computation, 12(2):284-302, April 2009.
	/// </summary>
	public class LZ09
	{
		#region Private Attributes

		int nvar;
		int nobj;
		int ltype;
		int dtype;
		int ptype;

		#endregion

		public LZ09(int nvar, int nobj, int ptype, int dtype, int ltype)
		{
			this.nvar = nvar;
			this.nobj = nobj;
			this.ltype = ltype;
			this.dtype = dtype;
			this.ptype = ptype;
		}

		public double GetVariableValue(Variable variable, SolutionType solutionType)
		{
			double result;

			if (solutionType.GetType() == typeof(BinaryRealSolutionType))
			{
				result = ((BinaryReal)variable).Value;
			}
			else
			{
				result = ((Real)variable).Value;
			}
			return result;
		}

		/// <summary>
		/// control the PF shape
		/// </summary>
		/// <param name="alpha"></param>
		/// <param name="x"></param>
		/// <param name="dim"></param>
		/// <param name="type"></param>
		void AlphaFunction(double[] alpha, List<double> x, int dim, int type)
		{
			if (dim == 2)
			{
				if (type == 21)
				{
					alpha[0] = x[0];
					alpha[1] = 1 - Math.Sqrt(x[0]);
				}

				if (type == 22)
				{
					alpha[0] = x[0];
					alpha[1] = 1 - x[0] * x[0];
				}

				if (type == 23)
				{
					alpha[0] = x[0];
					alpha[1] = 1 - Math.Sqrt(alpha[0]) - alpha[0]
							* Math.Sin(10 * alpha[0] * alpha[0] * Math.PI);
				}

				if (type == 24)
				{
					alpha[0] = x[0];
					alpha[1] = 1 - x[0] - 0.05 * Math.Sin(4 * Math.PI * x[0]);
				}
			}
			else
			{
				if (type == 31)
				{
					alpha[0] = Math.Cos(x[0] * Math.PI / 2) * Math.Cos(x[1] * Math.PI / 2);
					alpha[1] = Math.Cos(x[0] * Math.PI / 2) * Math.Sin(x[1] * Math.PI / 2);
					alpha[2] = Math.Sin(x[0] * Math.PI / 2);
				}

				if (type == 32)
				{
					alpha[0] = 1 - Math.Cos(x[0] * Math.PI / 2)
							* Math.Cos(x[1] * Math.PI / 2);
					alpha[1] = 1 - Math.Cos(x[0] * Math.PI / 2)
							* Math.Sin(x[1] * Math.PI / 2);
					alpha[2] = 1 - Math.Sin(x[0] * Math.PI / 2);
				}

				if (type == 33)
				{
					alpha[0] = x[0];
					alpha[1] = x[1];
					alpha[2] = 3
							- (Math.Sin(3 * Math.PI * x[0]) + Math.Sin(3 * Math.PI * x[1])) - 2
							* (x[0] + x[1]);
				}

				if (type == 34)
				{
					alpha[0] = x[0] * x[1];
					alpha[1] = x[0] * (1 - x[1]);
					alpha[2] = (1 - x[0]);
				}
			}
		}

		/// <summary>
		/// control the distance
		/// </summary>
		/// <param name="x"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		double BetaFunction(List<double> x, int type)
		{
			double beta;
			beta = 0;
			int dim = x.Count;

			if (dim == 0)
			{
				beta = 0;
			}

			if (type == 1)
			{
				beta = 0;
				for (int i = 0; i < dim; i++)
				{
					beta += x[i] * x[i];
				}
				beta = 2.0 * beta / dim;
			}

			if (type == 2)
			{
				beta = 0;
				for (int i = 0; i < dim; i++)
				{
					beta += Math.Sqrt(i + 1) * x[i] * x[i];
				}
				beta = 2.0 * beta / dim;
			}

			if (type == 3)
			{
				double sum = 0, xx;
				for (int i = 0; i < dim; i++)
				{
					xx = 2 * x[i];
					sum += (xx * xx - Math.Cos(4 * Math.PI * xx) + 1);
				}
				beta = 2.0 * sum / dim;
			}

			if (type == 4)
			{
				double sum = 0, prod = 1, xx;
				for (int i = 0; i < dim; i++)
				{
					xx = 2 * x[i];
					sum += xx * xx;
					prod *= Math.Cos(10 * Math.PI * xx / Math.Sqrt(i + 1));
				}
				beta = 2.0 * (sum - 2 * prod + 2) / dim;
			}

			return beta;
		}

		/// <summary>
		/// control the PS shape of 2-d instances
		/// </summary>
		/// <param name="x"></param>
		/// <param name="t1"></param>
		/// <param name="dim"></param>
		/// <param name="type">the type of curve</param>
		/// <param name="css">the class of index</param>
		/// <returns></returns>
		double Psfunc2(double x, double t1, int dim, int type, int css)
		{
			double beta;
			beta = 0.0;

			dim++;

			if (type == 21)
			{
				double xy = 2 * (x - 0.5);
				beta = xy - Math.Pow(t1, 0.5 * (nvar + 3 * dim - 8) / (nvar - 2));
			}

			if (type == 22)
			{
				double theta = 6 * Math.PI * t1 + dim * Math.PI / nvar;
				double xy = 2 * (x - 0.5);
				beta = xy - Math.Sin(theta);
			}

			if (type == 23)
			{
				double theta = 6 * Math.PI * t1 + dim * Math.PI / nvar;
				double ra = 0.8 * t1;
				double xy = 2 * (x - 0.5);
				if (css == 1)
				{
					beta = xy - ra * Math.Cos(theta);
				}
				else
				{
					beta = xy - ra * Math.Sin(theta);
				}
			}

			if (type == 24)
			{
				double theta = 6 * Math.PI * t1 + dim * Math.PI / nvar;
				double xy = 2 * (x - 0.5);
				double ra = 0.8 * t1;
				if (css == 1)
				{
					beta = xy - ra * Math.Cos(theta / 3);
				}
				else
				{
					beta = xy - ra * Math.Sin(theta);
				}
			}

			if (type == 25)
			{
				double rho = 0.8;
				double phi = Math.PI * t1;
				double theta = 6 * Math.PI * t1 + dim * Math.PI / nvar;
				double xy = 2 * (x - 0.5);
				if (css == 1)
				{
					beta = xy - rho * Math.Sin(phi) * Math.Sin(theta);
				}
				else if (css == 2)
				{
					beta = xy - rho * Math.Sin(phi) * Math.Cos(theta);
				}
				else
				{
					beta = xy - rho * Math.Cos(phi);
				}
			}

			if (type == 26)
			{
				double theta = 6 * Math.PI * t1 + dim * Math.PI / nvar;
				double ra = 0.3 * t1 * (t1 * Math.Cos(4 * theta) + 2);
				double xy = 2 * (x - 0.5);
				if (css == 1)
				{
					beta = xy - ra * Math.Cos(theta);
				}
				else
				{
					beta = xy - ra * Math.Sin(theta);
				}
			}

			return beta;
		}

		//	control the PS shapes of 3-D instances
		double psfunc3(double x, double t1, double t2, int dim, int type)
		{
			// type:  the type of curve 
			// css:   the class of index
			double beta;
			beta = 0.0;

			dim++;

			if (type == 31)
			{
				double xy = 4 * (x - 0.5);
				double rate = 1.0 * dim / nvar;
				beta = xy - 4 * (t1 * t1 * rate + t2 * (1.0 - rate)) + 2;
			}

			if (type == 32)
			{
				double theta = 2 * Math.PI * t1 + dim * Math.PI / nvar;
				double xy = 4 * (x - 0.5);
				beta = xy - 2 * t2 * Math.Sin(theta);
			}

			return beta;
		}

		public void Objective(List<double> x_var, List<double> y_obj)
		{
			// 2-objective case
			if (nobj == 2)
			{
				if (ltype == 21 || ltype == 22 || ltype == 23 || ltype == 24 || ltype == 26)
				{
					double g = 0, h = 0, a, b;
					List<double> aa = new List<double>();
					List<double> bb = new List<double>();
					for (int n = 1; n < nvar; n++)
					{

						if (n % 2 == 0)
						{
							a = Psfunc2(x_var[n], x_var[0], n, ltype, 1);  // linkage
							aa.Add(a);
						}
						else
						{
							b = Psfunc2(x_var[n], x_var[0], n, ltype, 2);
							bb.Add(b);
						}

					}

					g = BetaFunction(aa, dtype);
					h = BetaFunction(bb, dtype);

					double[] alpha = new double[2];
					AlphaFunction(alpha, x_var, 2, ptype);  // shape function
					y_obj[0] = alpha[0] + h;
					y_obj[1] = alpha[1] + g;
					aa.Clear();
					bb.Clear();
				}

				if (ltype == 25)
				{
					double g = 0, h = 0, a, b;
					double c;
					List<double> aa = new List<double>();
					List<double> bb = new List<double>();
					for (int n = 1; n < nvar; n++)
					{
						if (n % 3 == 0)
						{
							a = Psfunc2(x_var[n], x_var[0], n, ltype, 1);
							aa.Add(a);
						}
						else if (n % 3 == 1)
						{
							b = Psfunc2(x_var[n], x_var[0], n, ltype, 2);
							bb.Add(b);
						}
						else
						{
							c = Psfunc2(x_var[n], x_var[0], n, ltype, 3);
							if (n % 2 == 0)
							{
								aa.Add(c);
							}
							else
							{
								bb.Add(c);
							}
						}
					}
					g = BetaFunction(aa, dtype);          // distance function
					h = BetaFunction(bb, dtype);
					double[] alpha = new double[2];
					AlphaFunction(alpha, x_var, 2, ptype);  // shape function
					y_obj[0] = alpha[0] + h;
					y_obj[1] = alpha[1] + g;
					aa.Clear();
					bb.Clear();
				}
			}

			// 3-objective case
			if (nobj == 3)
			{
				if (ltype == 31 || ltype == 32)
				{
					double g = 0, h = 0, e = 0, a;
					List<double> aa = new List<double>();
					List<double> bb = new List<double>();
					List<double> cc = new List<double>();
					for (int n = 2; n < nvar; n++)
					{
						a = psfunc3(x_var[n], x_var[0], x_var[1], n, ltype);
						if (n % 3 == 0)
						{
							aa.Add(a);
						}
						else if (n % 3 == 1)
						{
							bb.Add(a);
						}
						else
						{
							cc.Add(a);
						}
					}

					g = BetaFunction(aa, dtype);
					h = BetaFunction(bb, dtype);
					e = BetaFunction(cc, dtype);

					double[] alpha = new double[3];
					AlphaFunction(alpha, x_var, 3, ptype);  // shape function
					y_obj[0] = alpha[0] + h;
					y_obj[1] = alpha[1] + g;
					y_obj[2] = alpha[2] + e;
					aa.Clear();
					bb.Clear();
					cc.Clear();
				}
			}
		}
	}
}

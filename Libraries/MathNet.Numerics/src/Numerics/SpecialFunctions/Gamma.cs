﻿// <copyright file="Gamma.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2010 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

// <contribution>
//    Cephes Math Library, Stephen L. Moshier
//    ALGLIB, Sergey Bochkanov
// </contribution>

namespace MathNet.Numerics
{
    using System;
    using Properties;

    public static partial class SpecialFunctions
    {
        /// <summary>
        /// The order of the <see cref="GammaLn"/> approximation.
        /// </summary>
        private const int Gamma_n = 10;

        /// <summary>
        /// Auxiliary variable when evaluating the <see cref="GammaLn"/> function.
        /// </summary>
        private const double Gamma_r = 10.900511;

        /// <summary>
        /// Polynomial coefficients for the <see cref="GammaLn"/> approximation.
        /// </summary>
        private static readonly double[] Gamma_dk =
            new[]
            {
                2.48574089138753565546e-5,
                1.05142378581721974210,
                -3.45687097222016235469,
                4.51227709466894823700,
                -2.98285225323576655721,
                1.05639711577126713077,
                -1.95428773191645869583e-1,
                1.70970543404441224307e-2,
                -5.71926117404305781283e-4,
                4.63399473359905636708e-6,
                -2.71994908488607703910e-9
            };

        /// <summary>
        /// Computes the logarithm of the Gamma function. 
        /// </summary>
        /// <param name="z">The argument of the gamma function.</param>
        /// <returns>The logarithm of the gamma function.</returns>
        /// <remarks>
        /// <para>This implementation of the computation of the gamma and logarithm of the gamma function follows the derivation in
        ///     "An Analysis Of The Lanczos Gamma Approximation", Glendon Ralph Pugh, 2004.
        /// We use the implementation listed on p. 116 which achieves an accuracy of 16 floating point digits. Although 16 digit accuracy
        /// should be sufficient for double values, improving accuracy is possible (see p. 126 in Pugh).</para>
        /// <para>Our unit tests suggest that the accuracy of the Gamma function is correct up to 14 floating point digits.</para>
        /// </remarks>
        public static double GammaLn(double z)
        {
            if (z < 0.5)
            {
                double s = Gamma_dk[0];
                for (int i = 1; i <= Gamma_n; i++)
                {
                    s += Gamma_dk[i] / (i - z);
                }

                return Constants.LnPi
                       - Math.Log(Math.Sin(Math.PI * z))
                       - Math.Log(s)
                       - Constants.LogTwoSqrtEOverPi
                       - ((0.5 - z) * Math.Log((0.5 - z + Gamma_r) / Math.E));
            }
            else
            {
                double s = Gamma_dk[0];
                for (int i = 1; i <= Gamma_n; i++)
                {
                    s += Gamma_dk[i] / (z + i - 1.0);
                }

                return Math.Log(s)
                       + Constants.LogTwoSqrtEOverPi
                       + ((z - 0.5) * Math.Log((z - 0.5 + Gamma_r) / Math.E));
            }
        }

        /// <summary>
        /// Computes the Gamma function. 
        /// </summary>
        /// <param name="z">The argument of the gamma function.</param>
        /// <returns>The logarithm of the gamma function.</returns>
        /// <remarks>
        /// <para>
        /// This implementation of the computation of the gamma and logarithm of the gamma function follows the derivation in
        ///     "An Analysis Of The Lanczos Gamma Approximation", Glendon Ralph Pugh, 2004.
        /// We use the implementation listed on p. 116 which should achieve an accuracy of 16 floating point digits. Although 16 digit accuracy
        /// should be sufficient for double values, improving accuracy is possible (see p. 126 in Pugh).
        /// </para>
        /// <para>Our unit tests suggest that the accuracy of the Gamma function is correct up to 13 floating point digits.</para>
        /// </remarks>
        public static double Gamma(double z)
        {
            if (z < 0.5)
            {
                double s = Gamma_dk[0];
                for (int i = 1; i <= Gamma_n; i++)
                {
                    s += Gamma_dk[i] / (i - z);
                }

                return Math.PI / (Math.Sin(Math.PI * z)
                                  * s
                                  * Constants.TwoSqrtEOverPi
                                  * Math.Pow((0.5 - z + Gamma_r) / Math.E, 0.5 - z));
            }
            else
            {
                double s = Gamma_dk[0];
                for (int i = 1; i <= Gamma_n; i++)
                {
                    s += Gamma_dk[i] / (z + i - 1.0);
                }

                return s * Constants.TwoSqrtEOverPi * Math.Pow((z - 0.5 + Gamma_r) / Math.E, z - 0.5);
            }
        }
    
        /// <summary>
        /// Returns the upper incomplete regularized gamma function
        /// Q(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        /// <param name="a">The argument for the gamma function.</param>
        /// <param name="x">The lower integral limit.</param>
        /// <returns>The upper incomplete regularized gamma function.</returns>
        public static double GammaUpperRegularized(double a, double x)
        {
            double result = 0;
            double igammaepsilon = 0;
            double igammabignumber = 0;
            double igammabignumberinv = 0;
            double ans = 0;
            double ax = 0;
            double c = 0;
            double yc = 0;
            double r = 0;
            double t = 0;
            double y = 0;
            double z = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;

            igammaepsilon = 0.000000000000001;
            igammabignumber = 4503599627370496.0;
            igammabignumberinv = 2.22044604925031308085 * 0.0000000000000001;
            if (x <= 0 | a <= 0)
            {
                result = 1;
                return result;
            }

            if (x < 1 | x < a)
            {
                result = 1 - GammaLowerRegularized(a, x);
                return result;
            }

            ax = a * Math.Log(x) - x - GammaLn(a);
            if (ax < -709.78271289338399)
            {
                result = 0;
                return result;
            }

            ax = Math.Exp(ax);
            y = 1 - a;
            z = x + y + 1;
            c = 0;
            pkm2 = 1;
            qkm2 = x;
            pkm1 = x + 1;
            qkm1 = z * x;
            ans = pkm1 / qkm1;
            do
            {
                c = c + 1;
                y = y + 1;
                z = z + 2;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if (qk != 0)
                {
                    r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1;
                }

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (Math.Abs(pk) > igammabignumber)
                {
                    pkm2 = pkm2 * igammabignumberinv;
                    pkm1 = pkm1 * igammabignumberinv;
                    qkm2 = qkm2 * igammabignumberinv;
                    qkm1 = qkm1 * igammabignumberinv;
                }
            }
            while (t > igammaepsilon);

            result = ans * ax;

            return result;
        }
    
        /// <summary>
        /// Returns the upper incomplete gamma function
        /// Gamma(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        /// <param name="a">The argument for the gamma function.</param>
        /// <param name="x">The lower integral limit.</param>
        /// <returns>The upper incomplete gamma function.</returns>
        public static double GammaUpperIncomplete(double a, double x)
        {
            return GammaUpperRegularized(a, x) * Gamma(a);
        }
    
        /// <summary>
        /// Returns the lower incomplete gamma function
        /// gamma(a,x) = int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        /// <param name="a">The argument for the gamma function.</param>
        /// <param name="x">The upper integral limit.</param>
        /// <returns>The lower incomplete gamma function.</returns>
        public static double GammaLowerIncomplete(double a, double x)
        {
            return GammaLowerRegularized(a, x) * Gamma(a);
        }

        /// <summary>
        /// Returns the lower incomplete regularized gamma function
        /// P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        /// <param name="a">The argument for the gamma function.</param>
        /// <param name="x">The upper integral limit.</param>
        /// <returns>The lower incomplete gamma function.</returns>
        public static double GammaLowerRegularized(double a, double x)
        {
            const double Epsilon = 0.000000000000001;
            const double BigNumber = 4503599627370496.0;
            const double BigNumberInverse = 2.22044604925031308085e-16;

            if (a < 0d || x < 0d)
            {
                throw new ArgumentOutOfRangeException("a,x", Properties.Resources.ArgumentNotNegative);
            }

            if (a.AlmostEqual(0.0))
            {
                if (x.AlmostEqual(0.0))
                {
                    // either 0 or 1, depending on the limit direction
                    return Double.NaN;
                }

                return 1d;
            }

            if (x.AlmostEqual(0.0))
            {
                return 0d;
            }

            double ax = (a * Math.Log(x)) - x - SpecialFunctions.GammaLn(a);
            if (ax < -709.78271289338399)
            {
                return 1d;
            }

            if (x <= 1 || x <= a)
            {
                double r2 = a;
                double c2 = 1;
                double ans2 = 1;

                do
                {
                    r2 = r2 + 1;
                    c2 = c2 * x / r2;
                    ans2 += c2;
                }
                while ((c2 / ans2) > Epsilon);

                return Math.Exp(ax) * ans2 / a;
            }

            int c = 0;
            double y = 1 - a;
            double z = x + y + 1;

            double p3 = 1;
            double q3 = x;
            double p2 = x + 1;
            double q2 = z * x;
            double ans = p2 / q2;

            double error = 0;

            do
            {
                c++;
                y += 1;
                z += 2;
                double yc = y * c;

                double p = (p2 * z) - (p3 * yc);
                double q = (q2 * z) - (q3 * yc);

                if (q != 0)
                {
                    double nextans = p / q;
                    error = Math.Abs((ans - nextans) / nextans);
                    ans = nextans;
                }
                else
                {
                    // zero div, skip
                    error = 1;
                }

                // shift
                p3 = p2;
                p2 = p;
                q3 = q2;
                q2 = q;

                // normalize fraction when the numerator becomes large
                if (Math.Abs(p) > BigNumber)
                {
                    p3 *= BigNumberInverse;
                    p2 *= BigNumberInverse;
                    q3 *= BigNumberInverse;
                    q2 *= BigNumberInverse;
                }
            }
            while (error > Epsilon);

            return 1d - (Math.Exp(ax) * ans);
        }
    }
}
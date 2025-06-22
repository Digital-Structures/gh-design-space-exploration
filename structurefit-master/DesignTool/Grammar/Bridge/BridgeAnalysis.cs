using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Analysis;
using StructureEngine.GraphicStatics;
using StructureEngine.Model;

namespace StructureEngine.Grammar.Bridge
{
    public class BridgeAnalysis : IAnalysis
    {

        public void SolveHorMoment(BridgeShape s)
        {
            int N = s.Deck.Count;
            int n = N + 1;
            int m = n;
            Matrix<double> A = new DenseMatrix(m, n, 0);
            Vector<double> b = new DenseVector(n, 0);

            // Setup linear system of equations based on Gere & Timoshenko, 1990, p. 557
            for (int i = 1; i < n - 1; i++)
            {
                ShapeLine ahead = s.Deck[i];
                ShapeLine behind = s.Deck[i - 1];

                A[i, i - 1] = behind.Length;
                A[i, i] = 2 * (behind.Length + ahead.Length);
                A[i, i + 1] = ahead.Length;
            }

            A[0, 0] = 1;
            A[m - 1, n - 1] = 1;
            
            // Solve for moments above each support
            DenseLU d = new DenseLU((DenseMatrix)A);
            Vector<double> M = d.Solve(b);

            // update moment demand property in the horizontal shapelines
            for (int i = 0; i < N; i++)
            {
                s.Deck[i].BendingMoment = Math.Max(Math.Abs(M[i]), Math.Abs(M[i + 1]));
            }

            // compute reaction forces, i.e. axial forces in vertical elements
            this.SolveVertForce(s, M);
        }

        private void SolveVertForce(BridgeShape s, Vector<double> moments)
        {
            // set vertical forces for internal infill elements
            int n = s.Infill2.Count;
            for (int i = 0; i < n; i++)
            {
                double qAhead = s.Deck[i + 1].DistLoad;
                double qBehind = s.Deck[i].DistLoad;
                double LAhead = s.Deck[i + 1].Length;
                double LBehind = s.Deck[i].Length;

                double MAhead = moments[i + 2];
                double MHere = moments[i + 1];
                double MBehind = moments[i];

                double R = qAhead * LAhead / 2.0 + qBehind * LBehind / 2.0
                           + MBehind / LBehind + MAhead / LAhead
                           - MHere / LBehind - MHere / LAhead;

                s.Infill2[i].VerticalForce = R;
            }

            foreach (ShapeLine l in s.Infill2)
            {
                //if (l.GetAxialForce() != null)
                //{
                    l.AxialForce = (double)l.GetAxialForce();
                //}
            }

            // if not a suspension bridge, also set forces in outline elements
            if (!s.IsSuspension)
            {
                double qAhead = s.Deck[0].DistLoad;
                double LAhead = s.Deck[0].Length;
                double MAhead = moments[1];
                double MHere = moments[0];

                double R = qAhead * LAhead / 2.0 + MAhead / LAhead - MHere / LAhead;
                s.Infill[0].VerticalForce = R;
                s.Infill[0].AxialForce = (double)s.Infill[0].GetAxialForce();

                double qBehind = s.Deck[n].DistLoad;
                double LBehind = s.Deck[n].Length;
                double MBehind = moments[n];
                MHere = moments[n + 1];

                R = qBehind * LBehind / 2.0 + MBehind / LBehind - MHere / LBehind;
                s.Infill[s.Infill.Count - 1].VerticalForce = R;
                s.Infill[s.Infill.Count - 1].AxialForce = (double)s.Infill[s.Infill.Count - 1].GetAxialForce();
            }


        }

        private void SolveFunicular(BridgeShape s, double force)
        {
            var loads = new List<ShapeLine>();
            var segs = new List<double>();

            for (int i = 0; i < s.Infill2.Count; i++)
            {
                double x = s.Infill2[i].Start.X;
                double deltax;
                if (i == 0)
                {
                    deltax = Math.Abs(s.Infill2[i].Start.X - s.Deck[0].Start.X);
                }
                else if (i == s.Infill2.Count - 1)
                {
                    deltax = Math.Abs(s.Infill2[i].Start.X - s.Infill2[i - 1].Start.X);
                }
                else
                {
                    deltax = Math.Abs(s.Infill2[i].Start.X - s.Infill2[i - 1].Start.X);
                }
                segs.Add(deltax);
                ShapePoint start = new ShapePoint(x, 0);
                //ShapePoint end = new ShapePoint(x, start.Y - s.Infill2[i].AxialForce);
                ShapeLine load = new ShapeLine(start, s.Infill2[i].Rotation + 180, s.Infill2[i].AxialForce);
                loads.Add(load);
            }
            segs.Add(Math.Abs(s.Deck[s.Deck.Count-1].End.X - s.Infill2[s.Infill2.Count - 1].Start.X));

            var gs = new ProblemSetup(s.Deck[0].Start, s.Deck[s.Deck.Count - 1].End, loads, segs); //
            gs.DrawFcP(force);
            gs.DrawFmD(force);
        }

        public double Analyze(IDesign d)
        {
            BridgeShape s = (BridgeShape)d;
            double rateSteel = 3000; // $/ton
            rateSteel = rateSteel / 2000.0; // $/pound
            rateSteel = rateSteel * 490; // $/ft^3
            rateSteel = rateSteel / Math.Pow((12.0), 3); // $/in^3

            double rateConcrete = 100; // $/yard^3
            rateConcrete = rateConcrete / 27.0; // $/ft^3
            rateConcrete = rateConcrete / Math.Pow((12.0), 3); // $/in^3

            this.SolveHorMoment(s);
            this.SizeMembers(s);

            double volSteel = 0;
            double volConcrete = 0;

            foreach (ShapeLine h in s.Deck)
            {
                volConcrete += h.ReqArea * h.Length * 12;
            }

            foreach (ShapeLine v in s.Infill2)
            {
                volSteel += v.ReqArea * v.Length * 12;
            }

            foreach (ShapeLine f in s.Infill)
            {
                volSteel += f.ReqArea * f.Length * 12;
            }

            double costSteel = volSteel * rateSteel;
            double costConcrete = volConcrete * rateConcrete;
            double score = costSteel + costConcrete;

            // penalize for connections
            foreach (ShapePoint sp in s.Points)
            {
                score += 50; //$50 per connection
            }

            s.Score = score * 2;
            return s.Score;
        }

        private void SizeMembers(BridgeShape s)
        {
            // get maximum bending moment;
            double Mu = 0;
            foreach (ShapeLine h in s.Deck)
            {
                Mu = Math.Max(h.BendingMoment, Mu);
            }
            double As = 30; // Area of Steel = 30 in^2
            double d = 1.6 * Mu / (4 * As); // distance
            double thickness = d + 2.5;
            thickness = Math.Max(5, thickness);
            foreach (ShapeLine h in s.Deck)
            {
                h.ReqThickness = thickness;
                h.ReqArea = thickness * 30 * 12; // 360 inches wide
            }

            foreach (ShapeLine v in s.Infill2)
            {
                this.SizeAxialSteel(v);
            }

            foreach (ShapeLine f in s.Infill)
            {
                this.SizeAxialSteel(f);
            }
        }

        private void SizeAxialSteel(ShapeLine l)
        {
            double AxialForce = l.AxialForce;
            if (AxialForce > 0)
            {
                l.ReqArea = Math.Abs(AxialForce) / 20;
                l.ReqThickness = Math.Sqrt(l.ReqArea / Math.PI) * 2;
            }
            else if (AxialForce < 0)// for compression, assume a hollow pipe with 5% wall thickness
            {
                // I = PI/4 * r^4 - PI/4 * (0.95r)^4 = PI/4 * (r^4 - (1-0.95^4)r^4)
                // = 0.145686 r^4
                double thickpercent = .05;
                double percent = 1 - thickpercent;

                // safety factor of 3
                double sf = 3;
                double Ireq = sf * (Math.Abs(AxialForce) * Math.Pow(l.Length * 12, 2)) /
                                    (Math.Pow(Math.PI, 2) * 29000);

                double rreq = Math.Pow((4 / ((1 - Math.Pow(percent, 4)) * Math.PI)) * Ireq, 0.25);

                double a_stress = Math.Abs(AxialForce) / 20;
                double a_buckling = Math.PI * Math.Pow(rreq, 2) * (1 - Math.Pow(percent, 2));

                l.ReqArea = Math.Max(a_stress, a_buckling);
                l.ReqThickness = 2 * rreq;
            }
            else
            {
                l.ReqArea = 0;
                l.ReqThickness = 0;
            }
        }


    }
}

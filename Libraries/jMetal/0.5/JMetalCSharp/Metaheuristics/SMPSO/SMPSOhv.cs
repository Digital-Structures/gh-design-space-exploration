using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.QualityIndicator.FastHypervolume;
using JMetalCSharp.QualityIndicator.Utils;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;
using System.IO;

namespace JMetalCSharp.Metaheuristics.SMPSO
{
	/// <summary>
	/// This class implements the SMPSOhv algorithm described in:
	/// A.J Nebro, J.J. Durillo, C.A. Coello Coello. Analysis of Leader Selection Strategies in a
	/// Multi-Objective Particle Swarm Optimizer. 2013 IEEE Congress on Evolutionary Computation. June 2013
	/// </summary>
	public class SMPSOhv : Algorithm
	{
		#region Private Attributes
		/// <summary>
		/// Stores the number of particles used
		/// </summary>
		private int swarmSize;

		/// <summary>
		/// Stores the maximum size for the archive
		/// </summary>
		private int archiveSize;

		/// <summary>
		/// Stores the maximum number of iteration
		/// </summary>
		private int maxIterations;

		/// <summary>
		/// Stores the current number of iteration
		/// </summary>
		private int iteration;

		/// <summary>
		/// Stores the particles
		/// </summary>
		private SolutionSet particles;

		/// <summary>
		/// Stores the best solutions founds so far for each particles
		/// </summary>
		private Solution[] best;

		/// <summary>
		/// Stores the speed of each particle
		/// </summary>
		private double[][] speed;

		/// <summary>
		/// Stores a comparator for checking dominance
		/// </summary>
		private IComparer<Solution> dominance;

		/// <summary>
		/// Stores a comparator for crowding checking
		/// </summary>
		private IComparer<Solution> crowdingDistanceComparator;

		/// <summary>
		/// Stores a <code>Distance</code> object
		/// </summary>
		private Distance distance;

		/// <summary>
		/// Stores a operator for non uniform mutations
		/// </summary>
		private Operator polynomialMutation;

		QualityIndicator.QualityIndicator indicators; // QualityIndicator object

		double r1Max;
		double r1Min;
		double r2Max;
		double r2Min;
		double C1Max;
		double C1Min;
		double C2Max;
		double C2Min;
		double WMax;
		double WMin;
		double ChVel1;
		double ChVel2;

		private double trueHypervolume;
		private HyperVolume hy;
		private SolutionSet trueFront;
		private double[] deltaMax;
		private double[] deltaMin;

		#endregion

		#region Public Properties
		/// <summary>
		/// Stores the leaders
		/// </summary>
		public FastHypervolumeArchive Leaders
		{
			get;
			private set;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public SMPSOhv(Problem problem)
			: base(problem)
		{
			r1Max = 1.0;
			r1Min = 0.0;
			r2Max = 1.0;
			r2Min = 0.0;
			C1Max = 2.5;
			C1Min = 1.5;
			C2Max = 2.5;
			C2Min = 1.5;
			WMax = 0.1;
			WMin = 0.1;
			ChVel1 = -1;
			ChVel2 = -1;
		}

		public SMPSOhv(Problem problem,
			List<double> variables,
			string trueParetoFront)
			: base(problem)
		{
			r1Max = variables[0];
			r1Min = variables[1];
			r2Max = variables[2];
			r2Min = variables[3];
			C1Max = variables[4];
			C1Min = variables[5];
			C2Max = variables[6];
			C2Min = variables[7];
			WMax = variables[8];
			WMin = variables[9];
			ChVel1 = variables[10];
			ChVel2 = variables[11];

			hy = new HyperVolume();
			MetricsUtil mu = new MetricsUtil();
			trueFront = mu.ReadNonDominatedSolutionSet(trueParetoFront);
			trueHypervolume = hy.Hypervolume(trueFront.WriteObjectivesToMatrix(), trueFront.WriteObjectivesToMatrix(), problem.NumberOfObjectives);

		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public SMPSOhv(Problem problem, string trueParetoFront)
			: base(problem)
		{
			hy = new HyperVolume();
			MetricsUtil mu = new MetricsUtil();
			trueFront = mu.ReadNonDominatedSolutionSet(trueParetoFront);
			trueHypervolume = hy.Hypervolume(trueFront.WriteObjectivesToMatrix(), trueFront.WriteObjectivesToMatrix(), problem.NumberOfObjectives);

			// Default configuration
			r1Max = 1.0;
			r1Min = 0.0;
			r2Max = 1.0;
			r2Min = 0.0;
			C1Max = 2.5;
			C1Min = 1.5;
			C2Max = 2.5;
			C2Min = 1.5;
			WMax = 0.1;
			WMin = 0.1;
			ChVel1 = -1;
			ChVel2 = -1;
		} // Constructor

		#endregion

		#region Public Methods
		/// <summary>
		/// Initialize all parameter of the algorithm
		/// </summary>
		public void InitParams()
		{
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "swarmSize", ref swarmSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "archiveSize", ref archiveSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxIterations", ref maxIterations);
			JMetalCSharp.Utils.Utils.GetIndicatorsFromParameters(this.InputParameters, "indicators", ref indicators);

			polynomialMutation = this.Operators["mutation"];

			iteration = 0;

			particles = new SolutionSet(swarmSize);
			best = new Solution[swarmSize];
			Leaders = new FastHypervolumeArchive(archiveSize, this.Problem.NumberOfObjectives);

			// Create comparators for dominance and crowding distance
			dominance = new DominanceComparator();
			crowdingDistanceComparator = new CrowdingDistanceComparator();
			distance = new Distance();

			// Create the speed vector
			speed = new double[swarmSize][];

			for (var i = 0; i < swarmSize; i++)
			{
				speed[i] = new double[this.Problem.NumberOfVariables];
			}

			deltaMax = new double[this.Problem.NumberOfVariables];
			deltaMin = new double[this.Problem.NumberOfVariables];
			for (int i = 0; i < this.Problem.NumberOfVariables; i++)
			{
				deltaMax[i] = (this.Problem.UpperLimit[i] - this.Problem.LowerLimit[i]) / 2.0;
				deltaMin[i] = -deltaMax[i];
			}
		}
		#endregion

		#region Public Overrides
		/// <summary>
		/// Runs of the SMPSO algorithm.
		/// </summary>
		/// <returns>A <code>SolutionSet</code> that is a set of non dominated solutions
		/// as a result of the algorithm execution  </returns>
		public override SolutionSet Execute()
		{
			InitParams();

			//->Step 1 (and 3) Create the initial population and evaluate
			for (int i = 0; i < swarmSize; i++)
			{
				Solution particle = new Solution(this.Problem);
				this.Problem.Evaluate(particle);
				this.Problem.EvaluateConstraints(particle);
				particles.Add(particle);
			}

			//-> Step2. Initialize the speed of each particle to 0
			for (int i = 0; i < swarmSize; i++)
			{
				for (int j = 0; j < this.Problem.NumberOfVariables; j++)
				{
					speed[i][j] = 0.0;
				}
			}

			// Step4 and 5   
			for (int i = 0; i < particles.Size(); i++)
			{
				Solution particle = new Solution(particles.Get(i));
				Leaders.Add(particle);
			}

			//-> Step 6. Initialize the memory of each particle
			for (int i = 0; i < particles.Size(); i++)
			{
				Solution particle = new Solution(particles.Get(i));
				best[i] = particle;
			}

			//Crowding the leaders

			Leaders.ComputeHVContribution();

			//-> Step 7. Iterations ..
			while (iteration < maxIterations)
			{
				try
				{
					//Compute the speed
					ComputeSpeed(iteration, maxIterations);
				}
				catch (IOException ex)
				{
					Logger.Log.Error(this.GetType().FullName + ".Execute()", ex);
				}

				//Compute the new positions for the particles
				ComputeNewPositions();

				//Mutate the particles
				MopsoMutation(iteration, maxIterations);

				//Evaluate the new particles in new positions
				for (int i = 0; i < particles.Size(); i++)
				{
					Solution particle = particles.Get(i);
					this.Problem.Evaluate(particle);
					this.Problem.EvaluateConstraints(particle);
				}

				//Actualize the archive          
				for (int i = 0; i < particles.Size(); i++)
				{
					Solution particle = new Solution(particles.Get(i));
					Leaders.Add(particle);
				}
				if (Leaders.Size() < archiveSize)
					Leaders.ComputeHVContribution();

				//Actualize the memory of this particle
				for (int i = 0; i < particles.Size(); i++)
				{
					int flag = dominance.Compare(particles.Get(i), best[i]);
					if (flag != 1)
					{ // the new particle is best than the older remeber        
						Solution particle = new Solution(particles.Get(i));
						best[i] = particle;
					}
				}

				iteration++;
			}
			Result = this.Leaders;
			return this.Leaders;
		}
		#endregion

		#region Private Methods

		// Adaptive inertia 
		private double InertiaWeight(int iter, int miter, double wma, double wmin)
		{
			return wma;
		}

		// constriction coefficient (M. Clerc)
		private double ConstrictionCoefficient(double c1, double c2)
		{
			double rho = c1 + c2;

			if (rho <= 4)
			{
				return 1.0;
			}
			else
			{
				return 2 / (2 - rho - Math.Sqrt(Math.Pow(rho, 2.0) - 4.0 * rho));
			}
		}


		// velocity bounds
		private double VelocityConstriction(double v, double[] deltaMax,
			double[] deltaMin, int variableIndex,
			int particleIndex)
		{


			double result;

			double dmax = deltaMax[variableIndex];
			double dmin = deltaMin[variableIndex];

			result = v;

			if (v > dmax)
			{
				result = dmax;
			}

			if (v < dmin)
			{
				result = dmin;
			}

			return result;
		}

		/// <summary>
		/// /Update the speed of each particle
		/// </summary>
		/// <param name="iter"></param>
		/// <param name="miter"></param>
		private void ComputeSpeed(int iter, int miter)
		{
			double r1, r2, W, C1, C2;
			double wmax, wmin;
			XReal bestGlobal;

			for (int i = 0; i < swarmSize; i++)
			{
				XReal particle = new XReal(particles.Get(i));
				XReal bestParticle = new XReal(best[i]);

				//Select a global best for calculate the speed of particle i, bestGlobal

				Solution one, two;
				int pos1 = JMetalRandom.Next(0, Leaders.Size() - 1);
				int pos2 = JMetalRandom.Next(0, Leaders.Size() - 1);
				one = Leaders.Get(pos1);
				two = Leaders.Get(pos2);

				if (crowdingDistanceComparator.Compare(one, two) < 1)
				{
					bestGlobal = new XReal(one);
				}
				else
				{
					bestGlobal = new XReal(two);
					//Params for velocity equation
				}

				r1 = JMetalRandom.NextDouble(r1Min, r1Max);
				r2 = JMetalRandom.NextDouble(r2Min, r2Max);
				C1 = JMetalRandom.NextDouble(C1Min, C1Max);
				C2 = JMetalRandom.NextDouble(C2Min, C2Max);
				W = JMetalRandom.NextDouble(WMin, WMax);

				wmax = WMax;
				wmin = WMin;

				for (int var = 0; var < particle.GetNumberOfDecisionVariables(); var++)
				{
					//Computing the velocity of this particle 
					speed[i][var] = VelocityConstriction(ConstrictionCoefficient(C1, C2) * (InertiaWeight(iter, miter, wmax, wmin) * speed[i][var] + C1 * r1 * (bestParticle.GetValue(var) - particle.GetValue(var)) + C2 * r2 * (bestGlobal.GetValue(var) - particle.GetValue(var))),
						deltaMax,
									deltaMin,
									var,
									i);
				}
			}
		}

		/// <summary>
		/// Update the position of each particle
		/// </summary>
		private void ComputeNewPositions()
		{
			for (int i = 0; i < swarmSize; i++)
			{
				XReal particle = new XReal(particles.Get(i));
				for (int var = 0; var < particle.GetNumberOfDecisionVariables(); var++)
				{
					particle.SetValue(var, particle.GetValue(var) + speed[i][var]);

					if (particle.GetValue(var) < this.Problem.LowerLimit[var])
					{
						particle.SetValue(var, this.Problem.LowerLimit[var]);
						speed[i][var] = speed[i][var] * ChVel1;
					}
					if (particle.GetValue(var) > this.Problem.UpperLimit[var])
					{
						particle.SetValue(var, this.Problem.UpperLimit[var]);
						speed[i][var] = speed[i][var] * ChVel2;
					}
				}
			}
		}

		/// <summary>
		/// Apply a mutation operator to some particles in the swarm
		/// </summary>
		/// <param name="actualIteration"></param>
		/// <param name="totalIterations"></param>
		private void MopsoMutation(int actualIteration, int totalIterations)
		{
			for (int i = 0; i < particles.Size(); i++)
			{
				if ((i % 6) == 0)
					polynomialMutation.Execute(particles.Get(i));
			}
		}

		#endregion
	}
}

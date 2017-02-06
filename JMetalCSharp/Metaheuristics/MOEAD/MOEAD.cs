using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace JMetalCSharp.Metaheuristics.MOEAD
{
	public class MOEAD : Algorithm
	{
		#region Private attributes

		private int populationSize;

		/// <summary>
		/// Store the population
		/// </summary>
		private SolutionSet population;

		/// <summary>
		/// Z vector (ideal point)
		/// </summary>
		private double[] z;

		/// <summary>
		/// Lambda vectors
		/// </summary>
		private double[][] lambda;

		/// <summary>
		/// neighbour size
		/// </summary>
		private int t;

		/// <summary>
		/// Neighborhood
		/// </summary>
		private int[][] neighborhood;

		/// <summary>
		/// probability that parent solutions are selected from neighbourhood
		/// </summary>
		private double delta;

		/// <summary>
		/// maximal number of solutions replaced by each child solution
		/// </summary>
		private int nr;

		private Solution[] indArray;

		private string functionType;

		private int evaluations;

		private Operator crossover;
		private Operator mutation;

		private string dataDirectory;

		#endregion

		#region Constructors

		public MOEAD(Problem problem)
			: base(problem)
		{
			functionType = "_TCHE1";
		}

		#endregion

		#region Override Functions

		public override SolutionSet Execute()
		{
			int maxEvaluations = -1;

			evaluations = 0;

			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetStringValueFromParameter(this.InputParameters, "dataDirectory", ref dataDirectory);

			Logger.Log.Info("POPSIZE: " + populationSize);
			Console.WriteLine("POPSIZE: " + populationSize);

			population = new SolutionSet(populationSize);
			indArray = new Solution[Problem.NumberOfObjectives];

			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "T", ref t);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "nr", ref nr);
			JMetalCSharp.Utils.Utils.GetDoubleValueFromParameter(this.InputParameters, "delta", ref delta);

			neighborhood = new int[populationSize][];
			for (int i = 0; i < populationSize; i++)
			{
				neighborhood[i] = new int[t];
			}

			z = new double[Problem.NumberOfObjectives];

			lambda = new double[populationSize][];
			for (int i = 0; i < populationSize; i++)
			{
				lambda[i] = new double[Problem.NumberOfObjectives];
			}
			crossover = this.Operators["crossover"];
			mutation = this.Operators["mutation"];

			//Step 1. Initialization
			//Step 1.1 Compute euclidean distances between weight vectors and find T
			InitUniformWeight();

			InitNeighborhood();

			//Step 1.2 Initialize population
			InitPoputalion();

			//Step 1.3 Initizlize z
			InitIdealPoint();

			//Step 2 Update
			do
			{
				int[] permutation = new int[populationSize];
				Utils.RandomPermutation(permutation, populationSize);

				for (int i = 0; i < populationSize; i++)
				{
					int n = permutation[i]; // or int n = i;

					int type;
					double rnd = JMetalRandom.NextDouble();

					// STEP 2.1. Mating selection based on probability
					if (rnd < delta) // if (rnd < realb)    
					{
						type = 1;   // neighborhood
					}
					else
					{
						type = 2;   // whole population
					}
					List<int> p = new List<int>();
					MatingSelection(p, n, 2, type);

					// STEP 2.2. Reproduction
					Solution child;
					Solution[] parents = new Solution[3];

					parents[0] = population.Get(p[0]);
					parents[1] = population.Get(p[1]);
					parents[2] = population.Get(n);

					// Apply DE crossover 
					child = (Solution)crossover.Execute(new object[] { population.Get(n), parents });

					// Apply mutation
					mutation.Execute(child);

					// Evaluation
					Problem.Evaluate(child);

					evaluations++;

					// STEP 2.3. Repair. Not necessary

					// STEP 2.4. Update z_
					UpdateReference(child);

					// STEP 2.5. Update of solutions
					UpdateProblem(child, n, type);
				}
			} while (evaluations < maxEvaluations);

			Result = population;

			return population;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// InitUniformWeight
		/// </summary>
		private void InitUniformWeight()
		{
			if ((Problem.NumberOfObjectives == 2) && (populationSize <= 300))
			{
				for (int n = 0; n < populationSize; n++)
				{
					double a = 1.0 * n / (populationSize - 1);
					lambda[n][0] = a;
					lambda[n][1] = 1 - a;
				}
			}
			else
			{
				string dataFileName;
				dataFileName = "W" + Problem.NumberOfObjectives + "D_" +
				  populationSize + ".dat";

				try
				{
					// Open the file
					using (StreamReader reader = new StreamReader(dataDirectory + "/" + dataFileName))
					{

						int numberOfObjectives = 0;
						int i = 0;
						int j = 0;
						string aux = reader.ReadLine();
						while (aux != null)
						{
							string[] st = aux.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
							j = 0;
							numberOfObjectives = st.Length;

							foreach (string s in st)
							{
								double value = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(s);
								lambda[i][j] = value;
								j++;
							}
							aux = reader.ReadLine();
							i++;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Log.Error("InitUniformWeight: failed when reading for file: " + dataDirectory + "/" + dataFileName, ex);
					Console.WriteLine("InitUniformWeight: failed when reading for file: " + dataDirectory + "/" + dataFileName);
				}
			}
		}

		private void InitNeighborhood()
		{
			double[] x = new double[populationSize];
			int[] idx = new int[populationSize];

			for (int i = 0; i < populationSize; i++)
			{
				// calculate the distances based on weight vectors
				for (int j = 0; j < populationSize; j++)
				{
					x[j] = Utils.DistVector(lambda[i], lambda[j]);
					idx[j] = j;
				} // for

				// find 'niche' nearest neighboring subproblems
				Utils.MinFastSort(x, idx, populationSize, t);

				Array.Copy(idx, 0, neighborhood[i], 0, t);
			} // for
		}

		private void InitPoputalion()
		{
			for (int i = 0; i < populationSize; i++)
			{
				Solution newSolution = new Solution(Problem);

				Problem.Evaluate(newSolution);
				evaluations++;
				population.Add(newSolution);
			}
		}

		private void InitIdealPoint()
		{
			for (int i = 0; i < Problem.NumberOfObjectives; i++)
			{
				z[i] = 1.0e+30;
				indArray[i] = new Solution(Problem);
				Problem.Evaluate(indArray[i]);
				evaluations++;
			} // for

			for (int i = 0; i < populationSize; i++)
			{
				UpdateReference(population.Get(i));
			}
		}

		private void UpdateReference(Solution individual)
		{
			for (int n = 0; n < Problem.NumberOfObjectives; n++)
			{
				if (individual.Objective[n] < z[n])
				{
					z[n] = individual.Objective[n];

					indArray[n] = individual;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list">the set of the indexes of selected mating parents</param>
		/// <param name="cid">the id of current subproblem</param>
		/// <param name="size">the number of selected mating parents</param>
		/// <param name="type">1 - neighborhood; otherwise - whole population</param>
		private void MatingSelection(List<int> list, int cid, int size, int type)
		{
			int ss;
			int r;
			int p;

			ss = neighborhood[cid].Length;
			while (list.Count < size)
			{
				if (type == 1)
				{
					r = JMetalRandom.Next(0, ss - 1);
					p = neighborhood[cid][r];
				}
				else
				{
					p = JMetalRandom.Next(0, populationSize - 1);
				}
				bool flag = true;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == p) // p is in the list
					{
						flag = false;
						break;
					}
				}

				if (flag)
				{
					list.Add(p);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="indiv">child solution</param>
		/// <param name="id">the id of current subproblem</param>
		/// <param name="type">update solutions in - neighborhood (1) or whole population (otherwise)</param>
		private void UpdateProblem(Solution indiv, int id, int type)
		{
			int size;
			int time;

			time = 0;

			if (type == 1)
			{
				size = neighborhood[id].Length;
			}
			else
			{
				size = population.Size();
			}
			int[] perm = new int[size];

			Utils.RandomPermutation(perm, size);

			for (int i = 0; i < size; i++)
			{
				int k;
				if (type == 1)
				{
					k = neighborhood[id][perm[i]];
				}
				else
				{
					k = perm[i];      // calculate the values of objective function regarding the current subproblem
				}
				double f1, f2;

				f1 = FitnessFunction(population.Get(k), lambda[k]);
				f2 = FitnessFunction(indiv, lambda[k]);

				if (f2 < f1)
				{
					population.Replace(k, new Solution(indiv));
					time++;
				}
				// the maximal number of solutions updated is not allowed to exceed 'limit'
				if (time >= nr)
				{
					return;
				}
			}
		}

		private double FitnessFunction(Solution individual, double[] lambda)
		{
			double fitness;
			fitness = 0.0;

			if (functionType == "_TCHE1")
			{
				double maxFun = -1.0e+30;

				for (int n = 0; n < Problem.NumberOfObjectives; n++)
				{
					double diff = Math.Abs(individual.Objective[n] - z[n]);

					double feval;
					if (lambda[n] == 0)
					{
						feval = 0.0001 * diff;
					}
					else
					{
						feval = diff * lambda[n];
					}
					if (feval > maxFun)
					{
						maxFun = feval;
					}
				}

				fitness = maxFun;
			}
			else
			{
				Logger.Log.Error("MOEAD.FitnessFunction: unknown type " + functionType);
				Console.WriteLine("MOEAD.FitnessFunction: unknown type " + functionType);
				Environment.Exit(-1);
				throw new Exception("MOEAD.FitnessFunction: unknown type " + functionType);

			}
			return fitness;
		}

		#endregion
	}
}

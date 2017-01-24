using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JMetalCSharp.Metaheuristics.MOEAD
{
	/// <summary>
	/// Class implemeting the pMOEA/D algorithm
	/// </summary>
	public class PMOEAD : Algorithm
	{
		#region Private Attribute

		private readonly object syncLock = new object();
		/// <summary>
		/// Population size
		/// </summary>
		private int populationSize;

		/// <summary>
		/// Stores the population
		/// </summary>
		private SolutionSet population;

		/// <summary>
		/// Number of threads
		/// </summary>
		private int numberOfThreads;

		/// <summary>
		/// Z vector (ideal point)
		/// </summary>
		double[] z;

		/// <summary>
		/// Lambda vectors
		/// </summary>
		double[][] lambda;

		/// <summary>
		/// Neighbour size
		/// </summary>
		int t;

		/// <summary>
		/// Neighborhood
		/// </summary>
		int[][] neighborhood;

		/// <summary>
		/// Probability that parent solutions are selected from neighbourhood
		/// </summary>
		double delta;

		/// <summary>
		/// Maximal number of solutions replaced by each child solution
		/// </summary>
		int nr;

		Solution[] indArray;

		string functionType;

		int evaluations;

		int maxEvaluations;

		Operator crossover;

		Operator mutation;
		int id;

		public Dictionary<string, object> map;

		PMOEAD parentThread;
		Task[] tasks;

		string dataDirectory;

		Barrier barrier;

		long initTime;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public PMOEAD(Problem problem)
			: base(problem)
		{

			parentThread = null;

			functionType = "_TCHE1";

			id = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentThread"></param>
		/// <param name="problem">Problem to solve</param>
		/// <param name="id"></param>
		/// <param name="numberOfThreads"></param>
		public PMOEAD(PMOEAD parentThread, Problem problem, int id, int numberOfThreads)
			: base(problem)
		{

			this.parentThread = parentThread;

			this.numberOfThreads = numberOfThreads;
			tasks = new Task[numberOfThreads];

			functionType = "_TCHE1";

			this.id = id;
		}


		#endregion

		#region Public Override

		public override SolutionSet Execute()
		{
			parentThread = this;

			evaluations = 0;
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetStringValueFromParameter(this.InputParameters, "dataDirectory", ref dataDirectory);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "numberOfThreads", ref numberOfThreads);

			tasks = new Task[numberOfThreads];

			barrier = new Barrier(numberOfThreads);

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

			crossover = Operators["crossover"]; // default: DE crossover
			mutation = Operators["mutation"];  // default: polynomial mutation

			// STEP 1. Initialization
			// STEP 1.1. Compute euclidean distances between weight vectors and find T
			InitUniformWeight();

			InitNeighborhood();

			// STEP 1.2. Initialize population
			InitPopulation();

			// STEP 1.3. Initialize z
			InitIdealPoint();

			initTime = Environment.TickCount;

			for (int j = 0; j < numberOfThreads; j++)
			{
				CreateTask(tasks, this, Problem, j, numberOfThreads);
				tasks[j].Start();
			}

			for (int i = 0; i < numberOfThreads; i++)
			{
				try
				{
					tasks[i].Wait();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error in " + this.GetType().FullName + ".Execute()");
					Logger.Log.Error(this.GetType().FullName + ".Execute()", ex);
				}
			}

			Result = population;

			return population;
		}

		private void CreateTask(Task[] tasks, PMOEAD parent, Core.Problem Problem, int j, int numberOfThreads)
		{
			tasks[j] = new Task(() => new PMOEAD(parent, Problem, j, numberOfThreads).Run());
		}

		#endregion

		#region Private Methods

		private void Run()
		{
			neighborhood = parentThread.neighborhood;
			Problem = parentThread.Problem;
			lambda = parentThread.lambda;
			population = parentThread.population;
			z = parentThread.z;
			indArray = parentThread.indArray;
			barrier = parentThread.barrier;


			int partitions = parentThread.populationSize / parentThread.numberOfThreads;

			evaluations = 0;
			maxEvaluations = parentThread.maxEvaluations / parentThread.numberOfThreads;

			barrier.SignalAndWait();

			int first;
			int last;

			first = partitions * id;
			if (id == (parentThread.numberOfThreads - 1))
			{
				last = parentThread.populationSize - 1;
			}
			else
			{
				last = first + partitions - 1;
			}

			Logger.Log.Info("Id: " + id + "  Partitions: " + partitions + " First: " + first + " Last: " + last);
			Console.WriteLine("Id: " + id + "  Partitions: " + partitions + " First: " + first + " Last: " + last);
			do
			{
				for (int i = first; i <= last; i++)
				{
					int n = i;
					int type;
					double rnd = JMetalRandom.NextDouble();

					// STEP 2.1. Mating selection based on probability
					if (rnd < parentThread.delta)
					{
						type = 1;   // neighborhood
					}
					else
					{
						type = 2;   // whole population
					}

					List<int> p = new List<int>();
					this.MatingSelection(p, n, 2, type);

					// STEP 2.2. Reproduction
					Solution child = null;
					Solution[] parents = new Solution[3];

					try
					{
						lock (parentThread)
						{
							parents[0] = parentThread.population.Get(p[0]);
							parents[1] = parentThread.population.Get(p[1]);
							parents[2] = parentThread.population.Get(n);
							// Apply DE crossover
							child = (Solution)parentThread.crossover.Execute(new object[] { parentThread.population.Get(n), parents });
						}
						// Apply mutation
						parentThread.mutation.Execute(child);

						// Evaluation
						parentThread.Problem.Evaluate(child);

					}
					catch (Exception ex)
					{
						Logger.Log.Error(this.GetType().FullName + ".Run()", ex);
						Console.WriteLine("Error in " + this.GetType().FullName + ".Run()");
					}

					evaluations++;

					// STEP 2.3. Repair. Not necessary

					// STEP 2.4. Update z
					UpdateReference(child);

					// STEP 2.5. Update of solutions
					UpdateOfSolutions(child, n, type);
				}
			} while (evaluations < maxEvaluations);

			long estimatedTime = Environment.TickCount - parentThread.initTime;
			Logger.Log.Info("Time thread " + id + ": " + estimatedTime);
			Console.WriteLine("Time thread " + id + ": " + estimatedTime);
		}

		private void InitUniformWeight()
		{
			if ((Problem.NumberOfObjectives == 2) && (populationSize < 300))
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
				dataFileName = "W" + Problem.NumberOfObjectives + "D_" + populationSize + ".dat";

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
				}

				// find 'niche' nearest neighboring subproblems
				Utils.MinFastSort(x, idx, populationSize, t);

				for (int k = 0; k < t; k++)
				{
					neighborhood[i][k] = idx[k];
				}
			}
		}

		private void InitPopulation()
		{
			for (int i = 0; i < populationSize; i++)
			{
				Solution newSolution = new Solution(Problem);

				Problem.Evaluate(newSolution);
				evaluations++;
				population.Add(newSolution);
			}
		}

		void InitIdealPoint()
		{
			for (int i = 0; i < Problem.NumberOfObjectives; i++)
			{
				z[i] = 1.0e+30;
				indArray[i] = new Solution(Problem);
				Problem.Evaluate(indArray[i]);
				evaluations++;
			}

			for (int i = 0; i < populationSize; i++)
			{
				UpdateReference(population.Get(i));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list">The set of the indexes of selected mating parents</param>
		/// <param name="cid">The id of current subproblem</param>
		/// <param name="size">The number of selected mating parents</param>
		/// <param name="type">1 - neighborhood; otherwise - whole population</param>
		private void MatingSelection(List<int> list, int cid, int size, int type)
		{
			int ss;
			int r;
			int p;

			ss = parentThread.neighborhood[cid].Length;
			while (list.Count < size)
			{
				if (type == 1)
				{
					r = JMetalRandom.Next(0, ss - 1);
					p = parentThread.neighborhood[cid][r];
				}
				else
				{
					p = JMetalRandom.Next(0, parentThread.populationSize - 1);
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

		private void UpdateReference(Solution individual)
		{
			lock (syncLock)
			{
				for (int n = 0; n < parentThread.Problem.NumberOfObjectives; n++)
				{
					if (individual.Objective[n] < z[n])
					{
						parentThread.z[n] = individual.Objective[n];

						parentThread.indArray[n] = individual;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="indiv">Child solution</param>
		/// <param name="id">The id of current subproblem</param>
		/// <param name="type">Update solutions in - neighborhood (1) or whole population (otherwise)</param>
		private void UpdateOfSolutions(Solution indiv, int id, int type)
		{
			int size;
			int time;

			time = 0;

			if (type == 1)
			{
				size = parentThread.neighborhood[id].Length;
			}
			else
			{
				size = parentThread.population.Size();
			}
			int[] perm = new int[size];

			Utils.RandomPermutation(perm, size);

			for (int i = 0; i < size; i++)
			{
				int k;
				if (type == 1)
				{
					k = parentThread.neighborhood[id][perm[i]];
				}
				else
				{
					k = perm[i];      // calculate the values of objective function regarding the current subproblem
				}
				double f1, f2;

				f2 = FitnessFunction(indiv, parentThread.lambda[k]);
				lock (parentThread)
				{
					f1 = FitnessFunction(parentThread.population.Get(k), parentThread.lambda[k]);

					if (f2 < f1)
					{
						parentThread.population.Replace(k, new Solution(indiv));
						time++;
					}
				}
				// the maximal number of solutions updated is not allowed to exceed 'limit'
				if (time >= parentThread.nr)
				{
					return;
				}
			}
		}

		double FitnessFunction(Solution individual, double[] lambda)
		{
			double fitness;
			fitness = 0.0;

			if (parentThread.functionType == "_TCHE1")
			{
				double maxFun = -1.0e+30;

				for (int n = 0; n < parentThread.Problem.NumberOfObjectives; n++)
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

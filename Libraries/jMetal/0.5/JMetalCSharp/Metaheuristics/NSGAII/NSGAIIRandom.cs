using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using JMetalCSharp.Utils.OffSpring;
using System;

namespace JMetalCSharp.Metaheuristics.NSGAII
{
	public class NSGAIIRandom : Algorithm
	{
		public int populationSize;
		public SolutionSet population;
		public SolutionSet offspringPopulation;
		public SolutionSet union;

		int maxEvaluations;
		int evaluations;

		int[] contributionCounter; // contribution per crossover operator
		double[] contribution; // contribution per crossover operator

		Operator selectionOperator;

		public NSGAIIRandom(Problem problem)
			: base(problem)
		{
		}

		public override SolutionSet Execute()
		{
			double[] contrReal = new double[3];

			contrReal[0] = contrReal[1] = contrReal[2] = 0;

			Distance distance = new Distance();

			//Read parameter values
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);

			//Init the variables
			population = new SolutionSet(populationSize);
			evaluations = 0;

			selectionOperator = Operators["selection"];

			Offspring[] getOffspring;
			int N_O; // number of offpring objects

			getOffspring = (Offspring[])GetInputParameter("offspringsCreators");
			N_O = getOffspring.Length;

			contribution = new double[N_O];
			contributionCounter = new int[N_O];

			contribution[0] = (double)(populationSize / (double)N_O) / (double)populationSize;
			for (int i = 1; i < N_O; i++)
			{
				contribution[i] = (double)(populationSize / (double)N_O) / (double)populationSize + (double)contribution[i - 1];
			}

			for (int i = 0; i < N_O; i++)
			{
				Console.WriteLine(getOffspring[i].Configuration());
				Console.WriteLine("Contribution: " + contribution[i]);
			}

			// Create the initial solutionSet
			Solution newSolution;
			for (int i = 0; i < populationSize; i++)
			{
				newSolution = new Solution(Problem);
				Problem.Evaluate(newSolution);
				Problem.EvaluateConstraints(newSolution);
				evaluations++;
				newSolution.Location = i;
				population.Add(newSolution);
			}

			while (evaluations < maxEvaluations)
			{

				// Create the offSpring solutionSet      
				offspringPopulation = new SolutionSet(populationSize);
				Solution[] parents = new Solution[2];
				for (int i = 0; i < (populationSize / 1); i++)
				{
					if (evaluations < maxEvaluations)
					{
						Solution individual = new Solution(population.Get(JMetalRandom.Next(0, populationSize - 1)));

						int selected = 0;
						bool found = false;
						Solution offSpring = null;
						double rnd = JMetalRandom.NextDouble();
						for (selected = 0; selected < N_O; selected++)
						{

							if (!found && (rnd <= contribution[selected]))
							{
								if ("DE" == getOffspring[selected].Id)
								{
									offSpring = getOffspring[selected].GetOffspring(population, i);
									//contrDE++;
								}
								else if ("SBXCrossover" == getOffspring[selected].Id)
								{
									offSpring = getOffspring[selected].GetOffspring(population);
									//contrSBX++;
								}
								else if ("PolynomialMutation" == getOffspring[selected].Id)
								{
									offSpring = ((PolynomialMutationOffspring)getOffspring[selected]).GetOffspring(individual);
									//contrPol++;
								}
								else
								{
									Logger.Log.Error("Error in NSGAIIRandom. Operator " + offSpring + " does not exist");
									Console.WriteLine("Error in NSGAIIRandom. Operator " + offSpring + " does not exist");
								}

								offSpring.Fitness = (int)selected;
								found = true;
							}
						}

						Problem.Evaluate(offSpring);
						offspringPopulation.Add(offSpring);
						evaluations += 1;
					}
				}

				// Create the solutionSet union of solutionSet and offSpring
				union = ((SolutionSet)population).Union(offspringPopulation);

				// Ranking the union
				Ranking ranking = new Ranking(union);

				int remain = populationSize;
				int index = 0;
				SolutionSet front = null;
				population.Clear();

				// Obtain the next front
				front = ranking.GetSubfront(index);

				while ((remain > 0) && (remain >= front.Size()))
				{
					//Assign crowding distance to individuals
					distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
					//Add the individuals of this front
					for (int k = 0; k < front.Size(); k++)
					{
						population.Add(front.Get(k));
					}

					//Decrement remain
					remain = remain - front.Size();

					//Obtain the next front
					index++;
					if (remain > 0)
					{
						front = ranking.GetSubfront(index);
					}
				}

				// Remain is less than front(index).size, insert only the best one
				if (remain > 0)
				{  // front contains individuals to insert                        
					distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
					front.Sort(new CrowdingComparator());
					for (int k = 0; k < remain; k++)
					{
						population.Add(front.Get(k));
					}
				}
			}

			// Return the first non-dominated front
			Ranking rank = new Ranking(population);

			Result = rank.GetSubfront(0);

			return Result;
		}
	}
}

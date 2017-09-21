using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Archive;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Metaheuristics.MOCell
{
	/// <summary>
	/// This class represents an asynchronous version of MOCell algorithm. It is
	/// based on aMOCell1 but replacing the worst neighbor. 
	/// </summary>
	public class aMOCell3 : Algorithm
	{

		#region Constructor

		public aMOCell3(Problem problem)
			: base(problem)
		{
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Execute the algorithm 
		/// </summary>
		/// <returns></returns>
		public override SolutionSet Execute()
		{
			int populationSize = -1,
				archiveSize = -1,
				maxEvaluations = -1,
				evaluations = -1,
				feedBack = -1;

			Operator mutationOperator, crossoverOperator, selectionOperator;
			SolutionSet currentPopulation;
			CrowdingArchive archive;
			SolutionSet[] neighbors;
			Neighborhood neighborhood;
			IComparer<Solution> dominance = new DominanceComparator();
			IComparer<Solution> crowdingComparator = new CrowdingComparator();
			Distance distance = new Distance();

			//Read the parameters
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "archiveSize", ref archiveSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "feedBack", ref feedBack);


			//Read the operators
			mutationOperator = Operators["mutation"];
			crossoverOperator = Operators["crossover"];
			selectionOperator = Operators["selection"];

			//Initialize the variables
			//Initialize the population and the archive
			currentPopulation = new SolutionSet(populationSize);
			archive = new CrowdingArchive(archiveSize, this.Problem.NumberOfObjectives);
			evaluations = 0;
			neighborhood = new Neighborhood(populationSize);
			neighbors = new SolutionSet[populationSize];

			//Create the comparator for check dominance
			dominance = new DominanceComparator();

			//Create the initial population
			for (int i = 0; i < populationSize; i++)
			{
				Solution individual = new Solution(this.Problem);
				this.Problem.Evaluate(individual);
				this.Problem.EvaluateConstraints(individual);
				currentPopulation.Add(individual);
				individual.Location = i;
				evaluations++;
			}

			while (evaluations < maxEvaluations)
			{
				for (int ind = 0; ind < currentPopulation.Size(); ind++)
				{
					Solution individual = new Solution(currentPopulation.Get(ind));

					Solution[] parents = new Solution[2];
					Solution[] offSpring;

					neighbors[ind] = neighborhood.GetEightNeighbors(currentPopulation, ind);
					neighbors[ind].Add(individual);

					//parents
					parents[0] = (Solution)selectionOperator.Execute(neighbors[ind]);
					parents[1] = (Solution)selectionOperator.Execute(neighbors[ind]);

					//Create a new individual, using genetic operators mutation and crossover
					offSpring = (Solution[])crossoverOperator.Execute(parents);
					mutationOperator.Execute(offSpring[0]);

					//->Evaluate individual an his constraints
					this.Problem.Evaluate(offSpring[0]);
					this.Problem.EvaluateConstraints(offSpring[0]);
					evaluations++;
					//<-Individual evaluated

					int flag = dominance.Compare(individual, offSpring[0]);

					if (flag == 1)
					{ //The new individuals dominate
						offSpring[0].Location = individual.Location;
						currentPopulation.Replace(offSpring[0].Location, offSpring[0]);
						archive.Add(new Solution(offSpring[0]));
					}
					else if (flag == 0)
					{//The individuals are non-dominates
						neighbors[ind].Add(offSpring[0]);
						offSpring[0].Location = -1;
						Ranking rank = new Ranking(neighbors[ind]);
						for (int j = 0; j < rank.GetNumberOfSubfronts(); j++)
						{
							distance.CrowdingDistanceAssignment(rank.GetSubfront(j), this.Problem.NumberOfObjectives);
						}
						Solution worst = neighbors[ind].Worst(crowdingComparator);

						if (worst.Location == -1)
						{//The worst is the offspring
							archive.Add(new Solution(offSpring[0]));
						}
						else
						{
							offSpring[0].Location = worst.Location;
							currentPopulation.Replace(offSpring[0].Location, offSpring[0]);
							archive.Add(new Solution(offSpring[0]));
						}
					}
				}

				//Store a portion of the archive into the population
				distance.CrowdingDistanceAssignment(archive, this.Problem.NumberOfObjectives);
				for (int j = 0; j < feedBack; j++)
				{
					if (archive.Size() > j)
					{
						int r = JMetalRandom.Next(0, currentPopulation.Size() - 1);
						if (r < currentPopulation.Size())
						{
							Solution individual = archive.Get(j);
							individual.Location = r;
							currentPopulation.Replace(r, new Solution(individual));
						}
					}
				}
			}
			Result = archive;
			return archive;
		}

		#endregion
	}
}

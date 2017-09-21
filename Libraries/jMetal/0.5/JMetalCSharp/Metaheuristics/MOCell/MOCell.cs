using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Archive;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Metaheuristics.MOCell
{
	/// <summary>
	/// This class represents the MOCell algorithm
	/// </summary>
	public class MOCell : Algorithm
	{
		#region Constructor
		public MOCell(Problem problem)
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
			//Init the parameters
			int populationSize = -1,
				archiveSize = -1,
				maxEvaluations = -1,
				evaluations = -1;

			Operator mutationOperator, crossoverOperator, selectionOperator;
			SolutionSet currentPopulation;
			CrowdingArchive archive;
			SolutionSet[] neighbors;
			Neighborhood neighborhood;
			IComparer<Solution> dominance = new DominanceComparator();
			IComparer<Solution> crowdingComparator = new CrowdingComparator();
			Distance distance = new Distance();

			// Read the parameters
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "archiveSize", ref archiveSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);

			// Read the operators
			mutationOperator = this.Operators["mutation"];
			crossoverOperator = this.Operators["crossover"];
			selectionOperator = this.Operators["selection"];

			// Initialize the variables    
			currentPopulation = new SolutionSet(populationSize);
			archive = new CrowdingArchive(archiveSize, this.Problem.NumberOfObjectives);
			evaluations = 0;
			neighborhood = new Neighborhood(populationSize);
			neighbors = new SolutionSet[populationSize];

			// Create the initial population
			for (int i = 0; i < populationSize; i++)
			{
				Solution individual = new Solution(this.Problem);
				this.Problem.Evaluate(individual);
				this.Problem.EvaluateConstraints(individual);
				currentPopulation.Add(individual);
				individual.Location = i;
				evaluations++;
			}

			// Main loop
			while (evaluations < maxEvaluations)
			{
				for (int ind = 0; ind < currentPopulation.Size(); ind++)
				{
					Solution individual = new Solution(currentPopulation.Get(ind));

					Solution[] parents = new Solution[2];
					Solution[] offSpring;

					neighbors[ind] = neighborhood.GetEightNeighbors(currentPopulation, ind);
					neighbors[ind].Add(individual);

					// parents
					parents[0] = (Solution)selectionOperator.Execute(neighbors[ind]);
					if (archive.Size() > 0)
					{
						parents[1] = (Solution)selectionOperator.Execute(archive);
					}
					else
					{
						parents[1] = (Solution)selectionOperator.Execute(neighbors[ind]);
					}

					// Create a new individual, using genetic operators mutation and crossover
					offSpring = (Solution[])crossoverOperator.Execute(parents);
					mutationOperator.Execute(offSpring[0]);

					// Evaluate individual an his constraints
					this.Problem.Evaluate(offSpring[0]);
					this.Problem.EvaluateConstraints(offSpring[0]);
					evaluations++;

					int flag = dominance.Compare(individual, offSpring[0]);

					if (flag == 1)
					{ //The new individual dominates
						offSpring[0].Location = individual.Location;
						currentPopulation.Replace(offSpring[0].Location, offSpring[0]);
						archive.Add(new Solution(offSpring[0]));
					}
					else if (flag == 0)
					{ //The new individual is non-dominated
						neighbors[ind].Add(offSpring[0]);
						offSpring[0].Location = -1;
						Ranking rank = new Ranking(neighbors[ind]);
						for (int j = 0; j < rank.GetNumberOfSubfronts(); j++)
						{
							distance.CrowdingDistanceAssignment(rank.GetSubfront(j), this.Problem.NumberOfObjectives);
						}
						Solution worst = neighbors[ind].Worst(crowdingComparator);

						if (worst.Location == -1)
						{ //The worst is the offspring
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
			}
			Result = archive;
			return archive;
		}

		#endregion
	}
}

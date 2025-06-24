using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Archive;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Metaheuristics.MOCell
{
	/// <summary>
	/// This class represents a synchronous version of MOCell algorithm, which
	/// applies an archive feedback through parent selection.
	/// </summary>
	public class sMOCell2 : Algorithm
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public sMOCell2(Problem problem)
			: base(problem)
		{
		}
		#endregion

		#region Public Overrides

		/// <summary>
		/// Runs of the sMOCell2 algorithm.
		/// </summary>
		/// <returns>A <code>SolutionSet</code> that is a set of non dominated solutions
		/// as a result of the algorithm execution</returns>
		public override SolutionSet Execute()
		{
			int populationSize = -1,
				archiveSize = -1,
				maxEvaluations = -1,
				evaluations = -1;

			Operator mutationOperator, crossoverOperator, selectionOperator;
			SolutionSet currentSolutionSet, newSolutionSet;
			CrowdingArchive archive;
			SolutionSet[] neighbors;
			Neighborhood neighborhood;
			IComparer<Solution> dominance = new DominanceComparator(),
			crowding = new CrowdingComparator();
			Distance distance = new Distance();

			//Read the params

			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "archiveSize", ref archiveSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);

			//Read the operators
			mutationOperator = Operators["mutation"];
			crossoverOperator = Operators["crossover"];
			selectionOperator = Operators["selection"];

			//Initialize the variables    
			currentSolutionSet = new SolutionSet(populationSize);
			newSolutionSet = new SolutionSet(populationSize);
			archive = new CrowdingArchive(archiveSize, this.Problem.NumberOfObjectives);
			evaluations = 0;
			neighborhood = new Neighborhood(populationSize);
			neighbors = new SolutionSet[populationSize];

			//Create the initial population
			for (int i = 0; i < populationSize; i++)
			{
				Solution solution = new Solution(this.Problem);
				this.Problem.Evaluate(solution);
				this.Problem.EvaluateConstraints(solution);
				currentSolutionSet.Add(solution);
				solution.Location = i;
				evaluations++;
			}


			while (evaluations < maxEvaluations)
			{
				newSolutionSet = new SolutionSet(populationSize);
				for (int ind = 0; ind < currentSolutionSet.Size(); ind++)
				{
					Solution individual = new Solution(currentSolutionSet.Get(ind));

					Solution[] parents = new Solution[2];
					Solution[] offSpring;

					neighbors[ind] = neighborhood.GetEightNeighbors(currentSolutionSet, ind);
					neighbors[ind].Add(individual);

					//parents
					parents[0] = (Solution)selectionOperator.Execute(neighbors[ind]);
					if (archive.Size() > 0)
					{
						parents[1] = (Solution)selectionOperator.Execute(archive);
					}
					else
					{
						parents[1] = (Solution)selectionOperator.Execute(neighbors[ind]);
					}

					//Create a new solution, using genetic operators mutation and crossover
					offSpring = (Solution[])crossoverOperator.Execute(parents);
					mutationOperator.Execute(offSpring[0]);

					//->Evaluate solution an his constraints
					this.Problem.Evaluate(offSpring[0]);
					this.Problem.EvaluateConstraints(offSpring[0]);
					evaluations++;
					//<-Individual evaluated

					int flag = dominance.Compare(individual, offSpring[0]);

					if (flag == -1)
					{
						newSolutionSet.Add(new Solution(currentSolutionSet.Get(ind)));
					}

					if (flag == 1)
					{//The new indivudlas dominate
						offSpring[0].Location = individual.Location;
						newSolutionSet.Add(offSpring[0]);
						archive.Add(new Solution(offSpring[0]));
					}
					else if (flag == 0)
					{ //The individuals are non-dominates
						neighbors[ind].Add(offSpring[0]);
						Ranking rank = new Ranking(neighbors[ind]);
						for (int j = 0; j < rank.GetNumberOfSubfronts(); j++)
						{
							distance.CrowdingDistanceAssignment(rank.GetSubfront(j), this.Problem.NumberOfObjectives);
						}

						bool deleteMutant = true;

						int compareResult = crowding.Compare(individual, offSpring[0]);
						if (compareResult == 1)
						{ //The offSpring[0] is better
							deleteMutant = false;
						}

						if (!deleteMutant)
						{
							offSpring[0].Location = individual.Location;

							newSolutionSet.Add(offSpring[0]);
							archive.Add(new Solution(offSpring[0]));
						}
						else
						{
							newSolutionSet.Add(new Solution(currentSolutionSet.Get(ind)));
							archive.Add(new Solution(offSpring[0]));
						}
					}
				}
				currentSolutionSet = newSolutionSet;
			}
			Result = archive;
			return archive;
		}

		#endregion
	}
}

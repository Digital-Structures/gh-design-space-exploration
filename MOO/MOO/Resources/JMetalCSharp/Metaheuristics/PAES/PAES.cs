using JMetalCSharp.Core;
using JMetalCSharp.Utils.Archive;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Metaheuristics.PAES
{
	/// <summary>
	/// This class implements the PAES algorithm. 
	/// </summary>
	public class PAES : Algorithm
	{

		#region Constructor
		/// <summary>
		/// Create a new PAES instance for resolve a problem
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public PAES(Problem problem)
			: base(problem)
		{
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Runs of the Paes algorithm.
		/// </summary>
		/// <returns>a <code>SolutionSet</code> that is a set of non dominated solutions
		/// as a result of the algorithm execution  </returns>
		public override SolutionSet Execute()
		{
			int bisections = -1,
				archiveSize = -1,
				maxEvaluations = -1,
				evaluations;

			AdaptiveGridArchive archive;
			Operator mutationOperator;
			IComparer<Solution> dominance;

			//Read the params
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "biSections", ref bisections);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "archiveSize", ref archiveSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);

			//Read the operators        
			mutationOperator = this.Operators["mutation"];

			//Initialize the variables                
			evaluations = 0;
			archive = new AdaptiveGridArchive(archiveSize, bisections, this.Problem.NumberOfObjectives);
			dominance = new DominanceComparator();

			//-> Create the initial solution and evaluate it and his constraints
			Solution solution = new Solution(this.Problem);
			this.Problem.Evaluate(solution);
			this.Problem.EvaluateConstraints(solution);
			evaluations++;

			// Add it to the archive
			archive.Add(new Solution(solution));

			//Iterations....
			do
			{
				// Create the mutate one
				Solution mutatedIndividual = new Solution(solution);
				mutationOperator.Execute(mutatedIndividual);

				this.Problem.Evaluate(mutatedIndividual);
				this.Problem.EvaluateConstraints(mutatedIndividual);
				evaluations++;

				// Check dominance
				int flag = dominance.Compare(solution, mutatedIndividual);

				if (flag == 1)
				{ //If mutate solution dominate
					solution = new Solution(mutatedIndividual);
					archive.Add(mutatedIndividual);
				}
				else if (flag == 0)
				{ //If none dominate the other
					if (archive.Add(mutatedIndividual))
					{
						solution = Test(solution, mutatedIndividual, archive);
					}
				}
			} while (evaluations < maxEvaluations);
			Result = archive;
			return archive;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Tests two solutions to determine which one becomes be the guide of PAES algorithm
		/// </summary>
		/// <param name="solution"> The actual guide of PAES</param>
		/// <param name="mutatedSolution">A candidate guide</param>
		/// <param name="archive"></param>
		/// <returns></returns>
		private Solution Test(Solution solution, Solution mutatedSolution, AdaptiveGridArchive archive)
		{

			int originalLocation = archive.Grid.Location(solution);
			int mutatedLocation = archive.Grid.Location(mutatedSolution);

			if (originalLocation == -1)
			{
				return new Solution(mutatedSolution);
			}

			if (mutatedLocation == -1)
			{
				return new Solution(solution);
			}

			if (archive.Grid.GetLocationDensity(mutatedLocation) <
				archive.Grid.GetLocationDensity(originalLocation))
			{
				return new Solution(mutatedSolution);
			}

			return new Solution(solution);
		}

		#endregion
	}
}

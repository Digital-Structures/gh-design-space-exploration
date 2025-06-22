using JMetalCSharp.Core;
using JMetalCSharp.Utils;

namespace JMetalCSharp.Metaheuristics.SPEA2
{

	/// <summary>
	/// This class representing the SPEA2 algorithm
	/// </summary>
	public class SPEA2 : Algorithm
	{

		#region Constants
		/// <summary>
		///  Defines the number of tournaments for creating the mating pool
		/// </summary>
		public static readonly int TOURNAMENTS_ROUNDS = 1;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// Create a new SPEA2 instance
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public SPEA2(Problem problem)
			: base(problem)
		{
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Runs of the Spea2 algorithm.
		/// </summary>
		/// <returns>A <code>SolutionSet</code> that is a set of non dominated solutions as a result of the algorithm execution</returns>
		public override SolutionSet Execute()
		{
			int populationSize = -1,
				archiveSize = -1,
				maxEvaluations = -1,
				evaluations;

			Operator crossoverOperator,
				mutationOperator,
				selectionOperator;

			SolutionSet solutionSet,
				archive,
				offSpringSolutionSet;

			//Read the params
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "archiveSize", ref archiveSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);

			//Read the operators
			crossoverOperator = Operators["crossover"];
			mutationOperator = Operators["mutation"];
			selectionOperator = Operators["selection"];

			//Initialize the variables
			solutionSet = new SolutionSet(populationSize);
			archive = new SolutionSet(archiveSize);
			evaluations = 0;

			//-> Create the initial solutionSet
			Solution newSolution;
			for (int i = 0; i < populationSize; i++)
			{
				newSolution = new Solution(Problem);
				Problem.Evaluate(newSolution);
				Problem.EvaluateConstraints(newSolution);
				evaluations++;
				solutionSet.Add(newSolution);
			}

			while (evaluations < maxEvaluations)
			{
				SolutionSet union = ((SolutionSet)solutionSet).Union(archive);
				Spea2Fitness spea = new Spea2Fitness(union);
				spea.FitnessAssign();
				archive = spea.EnvironmentalSelection(archiveSize);
				// Create a new offspringPopulation
				offSpringSolutionSet = new SolutionSet(populationSize);
				Solution[] parents = new Solution[2];
				while (offSpringSolutionSet.Size() < populationSize)
				{
					int j = 0;
					do
					{
						j++;
						parents[0] = (Solution)selectionOperator.Execute(archive);
					} while (j < SPEA2.TOURNAMENTS_ROUNDS);
					int k = 0;
					do
					{
						k++;
						parents[1] = (Solution)selectionOperator.Execute(archive);
					} while (k < SPEA2.TOURNAMENTS_ROUNDS);

					//make the crossover 
					Solution[] offSpring = (Solution[])crossoverOperator.Execute(parents);
					mutationOperator.Execute(offSpring[0]);
					Problem.Evaluate(offSpring[0]);
					Problem.EvaluateConstraints(offSpring[0]);
					offSpringSolutionSet.Add(offSpring[0]);
					evaluations++;
				}
				// End Create a offSpring solutionSet
				solutionSet = offSpringSolutionSet;
			}

			Ranking ranking = new Ranking(archive);
			Result = ranking.GetSubfront(0);
			return Result;
		}

		#endregion
	}
}

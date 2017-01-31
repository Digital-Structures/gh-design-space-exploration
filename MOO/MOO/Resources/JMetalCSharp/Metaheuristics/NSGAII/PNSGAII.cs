using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using JMetalCSharp.Utils.Parallel;
using System.Collections.Generic;

namespace JMetalCSharp.Metaheuristics.NSGAII
{
	/// <summary>
	/// Implementation of NSGA-II. This implementation of NSGA-II makes use of a
	/// QualityIndicator object to obtained the convergence speed of the algorithm.
	/// This version is used in the paper: A.J. Nebro, J.J. Durillo, C.A. Coello
	/// Coello, F. Luna, E. Alba "A Study of Convergence Speed in Multi-Objective
	/// Metaheuristics." To be presented in: PPSN'08. Dortmund. September 2008.
	/// </summary>
	public class PNSGAII : Algorithm
	{
		ISynchronousParallelRunner parallelEvaluator;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		/// <param name="evaluator">Parallel evaluator</param>
		public PNSGAII(Problem problem, ISynchronousParallelRunner evaluator)
			: base(problem)
		{
			parallelEvaluator = evaluator;
		}

		/// <summary>
		/// Runs the NSGA-II algorithm.
		/// </summary>
		/// <returns>a <code>SolutionSet</code> that is a set of non dominated solutions as a result of the algorithm execution</returns>
		public override SolutionSet Execute()
		{
			int populationSize = -1;
			int maxEvaluations = -1;
			int evaluations;

			JMetalCSharp.QualityIndicator.QualityIndicator indicators = null; // QualityIndicator object
			int requiredEvaluations; // Use in the example of use of the
			// indicators object (see below)

			SolutionSet population;
			SolutionSet offspringPopulation;
			SolutionSet union;

			Operator mutationOperator;
			Operator crossoverOperator;
			Operator selectionOperator;

			Distance distance = new Distance();

			//Read the parameters
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetIndicatorsFromParameters(this.InputParameters, "indicators", ref indicators);

			parallelEvaluator.StartParallelRunner(Problem); ;

			//Initialize the variables
			population = new SolutionSet(populationSize);
			evaluations = 0;

			requiredEvaluations = 0;

			//Read the operators
			mutationOperator = Operators["mutation"];
			crossoverOperator = Operators["crossover"];
			selectionOperator = Operators["selection"];

			// Create the initial solutionSet
			Solution newSolution;
			for (int i = 0; i < populationSize; i++)
			{
				newSolution = new Solution(Problem);
				parallelEvaluator.AddTaskForExecution(new object[] { newSolution }); ;
			}

			List<Solution> solutionList = (List<Solution>)parallelEvaluator.ParallelExecution();
			foreach (Solution solution in solutionList)
			{
				population.Add(solution);
				evaluations++;
			}

			// Generations 
			while (evaluations < maxEvaluations)
			{
				// Create the offSpring solutionSet
				offspringPopulation = new SolutionSet(populationSize);
				Solution[] parents = new Solution[2];
				for (int i = 0; i < (populationSize / 2); i++)
				{
					if (evaluations < maxEvaluations)
					{
						//obtain parents
						parents[0] = (Solution)selectionOperator.Execute(population);
						parents[1] = (Solution)selectionOperator.Execute(population);
						Solution[] offSpring = (Solution[])crossoverOperator.Execute(parents);
						mutationOperator.Execute(offSpring[0]);
						mutationOperator.Execute(offSpring[1]);
						parallelEvaluator.AddTaskForExecution(new object[] { offSpring[0] }); ;
						parallelEvaluator.AddTaskForExecution(new object[] { offSpring[1] }); ;
					}
				}

				List<Solution> solutions = (List<Solution>)parallelEvaluator.ParallelExecution();

				foreach (Solution solution in solutions)
				{
					offspringPopulation.Add(solution);
					evaluations++;
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

					remain = 0;
				}

				// This piece of code shows how to use the indicator object into the code
				// of NSGA-II. In particular, it finds the number of evaluations required
				// by the algorithm to obtain a Pareto front with a hypervolume higher
				// than the hypervolume of the true Pareto front.
				if ((indicators != null)
						&& (requiredEvaluations == 0))
				{
					double HV = indicators.GetHypervolume(population);
					if (HV >= (0.98 * indicators.TrueParetoFrontHypervolume))
					{
						requiredEvaluations = evaluations;
					}
				}
			}
			
			// Return as output parameter the required evaluations
			SetOutputParameter("evaluations", requiredEvaluations);

			// Return the first non-dominated front
			Ranking rank = new Ranking(population);

			Result = rank.GetSubfront(0);

			return this.Result;
		}
	}
}

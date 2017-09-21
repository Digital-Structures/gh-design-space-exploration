using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;

namespace JMetalCSharp.Metaheuristics.NSGAII
{
	/// <summary>
	/// This class implements a steady-state version of NSGA-II.
	/// </summary>
	public class SSNSGAII : Algorithm
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public SSNSGAII(Problem problem)
			: base(problem)
		{
		}

		/// <summary>
		/// Runs the SSNSGA-II algorithm.
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
				Problem.Evaluate(newSolution);
				Problem.EvaluateConstraints(newSolution);
				evaluations++;
				population.Add(newSolution);
			}

			// Generations ...
			while (evaluations < maxEvaluations)
			{

				// Create the offSpring solutionSet      
				offspringPopulation = new SolutionSet(populationSize);
				Solution[] parents = new Solution[2];

				//obtain parents
				parents[0] = (Solution)selectionOperator.Execute(population);
				parents[1] = (Solution)selectionOperator.Execute(population);

				// crossover
				Solution[] offSpring = (Solution[])crossoverOperator.Execute(parents);

				// mutation
				mutationOperator.Execute(offSpring[0]);

				// evaluation
				Problem.Evaluate(offSpring[0]);
				Problem.EvaluateConstraints(offSpring[0]);

				// insert child into the offspring population
				offspringPopulation.Add(offSpring[0]);

				evaluations++;

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
			return Result;
		}
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Metaheuristics.GDE3
{
	/// <summary>
	/// This class implements the GDE3 algorithm. 
	/// </summary>
	public class GDE3 : Algorithm
	{
		#region Constructor
		/// <summary>
		///  Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public GDE3(Problem problem)
			: base(problem)
		{

		}

		#endregion

		#region Public Override
		/**   
        * Runs of the GDE3 algorithm.
        * @return a <code>SolutionSet</code> that is a set of non dominated solutions
        * as a result of the algorithm execution  
         * @throws JMException 
        */
		public override SolutionSet Execute()
		{
			int populationSize = -1;
			int maxIterations = -1;
			int evaluations;
			int iterations;

			SolutionSet population;
			SolutionSet offspringPopulation;

			Distance distance;
			IComparer<Solution> dominance;

			Operator selectionOperator;
			Operator crossoverOperator;

			distance = new Distance();
			dominance = new DominanceComparator();

			Solution[] parent;

			//Read the parameters
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxIterations", ref maxIterations);

			selectionOperator = this.Operators["selection"];
			crossoverOperator = this.Operators["crossover"];

			//Initialize the variables
			population = new SolutionSet(populationSize);
			evaluations = 0;
			iterations = 0;

			// Create the initial solutionSet
			Solution newSolution;
			for (int i = 0; i < populationSize; i++)
			{
				newSolution = new Solution(this.Problem);
				this.Problem.Evaluate(newSolution);
				this.Problem.EvaluateConstraints(newSolution);
				evaluations++;
				population.Add(newSolution);
			}

			// Generations ...
			while (iterations < maxIterations)
			{
				// Create the offSpring solutionSet
				offspringPopulation = new SolutionSet(populationSize * 2);

				for (int i = 0; i < populationSize; i++)
				{
					// Obtain parents. Two parameters are required: the population and the 
					//                 index of the current individual
					parent = (Solution[])selectionOperator.Execute(new object[] { population, i });

					Solution child;
					// Crossover. Two parameters are required: the current individual and the 
					//            array of parents
					child = (Solution)crossoverOperator.Execute(new object[] { population.Get(i), parent });

					this.Problem.Evaluate(child);
					this.Problem.EvaluateConstraints(child);
					evaluations++;

					// Dominance test
					int result;
					result = dominance.Compare(population.Get(i), child);
					if (result == -1)
					{ // Solution i dominates child
						offspringPopulation.Add(population.Get(i));
					}
					else if (result == 1)
					{ // child dominates
						offspringPopulation.Add(child);
					}
					else
					{ // the two solutions are non-dominated
						offspringPopulation.Add(child);
						offspringPopulation.Add(population.Get(i));
					}
				}
				// Ranking the offspring population
				Ranking ranking = new Ranking(offspringPopulation);

				int remain = populationSize;
				int index = 0;
				SolutionSet front = null;
				population.Clear();

				// Obtain the next front
				front = ranking.GetSubfront(index);

				while ((remain > 0) && (remain >= front.Size()))
				{
					//Assign crowding distance to individuals
					distance.CrowdingDistanceAssignment(front, this.Problem.NumberOfObjectives);
					//Add the individuals of this front
					for (int k = 0; k < front.Size(); k++)
					{
						population.Add(front.Get(k));
					} // for

					//Decrement remain
					remain = remain - front.Size();

					//Obtain the next front
					index++;
					if (remain > 0)
					{
						front = ranking.GetSubfront(index);
					}
				}

				// remain is less than front(index).size, insert only the best one
				if (remain > 0)
				{  // front contains individuals to insert
					while (front.Size() > remain)
					{
						distance.CrowdingDistanceAssignment(front, this.Problem.NumberOfObjectives);
						front.Remove(front.IndexWorst(new CrowdingComparator()));
					}
					for (int k = 0; k < front.Size(); k++)
					{
						population.Add(front.Get(k));
					}

					remain = 0;
				}
				iterations++;
			}

			// Return the first non-dominated front
			Ranking rnk = new Ranking(population);
			this.Result = rnk.GetSubfront(0);
			return this.Result;
		}
		#endregion
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Operators.LocalSearch;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Archive;
using JMetalCSharp.Utils.Comparators;
using JMetalCSharp.Utils.Wrapper;
using System.Collections.Generic;

namespace JMetalCSharp.Metaheuristics.AbYSS
{
	/// <summary>
	/// This class implements the AbYSS algorithm. This algorithm is an adaptation
	/// of the single-objective scatter search template defined by F. Glover in:
	/// F. Glover. "A template for scatter search and path relinking", Lecture Notes 
	/// in Computer Science, Springer Verlag, 1997. AbYSS is described in: 
	/// A.J. Nebro, F. Luna, E. Alba, B. Dorronsoro, J.J. Durillo, A. Beham 
	/// "AbYSS: Adapting Scatter Search to Multiobjective Optimization." 
	/// IEEE Transactions on Evolutionary Computation. Vol. 12, 
	/// No. 4 (August 2008), pp. 439-457
	/// </summary>
	public class AbYSS : Algorithm
	{
		#region Private Attributes

		/// <summary>
		/// Stores the number of subranges in which each encodings.variable is divided. Used in
		/// the diversification method. By default it takes the value 4 (see the method
		/// <code>initParams</code>).
		/// </summary>
		private int numberOfSubranges;

		/// <summary>
		/// These variables are used in the diversification method.
		/// </summary>
		private int[] sumOfFrequencyValues;
		private int[] sumOfReverseFrequencyValues;
		private int[][] frequency;
		private int[][] reverseFrequency;

		/// <summary>
		/// Stores the initial solution set
		/// </summary>
		private SolutionSet solutionSet;

		/// <summary>
		/// Stores the external solution archive
		/// </summary>
		private CrowdingArchive archive;

		/// <summary>
		/// Stores the reference set one
		/// </summary>
		private SolutionSet refSet1;

		/// <summary>
		/// Stores the reference set two
		/// </summary>
		private SolutionSet refSet2;

		/// <summary>
		/// Stores the solutions provided by the subset generation method of the
		/// scatter search template
		/// </summary>
		private SolutionSet subSet;

		/// <summary>
		/// Maximum number of solution allowed for the initial solution set
		/// </summary>
		private int solutionSetSize;

		/// <summary>
		/// Maximum size of the external archive
		/// </summary>
		private int archiveSize;

		/// <summary>
		/// Maximum size of the reference set one
		/// </summary>
		private int refSet1Size;

		/// <summary>
		/// Maximum size of the reference set two
		/// </summary>
		private int refSet2Size;

		/// <summary>
		/// Maximum number of getEvaluations to carry out
		/// </summary>
		private int maxEvaluations;

		/// <summary>
		/// Stores the current number of performed getEvaluations
		/// </summary>
		private int evaluations;

		/// <summary>
		/// Stores the comparators for dominance and equality, respectively
		/// </summary>
		private IComparer<Solution> dominance;
		private IComparer<Solution> equal;
		private IComparer<Solution> fitness;
		private IComparer<Solution> crowdingDistance;

		/// <summary>
		/// Stores the crossover operator
		/// </summary>
		private Operator crossoverOperator;

		/// <summary>
		/// Stores the improvement operator
		/// </summary>
		private LocalSearch improvementOperator;

		/// <summary>
		/// Stores a <code>Distance</code> object
		/// </summary>
		private Distance distance;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public AbYSS(Problem problem)
			: base(problem)
		{
			//Initialize the fields 

			this.solutionSet = null;
			this.archive = null;
			this.refSet1 = null;
			this.refSet2 = null;
			this.subSet = null;
		}

		#endregion

		/// <summary>
		/// Reads the parameter from the parameter list using the
		/// <code>getInputParameter</code> method.
		/// </summary>
		public void InitParam()
		{
			//Read the parameters
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref solutionSetSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "refSet1Size", ref refSet1Size);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "refSet2Size", ref refSet2Size);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "archiveSize", ref archiveSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);

			//Initialize the variables
			solutionSet = new SolutionSet(solutionSetSize);
			archive = new CrowdingArchive(archiveSize, this.Problem.NumberOfObjectives);
			refSet1 = new SolutionSet(refSet1Size);
			refSet2 = new SolutionSet(refSet2Size);
			subSet = new SolutionSet(solutionSetSize * 1000);
			evaluations = 0;

			numberOfSubranges = 4;

			dominance = new DominanceComparator();
			equal = new EqualSolutions();
			fitness = new FitnessComparator();
			crowdingDistance = new CrowdingDistanceComparator();
			distance = new Distance();
			sumOfFrequencyValues = new int[this.Problem.NumberOfVariables];
			sumOfReverseFrequencyValues = new int[this.Problem.NumberOfVariables];
			frequency = new int[numberOfSubranges][];
			reverseFrequency = new int[numberOfSubranges][];

			for (int i = 0; i < numberOfSubranges; i++)
			{
				frequency[i] = new int[this.Problem.NumberOfVariables];
				reverseFrequency[i] = new int[this.Problem.NumberOfVariables];
			}

			// Read the operators of crossover and improvement
			crossoverOperator = this.Operators["crossover"];
			improvementOperator = (LocalSearch)this.Operators["improvement"];
			improvementOperator.SetParameter("archive", archive);
		}

		/// <summary>
		/// Returns a <code>Solution</code> using the diversification generation method 
		/// described in the scatter search template.
		/// </summary>
		/// <returns></returns>
		public Solution DiversificationGeneration()
		{
			Solution solution;
			solution = new Solution(this.Problem);
			XReal wrapperSolution = new XReal(solution);

			double value;
			int range;

			for (int i = 0; i < this.Problem.NumberOfVariables; i++)
			{
				sumOfReverseFrequencyValues[i] = 0;
				for (int j = 0; j < numberOfSubranges; j++)
				{
					reverseFrequency[j][i] = sumOfFrequencyValues[i] - frequency[j][i];
					sumOfReverseFrequencyValues[i] += reverseFrequency[j][i];
				}

				if (sumOfReverseFrequencyValues[i] == 0)
				{
					range = JMetalRandom.Next(0, numberOfSubranges - 1);
				}
				else
				{
					value = JMetalRandom.Next(0, sumOfReverseFrequencyValues[i] - 1);
					range = 0;
					while (value > reverseFrequency[range][i])
					{
						value -= reverseFrequency[range][i];
						range++;
					}
				}

				frequency[range][i]++;
				sumOfFrequencyValues[i]++;

				double low = this.Problem.LowerLimit[i] + range * (this.Problem.UpperLimit[i] -
							 this.Problem.LowerLimit[i]) / numberOfSubranges;
				double high = low + (this.Problem.UpperLimit[i] -
							 this.Problem.LowerLimit[i]) / numberOfSubranges;
				value = JMetalRandom.NextDouble(low, high);

				wrapperSolution.SetValue(i, value);
			}
			return solution;
		}


		/// <summary>
		/// Implements the referenceSetUpdate method.
		/// </summary>
		/// <param name="build">build if true, indicates that the reference has to be build for the
		/// first time; if false, indicates that the reference set has to be
		/// updated with new solutions</param>
		public void ReferenceSetUpdate(bool build)
		{
			if (build)
			{ // Build a new reference set
				// STEP 1. Select the p best individuals of P, where p is refSet1Size. 
				//         Selection Criterium: Spea2Fitness
				Solution individual;
				(new Spea2Fitness(solutionSet)).FitnessAssign();
				solutionSet.Sort(fitness);

				// STEP 2. Build the RefSet1 with these p individuals            
				for (int i = 0; i < refSet1Size; i++)
				{
					individual = solutionSet.Get(0);
					solutionSet.Remove(0);
					individual.UnMarked();
					refSet1.Add(individual);
				}

				// STEP 3. Compute Euclidean distances in SolutionSet to obtain q 
				//         individuals, where q is refSet2Size_
				for (int i = 0; i < solutionSet.Size(); i++)
				{
					individual = solutionSet.Get(i);
					individual.DistanceToSolutionSet = distance.DistanceToSolutionSetInSolutionSpace(individual, refSet1);
				}

				int size = refSet2Size;
				if (solutionSet.Size() < refSet2Size)
				{
					size = solutionSet.Size();
				}

				// STEP 4. Build the RefSet2 with these q individuals
				for (int i = 0; i < size; i++)
				{
					// Find the maximumMinimunDistanceToPopulation
					double maxMinimum = 0.0;
					int index = 0;
					for (int j = 0; j < solutionSet.Size(); j++)
					{
						if (solutionSet.Get(j).DistanceToSolutionSet > maxMinimum)
						{
							maxMinimum = solutionSet.Get(j).DistanceToSolutionSet;
							index = j;
						}
					}
					individual = solutionSet.Get(index);
					solutionSet.Remove(index);

					// Update distances to REFSET in population
					for (int j = 0; j < solutionSet.Size(); j++)
					{
						double aux = distance.DistanceBetweenSolutions(solutionSet.Get(j), individual);
						if (aux < individual.DistanceToSolutionSet)
						{
							solutionSet.Get(j).DistanceToSolutionSet = aux;
						}
					}

					// Insert the individual into REFSET2
					refSet2.Add(individual);

					// Update distances in REFSET2
					for (int j = 0; j < refSet2.Size(); j++)
					{
						for (int k = 0; k < refSet2.Size(); k++)
						{
							if (i != j)
							{
								double aux = distance.DistanceBetweenSolutions(refSet2.Get(j), refSet2.Get(k));
								if (aux < refSet2.Get(j).DistanceToSolutionSet)
								{
									refSet2.Get(j).DistanceToSolutionSet = aux;
								}
							}
						}
					}
				}

			}
			else
			{ // Update the reference set from the subset generation result
				Solution individual;
				for (int i = 0; i < subSet.Size(); i++)
				{
					individual = (Solution)improvementOperator.Execute(subSet.Get(i));
					evaluations += improvementOperator.GetEvaluations();

					if (RefSet1Test(individual))
					{ //Update distance of RefSet2
						for (int indSet2 = 0; indSet2 < refSet2.Size(); indSet2++)
						{
							double aux = distance.DistanceBetweenSolutions(individual, refSet2.Get(indSet2));
							if (aux < refSet2.Get(indSet2).DistanceToSolutionSet)
							{
								refSet2.Get(indSet2).DistanceToSolutionSet = aux;
							}
						}
					}
					else
					{
						RefSet2Test(individual);
					}
				}
				subSet.Clear();
			}
		}

		/// <summary>
		/// Tries to update the reference set 2 with a <code>Solution</code>
		/// </summary>
		/// <param name="solution">The <code>Solution</code></param>
		/// <returns>true if the <code>Solution</code> has been inserted, false 
		/// otherwise.</returns>
		public bool RefSet2Test(Solution solution)
		{
			double aux;
			if (refSet2.Size() < refSet2Size)
			{
				solution.DistanceToSolutionSet = distance.DistanceToSolutionSetInSolutionSpace(solution, refSet1);
				aux = distance.DistanceToSolutionSetInSolutionSpace(solution, refSet2);
				if (aux < solution.DistanceToSolutionSet)
				{
					solution.DistanceToSolutionSet = aux;
				}
				refSet2.Add(solution);
				return true;
			}

			solution.DistanceToSolutionSet = distance.DistanceToSolutionSetInSolutionSpace(solution, refSet1);
			aux = distance.DistanceToSolutionSetInSolutionSpace(solution, refSet2);
			if (aux < solution.DistanceToSolutionSet)
			{
				solution.DistanceToSolutionSet = aux;
			}

			double peor = 0.0;
			int index = 0;
			for (int i = 0; i < refSet2.Size(); i++)
			{
				aux = refSet2.Get(i).DistanceToSolutionSet;
				if (aux > peor)
				{
					peor = aux;
					index = i;
				}
			}

			if (solution.DistanceToSolutionSet < peor)
			{
				refSet2.Remove(index);
				//Update distances in REFSET2
				for (int j = 0; j < refSet2.Size(); j++)
				{
					aux = distance.DistanceBetweenSolutions(refSet2.Get(j), solution);
					if (aux < refSet2.Get(j).DistanceToSolutionSet)
					{
						refSet2.Get(j).DistanceToSolutionSet = aux;
					}
				}
				solution.UnMarked();
				refSet2.Add(solution);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Tries to update the reference set one with a <code>Solution</code>.
		/// </summary>
		/// <param name="solution">The <code>Solution</code></param>
		/// <returns>true if the <code>Solution</code> has been inserted, false
		/// otherwise.</returns>
		public bool RefSet1Test(Solution solution)
		{
			bool dominated = false;
			int flag;
			int i = 0;
			while (i < refSet1.Size())
			{
				flag = dominance.Compare(solution, refSet1.Get(i));
				if (flag == -1)
				{ //This is: solution dominates 
					refSet1.Remove(i);
				}
				else if (flag == 1)
				{
					dominated = true;
					i++;
				}
				else
				{
					flag = equal.Compare(solution, refSet1.Get(i));
					if (flag == 0)
					{
						return true;
					}
					i++;
				}
			}

			if (!dominated)
			{
				solution.UnMarked();
				if (refSet1.Size() < refSet1Size)
				{ //refSet1 isn't full
					refSet1.Add(solution);
				}
				else
				{
					archive.Add(solution);
				}
			}
			else
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Implements the subset generation method described in the scatter search
		/// template
		/// </summary>
		/// <returns>Number of solutions created by the method</returns>
		public int SubSetGeneration()
		{
			Solution[] parents = new Solution[2];
			Solution[] offSpring;

			subSet.Clear();

			//All pairs from refSet1
			for (int i = 0; i < refSet1.Size(); i++)
			{
				parents[0] = refSet1.Get(i);
				for (int j = i + 1; j < refSet1.Size(); j++)
				{
					parents[1] = refSet1.Get(j);
					if (!parents[0].IsMarked() || !parents[1].IsMarked())
					{
						offSpring = (Solution[])crossoverOperator.Execute(parents);
						this.Problem.Evaluate(offSpring[0]);
						this.Problem.Evaluate(offSpring[1]);
						this.Problem.EvaluateConstraints(offSpring[0]);
						this.Problem.EvaluateConstraints(offSpring[1]);
						evaluations += 2;
						if (evaluations < maxEvaluations)
						{
							subSet.Add(offSpring[0]);
							subSet.Add(offSpring[1]);
						}
						parents[0].Marked();
						parents[1].Marked();
					}
				}
			}

			// All pairs from refSet2
			for (int i = 0; i < refSet2.Size(); i++)
			{
				parents[0] = refSet2.Get(i);
				for (int j = i + 1; j < refSet2.Size(); j++)
				{
					parents[1] = refSet2.Get(j);
					if (!parents[0].IsMarked() || !parents[1].IsMarked())
					{
						offSpring = (Solution[])crossoverOperator.Execute(parents);
						this.Problem.EvaluateConstraints(offSpring[0]);
						this.Problem.EvaluateConstraints(offSpring[1]);
						this.Problem.Evaluate(offSpring[0]);
						this.Problem.Evaluate(offSpring[1]);
						evaluations += 2;
						if (evaluations < maxEvaluations)
						{
							subSet.Add(offSpring[0]);
							subSet.Add(offSpring[1]);
						}
						parents[0].Marked();
						parents[1].Marked();
					}
				}
			}

			return subSet.Size();
		}

		/// <summary>
		/// Runs of the AbYSS algorithm.
		/// </summary>
		/// <returns>a <code>SolutionSet</code> that is a set of non dominated solutions
		/// as a result of the algorithm execution  </returns>
		public override SolutionSet Execute()
		{
			// STEP 1. Initialize parameters
			InitParam();

			// STEP 2. Build the initial solutionSet
			Solution solution;
			for (int i = 0; i < solutionSetSize; i++)
			{
				solution = DiversificationGeneration();
				this.Problem.Evaluate(solution);
				this.Problem.EvaluateConstraints(solution);
				evaluations++;
				solution = (Solution)improvementOperator.Execute(solution);
				evaluations += improvementOperator.GetEvaluations();
				solutionSet.Add(solution);
			}

			// STEP 3. Main loop
			int newSolutions = 0;
			while (evaluations < maxEvaluations)
			{
				ReferenceSetUpdate(true);
				newSolutions = SubSetGeneration();
				while (newSolutions > 0)
				{ // New solutions are created
					ReferenceSetUpdate(false);
					if (evaluations >= maxEvaluations)
					{
						Result = archive;
						return archive;
					}
					newSolutions = SubSetGeneration();
				} // while

				// RE-START
				if (evaluations < maxEvaluations)
				{
					solutionSet.Clear();
					// Add refSet1 to SolutionSet
					for (int i = 0; i < refSet1.Size(); i++)
					{
						solution = refSet1.Get(i);
						solution.UnMarked();
						solution = (Solution)improvementOperator.Execute(solution);
						evaluations += improvementOperator.GetEvaluations();
						solutionSet.Add(solution);
					}
					// Remove refSet1 and refSet2
					refSet1.Clear();
					refSet2.Clear();

					// Sort the archive and insert the best solutions
					distance.CrowdingDistanceAssignment(archive, this.Problem.NumberOfObjectives);
					archive.Sort(crowdingDistance);

					int insert = solutionSetSize / 2;
					if (insert > archive.Size())
						insert = archive.Size();

					if (insert > (solutionSetSize - solutionSet.Size()))
						insert = solutionSetSize - solutionSet.Size();

					// Insert solutions 
					for (int i = 0; i < insert; i++)
					{
						solution = new Solution(archive.Get(i));
						solution.UnMarked();
						solutionSet.Add(solution);
					}

					// Create the rest of solutions randomly
					while (solutionSet.Size() < solutionSetSize)
					{
						solution = DiversificationGeneration();
						this.Problem.EvaluateConstraints(solution);
						this.Problem.Evaluate(solution);
						evaluations++;
						solution = (Solution)improvementOperator.Execute(solution);
						evaluations += improvementOperator.GetEvaluations();
						solution.UnMarked();
						solutionSet.Add(solution);
					}
				}
			}

			// STEP 4. Return the archive
			Result = archive;
			return archive;
		}
	}
}

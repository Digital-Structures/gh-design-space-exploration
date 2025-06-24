using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.QualityIndicator.Utils;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JMetalCSharp.Metaheuristics.SMSEMOA
{
	/// <summary>
	/// This class implements the SMS-EMOA algorithm, as described in
	///
	/// Michael Emmerich, Nicola Beume, and Boris Naujoks.
	/// An EMO algorithm using the hypervolume measure as selection criterion.
	/// In C. A. Coello Coello et al., Eds., Proc. Evolutionary Multi-Criterion Optimization,
	/// 3rd Int'l Conf. (EMO 2005), LNCS 3410, pp. 62-76. Springer, Berlin, 2005.
	///
	/// and
	/// 
	/// Boris Naujoks, Nicola Beume, and Michael Emmerich.
	/// Multi-objective optimisation using S-metric selection: Application to
	/// three-dimensional solution spaces. In B. McKay et al., Eds., Proc. of the 2005
	/// Congress on Evolutionary Computation (CEC 2005), Edinburgh, Band 2, pp. 1282-1289.
	/// IEEE Press, Piscataway NJ, 2005.
	/// </summary>
	public class SMSEMOA : Algorithm
	{
		#region Private Attributes
		private MetricsUtil utils;
		private HyperVolume hv;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public SMSEMOA(Problem problem)
			: base(problem)
		{
			utils = new MetricsUtil();
			hv = new HyperVolume();
		}

		#endregion

		#region Public Overrides
		/// <summary>
		/// Runs the SMS-EMOA algorithm.
		/// </summary>
		/// <returns>A <code>SolutionSet</code> that is a set of non dominated solutions as a result of the algorithm execution</returns>
		public override SolutionSet Execute()
		{
			int populationSize = -1;
			int maxEvaluations = -1;
			int evaluations;
			double offset = 100.0;

			QualityIndicator.QualityIndicator indicators = null; // QualityIndicator object
			int requiredEvaluations; // Use in the example of use of the indicators object (see below)

			SolutionSet population;
			SolutionSet offspringPopulation;
			SolutionSet union;

			Operator mutationOperator;
			Operator crossoverOperator;
			Operator selectionOperator;

			//Read the parameters
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
			JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);
			JMetalCSharp.Utils.Utils.GetIndicatorsFromParameters(this.InputParameters, "indicators", ref indicators);
			JMetalCSharp.Utils.Utils.GetDoubleValueFromParameter(this.InputParameters, "offset", ref offset);

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

				// select parents
				offspringPopulation = new SolutionSet(populationSize);
				List<Solution> selectedParents = new List<Solution>();
				Solution[] parents = new Solution[0];
				while (selectedParents.Count < 2)
				{
					object selected = selectionOperator.Execute(population);
					try
					{
						Solution parent = (Solution)selected;
						selectedParents.Add(parent);
					}
					catch (InvalidCastException e)
					{
						parents = (Solution[])selected;
						selectedParents.AddRange(parents);
					}
				}
				parents = selectedParents.ToArray<Solution>();

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

				// Ranking the union (non-dominated sorting)
				Ranking ranking = new Ranking(union);

				// ensure crowding distance values are up to date
				// (may be important for parent selection)
				for (int j = 0; j < population.Size(); j++)
				{
					population.Get(j).CrowdingDistance = 0.0;
				}

				SolutionSet lastFront = ranking.GetSubfront(ranking.GetNumberOfSubfronts() - 1);
				if (lastFront.Size() > 1)
				{
					double[][] frontValues = lastFront.WriteObjectivesToMatrix();
					int numberOfObjectives = Problem.NumberOfObjectives;
					// STEP 1. Obtain the maximum and minimum values of the Pareto front
					double[] maximumValues = utils.GetMaximumValues(union.WriteObjectivesToMatrix(), numberOfObjectives);
					double[] minimumValues = utils.GetMinimumValues(union.WriteObjectivesToMatrix(), numberOfObjectives);
					// STEP 2. Get the normalized front
					double[][] normalizedFront = utils.GetNormalizedFront(frontValues, maximumValues, minimumValues);
					// compute offsets for reference point in normalized space
					double[] offsets = new double[maximumValues.Length];
					for (int i = 0; i < maximumValues.Length; i++)
					{
						offsets[i] = offset / (maximumValues[i] - minimumValues[i]);
					}
					// STEP 3. Inverse the pareto front. This is needed because the original
					//metric by Zitzler is for maximization problems
					double[][] invertedFront = utils.InvertedFront(normalizedFront);
					// shift away from origin, so that boundary points also get a contribution > 0
					for (int j = 0, lj = invertedFront.Length; j < lj; j++)
					{
						var point = invertedFront[j];
						for (int i = 0; i < point.Length; i++)
						{
							point[i] += offsets[i];
						}
					}

					// calculate contributions and sort
					double[] contributions = HvContributions(invertedFront);
					for (int i = 0; i < contributions.Length; i++)
					{
						// contribution values are used analogously to crowding distance
						lastFront.Get(i).CrowdingDistance = contributions[i];
					}

					lastFront.Sort(new CrowdingDistanceComparator());
				}

				// all but the worst are carried over to the survivor population
				SolutionSet front = null;
				population.Clear();
				for (int i = 0; i < ranking.GetNumberOfSubfronts() - 1; i++)
				{
					front = ranking.GetSubfront(i);
					for (int j = 0; j < front.Size(); j++)
					{
						population.Add(front.Get(j));
					}
				}
				for (int i = 0; i < lastFront.Size() - 1; i++)
				{
					population.Add(lastFront.Get(i));
				}

				// This piece of code shows how to use the indicator object into the code
				// of SMS-EMOA. In particular, it finds the number of evaluations required
				// by the algorithm to obtain a Pareto front with a hypervolume higher
				// than the hypervolume of the true Pareto front.
				if (indicators != null && requiredEvaluations == 0)
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
			Ranking rnk = new Ranking(population);

			Result = rnk.GetSubfront(0);
			return Result;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Calculates how much hypervolume each point dominates exclusively. The points
		/// have to be transformed beforehand, to accommodate the assumptions of Zitzler's
		/// hypervolume code.
		/// </summary>
		/// <param name="front">transformed objective values</param>
		/// <returns>HV contributions</returns>
		private double[] HvContributions(double[][] front)
		{
			int numberOfObjectives = Problem.NumberOfObjectives;
			double[] contributions = new double[front.Length];
			double[][] frontSubset = new double[front.Length - 1][];
			for (int i = 0; i < front.Length - 1; i++)
			{
				frontSubset[i] = new double[front[0].Length];
			}

			List<double[]> frontCopy = new List<double[]>();
			frontCopy.AddRange(front);
			double[][] totalFront = frontCopy.ToArray<double[]>();
			double totalVolume = hv.CalculateHypervolume(totalFront, totalFront.Length, numberOfObjectives);
			for (int i = 0; i < front.Length; i++)
			{
				double[] evaluatedPoint = frontCopy[i];
				frontCopy.RemoveAt(i);

				frontSubset = frontCopy.ToArray<double[]>();
				// STEP4. The hypervolume (control is passed to java version of Zitzler code)
				double chv = hv.CalculateHypervolume(frontSubset, frontSubset.Length, numberOfObjectives);
				double contribution = totalVolume - chv;
				contributions[i] = contribution;
				// put point back
				frontCopy.Insert(i, evaluatedPoint);
			}
			return contributions;
		}

		#endregion
	}
}

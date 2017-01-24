using JMetalCSharp.Core;
using System;
using System.Text;

namespace JMetalCSharp.Utils
{
	/// <summary>
	/// This class defines an adaptive grid over a SolutionSet as the one used the algorithm PAES.
	/// </summary>
	public class AdaptiveGrid
	{
		/// <summary>
		/// Number of bi-divisions of the objective space
		/// </summary>
		public int Bisections { get; private set; }

		/// <summary>
		/// Objectives of the problem
		/// </summary>
		private int objectives;

		/// <summary>
		/// Number of solutions into a specific hypercube in the adaptative grid
		/// </summary>
		private int[] hypercubes;

		/// <summary>
		/// Grid lower bounds
		/// </summary>
		private double[] lowerLimits;

		/// <summary>
		/// Grid upper bounds
		/// </summary>
		private double[] upperLimits;

		/// <summary>
		/// Size of hypercube for each dimension
		/// </summary>
		private double[] divisionSize;

		/// <summary>
		/// Hypercube with maximum number of solutions
		/// </summary>
		public int MostPopulated { get; private set; }

		/// <summary>
		/// Indicates when an hypercube has solutions
		/// </summary>
		private int[] occupied;

		/// <summary>
		/// Constructor. 
		/// Creates an instance of AdaptativeGrid.
		/// </summary>
		/// <param name="bisections">Number of bi-divisions of the objective space.</param>
		/// <param name="objetives">Number of objectives of the problem.</param>
		public AdaptiveGrid(int bisections, int objetives)
		{
			this.Bisections = bisections;
			this.objectives = objetives;
			this.lowerLimits = new double[objectives];
			this.upperLimits = new double[objectives];
			this.divisionSize = new double[objectives];
			this.hypercubes = new int[(int)Math.Pow(2.0, bisections * objectives)];

			for (int i = 0; i < this.hypercubes.Length; i++)
			{
				this.hypercubes[i] = 0;
			}
		}

		/// <summary>
		/// Updates the grid limits considering the solutions contained in a <code>SolutionSet</code>.
		/// </summary>
		/// <param name="solutionSet">The <code>SolutionSet</code> considered.</param>
		private void UpdateLimits(SolutionSet solutionSet)
		{
			//Init the lower and upper limits 
			for (int obj = 0; obj < objectives; obj++)
			{
				//Set the lower limits to the max real
				lowerLimits[obj] = double.MaxValue;
				//Set the upper limits to the min real
				upperLimits[obj] = double.MinValue;
			}

			//Find the max and min limits of objetives into the population
			for (int ind = 0; ind < solutionSet.Size(); ind++)
			{
				Solution tmpIndividual = solutionSet.Get(ind);
				for (int obj = 0; obj < objectives; obj++)
				{
					if (tmpIndividual.Objective[obj] < lowerLimits[obj])
					{
						lowerLimits[obj] = tmpIndividual.Objective[obj];
					}
					if (tmpIndividual.Objective[obj] > upperLimits[obj])
					{
						upperLimits[obj] = tmpIndividual.Objective[obj];
					}
				}
			}
		}

		/// <summary>
		/// Updates the grid adding solutions contained in a specific
		/// <code>SolutionSet</code>.
		/// <b>REQUIRE</b> The grid limits must have been previously calculated.
		/// </summary>
		/// <param name="solutionSet">The <code>SolutionSet</code> considered.</param>
		private void AddSolutionSet(SolutionSet solutionSet)
		{
			//Calculate the location of all individuals and update the grid
			MostPopulated = 0;
			int location;

			for (int ind = 0; ind < solutionSet.Size(); ind++)
			{
				location = Location(solutionSet.Get(ind));
				hypercubes[location]++;
				if (hypercubes[location] > hypercubes[MostPopulated])
				{
					MostPopulated = location;
				}
			}

			//The grid has been updated, so also update ocuppied's hypercubes
			CalculateOccupied();
		}

		/// <summary>
		/// Updates the grid limits and the grid content adding the solutions
		/// contained in a specific <code>SolutionSet</code>.
		/// </summary>
		/// <param name="solutionSet">The <code>SolutionSet</code>.</param>
		public void UpdateGrid(SolutionSet solutionSet)
		{
			//Update lower and upper limits
			UpdateLimits(solutionSet);

			//Calculate the division size
			for (int obj = 0; obj < objectives; obj++)
			{
				divisionSize[obj] = upperLimits[obj] - lowerLimits[obj];
			}

			//Clean the hypercubes
			for (int i = 0; i < hypercubes.Length; i++)
			{
				hypercubes[i] = 0;
			}

			//Add the population
			AddSolutionSet(solutionSet);
		}

		/// <summary>
		/// Updates the grid limits and the grid content adding a new
		/// <code>Solution</code>. If the solution falls out of the grid bounds, the
		/// limits and content of the grid must be re-calculated.
		/// </summary>
		/// <param name="solution"><code>Solution</code> considered to update the grid.</param>
		/// <param name="solutionSet"><code>SolutionSet</code> used to update the grid.</param>
		public void UpdateGrid(Solution solution, SolutionSet solutionSet)
		{

			int location = Location(solution);
			if (location == -1)
			{//Re-build the Adaptative-Grid
				//Update lower and upper limits
				UpdateLimits(solutionSet);

				//Actualize the lower and upper limits whit the individual      
				for (int obj = 0; obj < objectives; obj++)
				{
					if (solution.Objective[obj] < lowerLimits[obj])
					{
						lowerLimits[obj] = solution.Objective[obj];
					}
					if (solution.Objective[obj] > upperLimits[obj])
					{
						upperLimits[obj] = solution.Objective[obj];
					}
				}

				//Calculate the division size
				for (int obj = 0; obj < objectives; obj++)
				{
					divisionSize[obj] = upperLimits[obj] - lowerLimits[obj];
				}

				//Clean the hypercube
				for (int i = 0; i < hypercubes.Length; i++)
				{
					hypercubes[i] = 0;
				}

				//add the population
				AddSolutionSet(solutionSet);
			}
		}

		/// <summary>
		/// Calculates the hypercube of a solution.
		/// </summary>
		/// <param name="solution">The <code>Solution</code>.</param>
		/// <returns></returns>
		public int Location(Solution solution)
		{
			//Create a int [] to store the range of each objetive
			int[] position = new int[objectives];

			//Calculate the position for each objetive
			for (int obj = 0; obj < objectives; obj++)
			{

				if ((solution.Objective[obj] > upperLimits[obj])
						|| (solution.Objective[obj] < lowerLimits[obj]))
				{
					return -1;
				}
				else if (solution.Objective[obj] == lowerLimits[obj])
				{
					position[obj] = 0;
				}
				else if (solution.Objective[obj] == upperLimits[obj])
				{
					position[obj] = ((int)Math.Pow(2.0, Bisections)) - 1;
				}
				else
				{
					double tmpSize = divisionSize[obj];
					double value = solution.Objective[obj];
					double account = lowerLimits[obj];
					int ranges = (int)Math.Pow(2.0, Bisections);
					for (int b = 0; b < Bisections; b++)
					{
						tmpSize /= 2.0;
						ranges /= 2;
						if (value > (account + tmpSize))
						{
							position[obj] += ranges;
							account += tmpSize;
						}
					}
				}
			}

			//Calcualate the location into the hypercubes
			int location = 0;
			for (int obj = 0; obj < objectives; obj++)
			{
				location += position[obj] * (int)Math.Pow(2.0, obj * Bisections);
			}
			return location;
		}

		/// <summary>
		/// Returns the number of solutions into a specific hypercube.
		/// </summary>
		/// <param name="location">Number of the hypercube.</param>
		/// <returns>The number of solutions into a specific hypercube.</returns>
		public int GetLocationDensity(int location)
		{
			return hypercubes[location];
		}

		/// <summary>
		/// /Decreases the number of solutions into a specific hypercube.
		/// </summary>
		/// <param name="location">Number of hypercube.</param>
		public void RemoveSolution(int location)
		{
			//Decrease the solutions in the location specified.
			hypercubes[location]--;

			//Update the most poblated hypercube
			if (location == MostPopulated)
			{
				for (int i = 0; i < hypercubes.Length; i++)
				{
					if (hypercubes[i] > hypercubes[MostPopulated])
					{
						MostPopulated = i;
					}
				}
			}

			//If hypercubes[location] now becomes to zero, then update ocupped hypercubes
			if (hypercubes[location] == 0)
			{
				this.CalculateOccupied();
			}
		}

		/// <summary>
		/// Increases the number of solutions into a specific hypercube.
		/// </summary>
		/// <param name="location">Number of hypercube.</param>
		public void AddSolution(int location)
		{
			//Increase the solutions in the location specified.
			hypercubes[location]++;

			//Update the most poblated hypercube
			if (hypercubes[location] > hypercubes[MostPopulated])
			{
				MostPopulated = location;
			}

			//if hypercubes[location] becomes to one, then recalculate 
			//the occupied hypercubes
			if (hypercubes[location] == 1)
			{
				this.CalculateOccupied();
			}
		}

		/// <summary>
		/// Retunrns a string representing the grid.
		/// </summary>
		/// <returns>The String.</returns>
		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			result.Append("Grid\n");
			for (int obj = 0; obj < objectives; obj++)
			{
				result.Append("Objective " + obj + " " + lowerLimits[obj] + " " + upperLimits[obj] + "\n");
			}
			return result.ToString();
		}

		/// <summary>
		/// Returns a random hypercube using a rouleteWheel method.
		/// </summary>
		/// <returns>The number of the selected hypercube.</returns>
		public int RouletteWheel()
		{
			//Calculate the inverse sum
			double inverseSum = 0.0;
			foreach (int aHypercubes in hypercubes)
			{
				if (aHypercubes > 0)
				{
					inverseSum += 1.0 / (double)aHypercubes;
				}
			}

			//Calculate a random value between 0 and sumaInversa
			double random = JMetalRandom.NextDouble(0.0, inverseSum);
			int hypercube = 0;
			double accumulatedSum = 0.0;
			while (hypercube < hypercubes.Length)
			{
				if (hypercubes[hypercube] > 0)
				{
					accumulatedSum += 1.0 / (double)hypercubes[hypercube];
				}

				if (accumulatedSum > random)
				{
					return hypercube;
				}

				hypercube++;
			}

			return hypercube;
		}

		/// <summary>
		/// Calculates the number of hypercubes having one or more solutions. return
		/// the number of hypercubes with more than zero solutions.
		/// </summary>
		/// <returns></returns>
		public int CalculateOccupied()
		{
			int total = 0;
			foreach (int aHypercubes in hypercubes)
			{
				if (aHypercubes > 0)
				{
					total++;
				}
			}

			occupied = new int[total];
			int bas = 0;
			for (int i = 0; i < hypercubes.Length; i++)
			{
				if (hypercubes[i] > 0)
				{
					occupied[bas] = i;
					bas++;
				}
			}

			return total;
		}

		/// <summary>
		/// Returns the number of hypercubes with more than zero solutions.
		/// </summary>
		/// <returns>The number of hypercubes with more than zero solutions.</returns>
		public int OccupiedHypercubes()
		{
			return occupied.Length;
		}

		/// <summary>
		/// Returns a random hypercube that has more than zero solutions.
		/// </summary>
		/// <returns>The hypercube.</returns>
		public int RandomOccupiedHypercube()
		{
			int rand = JMetalRandom.Next(0, occupied.Length - 1);
			return occupied[rand];
		}
	}
}

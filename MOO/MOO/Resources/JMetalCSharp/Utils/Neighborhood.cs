using JMetalCSharp.Core;
using System;

namespace JMetalCSharp.Utils
{
	/// <summary>
	/// Class representing neighborhoods for a <code>Solution</code> into a
	/// <code>SolutionSet</code>.
	/// </summary>
	public class Neighborhood
	{

		#region Private Attributes
		/// <summary>
		/// Maximum rate considered
		/// </summary>
		private static int MAXRADIO = 2;

		/// <summary>
		/// Stores the neighborhood.
		///   structure [i] represents a neighborhood for a solution.
		///   structure [i][j] represents a neighborhood with a ratio.
		///   structure [i][j][k] represents a neighbor of a solution.
		/// </summary>
		private int[][][] structure;

		/// <summary>
		/// Stores the size of the solutionSet.
		/// </summary>
		private int solutionSetSize;

		/// <summary>
		/// Stores the size for each row
		/// </summary>
		private int rowSize;

		/// <summary>
		/// Enum type for defining the North, South, East, West, North-West, South-West,
		/// North-East, South-East neighbor.
		/// </summary>
		private enum Row { N, S, E, W, NW, SW, NE, SE }

		#endregion

		/// <summary>
		/// Constructor.
		/// Defines a neighborhood of a given size.
		/// </summary>
		/// <param name="solutionSetSize">The size.</param>
		public Neighborhood(int solutionSetSize)
		{
			this.solutionSetSize = solutionSetSize;
			//Create the structure_ for store the neighborhood
			this.structure = new int[solutionSetSize][][];

			for (int i = 0; i < solutionSetSize; i++)
			{
				this.structure[i] = new int[MAXRADIO][];
			}

			//For each individual, and different rates the individual has a different 
			//number of neighborhoods
			for (int ind = 0; ind < solutionSetSize; ind++)
			{
				for (int radio = 0; radio < MAXRADIO; radio++)
				{
					if (radio == 0)
					{//neighboors whit rate 1
						structure[ind][radio] = new int[8];
					}
					else if (radio == 1)
					{ //neighboors whit rate 2
						structure[ind][radio] = new int[24];
					}
				}
			}

			//Calculate the size of a row
			rowSize = (int)Math.Sqrt((double)solutionSetSize);


			//Calculates the neighbors of a individual 
			for (int ind = 0; ind < solutionSetSize; ind++)
			{
				//rate 1
				//North neighbors
				if (ind > rowSize - 1)
				{
					structure[ind][0][(int)Row.N] = ind - rowSize;
				}
				else
				{
					structure[ind][0][(int)Row.N] = (ind - rowSize + solutionSetSize) % solutionSetSize;
				}

				//East neighbors
				if ((ind + 1) % rowSize == 0)
					structure[ind][0][(int)Row.E] = (ind - (rowSize - 1));
				else
					structure[ind][0][(int)Row.E] = (ind + 1);

				//Western neigbors
				if (ind % rowSize == 0)
				{
					structure[ind][0][(int)Row.W] = (ind + (rowSize - 1));
				}
				else
				{
					structure[ind][0][(int)Row.W] = (ind - 1);
				}

				//South neigbors
				structure[ind][0][(int)Row.S] = (ind + rowSize) % solutionSetSize;
			}

			for (int ind = 0; ind < solutionSetSize; ind++)
			{
				structure[ind][0][(int)Row.NE] = structure[structure[ind][0][(int)Row.N]][0][(int)Row.E];
				structure[ind][0][(int)Row.NW] = structure[structure[ind][0][(int)Row.N]][0][(int)Row.W];
				structure[ind][0][(int)Row.SE] = structure[structure[ind][0][(int)Row.S]][0][(int)Row.E];
				structure[ind][0][(int)Row.SW] = structure[structure[ind][0][(int)Row.S]][0][(int)Row.W];
			}
		}

		/// <summary>
		/// Returns a <code>SolutionSet</code> with the North, Sout, East and West
		/// neighbors solutions of ratio 0 of a given location into a given
		/// <code>SolutionSet</code>.
		/// </summary>
		/// <param name="solutionSet">The <code>SolutionSet</code>.</param>
		/// <param name="location">The location.</param>
		/// <returns>A <code>SolutionSet</code> with the neighbors.</returns>
		public SolutionSet GetFourNeighbors(SolutionSet solutionSet, int location)
		{
			//SolutionSet that contains the neighbors (to return)
			SolutionSet neighbors;

			//instance the solutionSet to a non dominated li of individuals
			neighbors = new SolutionSet(24);

			//Gets the neighboords N, S, E, W
			int index;

			//North
			index = structure[location][0][(int)Row.N];
			neighbors.Add(solutionSet.Get(index));

			//South
			index = structure[location][0][(int)Row.S];
			neighbors.Add(solutionSet.Get(index));

			//East
			index = structure[location][0][(int)Row.E];
			neighbors.Add(solutionSet.Get(index));

			//West
			index = structure[location][0][(int)Row.W];
			neighbors.Add(solutionSet.Get(index));

			//Return the list of non-dominated individuals
			return neighbors;
		}

		/// <summary>
		/// Returns a <code>SolutionSet</code> with the North, Sout, East, West,
		/// North-West, South-West, North-East and South-East neighbors solutions of
		/// ratio 0 of a given location into a given <code>SolutionSet</code>.
		/// solutions of a given location into a given <code>SolutionSet</code>.
		/// </summary>
		/// <param name="population">The <code>SolutionSet</code>.</param>
		/// <param name="individual">The individual.</param>
		/// <returns>A <code>SolutionSet</code> with the neighbors.</returns>
		public SolutionSet GetEightNeighbors(SolutionSet population, int individual)
		{
			//SolutionSet that contains the neighbors (to return)
			SolutionSet neighbors;

			//instance the population to a non dominated li of individuals
			neighbors = new SolutionSet(24);

			//Gets the neighboords N, S, E, W
			int index;

			//N
			index = this.structure[individual][0][(int)Row.N];
			neighbors.Add(population.Get(index));

			//S
			index = this.structure[individual][0][(int)Row.S];
			neighbors.Add(population.Get(index));

			//E
			index = this.structure[individual][0][(int)Row.E];
			neighbors.Add(population.Get(index));

			//W
			index = this.structure[individual][0][(int)Row.W];
			neighbors.Add(population.Get(index));

			//NE
			index = this.structure[individual][0][(int)Row.NE];
			neighbors.Add(population.Get(index));

			//NW
			index = this.structure[individual][0][(int)Row.NW];
			neighbors.Add(population.Get(index));

			//SE
			index = this.structure[individual][0][(int)Row.SE];
			neighbors.Add(population.Get(index));

			//SW
			index = this.structure[individual][0][(int)Row.SW];
			neighbors.Add(population.Get(index));

			//Return the list of non-dominated individuals
			return neighbors;
		}
	}
}

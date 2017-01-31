using JMetalCSharp.Core;
using JMetalCSharp.Utils.Comparators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JMetalCSharp.Utils
{
	/// <summary>
	/// This class implements some facilities for calculating the Spea2 fitness
	/// </summary>
	public class Spea2Fitness
	{
		#region Private Attribute
		/// <summary>
		/// Stores the distance between solutions
		/// </summary>
		private double[][] distanceMatrix = null;

		/// <summary>
		/// Stores the solutionSet to assign the fitness
		/// </summary>
		private SolutionSet solutionSet = null;

		/// <summary>
		/// Stores a <code>Distance</code> object
		/// </summary>
		private readonly Distance distance = new Distance();

		/// <summary>
		/// Stores a <code>Comparator</code> for distance between nodes checking
		/// </summary>
		private readonly IComparer<DistanceNode> distanceNodeComparator = new DistanceNodeComparator();

		/// <summary>
		/// stores a <code>Comparator</code> for dominance checking
		/// </summary>
		private readonly IComparer<Solution> dominance = new DominanceComparator();

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// Creates a new instance of Spea2Fitness for a given <code>SolutionSet</code>.
		/// </summary>
		/// <param name="solutionSet">The <code>SolutionSet</code></param>
		public Spea2Fitness(SolutionSet solutionSet)
		{
			this.distanceMatrix = distance.DistanceMatrix(solutionSet);
			this.solutionSet = solutionSet;
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				solutionSet.Get(i).Location = i;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Assigns fitness for all the solutions.
		/// </summary>
		public void FitnessAssign()
		{
			double[] strength = new double[solutionSet.Size()];
			double[] rawFitness = new double[solutionSet.Size()];
			double kDistance;


			//Calculate the strength value
			// strength(i) = |{j | j <- SolutionSet and i dominate j}|
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				for (int j = 0; j < solutionSet.Size(); j++)
				{
					if (dominance.Compare(solutionSet.Get(i), solutionSet.Get(j)) == -1)
					{
						strength[i] += 1.0;
					}
				}
			}


			//Calculate the raw fitness
			// rawFitness(i) = |{sum strenght(j) | j <- SolutionSet and j dominate i}|
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				for (int j = 0; j < solutionSet.Size(); j++)
				{
					if (dominance.Compare(solutionSet.Get(i), solutionSet.Get(j)) == 1)
					{
						rawFitness[i] += strength[j];
					}
				}
			}


			// Add the distance to the k-th individual. In the reference paper of SPEA2, 
			// k = sqrt(population.size()), but a value of k = 1 recommended. See
			// http://www.tik.ee.ethz.ch/pisa/selectors/spea2/spea2_documentation.txt
			int k = 1;
			for (int i = 0; i < distanceMatrix.Length; i++)
			{

				Array.Sort(distanceMatrix[i]);
				kDistance = 1.0 / (distanceMatrix[i][k] + 2.0); // Calcule de D(i) distance
				solutionSet.Get(i).Fitness = rawFitness[i] + kDistance;
			}
		}


		/// <summary>
		/// Gets 'size' elements from a population of more than 'size' elements
		/// using for this de enviromentalSelection truncation
		/// </summary>
		/// <param name="size">The number of elements to get.</param>
		/// <returns></returns>
		public SolutionSet EnvironmentalSelection(int size)
		{

			if (solutionSet.Size() < size)
			{
				size = solutionSet.Size();
			}

			// Create a new auxiliar population for no alter the original population
			SolutionSet aux = new SolutionSet(solutionSet.Size());

			int i = 0;
			while (i < solutionSet.Size())
			{
				if (solutionSet.Get(i).Fitness < 1.0)
				{
					aux.Add(solutionSet.Get(i));
					solutionSet.Remove(i);
				}
				else
				{
					i++;
				}
			}

			if (aux.Size() < size)
			{
				IComparer<Solution> comparator = new FitnessComparator();
				solutionSet.Sort(comparator);
				int remain = size - aux.Size();
				for (i = 0; i < remain; i++)
				{
					aux.Add(solutionSet.Get(i));
				}
				return aux;
			}
			else if (aux.Size() == size)
			{
				return aux;
			}

			double[][] distanceMatrix = distance.DistanceMatrix(aux);
			List<List<DistanceNode>> distanceList = new List<List<DistanceNode>>();
			for (int pos = 0; pos < aux.Size(); pos++)
			{
				aux.Get(pos).Location = pos;
				List<DistanceNode> distanceNodeList = new List<DistanceNode>();
				for (int refe = 0; refe < aux.Size(); refe++)
				{
					if (pos != refe)
					{
						distanceNodeList.Add(new DistanceNode(distanceMatrix[pos][refe], refe));
					}
				}
				distanceList.Add(distanceNodeList);
			}


			foreach (List<DistanceNode> aDistanceList in distanceList)
			{
				aDistanceList.Sort(distanceNodeComparator);

			}

			while (aux.Size() > size)
			{
				double minDistance = double.MaxValue;
				int toRemove = 0;
				i = 0;
				foreach (List<DistanceNode> dn in distanceList)
				{
					if (dn[0].Distance < minDistance)
					{
						toRemove = i;
						minDistance = dn[0].Distance;
						//i and toRemove have the same distance to the first solution
					}
					else if (dn[0].Distance == minDistance)
					{
						int k = 0;
						while ((dn[k].Distance == distanceList[toRemove][k].Distance) &&
								k < (distanceList[i].Count - 1))
						{
							k++;
						}

						if (dn[k].Distance < distanceList[toRemove][k].Distance)
						{
							toRemove = i;
						}
					}
					i++;
				}

				int tmp = aux.Get(toRemove).Location;
				aux.Remove(toRemove);
				distanceList.RemoveAt(toRemove);

				foreach (List<DistanceNode> aDistanceList in distanceList)
				{
					List<DistanceNode> interIterator = aDistanceList.ToList();
					foreach (var element in interIterator)
					{
						if (element.Reference == tmp)
						{
							aDistanceList.Remove(element);
						}
					}
				}
			}
			return aux;
		}

		#endregion
	}
}

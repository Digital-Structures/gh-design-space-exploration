using JMetalCSharp.Core;
using JMetalCSharp.Utils.Comparators;
using JMetalCSharp.Utils.Wrapper;
using System;

namespace JMetalCSharp.Utils
{
	/// <summary>
	/// This class implements some utilities for calculating distances
	/// </summary>
	public class Distance
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public Distance()
		{
			//do nothing
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns a matrix with distances between solutions in a <code>SolutionSet</code>.
		/// </summary>
		/// <param name="solutionSet">The <code>SolutionSet</code>.</param>
		/// <returns>a matrix with distances.</returns>
		public double[][] DistanceMatrix(SolutionSet solutionSet)
		{
			Solution solutionI, solutionJ;

			//The matrix of distances
			double[][] distance = new double[solutionSet.Size()][];

			for (int i = 0; i < solutionSet.Size(); i++)
			{
				distance[i] = new double[solutionSet.Size()];
			}

			//-> Calculate the distances
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				distance[i][i] = 0.0;
				solutionI = solutionSet.Get(i);
				for (int j = i + 1; j < solutionSet.Size(); j++)
				{
					solutionJ = solutionSet.Get(j);
					distance[i][j] = this.DistanceBetweenObjectives(solutionI, solutionJ);
					distance[j][i] = distance[i][j];
				}
			}

			//->Return the matrix of distances
			return distance;
		}

		/// <summary>
		/// Returns the minimum distance from a <code>Solution</code> to a
		/// <code>SolutionSet according to the objective values</code>.
		/// </summary>
		/// <param name="solution">The <code>Solution</code>.</param>
		/// <param name="solutionSet">The <code>SolutionSet</code>.</param>
		/// <returns>The minimum distance between solution and the set.</returns>
		public double DistanceToSolutionSetInObjectiveSpace(Solution solution, SolutionSet solutionSet)
		{
			//At start point the distance is the max
			double distance = double.MaxValue;

			// found the min distance respect to population
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				double aux = this.DistanceBetweenObjectives(solution, solutionSet.Get(i));
				if (aux < distance)
					distance = aux;
			}

			//->Return the best distance
			return distance;
		}

		/// <summary>
		/// Returns the minimum distance from a <code>Solution</code> to a 
		/// <code>SolutionSet according to the encodings.variable values</code>.
		/// </summary>
		/// <param name="solution">The <code>Solution</code></param>
		/// <param name="solutionSet">The <code>SolutionSet</code>.</param>
		/// <returns>The minimum distance between solution and the set.</returns>
		public double DistanceToSolutionSetInSolutionSpace(Solution solution, SolutionSet solutionSet)
		{
			//At start point the distance is the max
			double distance = double.MaxValue;

			// found the min distance respect to population
			for (int i = 0; i < solutionSet.Size(); i++)
			{
				double aux = this.DistanceBetweenSolutions(solution, solutionSet.Get(i));
				if (aux < distance)
					distance = aux;
			}

			//->Return the best distance
			return distance;
		}

		/// <summary>
		/// Returns the distance between two solutions in the search space.
		/// </summary>
		/// <param name="solutionI">The first <code>Solution</code>.</param>
		/// <param name="solutionJ">The second <code>Solution</code>.</param>
		/// <returns>The distance between solutions.</returns>
		public double DistanceBetweenSolutions(Solution solutionI, Solution solutionJ)
		{
			double distance = 0.0;
			XReal solI = new XReal(solutionI);
			XReal solJ = new XReal(solutionJ);

			double diff;    //Auxiliar var
			//-> Calculate the Euclidean distance
			for (int i = 0; i < solI.GetNumberOfDecisionVariables(); i++)
			{
				diff = solI.GetValue(i) - solJ.GetValue(i);
				distance += Math.Pow(diff, 2.0);
			}
			//-> Return the euclidean distance
			return Math.Sqrt(distance);
		}

		/// <summary>
		/// Returns the distance between two solutions in objective space.
		/// </summary>
		/// <param name="solutionI">The first <code>Solution</code>.</param>
		/// <param name="solutionJ">The second <code>Solution</code>.</param>
		/// <returns>The distance between solutions in objective space.</returns>
		public double DistanceBetweenObjectives(Solution solutionI, Solution solutionJ)
		{
			double diff;    //Auxiliar var
			double distance = 0.0;
			//-> Calculate the euclidean distance
			for (int nObj = 0; nObj < solutionI.NumberOfObjectives; nObj++)
			{
				diff = solutionI.Objective[nObj] - solutionJ.Objective[nObj];
				distance += Math.Pow(diff, 2.0);
			}

			//Return the euclidean distance
			return Math.Sqrt(distance);
		}

		/// <summary>
		/// Return the index of the nearest solution in the solution set to a given solution
		/// </summary>
		/// <param name="solution"></param>
		/// <param name="solutionSet"></param>
		/// <returns>The index of the nearest solution; -1 if the solutionSet is empty</returns>
		public int IndexToNearestSolutionInSolutionSpace(Solution solution, SolutionSet solutionSet)
		{
			int index = -1;
			double minimumDistance = double.MaxValue;
			try
			{
				for (int i = 0; i < solutionSet.Size(); i++)
				{
					double distance = 0;
					distance = this.DistanceBetweenSolutions(solution, solutionSet.Get(i));
					if (distance < minimumDistance)
					{
						minimumDistance = distance;
						index = i;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".IndexToNearestSolutionInSolutionSpace()", ex);
				Console.WriteLine("Exception in " + this.GetType().FullName + ".IndexToNearestSolutionInSolutionSpace()");
			}
			return index;
		}

		/// <summary>
		/// Assigns crowding distances to all solutions in a <code>SolutionSet</code>.
		/// </summary>
		/// <param name="solutionSet">The <code>SolutionSet</code>.</param>
		/// <param name="nObjs">Number of objectives.</param>
		public void CrowdingDistanceAssignment(SolutionSet solutionSet, int nObjs)
		{
			int size = solutionSet.Size();

			if (size == 0)
				return;

			if (size == 1)
			{
				solutionSet.Get(0).CrowdingDistance = double.PositiveInfinity;
				return;
			}

			if (size == 2)
			{
				solutionSet.Get(0).CrowdingDistance = double.PositiveInfinity;
				solutionSet.Get(1).CrowdingDistance = double.PositiveInfinity;
				return;
			}

			//Use a new SolutionSet to evite alter original solutionSet
			SolutionSet front = new SolutionSet(size);
			for (int i = 0; i < size; i++)
			{
				front.Add(solutionSet.Get(i));
			}

			for (int i = 0; i < size; i++)
				front.Get(i).CrowdingDistance = 0.0;

			double objetiveMaxn;
			double objetiveMinn;
			double distance;

			for (int i = 0; i < nObjs; i++)
			{
				// Sort the population by Obj n            
				front.Sort(new ObjectiveComparator(i));
				objetiveMinn = front.Get(0).Objective[i];
				objetiveMaxn = front.Get(front.Size() - 1).Objective[i];

				//Set de crowding distance            
				front.Get(0).CrowdingDistance = double.PositiveInfinity;
				front.Get(size - 1).CrowdingDistance = double.PositiveInfinity;

				for (int j = 1; j < size - 1; j++)
				{
					distance = front.Get(j + 1).Objective[i] - front.Get(j - 1).Objective[i];
					distance = distance / (objetiveMaxn - objetiveMinn);
					distance += front.Get(j).CrowdingDistance;
					front.Get(j).CrowdingDistance = distance;
				}
			}
		}

		#endregion
	}
}

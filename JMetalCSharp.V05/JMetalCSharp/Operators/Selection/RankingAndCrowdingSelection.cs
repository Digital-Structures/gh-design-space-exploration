using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// This class implements a selection for selecting a number of solutions from
	/// a solutionSet. The solutions are taken by mean of its ranking and 
	/// crowding ditance values.
	/// NOTE: if you use the default constructor, the problem has to be passed as
	/// a parameter before invoking the execute() method -- see lines 67 - 74
	/// </summary>
	class RankingAndCrowdingSelection : Selection
	{
		#region Private Attributes
		/// <summary>
		/// Stores the problem to solve
		/// </summary>
		private Problem problem = null;

		/// <summary>
		/// Stores a <code>Comparator</code> for crowding comparator checking.
		/// </summary>
		private static readonly IComparer<Solution> crowdingComparator = new CrowdingComparator();

		/// <summary>
		/// Stores a <code>Distance</code> object for distance utilities.
		/// </summary>
		private static readonly Distance distance = new Distance();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parameters"></param>
		public RankingAndCrowdingSelection(Dictionary<string, object> parameters)
			: base(parameters)
		{
			object value;

			if (parameters.TryGetValue("problem", out value))
			{
				problem = (Problem)value;
			}

			if (problem == null)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".RankingAndCrowdingSelection()");
				Console.WriteLine("Exception in " + this.GetType().FullName + ".RankingAndCrowdingSelection()");
				throw new Exception("Exception in " + this.GetType().FullName + ".RankingAndCrowdingSelection()");
			}

		}

		#endregion

		/// <summary>
		/// Performs the operation
		/// </summary>
		/// <param name="obj">Object representing a SolutionSet.</param>
		/// <returns>An object representing a <code>SolutionSet<code> with the selected parents</returns>
		public override object Execute(object obj)
		{
			SolutionSet population = (SolutionSet)obj;
			int populationSize = (int)Parameters["populationSize"];
			SolutionSet result = new SolutionSet(populationSize);

			//->Ranking the union
			Ranking ranking = new Ranking(population);

			int remain = populationSize;
			int index = 0;
			SolutionSet front = null;
			population.Clear();

			//-> Obtain the next front
			front = ranking.GetSubfront(index);

			while ((remain > 0) && (remain >= front.Size()))
			{
				//Asign crowding distance to individuals
				distance.CrowdingDistanceAssignment(front, problem.NumberOfObjectives);
				//Add the individuals of this front
				for (int k = 0; k < front.Size(); k++)
				{
					result.Add(front.Get(k));
				}

				//Decrement remaint
				remain = remain - front.Size();

				//Obtain the next front
				index++;
				if (remain > 0)
				{
					front = ranking.GetSubfront(index);
				}
			}

			//-> remain is less than front(index).size, insert only the best one
			if (remain > 0)
			{  // front containt individuals to insert                        
				distance.CrowdingDistanceAssignment(front, problem.NumberOfObjectives);
				front.Sort(crowdingComparator);
				for (int k = 0; k < remain; k++)
				{
					result.Add(front.Get(k));
				}

				remain = 0;
			}

			return result;
		}
	}
}

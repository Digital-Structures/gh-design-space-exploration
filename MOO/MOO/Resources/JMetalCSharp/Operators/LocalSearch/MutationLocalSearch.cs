using JMetalCSharp.Core;
using JMetalCSharp.Utils.Comparators;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.LocalSearch
{
	/// <summary>
	/// This class implements an local search operator based in the use of a 
	/// mutation operator. An archive is used to store the non-dominated solutions
	/// found during the search.
	/// </summary>
	public class MutationLocalSearch : LocalSearch
	{

		#region Private Attibutes
		/// <summary>
		/// Stores the problem to solve
		/// </summary>
		private Problem problem;

		/// <summary>
		/// Stores a reference to the archive in which the non-dominated solutions are
		/// inserted
		/// </summary>
		private SolutionSet archive;

		private int improvementRounds;

		/// <summary>
		/// Stores comparators for dealing with constraints and dominance checking, 
		/// respectively.
		/// </summary>
		private IComparer<Solution> constraintComparator;
		private IComparer<Solution> dominanceComparator;

		/// <summary>
		/// Stores the mutation operator
		/// </summary>
		private Operator mutationOperator;

		/// <summary>
		/// Stores the number of evaluations carried out
		/// </summary>
		private int evaluations;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// Creates a new local search object.
		/// </summary>
		/// <param name="parameters">The parameters</param>
		public MutationLocalSearch(Dictionary<string, object> parameters)
			: base(parameters)
		{
			Utils.Utils.GetProblemFromParameters(parameters, "problem", ref problem);
			Utils.Utils.GetIntValueFromParameter(parameters, "improvementRounds", ref improvementRounds);
			Utils.Utils.GetMutationFromParameters(parameters, "mutation", ref mutationOperator);

			evaluations = 0;
			archive = null;
			dominanceComparator = new DominanceComparator();
			constraintComparator = new OverallConstraintViolationComparator();
		}

		#endregion

		#region Public Overrides
		/// <summary>
		///  Executes the local search. The maximum number of iterations is given by
		///  the param "improvementRounds", which is in the parameter list of the
		///  operator. The archive to store the non-dominated solutions is also in the
		///  parameter list.
		/// </summary>
		/// <param name="obj">Object representing a solution</param>
		/// <returns>An object containing the new improved solution</returns>
		public override object Execute(object obj)
		{
			int i = 0;
			int best = 0;
			evaluations = 0;
			Solution solution = (Solution)obj;

			int rounds = improvementRounds;
			archive = (SolutionSet)GetParameter("archive");

			if (rounds <= 0)
				return new Solution(solution);

			do
			{
				i++;
				Solution mutatedSolution = new Solution(solution);
				mutationOperator.Execute(mutatedSolution);

				// Evaluate the getNumberOfConstraints
				if (problem.NumberOfConstraints > 0)
				{
					problem.EvaluateConstraints(mutatedSolution);
					best = constraintComparator.Compare(mutatedSolution, solution);
					if (best == 0) //none of then is better that the other one
					{
						problem.Evaluate(mutatedSolution);
						evaluations++;
						best = dominanceComparator.Compare(mutatedSolution, solution);
					}
					else if (best == -1) //mutatedSolution is best
					{
						problem.Evaluate(mutatedSolution);
						evaluations++;
					}
				}
				else
				{
					problem.Evaluate(mutatedSolution);
					evaluations++;
					best = dominanceComparator.Compare(mutatedSolution, solution);
				}
				if (best == -1) // This is: Mutated is best
					solution = mutatedSolution;
				else if (best == 1) // This is: Original is best
					//delete mutatedSolution
					;
				else // This is mutatedSolution and original are non-dominated
				{
					if (archive != null)
						archive.Add(mutatedSolution);
				}
			}
			while (i < rounds);
			return new Solution(solution);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns the number of evaluations maded
		/// </summary>
		/// <returns></returns>
		public override int GetEvaluations()
		{
			return evaluations;
		}

		#endregion
	}
}

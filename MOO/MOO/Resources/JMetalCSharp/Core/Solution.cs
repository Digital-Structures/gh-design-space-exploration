using JMetalCSharp.Encoding.Variable;
using System;

namespace JMetalCSharp.Core
{
	/// <summary>
	/// Class representing a solution for a problem.Class representing a solution for a problem.
	/// </summary>
	public class Solution
	{
		/// <summary>
		/// Stores the problem
		/// </summary>
		public Problem problem
		{
			get;
			private set;
		}

		/// <summary>
		/// Stores the type of the encoding.variable
		/// </summary>
		public SolutionType Type { get; set; }

		/// <summary>
		/// Stores the decision variables of the solution
		/// </summary>
		public Variable[] Variable { get; set; }

		double[] o;
		/// <summary>
		/// Stores the objectives values of the solution
		/// </summary>
		public double[] Objective
		{
			get
			{
				return o;
			}
			set
			{
				o = value;
			}
		}

		/// <summary>
		/// Stores the number of objective values of the solution
		/// </summary>
		private int numberOfObjectives;

		/// <summary>
		/// Returns the number of objectives.
		/// </summary>
		public int NumberOfObjectives
		{
			get
			{
				if (this.Objective == null)
				{
					return 0;
				}
				else
				{
					return numberOfObjectives;
				};
			}
		}

		/// <summary>
		/// Stores the so called fitness value. Used in some metaheuristics
		/// </summary>
		public double Fitness { get; set; }

		/// <summary>
		/// Used in algorithm AbYSS, this field is intended to be used to know
		/// when a <code>Solution</code> is marked.
		/// </summary>
		private bool marked;

		/// <summary>
		/// Stores the so called rank of the solution. Used in NSGA-II
		/// </summary>
		public int Rank { get; set; }

		/// <summary>
		/// Stores the overall constraint violation of the solution
		/// </summary>
		public double OverallConstraintViolation { get; set; }

		/// <summary>
		/// Stores the number of constraints violated by the solution
		/// </summary>
		public int NumberOfViolatedConstraints { get; set; }

		/// <summary>
		/// This field is intended to be used to know the location of
		/// a solution into a <code>SolutionSet</code>. Used in MOCell
		/// </summary>
		public int Location { get; set; }

		/// <summary>
		/// Stores the distance to his k-nearest neighbor into a 
		/// <code>SolutionSet</code>. Used in SPEA2.
		/// </summary>
		public double KDistance { get; set; }

		/// <summary>
		/// Stores the crowding distance of the the solution in a 
		/// <code>SolutionSet</code>. Used in NSGA-II.
		/// </summary>
		public double CrowdingDistance { get; set; }

		/// <summary>
		/// Stores the distance between this solution and a <code>SolutionSet</code>.
		/// Used in AbySS.
		/// </summary>
		public double DistanceToSolutionSet { get; set; }

		#region Constructors
		public Solution()
		{
			this.problem = null;
			this.marked = false;
			this.OverallConstraintViolation = 0.0;
			this.NumberOfViolatedConstraints = 0;
			this.Type = null;
			this.Variable = null;
			this.Objective = null;
		}

		/// <summary>
		/// This constructor is used mainly to read objective values from a file to
		/// variables of a SolutionSet to apply quality indicators
		/// </summary>
		/// <param name="numberOfObjectives">Number of objectives of the solution</param>
		public Solution(int numberOfObjectives)
		{
			this.numberOfObjectives = numberOfObjectives;
			this.Objective = new double[numberOfObjectives];
		}

		public Solution(Problem problem)
		{
			this.problem = problem;
			this.Type = problem.SolutionType;
			numberOfObjectives = problem.NumberOfObjectives;
			this.Objective = new double[numberOfObjectives];

			this.Fitness = 0.0;
			this.KDistance = 0.0;
			this.CrowdingDistance = 0.0;
			this.DistanceToSolutionSet = double.PositiveInfinity;

			this.Variable = Type.CreateVariables();
		}

		public Solution(Problem problem, Variable[] variables)
		{
			this.problem = problem;
			this.Type = problem.SolutionType;
			this.numberOfObjectives = problem.NumberOfObjectives;
			this.Objective = new double[this.numberOfObjectives];

			this.Fitness = 0.0;
			this.KDistance = 0.0;
			this.CrowdingDistance = 0.0;
			this.DistanceToSolutionSet = double.PositiveInfinity;

			this.Variable = variables;
		}

		public Solution(Solution solution)
		{
			this.problem = solution.problem;
			this.Type = solution.Type;

			this.numberOfObjectives = solution.NumberOfObjectives;
			this.Objective = new double[this.numberOfObjectives];

			for (int i = 0; i < this.Objective.Length; i++)
			{
				this.Objective[i] = solution.Objective[i];
			}

			this.Variable = this.Type.CopyVariables(solution.Variable);
			this.OverallConstraintViolation = solution.OverallConstraintViolation;
			this.NumberOfViolatedConstraints = solution.NumberOfViolatedConstraints;
			this.DistanceToSolutionSet = solution.DistanceToSolutionSet;
			this.CrowdingDistance = solution.CrowdingDistance;
			this.KDistance = solution.KDistance;
			this.Fitness = solution.Fitness;
			this.marked = solution.marked;
			this.Rank = solution.Rank;
			this.Location = solution.Location;
		}

		#endregion

		public static Solution GetNewSolution(Problem problem)
		{
			return new Solution(problem);
		}

		/// <summary>
		/// Returns the number of decision variables of the solution.
		/// </summary>
		/// <returns></returns>
		public int NumberOfVariables()
		{
			return this.problem.NumberOfVariables;
		}

		/// <summary>
		/// Returns a string representing the solution.
		/// </summary>
		/// <returns>The string</returns>
		public override string ToString()
		{
			string aux = "";
			for (int i = 0; i < this.numberOfObjectives; i++)
			{
				aux = aux + this.Objective[i] + " ";
			}

			return aux;
		}

		/// <summary>
		/// Indicates if the solution is marked.
		/// </summary>
		/// <returns>
		/// true if the method <code>marked</code> has been called and, after 
		/// that, the method <code>unmarked</code> hasn't been called. False in other
		/// case.
		/// </returns>
		public bool IsMarked()
		{
			return this.marked;
		}

		/// <summary>
		/// Establishes the solution as marked.
		/// </summary>
		public void Marked()
		{
			this.marked = true;
		}

		/// <summary>
		/// Established the solution as unmarked.
		/// </summary>
		public void UnMarked()
		{
			this.marked = false;
		}

		/// <summary>
		/// Returns the aggregative value of the solution
		/// </summary>
		/// <returns>The aggregative value</returns>
		public double GetAggregativeValue()
		{
			double value = 0.0;
			for (int i = 0; i < this.numberOfObjectives; i++)
			{
				value += this.Objective[i];
			}

			return value;
		}

		/// <summary>
		/// Returns the number of bits of the chromosome in case of using a binary
		/// representation
		/// </summary>
		/// <returns>The number of bits if the case of binary variables, 0 otherwise</returns>
		public int GetNumberOfBits()
		{
			int bits = 0;

			for (int i = 0; i < Variable.Length; i++)
			{
				if ((Variable[i].GetVariableType() == typeof(Binary) ||
					(Variable[i].GetVariableType() == typeof(BinaryReal))))

					bits += ((Binary)(Variable[i])).NumberOfBits;
			}
			return bits;
		}
	}
}
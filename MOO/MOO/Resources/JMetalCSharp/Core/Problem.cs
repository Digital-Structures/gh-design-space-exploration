namespace JMetalCSharp.Core
{
	/// <summary>
	/// Abstract class representing a multiobjective optimization problem
	/// </summary>
	public abstract class Problem
	{
		#region selaed vars

		/// <summary>
		/// Defines the default precision of binary-coded variables
		/// </summary>
		private static int DEFAULT_PRECISSION = 16;

		#endregion

		#region private vars

		/// <summary>
		/// Stores the number of bits used by binary-coded variables (e.g., BinaryReal
		/// variables). By default, they are initialized to DEFAULT_PRECISION)
		/// </summary>
		public int[] Precision { get; set; }

		#endregion

		#region protected vars
		/// <summary>
		/// Stores the number of variables of the problem
		/// </summary>
		public int NumberOfVariables { get; protected set; }

		/// <summary>
		/// Stores the number of objectives of the problem
		/// </summary>
		public int NumberOfObjectives { get; protected set; }

		/// <summary>
		/// Stores the number of constraints of the problem
		/// </summary>
		public int NumberOfConstraints { get; set; }

		/// <summary>
		/// Stores the problem name
		/// </summary>
		public string ProblemName { get; set; }

		/// <sumary>
		/// Stores the type of the solutions of the problem
		/// /summary>
		public SolutionType SolutionType { get; protected set; }

		/// <summary>
		/// Stores the lower bound values for each encodings.variable (only if needed)
		/// </summary>
		public double[] LowerLimit { get; protected set; }

		/// <summary>
		/// Stores the upper bound values for each encodings.variable (only if needed)
		/// </summary>
		public double[] UpperLimit { get; protected set; }

		/// <summary>
		/// Stores the length of each encodings.variable when applicable (e.g., Binary and
		/// Permutation variables)
		/// </summary>
		protected int[] Length { get; set; }

		#endregion

		#region Contructors

		public Problem()
		{
			SolutionType = null;
		}

		#endregion


		/**
         * Evaluates a <code>Solution</code> object.
         * @param solution The <code>Solution</code> to evaluate.
         */
		public abstract void Evaluate(Solution solution);


		/**
		 * Evaluates the overall constraint violation of a <code>Solution</code> 
		 * object.
		 * @param solution The <code>Solution</code> to evaluate.
		 */
		public virtual void EvaluateConstraints(Solution solution)
		{
			// The default behavior is to do nothing. Only constrained problems have to
			// re-define this method
		}

		/// <summary>
		/// Returns the length of the encodings.variable.
		/// </summary>
		/// <param name="var"></param>
		/// <returns>the encodings.variable length.</returns>
		public int GetLength(int var)
		{
			if (Length == null)
				return DEFAULT_PRECISSION;
			return Length[var];
		}

		/// <summary>
		/// Returns the number of bits of the solutions of the problem
		/// </summary>
		/// <returns>The number of bits solutions of the problem</returns>
		public int GetNumberOfBits()
		{
			int result = 0;
			for (int var = 0; var < NumberOfVariables; var++)
			{
				result += GetLength(var);
			}
			return result;
		}
	}
}

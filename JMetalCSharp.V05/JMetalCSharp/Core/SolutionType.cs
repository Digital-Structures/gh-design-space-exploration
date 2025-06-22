namespace JMetalCSharp.Core
{
	/// <summary>
	/// Abstract class representing solution types, which define the types of the 
	/// variables constituting a solution 
	/// </summary>
	public abstract class SolutionType
	{
		/// <summary>
		/// Problem to be solved
		/// </summary>
		public Problem Problem { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">The problem to solve</param>
		public SolutionType(Problem problem)
		{
			Problem = problem;
		}

		/// <summary>
		/// Abstract method to create the variables of the solution
		/// </summary>
		/// <returns></returns>
		public abstract Variable[] CreateVariables();

		/// <summary>
		/// Copies the decision variables
		/// </summary>
		/// <param name="vars"></param>
		/// <returns>An array of variables</returns>
		public virtual Variable[] CopyVariables(Variable[] vars)
		{
			Variable[] variables;

			variables = new Variable[vars.Length];

			for (int var = 0; var < vars.Length; var++)
			{
				variables[var] = vars[var].DeepCopy();
			}

			return variables;
		}
	}
}

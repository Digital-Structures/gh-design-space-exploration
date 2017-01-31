using System.Collections.Generic;

namespace JMetalCSharp.Core
{
	public abstract class Algorithm
	{
		/// <summary>
		/// Stores the problem to solve
		/// </summary>
		protected Problem Problem { get; set; }

		/// <summary>
		/// Stores the operators used by the algorithm, such as selection, crossover, etc.
		/// </summary>
		protected Dictionary<string, Operator> Operators { get; set; }

		/// <summary>
		/// Stores algorithm specific parameters. For example, in NSGA-II these parameters include the population size and the maximum number of function evaluations.
		/// </summary>
		protected Dictionary<string, object> InputParameters { get; set; }

		/// <summary>
		/// Stores output parameters, which are retrieved by Main object to obtain information from an algorithm.
		/// </summary>
		private Dictionary<string, object> OutputParameters { get; set; }

		public SolutionSet Result { get; protected set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">The problem to be solved</param>
		public Algorithm(Problem problem)
		{
			this.Problem = problem;
		}

		/// <summary>
		/// Launches the execution of an specific algorithm.
		/// </summary>
		/// <returns>a <code>SolutionSet</code> that is a set of non dominated solutions as a result of the algorithm execution</returns>
		public abstract SolutionSet Execute();

		/// <summary>
		/// Offers facilities for add new operators for the algorithm. To use an operator, an algorithm has to obtain it through the <code>GetOperator</code> method.
		/// </summary>
		/// <param name="name">The operator name</param>
		/// <param name="op">The operator</param>
		public void AddOperator(string name, Operator op)
		{
			if (this.Operators == null)
			{
				this.Operators = new Dictionary<string, Operator>();
			}

			this.Operators[name] = op;
		}

		/// <summary>
		/// Gets an operator through his name. If the operator doesn't exist or the name is wrong this method returns null. The client of this method have to check the result of the method.
		/// </summary>
		/// <param name="name">The operator name</param>
		/// <returns>The operator if exists, null in another case.</returns>
		public Operator GetOperator(string name)
		{
			return this.Operators[name];
		}

		/// <summary>
		/// Sets an input parameter to an algorithm. Typically, the method is invoked by a Main object before running an algorithm. The parameters have to been inserted using their name to access them through the <code>GetInputParameter</code> method.
		/// </summary>
		/// <param name="name">The parameter name</param>
		/// <param name="obj">object that represent a parameter for the algorithm.</param>
		public void SetInputParameter(string name, object obj)
		{
			if (this.InputParameters == null)
			{
				this.InputParameters = new Dictionary<string, object>();
			}

			this.InputParameters[name] = obj;
		}

		/// <summary>
		/// Gets an input parameter through its name. Typically, the method is invoked by an object representing an algorithm
		/// </summary>
		/// <param name="name">The parameter name</param>
		/// <returns>object representing the parameter or null if the parameter doesn't exist or the name is wrong</returns>
		public object GetInputParameter(string name)
		{
			object value;

			this.InputParameters.TryGetValue(name, out value);
			return value;
		}

		/// <summary>
		/// Sets an output parameter that can be obtained by invoking <code>GetOutputParame</code>. Typically this algorithm is invoked by an algorithm at the end of the <code>Execute</code> to retrieve output information
		/// </summary>
		/// <param name="name">The output parameter name</param>
		/// <param name="obj">object representing the output parameter</param>
		public void SetOutputParameter(string name, object obj)
		{
			if (this.OutputParameters == null)
			{
				this.OutputParameters = new Dictionary<string, object>();
			}

			this.OutputParameters[name] = obj;
		}

		/// <summary>
		/// ets an output parameter through its name. Typically, the method is invoked by a Main object after the execution of an algorithm.
		/// </summary>
		/// <param name="name">The output parameter name</param>
		/// <returns>object representing the output parameter, or null if the parameter doesn't exist or the name is wrong.</returns>
		public object GetOutputParameter(string name)
		{
			if (this.OutputParameters != null)
			{
				return this.OutputParameters[name];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the problem to solve
		/// </summary>
		/// <returns>The problem to solve</returns>
		public Problem GetProblem()
		{
			return this.Problem;
		}
	}
}

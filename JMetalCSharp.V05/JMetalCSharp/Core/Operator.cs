using System.Collections.Generic;

namespace JMetalCSharp.Core
{
	public abstract class Operator
	{
		/// <summary>
		/// Stores the current operator parameters. 
		/// It is defined as a Map of pairs <<code>string</code>, <code>object</code>>, 
		/// and it allow objects to be accessed by their names, which  are specified 
		/// by the string.
		/// </summary>
		protected Dictionary<string, object> Parameters;

		#region Constructor
		public Operator(Dictionary<string, object> parameters)
		{
			this.Parameters = parameters;
		}
		#endregion

		/// <summary>
		/// Abstract method that must be defined by all the operators. When invoked, 
		/// this method executes the operator represented by the current object.
		/// </summary>
		/// <param name="obj">This param inherits from object to allow different kinds 
		/// of parameters for each operator. For example, a selection 
		/// operator typically receives a <code>SolutionSet</code> as 
		/// a parameter, while a mutation operator receives a 
		/// <code>Solution</code>.</param>
		/// <returns></returns>
		public abstract object Execute(object obj);

		/// <summary>
		/// Sets a new <code>object</code> parameter to the operator.
		/// </summary>
		/// <param name="name">The parameter name</param>
		/// <param name="value">object representing the parameter.</param>
		public void SetParameter(string name, object value)
		{
			this.Parameters.Add(name, value);
		}

		/// <summary>
		/// Returns an object representing a parameter of the <code>Operator</code>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <returns>the parameter</returns>
		public object GetParameter(string name)
		{
			object parameter;
			this.Parameters.TryGetValue(name, out parameter);
			return parameter;
		}
	}
}

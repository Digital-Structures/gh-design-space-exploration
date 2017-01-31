using JMetalCSharp.Core;
using JMetalCSharp.Operators.Selection;
using System;

namespace JMetalCSharp.Utils.OffSpring
{
	/// <summary>
	/// This is the base class to define offspring classes, which create one offspring solution from a solution set
	/// </summary>
	public class Offspring
	{
		public string Id { get; internal set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public Offspring()
		{
			Id = null;
		}

		/// <summary>
		/// Return n offspring from a solution set, indicating the selection operator
		/// </summary>
		/// <param name="solutionSet"></param>
		/// <param name="numberOfParents"></param>
		/// <param name="index"></param>
		/// <param name="selectionOperator"></param>
		/// <returns>the offspring</returns>
		public virtual Solution GetOffspring(SolutionSet solutionSet, int numberOfParents, int index, Selection selectionOperator)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		/// <summary>
		/// Return on offspring from a solution set
		/// </summary>
		/// <param name="solutionSet"></param>
		/// <returns>The offspring</returns>
		public virtual Solution GetOffspring(SolutionSet solutionSet)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		/// <summary>
		/// Return on offspring from a solution set and a given solution
		/// </summary>
		/// <param name="solutionSet"></param>
		/// <param name="solution"></param>
		/// <returns>The offspring</returns>
		public virtual Solution GetOffspring(SolutionSet solutionSet, Solution solution)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		/// <summary>
		/// Return on offspring from a solution set and a given solution
		/// </summary>
		/// <param name="solutionSet"></param>
		/// <param name="solution"></param>
		/// <returns>The offspring</returns>
		public virtual Solution GetOffspring(Solution[] solutionSet, Solution solution)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		/// <summary>
		/// Return on offspring from two solution sets
		/// </summary>
		/// <param name="solutionSet1"></param>
		/// <param name="archive2"></param>
		/// <returns>The offspring</returns>
		public virtual Solution GetOffspring(SolutionSet solutionSet1, SolutionSet archive2)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		public virtual Solution GetOffspring(Solution[] solutions)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		/// <summary>
		/// Return on offspring from a solution set and the index of the current individual
		/// </summary>
		/// <param name="solutionSet1"></param>
		/// <param name="index"></param>
		/// <returns>The offspring</returns>
		public virtual Solution GetOffspring(SolutionSet solutionSet1, int index)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		public virtual Solution GetOffspring(SolutionSet solutionSet1, SolutionSet solutionSet2, int index)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		public virtual Solution GetOffspring(SolutionSet solutionSet, Solution solution, int index)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		public virtual Solution GetOffspring(SolutionSet solutionSet, SolutionSet archive, Solution solution, int index)
		{
			Logger.Log.Error(typeof(Offspring).FullName + ": Method not implemented");
			Console.Error.WriteLine(typeof(Offspring).FullName + ": Method not implemented");
			return null;
		}

		public virtual string Configuration()
		{
			return null;
		}
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems
{
	/// <summary>
	/// This class represents a factory for problems
	/// </summary>
	public static class ProblemFactory
	{
		/// <summary>
		/// Creates an object representing a problem
		/// </summary>
		/// <param name="name">Name of the problem</param>
		/// <param name="param">Parameters characterizing the problem</param>
		/// <returns>The object representing the problem</returns>
		public static Problem GetProblem(string name, object[] param)
		{
			// Params are the arguments
			// The number of argument must correspond with the problem constructor params

			string baseNamespace = "JMetalCSharp.Problems.";
			if (name == "TSP" || name == "OneMax")
			{
				baseNamespace += "singleObjective.";
			}
			else if (name == "mQAP")
			{
				baseNamespace += "mqap.";
			}
			else if (name.Substring(0, name.Length - 1).Equals("DTLZ", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "DTLZ.";
			}
			else if (name.Substring(0, name.Length - 3).Equals("LZ09", StringComparison.InvariantCultureIgnoreCase)
				|| name.Equals("LZ09", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "LZ09.";
			}
			else if (name.Substring(0, name.Length - 1).Equals("WFG", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "WFG.";
			}
			else if (name.Substring(0, name.Length - 1).Equals("UF", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "cec2009Competition.";
			}
			else if (name.Substring(0, name.Length - 2).Equals("UF", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "cec2009Competition.";
			}
			else if (name.Substring(0, name.Length - 1).Equals("ZDT", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "ZDT.";
			}
			else if (name.Substring(0, name.Length - 3).Equals("ZZJ07", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "ZZJ07.";
			}
			else if (name.Substring(0, name.Length - 3).Equals("LZ09", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "LZ09.";
			}
			else if (name.Substring(0, name.Length - 4).Equals("ZZJ07", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "ZZJ07.";
			}
			else if (name.Substring(0, name.Length - 3).Equals("LZ06", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "LZ06.";
			}
			else if (name.Equals("Kursawe", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "Kursawe.";
			}
			else if (name.Equals("Water", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "Water.";
			}
			else if (name.Equals("IntRealProblem", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "IntRealProblem.";
			}
			else if (name.Equals("OneMax", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "OneMax.";
			}
			else if (name.Equals("OneZeroMax", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "OneZeroMax.";
			}
			else if (name.Equals("Fonseca", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "Fonseca.";
			}
			else if (name.Equals("Schaffer", StringComparison.InvariantCultureIgnoreCase))
			{
				baseNamespace += "Schaffer.";
			}

			try
			{
				Type problemType = Type.GetType(baseNamespace + name);
				Problem problem = (Problem)Activator.CreateInstance(problemType, param);

				return problem;
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Exception in " + name + ".GetProblem()", ex);
				throw new Exception("Exception in " + name + ".GetProblem()");
			}
		}
	}
}

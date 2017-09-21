using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Metaheuristics
{
	public static class AlgorithmFactory
	{
		public static Algorithm GetAlgorithm(string name, string type, Problem problem)
		{
			string completeNamespace = "JMetalCSharp.Metaheuristics." + type + "." + name;

			try
			{
				Type algorithmType = Type.GetType(completeNamespace);
				Algorithm algorithm = (Algorithm)Activator.CreateInstance(algorithmType, problem);

				return algorithm;
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Exception in Algorithm.GetAlgorithm()", ex);
				throw new Exception("Exception in Algorithm.GetAlgorithm()");
			}
		}
	}
}

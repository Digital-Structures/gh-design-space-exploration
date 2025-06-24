using JMetalCSharp.Core;
using System.Collections.Generic;

namespace JMetalCSharp.Experiments
{
	public class ExperimentProblem
	{
		public string Alias { get; set; }
		public string ProblemName { get; set; }
		public string ParetoFront { get; set; }
		public IDictionary<string, IEnumerable<Algorithm>> AlgorithmDictionary { get; set; }

		public ExperimentProblem()
		{
			AlgorithmDictionary = new Dictionary<string, IEnumerable<Algorithm>>();
		}
	}
}

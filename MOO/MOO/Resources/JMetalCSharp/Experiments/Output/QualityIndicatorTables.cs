using JMetalCSharp.QualityIndicator;
using JMetalCSharp.QualityIndicator.Utils;
using JMetalCSharp.Utils;
using System.Collections.Generic;
using System.IO;

namespace JMetalCSharp.Experiments.Output
{
	public class QualityIndicatorTables
	{
		private Experiment experiment;
		public QualityIndicatorTables(Experiment experiment)
		{
			this.experiment = experiment;
		}

		public IDictionary<string, IDictionary<string, IDictionary<string, List<double>>>> GetIndicators()
		{
			GenerateReferenceParetoFronts();

			var indicators = new Dictionary<string, IDictionary<string, IDictionary<string, List<double>>>>();
			MetricsUtil utils = new MetricsUtil();

			for (int i = 0, li = experiment.QualityIndicators.Count; i < li; i++)
			{
				string indicatorString = experiment.QualityIndicators[i].ToUpper();
				double value = 0;

				var problems = new Dictionary<string, IDictionary<string, List<double>>>();
				indicators.Add(indicatorString, problems);

				foreach (var problem in experiment.ExperimentProblems)
				{
					var algorithm = new Dictionary<string, List<double>>();
					problems.Add(problem.Alias, algorithm);

					var trueFront = utils.ReadFront(problem.ParetoFront);

					foreach (var algorithmDictionary in problem.AlgorithmDictionary)
					{
						var indicator = new List<double>();

						algorithm.Add(algorithmDictionary.Key, indicator);

						foreach (var alg in algorithmDictionary.Value)
						{
							var solutionFront = alg.Result.GetObjectives();

							switch (indicatorString)
							{
								case "HV":
									HyperVolume hv = new HyperVolume();
									value = hv.Hypervolume(solutionFront, trueFront, trueFront[0].Length);
									break;
								case "SPREAD":
									Spread spread = new Spread();
									value = spread.CalculateSpread(solutionFront, trueFront, trueFront[0].Length);
									break;
								case "IGD":
									InvertedGenerationalDistance igd = new InvertedGenerationalDistance();
									value = igd.CalculateInvertedGenerationalDistance(solutionFront, trueFront, trueFront[0].Length);
									break;
								case "EPSILON":
									Epsilon epsilon = new Epsilon();
									value = epsilon.CalcualteEpsilon(solutionFront, trueFront, trueFront[0].Length);
									break;
							}

							indicator.Add(value);
						}
					}
				}
			}
			return indicators;
		}

		private void GenerateReferenceParetoFronts()
		{
			var problems = experiment.ExperimentProblems;
			var referencePath = Path.Combine(experiment.ExperimentBaseDirectory, "ReferenceParetoFront");

			if (!Directory.Exists(referencePath))
			{
				Directory.CreateDirectory(referencePath);
			}

			foreach (var problem in problems)
			{
				if (string.IsNullOrEmpty(problem.ParetoFront))
				{
					NonDominatedSolutionList solutionSet = new NonDominatedSolutionList();
					string file = Path.Combine(referencePath, problem.Alias + ".pf");
					MetricsUtil metrics = new MetricsUtil();

					foreach (var algorithmDictionary in problem.AlgorithmDictionary)
					{
						foreach (var algorithm in algorithmDictionary.Value)
						{
							metrics.ReadNonDominatedSolutionSet(algorithm.Result.GetObjectives(), solutionSet);
						}
					}

					solutionSet.PrintObjectivesToFile(file);
					problem.ParetoFront = file;
				}
			}
		}
	}
}

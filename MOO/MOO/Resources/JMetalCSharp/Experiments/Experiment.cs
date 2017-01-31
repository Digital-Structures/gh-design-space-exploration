using JMetalCSharp.Experiments.Output;
using JMetalCSharp.Utils.Parallel;
using System;
using System.Collections.Generic;
using System.IO;

namespace JMetalCSharp.Experiments
{
	public class Experiment
	{
		public string Name { get; set; }
		public int NumberOfExecutions { get; set; }
		public IEnumerable<ExperimentProblem> ExperimentProblems { get; set; }
		public IDictionary<string, bool> IndicatorsMinimize { get; set; }
		public List<string> QualityIndicators { get; set; }
		public string ExperimentBaseDirectory { get; set; }
		public bool GenerateQualityTables { get; set; }
		public bool GenerateWilcoxonTables { get; set; }
		public bool GenerateBoxPlots { get; set; }
		public bool GenerateFriedmanTables { get; set; }

		public Experiment(string name)
		{
			this.Name = name;
			this.ExperimentProblems = new List<ExperimentProblem>();

			// 50 executions by default
			this.NumberOfExecutions = 50;

			IndicatorsMinimize = new Dictionary<string, bool>();

			IndicatorsMinimize.Add("HV", false);
			IndicatorsMinimize.Add("SPREAD", true);
			IndicatorsMinimize.Add("GD", true);
			IndicatorsMinimize.Add("IGD", true);
			IndicatorsMinimize.Add("EPSILON", true);
		}

		public ExperimentResult RunExperiment()
		{
			MultithreadedAlgorithmRunner runner = new MultithreadedAlgorithmRunner();
			ExperimentResult result = new ExperimentResult();
			runner.StartParallelRunner(this);

			foreach (var experimentProblem in ExperimentProblems)
			{
				foreach (var algorithmDictionary in experimentProblem.AlgorithmDictionary)
				{
					var algorithmList = algorithmDictionary.Value;

					foreach (var algorithm in algorithmList)
					{

						runner.AddTaskForExecution(new object[] { algorithm });
					}
				}
			}

			//run algorithms
			long initTime = Environment.TickCount;
			runner.ParallelExecution();
			long elapsedTime = Environment.TickCount - initTime;

			result.ElapsedTime = elapsedTime;

			//process results
			result.Indicators = new QualityIndicatorTables(this).GetIndicators();

			if (this.GenerateQualityTables)
			{
				result.LatexPath = Path.Combine(this.ExperimentBaseDirectory, this.Name + ".tex");
				result.QualityTables = new LatexTables(this, result.LatexPath, result.Indicators).Generate();
			}

			if (this.GenerateBoxPlots)
			{
				result.BoxPlots = new BoxPlots(this, result.Indicators).Generate();
			}

			if (this.GenerateFriedmanTables)
			{
				new FriedmanTables(this, this.ExperimentBaseDirectory, result.Indicators).generate();
			}

			return result;
		}
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.Problems.MTSP;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalRunners.NSGAII
{
	public class NSGAIImTSP
	{
		/// <summary>
		/// Usage: 
		///     - NSGAIImTSP
		/// </summary>
		/// <param name="args">Command line arguments.</param>
		public static void Main(string[] args)
		{
			Problem problem; // The problem to solve
			Algorithm algorithm; // The algorithm to use
			Operator crossover; // Crossover operator
			Operator mutation; // Mutation operator
			Operator selection; // Selection operator

			Dictionary<string, object> parameters; // Operator parameters

			QualityIndicator indicators; // Object to get quality indicators

			// Logger object and file to store log messages
			var logger = Logger.Log;

			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "NSGAIImTSP.log";
			fileAppender.ActivateOptions();

			indicators = null;
			problem = new MTSP("Permutation", "kroA150.tsp", "kroB150.tsp");

			algorithm = new JMetalCSharp.Metaheuristics.NSGAII.NSGAII(problem);
			//algorithm = new ssNSGAII(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("populationSize", 100);
			algorithm.SetInputParameter("maxEvaluations", 10000000);

			/* Crossver operator */
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 0.95);
			//crossover = CrossoverFactory.getCrossoverOperator("TwoPointsCrossover", parameters);
			crossover = CrossoverFactory.GetCrossoverOperator("PMXCrossover", parameters);

			/* Mutation operator */
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 0.2);
			mutation = MutationFactory.GetMutationOperator("SwapMutation", parameters);

			/* Selection Operator */
			parameters = null;
			selection = SelectionFactory.GetSelectionOperator("BinaryTournament", parameters);

			// Add the operators to the algorithm
			algorithm.AddOperator("crossover", crossover);
			algorithm.AddOperator("mutation", mutation);
			algorithm.AddOperator("selection", selection);

			// Add the indicator object to the algorithm
			algorithm.SetInputParameter("indicators", indicators);

			// Execute the Algorithm
			long initTime = Environment.TickCount;
			SolutionSet population = algorithm.Execute();
			long estimatedTime = Environment.TickCount - initTime;

			// Result messages 
			logger.Info("Total execution time: " + estimatedTime + "ms");
			logger.Info("Variables values have been writen to file VAR");
			population.PrintVariablesToFile("VAR");
			logger.Info("Objectives values have been writen to file FUN");
			population.PrintObjectivesToFile("FUN");

			if (indicators != null)
			{
				logger.Info("Quality indicators");
				logger.Info("Hypervolume: " + indicators.GetHypervolume(population));
				logger.Info("GD         : " + indicators.GetGD(population));
				logger.Info("IGD        : " + indicators.GetIGD(population));
				logger.Info("Spread     : " + indicators.GetSpread(population));
				logger.Info("Epsilon    : " + indicators.GetEpsilon(population));

				int evaluations = (int)algorithm.GetOutputParameter("evaluations");
				logger.Info("Speed      : " + evaluations + " evaluations");
			}
		}
	}
}

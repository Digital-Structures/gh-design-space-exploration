using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.Problems;
using JMetalCSharp.Problems.Kursawe;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalRunners
{
	public class GDE3
	{

		/// <summary>
		///Usage: three choices
		///     - GDE3
		///     - GDE3 problemName
		///     - GDE3 problemName paretoFrontFile
		/// </summary>
		/// <param name="args">Command line arguments.</param>
		public static void Main(string[] args)
		{
			Problem problem; // The problem to solve
			Algorithm algorithm; // The algorithm to use
			Operator selection;
			Operator crossover;

			Dictionary<string, object> parameters; // Operator parameters

			QualityIndicator indicators; // Object to get quality indicators

			// Logger object and file to store log messages
			var logger = Logger.Log;

			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "GDE3.log";
			fileAppender.ActivateOptions();

			indicators = null;
			if (args.Length == 1)
			{
				object[] param = { "Real" };
				problem = ProblemFactory.GetProblem(args[0], param);
			}
			else if (args.Length == 2)
			{
				object[] param = { "Real" };
				problem = ProblemFactory.GetProblem(args[0], param);
				indicators = new QualityIndicator(problem, args[1]);
			}
			else
			{ // Default problem
				problem = new Kursawe("Real", 3);
				//problem = new Water("Real");
				//problem = new ZDT1("ArrayReal", 100);
				//problem = new ConstrEx("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
			}

			algorithm = new JMetalCSharp.Metaheuristics.GDE3.GDE3(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("populationSize", 100);
			algorithm.SetInputParameter("maxIterations", 250);

			// Crossover operator 
			parameters = new Dictionary<string, object>();
			parameters.Add("CR", 0.5);
			parameters.Add("F", 0.5);
			crossover = CrossoverFactory.GetCrossoverOperator("DifferentialEvolutionCrossover", parameters);

			// Add the operators to the algorithm
			parameters = null;
			selection = SelectionFactory.GetSelectionOperator("DifferentialEvolutionSelection", parameters);

			algorithm.AddOperator("crossover", crossover);
			algorithm.AddOperator("selection", selection);

			// Execute the Algorithm 
			long initTime = Environment.TickCount;
			SolutionSet population = algorithm.Execute();
			long estimatedTime = Environment.TickCount - initTime;

			// Result messages 
			logger.Info("Total execution time: " + estimatedTime + "ms");
			logger.Info("Objectives values have been writen to file FUN");
			population.PrintObjectivesToFile("FUN");
			logger.Info("Variables values have been writen to file VAR");
			population.PrintVariablesToFile("VAR");

			if (indicators != null)
			{
				logger.Info("Quality indicators");
				logger.Info("Hypervolume: " + indicators.GetHypervolume(population));
				logger.Info("GD         : " + indicators.GetGD(population));
				logger.Info("IGD        : " + indicators.GetIGD(population));
				logger.Info("Spread     : " + indicators.GetSpread(population));
				logger.Info("Epsilon    : " + indicators.GetEpsilon(population));
			}

			Console.WriteLine("Total execution time: " + estimatedTime + "ms");
			Console.ReadLine();
		}
	}
}

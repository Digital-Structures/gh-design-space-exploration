using JMetalCSharp.Core;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.Problems;
using JMetalCSharp.Problems.Kursawe;
using JMetalCSharp.Problems.LZ09;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.OffSpring;
using System;
using System.Collections.Generic;

namespace JMetalRunners.NSGAII
{
	public class NSGAIIAdaptive
	{
		/// <summary>
		/// Usage: three options 
		///     - NSGAIIAdaptive 
		///     - NSGAIIAdaptive problemName 
		///     - NSGAIIAdaptive problemName paretoFrontFile
		/// </summary>
		/// <param name="args">Command line arguments.</param>
		public static void Main(string[] args)
		{
			Problem problem; // The problem to solve
			Algorithm algorithm; // The algorithm to use
			Operator selection; // Selection operator

			Dictionary<string, object> parameters; // Operator parameters

			QualityIndicator indicators; // Object to get quality indicators

			// Logger object and file to store log messages
			var logger = Logger.Log;

			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "NSGAIIAdaptive.log";
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
				//problem = new Kursawe("BinaryReal", 3);
				//problem = new Water("Real");
				//problem = new ZDT1("ArrayReal", 100);
				//problem = new ConstrEx("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
			}
			problem = new LZ09_F3("Real");
			algorithm = new JMetalCSharp.Metaheuristics.NSGAII.NSGAIIAdaptive(problem);
			//algorithm = new ssNSGAIIAdaptive(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("populationSize", 100);
			algorithm.SetInputParameter("maxEvaluations", 150000);

			// Selection Operator 
			parameters = null;
			selection = SelectionFactory.GetSelectionOperator("BinaryTournament2", parameters);

			// Add the operators to the algorithm
			algorithm.AddOperator("selection", selection);

			// Add the indicator object to the algorithm
			algorithm.SetInputParameter("indicators", indicators);

			Offspring[] getOffspring = new Offspring[3];
			double CR, F;
			CR = 1.0;
			F = 0.5;
			getOffspring[0] = new DifferentialEvolutionOffspring(CR, F);

			getOffspring[1] = new SBXCrossoverOffspring(1.0, 20);
			//getOffspring[1] = new BLXAlphaCrossoverOffspring(1.0, 0.5); 

			getOffspring[2] = new PolynomialMutationOffspring(1.0 / problem.NumberOfVariables, 20);
			//getOffspring[2] = new NonUniformMutationOffspring(1.0/problem.getNumberOfVariables(), 0.5, 150000);

			algorithm.SetInputParameter("offspringsCreators", getOffspring);

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

			Console.WriteLine("Time: " + estimatedTime);
			Console.ReadLine();
			if (indicators != null)
			{
				logger.Info("Quality indicators");
				logger.Info("Hypervolume: " + indicators.GetHypervolume(population));
				logger.Info("GD         : " + indicators.GetGD(population));
				logger.Info("IGD        : " + indicators.GetIGD(population));
				logger.Info("Spread     : " + indicators.GetSpread(population));
				logger.Info("Epsilon    : " + indicators.GetEpsilon(population));
			}
		}
	}
}

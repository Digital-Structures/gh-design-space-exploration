using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.Problems;
using JMetalCSharp.Problems.ZDT;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalRunners.NSGAII
{
	/// <summary>
	/// Class to configure and execute the NSGA-II algorithm.
	/// Besides the classic NSGA-II, a steady-state version (ssNSGAII) is also
	/// included (See: J.J. Durillo, A.J. Nebro, F. Luna and E. Alba "On the Effect
	/// of the Steady-State Selection Scheme in Multi-Objective Genetic Algorithms"
	/// 5th International Conference, EMO 2009, pp: 183-197. April 2009)
	/// </summary>
	public class NSGAII
	{

		/// <summary>
		/// Usage: three options 
		///     - NSGAII 
		///     - NSGAII problemName 
		///     - NSGAII problemName paretoFrontFile
		/// </summary>
		/// <param name="args"></param>
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
			fileAppender.File = "NSGAII.log";
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
				//problem = new Kursawe("Real", 3);
				//problem = new Kursawe("BinaryReal", 3);
				//problem = new Water("Real");
				problem = new ZDT3("ArrayReal", 30);
				//problem = new ConstrEx("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
			}

			algorithm = new JMetalCSharp.Metaheuristics.NSGAII.NSGAII(problem);
			//algorithm = new ssNSGAII(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("populationSize", 100);
			algorithm.SetInputParameter("maxEvaluations", 25000);

			// Mutation and Crossover for Real codification 
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 0.9);
			parameters.Add("distributionIndex", 20.0);
			crossover = CrossoverFactory.GetCrossoverOperator("SBXCrossover", parameters);

			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 1.0 / problem.NumberOfVariables);
			parameters.Add("distributionIndex", 20.0);
			mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

			// Selection Operator 
			parameters = null;
			selection = SelectionFactory.GetSelectionOperator("BinaryTournament2", parameters);

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

				int evaluations = (int)algorithm.GetOutputParameter("evaluations");
				logger.Info("Speed      : " + evaluations + " evaluations");
			}
		}
	}
}

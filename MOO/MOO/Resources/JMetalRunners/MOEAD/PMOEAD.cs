using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Problems;
using JMetalCSharp.Problems.Kursawe;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalRunners
{
	/// <summary>
	/// This class executes a parallel version of the MOEAD algorithm described in:
	/// A.J. Nebro, J.J. Durillo, 
	/// "A Study of the parallelization of the multi-objective metaheuristic 
	/// MOEA/D"
	/// LION 4, Venice, January 2010.
	/// </summary>
	public class PMOEAD
	{
		/// <summary>
		/// Usage: three options
		///      - PMOEAD
		///      - PMOEAD problemName
		///      - PMOEAD problemName ParetoFrontFile
		///      - PMOEAD problemName numberOfThreads dataDirectory
		/// </summary>
		/// <param name="args">Command line arguments. The first (optional) argument specifies the problem to solve.</param>
		public static void Main(string[] args)
		{
			Problem problem;         // The problem to solve
			Algorithm algorithm;         // The algorithm to use
			Operator crossover;         // Crossover operator
			Operator mutation;         // Mutation operator

			QualityIndicator indicators; // Object to get quality indicators

			Dictionary<string, object> parameters; // Operator parameters

			int numberOfThreads = 4;
			string dataDirectory = "";

			// Logger object and file to store log messages
			var logger = Logger.Log;

			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "PMOEAD.log";
			fileAppender.ActivateOptions();

			indicators = null;
			if (args.Length == 1)
			{ // args[0] = problem name
				object[] paramsList = { "Real" };
				problem = ProblemFactory.GetProblem(args[0], paramsList);
			}
			else if (args.Length == 2)
			{ // args[0] = problem name, [1] = pareto front file
				object[] paramsList = { "Real" };
				problem = ProblemFactory.GetProblem(args[0], paramsList);
				indicators = new QualityIndicator(problem, args[1]);
			}
			else if (args.Length == 3)
			{ // args[0] = problem name, [1] = threads, [2] = data directory
				object[] paramsList = { "Real" };
				problem = ProblemFactory.GetProblem(args[0], paramsList);
				numberOfThreads = int.Parse(args[1]);
				dataDirectory = args[2];
			}
			else
			{ // Problem + number of threads + data directory
				problem = new Kursawe("Real", 3);
				//problem = new Kursawe("BinaryReal", 3);
				//problem = new Water("Real");
				//problem = new ZDT1("ArrayReal", 100);
				//problem = new ConstrEx("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
			}

			algorithm = new JMetalCSharp.Metaheuristics.MOEAD.PMOEAD(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("populationSize", 300);
			algorithm.SetInputParameter("maxEvaluations", 150000);
			algorithm.SetInputParameter("numberOfThreads", numberOfThreads);

			// Directory with the files containing the weight vectors used in 
			// Q. Zhang,  W. Liu,  and H Li, The Performance of a New Version of MOEA/D 
			// on CEC09 Unconstrained MOP Test Instances Working Report CES-491, School 
			// of CS & EE, University of Essex, 02/2009.
			// http://dces.essex.ac.uk/staff/qzhang/MOEAcompetition/CEC09final/code/ZhangMOEADcode/moead0305.rar
			algorithm.SetInputParameter("dataDirectory", "Data/Parameters/Weight");

			algorithm.SetInputParameter("T", 20);
			algorithm.SetInputParameter("delta", 0.9);
			algorithm.SetInputParameter("nr", 2);

			// Crossover operator 
			parameters = new Dictionary<string, object>();
			parameters.Add("CR", 1.0);
			parameters.Add("F", 0.5);
			crossover = CrossoverFactory.GetCrossoverOperator("DifferentialEvolutionCrossover", parameters);

			// Mutation operator
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 1.0 / problem.NumberOfVariables);
			parameters.Add("distributionIndex", 20.0);
			mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

			algorithm.AddOperator("crossover", crossover);
			algorithm.AddOperator("mutation", mutation);

			// Execute the Algorithm
			long initTime = Environment.TickCount;
			SolutionSet population = algorithm.Execute();
			long estimatedTime = Environment.TickCount - initTime;

			// Result messages 
			logger.Info("Total execution time: " + estimatedTime + " ms");
			logger.Info("Objectives values have been writen to file FUN");
			population.PrintObjectivesToFile("FUN");
			logger.Info("Variables values have been writen to file VAR");
			population.PrintVariablesToFile("VAR");
			Console.ReadLine();

			if (indicators != null)
			{
				logger.Info("Quality indicators");
				logger.Info("Hypervolume: " + indicators.GetHypervolume(population));
				logger.Info("GD         : " + indicators.GetGD(population));
				logger.Info("IGD        : " + indicators.GetIGD(population));
				logger.Info("Spread     : " + indicators.GetSpread(population));
			}
		}
	}
}

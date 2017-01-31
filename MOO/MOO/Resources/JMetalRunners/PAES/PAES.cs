using JMetalCSharp.Core;
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
	/// Class for configuring and running the PAES algorithm
	/// </summary>
	public class PAES
	{
		/// <summary>
		/// Usage: three options
		///    - PAES
		///    - PAES problemName
		///    - PAES problemName ParetoFrontFile
		/// </summary>
		/// <param name="args">Command line arguments. The first (optional) argument specifies 
		/// the problem to solve.</param>
		public static void Main(string[] args)
		{
			Problem problem;// The problem to solve
			Algorithm algorithm;// The algorithm to use
			Operator mutation;// Mutation operator

			QualityIndicator indicators; // Object to get quality indicators

			Dictionary<string, object> parameters; // Operator parameters

			// Logger object and file to store log messages
			var logger = Logger.Log;

			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "PAES.log";
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
				problem = new Kursawe("ArrayReal", 3);
				//problem = new Fonseca("Real"); 
				//problem = new Kursawe("BinaryReal",3);
				//problem = new Water("Real");
				//problem = new ZDT4("Real", 1000);
				//problem = new WFG1("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
			}

			algorithm = new JMetalCSharp.Metaheuristics.PAES.PAES(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("archiveSize", 100);
			algorithm.SetInputParameter("biSections", 5);
			algorithm.SetInputParameter("maxEvaluations", 25000);

			// Mutation (Real variables)
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 1.0 / problem.NumberOfVariables);
			parameters.Add("distributionIndex", 20.0);
			mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

			// Add the operators to the algorithm
			algorithm.AddOperator("mutation", mutation);

			// Execute the Algorithm 
			long initTime = Environment.TickCount;
			SolutionSet population = algorithm.Execute();
			long estimatedTime = Environment.TickCount - initTime;

			// Result messages 
			// STEP 8. Print the results
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

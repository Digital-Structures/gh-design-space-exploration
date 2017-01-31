using JMetalCSharp.Core;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Problems;
using JMetalCSharp.Problems.ZDT;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalRunners
{
	/// <summary>
	/// This class executes the SMPSO algorithm described in:
	/// A.J. Nebro, J.J. Durillo, J. Garcia-Nieto, C.A. Coello Coello, F. Luna and E. Alba
	/// "SMPSO: A New PSO-based Metaheuristic for Multi-objective Optimization". 
	/// IEEE Symposium on Computational Intelligence in Multicriteria Decision-Making 
	/// (MCDM 2009), pp: 66-73. March 2009
	/// </summary>
	public class SMPSO
	{
		/// <summary>
		/// Usage: three options
		///   - SMPSO
		///   - SMPSO problemName
		///   - SMPSO problemName ParetoFrontFile
		/// </summary>
		/// <param name="args">Command line arguments. The first (optional) argument specifies the problem to solve.</param>
		public static void Main(string[] args)
		{
			Problem problem;  // The problem to solve
			Algorithm algorithm;  // The algorithm to use
			Mutation mutation;  // "Turbulence" operator

			QualityIndicator indicators; // Object to get quality indicators

			Dictionary<string, object> parameters; // Operator parameters

			// Logger object and file to store log messages
			var logger = Logger.Log;

			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "SMPSO.log";
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
				//problem = new Water("Real");
				//problem = new ZDT1("ArrayReal", 1000);
				//problem = new ZDT4("BinaryReal");
				//problem = new WFG1("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
				//problem = new DTLZ1("Real",7,5);
				problem = new ZDT4("Real");
			}

			algorithm = new JMetalCSharp.Metaheuristics.SMPSO.SMPSO(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("swarmSize", 100);
			algorithm.SetInputParameter("archiveSize", 100);
			algorithm.SetInputParameter("maxIterations", 250);

			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 1.0 / problem.NumberOfVariables);
			parameters.Add("distributionIndex", 20.0);
			mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

			algorithm.AddOperator("mutation", mutation);

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

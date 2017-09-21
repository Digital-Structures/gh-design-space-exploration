using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Problems;
using JMetalCSharp.Problems.WFG;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalRunners
{
	/// <summary>
	/// This class executes the algorithm described in:
	///   H. Li and Q. Zhang, 
	///   "Multiobjective Optimization Problems with Complicated Pareto Sets,  MOEA/D 
	///   and NSGA-II". IEEE Trans on Evolutionary Computation, vol. 12,  no 2,  
	///   pp 284-302, April/2009.
	///   Usage: three options
	///      - MOEAD
	///      - MOEAD problemName
	///      - MOEAD problemName ParetoFrontFile
	/// </summary>
	public class MOEAD
	{
		/// <summary>
		/// Command line arguments. The first (optional) argument specifies the problem to solve.
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			Problem problem; // The problem to solve
			Algorithm algorithm; // The algorithm to use
			Operator crossover; // Crossover operator
			Operator mutation; // Mutation operator

			QualityIndicator indicators = null; // Object to get quality indicators

			Dictionary<string, object> parameters; // Operator parameters

			// Logger object and file to store log messages
			var logger = Logger.Log;

			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "MOEAD.log";
			fileAppender.ActivateOptions();

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
				//problem = new Fonseca("Real");
				//problem = new Schaffer("Real");
				problem = new WFG9("Real");
				//problem = new Kursawe("BinaryReal", 3);
				//problem = new Water("Real");
				//problem = new ZDT1("ArrayReal", 100);
				//problem = new ConstrEx("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
			}

			algorithm = new JMetalCSharp.Metaheuristics.MOEAD.MOEAD(problem);
			//algorithm = new MOEAD_DRA(problem);

			// Algorithm parameters
			algorithm.SetInputParameter("populationSize", 300);
			algorithm.SetInputParameter("maxEvaluations", 150000);

			// Directory with the files containing the weight vectors used in 
			// Q. Zhang,  W. Liu,  and H Li, The Performance of a New Version of MOEA/D 
			// on CEC09 Unconstrained MOP Test Instances Working Report CES-491, School 
			// of CS & EE, University of Essex, 02/2009.
			// http://dces.essex.ac.uk/staff/qzhang/MOEAcompetition/CEC09final/code/ZhangMOEADcode/moead0305.rar
			algorithm.SetInputParameter("dataDirectory", "Data/Parameters/Weight");

			algorithm.SetInputParameter("finalSize", 300); // used by MOEAD_DRA

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
			logger.Info("Total execution time: " + estimatedTime + "ms");
			logger.Info("Objectives values have been writen to file FUN");
			population.PrintObjectivesToFile("FUN");
			logger.Info("Variables values have been writen to file VAR");
			population.PrintVariablesToFile("VAR");

			if (indicators != null)
			{
				logger.Info("Quality indicators");
				logger.Info("Hypervolume: " + indicators.GetHypervolume(population));
				logger.Info("EPSILON    : " + indicators.GetEpsilon(population));
				logger.Info("GD         : " + indicators.GetGD(population));
				logger.Info("IGD        : " + indicators.GetIGD(population));
				logger.Info("Spread     : " + indicators.GetSpread(population));
			}

			Console.WriteLine("Total execution time: " + estimatedTime + "ms");
			Console.ReadLine();
		}

	}
}

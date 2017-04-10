using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.LocalSearch;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Problems;
using JMetalCSharp.Problems.Kursawe;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;

namespace JMetalRunners
{
	public class AbYSS
	{
		/// <summary>
		///Usage: three choices
		///     - AbYSS
		///     - AbYSS problemName
		///     - AbYSS problemName paretoFrontFile
		/// </summary>
		/// <param name="args">Command line arguments.</param>
		public static void Main(string[] args)
		{
			Problem problem; // The problem to solve
			Algorithm algorithm; // The algorithm to use
			Operator crossover; // Crossover operator
			Operator mutation; // Mutation operator
			Operator improvement; // Operator for improvement

			Dictionary<string, object> parameters; // Operator parameters

			QualityIndicator indicators; // Object to get quality indicators

			// Logger object and file to store log messages
			var logger = Logger.Log;
			var appenders = logger.Logger.Repository.GetAppenders();
			var fileAppender = appenders[0] as log4net.Appender.FileAppender;
			fileAppender.File = "AbYSS.log";
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
				//problem = new ZDT4("ArrayReal", 10);
				//problem = new ConstrEx("Real");
				//problem = new DTLZ1("Real");
				//problem = new OKA2("Real") ;
			}

			// STEP 2. Select the algorithm (AbYSS)
			algorithm = new JMetalCSharp.Metaheuristics.AbYSS.AbYSS(problem);

			// STEP 3. Set the input parameters required by the metaheuristic
			algorithm.SetInputParameter("populationSize", 20);
			algorithm.SetInputParameter("refSet1Size", 10);
			algorithm.SetInputParameter("refSet2Size", 10);
			algorithm.SetInputParameter("archiveSize", 100);
			algorithm.SetInputParameter("maxEvaluations", 25000);

			// STEP 4. Specify and configure the crossover operator, used in the
			//         solution combination method of the scatter search
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 0.9);
			parameters.Add("distributionIndex", 20.0);
			crossover = CrossoverFactory.GetCrossoverOperator("SBXCrossover", parameters);

			// STEP 5. Specify and configure the improvement method. We use by default
			//         a polynomial mutation in this method.
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", 1.0 / problem.NumberOfVariables);
			parameters.Add("distributionIndex", 20.0);
			mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

			parameters = new Dictionary<string, object>();
			parameters.Add("improvementRounds", 1);
			parameters.Add("problem", problem);
			parameters.Add("mutation", mutation);
			improvement = new MutationLocalSearch(parameters);

			// STEP 6. Add the operators to the algorithm
			algorithm.AddOperator("crossover", crossover);
			algorithm.AddOperator("improvement", improvement);

			long initTime = Environment.TickCount;

			// STEP 7. Run the algorithm 
			SolutionSet population = algorithm.Execute();
			long estimatedTime = Environment.TickCount - initTime;

			// STEP 8. Print the results
			logger.Info("Total execution time: " + estimatedTime + "ms");
			logger.Info("Variables values have been writen to file VAR");
			population.PrintVariablesToFile("VAR");
			logger.Info("Objectives values have been writen to file FUN");
			population.PrintObjectivesToFile("FUN");

			Console.WriteLine("Total execution time: " + estimatedTime + "ms");
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

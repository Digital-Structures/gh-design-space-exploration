using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System;

namespace MOO
{
    /// <summary>
    /// Implementation of NSGA-II. This implementation of NSGA-II makes use of a
    /// QualityIndicator object to obtained the convergence speed of the algorithm.
    /// This version is used in the paper: A.J. Nebro, J.J. Durillo, C.A. Coello
    /// Coello, F. Luna, E. Alba "A Study of Convergence Speed in Multi-Objective
    /// Metaheuristics." To be presented in: PPSN'08. Dortmund. September 2008.
    /// </summary>
    public class NSGAII : Algorithm
    {
        MOO comp;
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problem">Problem to solve</param>
        public NSGAII(Problem problem, MOO comp)
            : base(problem)
        {
            this.comp = comp;
        }

        #endregion

        #region Public Override

        /// <summary>
        /// Runs the NSGA-II algorithm.
        /// </summary>
        /// <returns>a <code>SolutionSet</code> that is a set of non dominated solutions as a result of the algorithm execution</returns>
        public override SolutionSet Execute()
        {
            // !!!! NEEDED PARAMETES !!! start
            //J* Parameters
            int populationSize = -1; // J* store population size
            int maxEvaluations = -1; // J* store number of max Evaluations
            int evaluations;        // J* number of current evaluations

            // J* Objects needed to illustrate the use of quality indicators inside the algorithms
            QualityIndicator indicators = null; // QualityIndicator object
            int requiredEvaluations; // Use in the example of use of the
            // indicators object (see below)

            // J* populations needed to implement NSGA-II
            SolutionSet population; //J* Current population
            SolutionSet offspringPopulation; //J* offspring population
            SolutionSet union;  //J* population resultant from current and offpring population

            //J* Genetic Operators
            Operator mutationOperator;
            Operator crossoverOperator;
            Operator selectionOperator;

            //J* Used to evaluate crowding distance
            Distance distance = new Distance();

            //J* !!!! NEEDED PARAMETES !!! end

            //J* !!! INITIALIZING PARAMETERS - start !!!

            //Read the parameters
            JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvals", ref maxEvaluations);  //J* required
            JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);  //J* required
            JMetalCSharp.Utils.Utils.GetIndicatorsFromParameters(this.InputParameters, "indicators", ref indicators);       //J* optional

            //Initialize the variables
            population = new SolutionSet(populationSize);
            evaluations = 0;

            requiredEvaluations = 0;

            //Read the operators
            mutationOperator = Operators["mutation"];
            crossoverOperator = Operators["crossover"];
            selectionOperator = Operators["selection"];

            //J* !!! INITIALIZING PARAMETERS - end !!!


            //J* !!! Creating first population !!!
            Random random = new Random(2);
            JMetalRandom.SetRandom(random);
            comp.LogAddMessage("Random seed = " + 2);

            // Create the initial solutionSet
            Solution newSolution;
            for (int i = 0; i < populationSize; i++)
            {
                newSolution = new Solution(Problem);
                Problem.Evaluate(newSolution);
                Problem.EvaluateConstraints(newSolution);
                evaluations++;
                population.Add(newSolution);
            }

            // Generations 
            while (evaluations < maxEvaluations)
            {

                // Create the offSpring solutionSet      
                offspringPopulation = new SolutionSet(populationSize);
                Solution[] parents = new Solution[2];
                for (int i = 0; i < (populationSize / 2); i++)
                {
                    if (evaluations < maxEvaluations)
                    {
                        //obtain parents
                        parents[0] = (Solution)selectionOperator.Execute(population);
                        parents[1] = (Solution)selectionOperator.Execute(population);
                        Solution[] offSpring = (Solution[])crossoverOperator.Execute(parents);
                        mutationOperator.Execute(offSpring[0]);
                        mutationOperator.Execute(offSpring[1]);
                        Problem.Evaluate(offSpring[0]);
                        Problem.EvaluateConstraints(offSpring[0]);
                        Problem.Evaluate(offSpring[1]);
                        Problem.EvaluateConstraints(offSpring[1]);
                        offspringPopulation.Add(offSpring[0]);
                        offspringPopulation.Add(offSpring[1]);
                        evaluations += 2;
                    }
                }

                // Create the solutionSet union of solutionSet and offSpring
                union = ((SolutionSet)population).Union(offspringPopulation);

                // Ranking the union
                Ranking ranking = new Ranking(union);

                int remain = populationSize;
                int index = 0;
                SolutionSet front = null;
                population.Clear();

                // Obtain the next front
                front = ranking.GetSubfront(index);

                while ((remain > 0) && (remain >= front.Size()))
                {
                    //Assign crowding distance to individuals
                    distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
                    //Add the individuals of this front
                    for (int k = 0; k < front.Size(); k++)
                    {
                        population.Add(front.Get(k));
                    }

                    //Decrement remain
                    remain = remain - front.Size();

                    //Obtain the next front
                    index++;
                    if (remain > 0)
                    {
                        front = ranking.GetSubfront(index);
                    }
                }

                // Remain is less than front(index).size, insert only the best one
                if (remain > 0)
                {  // front contains individuals to insert                        
                    distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
                    front.Sort(new CrowdingComparator());
                    for (int k = 0; k < remain; k++)
                    {
                        population.Add(front.Get(k));
                    }

                    remain = 0;
                }

                // This piece of code shows how to use the indicator object into the code
                // of NSGA-II. In particular, it finds the number of evaluations required
                // by the algorithm to obtain a Pareto front with a hypervolume higher
                // than the hypervolume of the true Pareto front.
                if ((indicators != null) && (requiredEvaluations == 0))
                {
                    double HV = indicators.GetHypervolume(population);
                    if (HV >= (0.98 * indicators.TrueParetoFrontHypervolume))
                    {
                        requiredEvaluations = evaluations;
                    }
                }
            }



            // Return as output parameter the required evaluations
            SetOutputParameter("evaluations", requiredEvaluations);

            comp.LogAddMessage("Evaluations = " + evaluations);
            // Return the first non-dominated front
            Ranking rank = new Ranking(population);

            Result = rank.GetSubfront(0);

            return Result;
        }

        #endregion
    }
}

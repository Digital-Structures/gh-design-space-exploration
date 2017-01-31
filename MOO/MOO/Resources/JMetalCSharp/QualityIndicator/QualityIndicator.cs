using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator.Utils;

namespace JMetalCSharp.QualityIndicator
{
	public class QualityIndicator
	{
		#region Private Attributes

		private SolutionSet trueParetoFront;

		private Problem problem;
		private MetricsUtil Utils { get; set; }
		#endregion

		#region Properties

		/// <summary>
		/// Hypervolume of the true Pareto front
		/// </summary>
		public double TrueParetoFrontHypervolume { get; private set; }

		#endregion

		#region Constructos

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">The problem</param>
		/// <param name="paretoFrontFile">Pareto front file</param>
		public QualityIndicator(Problem problem, string paretoFrontFile)
		{
			this.problem = problem;
			Utils = new MetricsUtil();
			trueParetoFront = Utils.ReadNonDominatedSolutionSet(paretoFrontFile);
			TrueParetoFrontHypervolume = new HyperVolume().Hypervolume(
					trueParetoFront.WriteObjectivesToMatrix(),
					trueParetoFront.WriteObjectivesToMatrix(),
					problem.NumberOfObjectives);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns the hypervolume of solution set
		/// </summary>
		/// <param name="solutionSet">Solution set</param>
		/// <returns>The value of the hypervolume indicator</returns>
		public double GetHypervolume(SolutionSet solutionSet)
		{
			return new HyperVolume().Hypervolume(solutionSet.WriteObjectivesToMatrix(),
					trueParetoFront.WriteObjectivesToMatrix(),
					problem.NumberOfObjectives);
		}

		/// <summary>
		/// Returns the inverted generational distance of solution set
		/// </summary>
		/// <param name="solutionSet">Solution set</param>
		/// <returns>The value of the hypervolume indicator</returns>
		public double GetIGD(SolutionSet solutionSet)
		{
			return new InvertedGenerationalDistance().CalculateInvertedGenerationalDistance(
				solutionSet.WriteObjectivesToMatrix(),
				trueParetoFront.WriteObjectivesToMatrix(),
				problem.NumberOfObjectives);
		}

		/// <summary>
		/// Returns the generational distance of solution set
		/// </summary>
		/// <param name="solutionSet">Solution set</param>
		/// <returns>The value of the hypervolume indicator</returns>
		public double GetGD(SolutionSet solutionSet)
		{
			return new GenerationalDistance().CalculateGenerationalDistance(
				solutionSet.WriteObjectivesToMatrix(),
				trueParetoFront.WriteObjectivesToMatrix(),
				problem.NumberOfObjectives);
		}

		/// <summary>
		/// Returns the spread of solution set
		/// </summary>
		/// <param name="solutionSet">Solution set</param>
		/// <returns>The value of the hypervolume indicator</returns>
		public double GetSpread(SolutionSet solutionSet)
		{
			return new Spread().CalculateSpread(solutionSet.WriteObjectivesToMatrix(),
				trueParetoFront.WriteObjectivesToMatrix(),
				problem.NumberOfObjectives);
		}


		/// <summary>
		/// Returns the epsilon indicator of solution set
		/// </summary>
		/// <param name="solutionSet">Solution set</param>
		/// <returns>The value of the hypervolume indicator</returns>
		public double GetEpsilon(SolutionSet solutionSet)
		{
			return new Epsilon().CalcualteEpsilon(solutionSet.WriteObjectivesToMatrix(),
					trueParetoFront.WriteObjectivesToMatrix(),
					problem.NumberOfObjectives);
		}

		#endregion
	}
}

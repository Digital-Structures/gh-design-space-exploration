using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Selection;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.OffSpring
{
	public class DifferentialEvolutionOffspring : Offspring
	{
		#region Private Attributes

		private double CR;
		private double F;

		private Operator crossover;
		private Operator selection;

		#endregion

		#region Constructors

		private DifferentialEvolutionOffspring()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="CR"></param>
		/// <param name="F"></param>
		public DifferentialEvolutionOffspring(double CR, double F)
		{
			Dictionary<string, object> parameters = null;
			this.CR = CR;
			this.F = F;
			try
			{
				// Crossover operator
				parameters = new Dictionary<string, object>();
				parameters.Add("CR", CR);
				parameters.Add("F", F);
				crossover = new DifferentialEvolutionCrossover(parameters);

				// Selecion operator
				parameters = null;
				selection = SelectionFactory.GetSelectionOperator("DifferentialEvolutionSelection", parameters);
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()", ex);
				Console.Error.WriteLine("Error in: " + this.GetType().FullName + "\r\n" + ex.StackTrace);
			}
			Id = "DE";
		}

		#endregion

		#region Public Overrides

		public override Solution GetOffspring(SolutionSet solutionSet, int index)
		{
			Solution[] parents = new Solution[3];
			Solution offSpring = null;

			try
			{
				int r1, r2;
				do
				{
					r1 = JMetalRandom.Next(0, solutionSet.Size() - 1);
				} while (r1 == index);
				do
				{
					r2 = JMetalRandom.Next(0, solutionSet.Size() - 1);
				} while (r2 == index || r2 == r1);

				parents[0] = solutionSet.Get(r1);
				parents[1] = solutionSet.Get(r2);
				parents[2] = solutionSet.Get(index);

				offSpring = (Solution)crossover.Execute(new object[] { solutionSet.Get(index), parents });
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".GetOffSpring()", ex);
				Console.Error.WriteLine("Exception in " + this.GetType().FullName + ".GetOffSpring()");
			}

			//Create a new solution, using DE
			return offSpring;
		}


		public override Solution GetOffspring(Solution[] parentSolutions, Solution currentSolution)
		{
			Solution[] parents = new Solution[3];
			Solution offspring = null;

			try
			{
				parents[0] = parentSolutions[0];
				parents[1] = parentSolutions[1];
				parents[2] = currentSolution;

				offspring = (Solution)crossover.Execute(new object[] { currentSolution, parents });
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".GetOffSpring()", ex);
				Console.Error.WriteLine("Exception in " + this.GetType().FullName + ".GetOffSpring()", ex);
			}

			//Create a new solution, using DE
			return offspring;
		}

		public override string Configuration()
		{
			string result = "-----\n";
			result += "Operator: " + Id + "\n";
			result += "CR: " + CR + "\n";
			result += "F: " + F;

			return result;
		}

		#endregion
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.OffSpring
{
	public class PolynomialMutationOffspring : Offspring
	{
		private Operator mutation;
		private Operator selection;

		private double mutationProbability;
		private double distributionIndex;

		public PolynomialMutationOffspring(double mutationProbability, double distributionIndexForMutation)
		{
			Dictionary<string, object> parameters; // Operator parameters
			parameters = new Dictionary<string, object>();
			parameters.Add("probability", this.mutationProbability = mutationProbability);
			parameters.Add("distributionIndex", this.distributionIndex = distributionIndexForMutation);
			mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

			selection = SelectionFactory.GetSelectionOperator("BinaryTournament", null);
			Id = "PolynomialMutation";
		}

		public Solution GetOffspring(Solution solution)
		{
			Solution res = new Solution(solution);
			try
			{
				mutation.Execute(res);
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".GetOffspring()", ex);
			}
			return res;
		}

		public override string Configuration()
		{
			string result = "-----\n";
			result += "Operator: " + Id + "\n";
			result += "Probability: " + mutationProbability + "\n";
			result += "DistributionIndex: " + distributionIndex;

			return result;
		}
	}
}

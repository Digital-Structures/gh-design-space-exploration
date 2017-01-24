using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JMetalCSharp.Experiments.Output
{
	public class FriedmanTables
	{
		private string basePath;

		private IDictionary<string, IDictionary<string, IDictionary<string, List<double>>>> indicators;

		private Experiment experiment;

		public FriedmanTables(Experiment experiment, string basePath, IDictionary<string, IDictionary<string, IDictionary<string, List<double>>>> indicators)
		{
			this.basePath = basePath;
			this.indicators = indicators;
			this.experiment = experiment;
		}

		public void generate()
		{
			foreach (var indicator in this.indicators)
			{
				ExecuteTest(indicator);
			}

		}

		private void ExecuteTest(KeyValuePair<string, IDictionary<string, IDictionary<string, List<double>>>> indicator)
		{
			string indicatorName = indicator.Key;

			Dictionary<string, Dictionary<string, double>> values = new Dictionary<string, Dictionary<string, double>>();
			string directory = Path.Combine(this.basePath, "FriedmanTest");
			string outFile = Path.Combine(directory, indicatorName + ".tex");

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			StringBuilder output = new StringBuilder();
			output.Append("\\documentclass{article}\n" +
					"\\usepackage{graphicx}\n" +
					"\\title{Results}\n" +
					"\\author{}\n" +
					"\\date{\\today}\n" +
					"\\begin{document}\n" +
					"\\oddsidemargin 0in \\topmargin 0in" +
					"\\maketitle\n" +
					"\\section{Tables of Friedman Tests}");

			var problems = indicator.Value;
			foreach (var problem in problems)
			{
				var algorithmsDictionary = new Dictionary<string, double>();
				var algorithms = problem.Value;
				values.Add(problem.Key, algorithmsDictionary);

				foreach (var algorithm in algorithms)
				{
					if (algorithm.Value.Count > 0)
					{
						algorithmsDictionary.Add(algorithm.Key, algorithm.Value.Sum() / algorithm.Value.Count);
					}
					else
					{
						algorithmsDictionary.Add(algorithm.Key, 0);
					}
				}
			}

			var order = new Pair[problems.Count][];
			var firstAlgorithm = problems.Values.First();

			for (int i = 0; i < order.Length; i++)
			{
				order[i] = new Pair[firstAlgorithm.Count];
			}

			int problemCount = 0;
			int algorithmCount = 0;
			foreach (var p in values.Values)
			{
				algorithmCount = 0;
				foreach (var a in p.Values)
				{
					order[problemCount][algorithmCount] = new Pair(algorithmCount, a);
					algorithmCount++;
				}
				Array.Sort<Pair>(order[problemCount]);
				problemCount++;
			}

			//building of the rankings table per algorithms and data sets

			var rank = new Pair[problems.Count][];
			for (int i = 0; i < order.Length; i++)
			{
				rank[i] = new Pair[firstAlgorithm.Count];
			}

			var position = 0;
			bool found = false;
			for (int i = 0; i < problems.Count; i++)
			{
				for (int j = 0; j < firstAlgorithm.Count; j++)
				{
					found = false;
					for (var k = 0; k < firstAlgorithm.Count && !found; k++)
					{
						if (order[i][k].Index == j)
						{
							found = true;
							position = k + 1;
						}
					}
					rank[i][j] = new Pair(position, order[i][position - 1].Value);
				}
			}

			bool[] check;
			IList<int> notVisitedYet;
			double sum;
			int ig;
			//In the case of having the same performance, the rankings are equal
			for (int i = 0, li = problems.Count; i < li; i++)
			{
				check = new bool[firstAlgorithm.Count];
				notVisitedYet = new List<int>();
				for (int j = 0, lj = firstAlgorithm.Count; j < lj; j++)
				{
					notVisitedYet.Clear();
					sum = rank[i][j].Index;
					check[j] = true;
					ig = 1;
					for (int k = j + 1, lk = firstAlgorithm.Count; k < lk; k++)
					{
						if (rank[i][j].Value == rank[i][k].Value && !check[k])
						{
							sum += rank[i][k].Index;
							ig++;
							notVisitedYet.Add(k);
							check[k] = true;
						}
					}
					sum /= (double)ig;
					rank[i][j].Index = sum;
					for (int k = 0, lk = notVisitedYet.Count; k < lk; k++)
					{
						rank[i][notVisitedYet[k]].Index = sum;
					}
				}
			}

			//compute the average ranking for each algorithm
			var rj = new double[firstAlgorithm.Count];
			for (int i = 0, li = firstAlgorithm.Count; i < li; i++)
			{
				rj[i] = 0;
				for (int j = 0, lj = problems.Count; j < lj; j++)
				{
					rj[i] += rank[j][i].Index / ((double)problems.Count);
				}
			}

			//Print the average ranking per algorithm
			output.Append("\n" + ("\\begin{table}[!htp]\n" +
					"\\centering\n" +
					"\\caption{Average Rankings of the algorithms\n}" +
					"\\begin{tabular}{c|c}\n" +
					"Algorithm&Ranking\\\\\n\\hline"));

			for (int i = 0, li = firstAlgorithm.Count; i < li; i++)
			{
				output.Append("\n" + firstAlgorithm.ElementAt(i).Key + "&" + rj[i] + "\\\\");
			}

			output.Append("\n" +
					"\\end{tabular}\n" +
					"\\end{table}");

			//Compute the Friedman statistic
			var term1 = (12 * (double)problems.Count) / ((double)firstAlgorithm.Count * ((double)firstAlgorithm.Count + 1));
			var term2 = (double)firstAlgorithm.Count * ((double)firstAlgorithm.Count + 1) * ((double)firstAlgorithm.Count + 1) / (4.0);
			double sumSq = 0;
			for (int i = 0, li = firstAlgorithm.Count; i < li; i++)
			{
				sumSq += rj[i] * rj[i];
			}
			var friedman = (sumSq - term2) * term1;

			output.Append("\n" + "\n\nFriedman statistic considering reduction performance (distributed according to chi-square with " + (firstAlgorithm.Count - 1) + " degrees of freedom: " + friedman + ").\n\n");

			output.Append("\n" + "\\end{document}");

			using (StreamWriter writer = new StreamWriter(outFile))
			{
				writer.Write(output.ToString());
				writer.Flush();
			}
		}
	}
}

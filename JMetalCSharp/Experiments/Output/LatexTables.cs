using JMetalCSharp.Experiments.Utils;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace JMetalCSharp.Experiments.Output
{
	public class LatexTables
	{
		private IDictionary<string, IDictionary<string, IDictionary<string, List<double>>>> indicators;
		private string path;
		private Experiment experiment;


		public LatexTables(Experiment experiment, string path, IDictionary<string, IDictionary<string, IDictionary<string, List<double>>>> indicators)
		{
			this.path = path;
			this.indicators = indicators;
			this.experiment = experiment;
		}

		public IDictionary<string, IDictionary<string, IDictionary<string, LatexTableData>>> Generate()
		{
			var values = new Dictionary<string, IDictionary<string, IDictionary<string, LatexTableData>>>();

			foreach (var indicator in indicators)
			{
				var problems = new Dictionary<string, IDictionary<string, LatexTableData>>();
				values.Add(indicator.Key, problems);

				foreach (var problem in indicator.Value)
				{
					var algorithms = new Dictionary<string, LatexTableData>();
					problems.Add(problem.Key, algorithms);

					foreach (var algorithm in problem.Value)
					{
						var value = calculateStatistics(algorithm.Value);
						value.Color = CellColorEnum.White.ToString();
						algorithms.Add(algorithm.Key, value);
					}
				}
			}

			//write latex file
			try
			{
				using (StreamWriter writer = new StreamWriter(path))
				{

					PrintHeaderLatexCommands(writer);
					foreach (var indicator in values)
					{
						PrintMeanStdDev(writer, indicator);
						PrintMedianIQR(writer, indicator);
					}
					PrintEndLatexCommands(writer);
					writer.Flush();
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("LatexTables.Generate", ex);
				Console.WriteLine(ex.StackTrace);
			}

			return values;
		}

		private LatexTableData calculateStatistics(List<double> valueList)
		{
			var result = new LatexTableData();

			if (valueList.Count > 0)
			{
				double sum,
						sqsum,
						min,
						max,
						median,
						mean,
						stdDeviation;

				sqsum = 0.0;
				sum = 0.0;
				min = double.MaxValue;
				max = double.MinValue;
				median = 0;

				for (int i = 0; i < valueList.Count; i++)
				{
					double val = valueList[i];

					sqsum += val * val;
					sum += val;
					if (val < min)
					{
						min = val;
					}
					if (val > max)
					{
						max = val;
					}
				}

				// Mean
				mean = sum / valueList.Count;

				// Standard deviation
				if (sqsum / valueList.Count - mean * mean < 0.0)
				{
					stdDeviation = 0.0;
				}
				else
				{
					stdDeviation = Math.Sqrt(sqsum / valueList.Count - mean * mean);
				}

				// Median
				if (valueList.Count % 2 != 0)
				{
					median = valueList[valueList.Count / 2];
				}
				else
				{
					median = (valueList[valueList.Count / 2 - 1] + valueList[valueList.Count / 2]) / 2.0;
				}

				result.Mean = mean;
				result.Median = Statistics.CalculateMedian(valueList);
				result.IQR = Statistics.CalculateIQR(valueList);
				result.StdDeviation = stdDeviation;
				result.Min = min;
				result.Max = max;
			}
			else
			{
				result.Mean = double.NaN;
				result.Median = double.NaN;
				result.IQR = double.NaN;
				result.StdDeviation = double.NaN;
				result.Min = double.NaN;
				result.Max = double.NaN;
			}

			return result;
		}

		private void PrintMeanStdDev(StreamWriter writer, KeyValuePair<string, IDictionary<string, IDictionary<string, LatexTableData>>> indicator)
		{
			writer.WriteLine("\\");
			writer.WriteLine("\\begin{table}");
			writer.WriteLine("\\caption{" + indicator.Key + ". Mean and standard deviation}");
			writer.WriteLine("\\label{table:mean." + indicator.Key + "}");
			writer.WriteLine("\\centering");
			writer.WriteLine("\\begin{scriptsize}");
			writer.Write("\\begin{tabular}{l");
			var problems = indicator.Value;
			var problem = problems.First();
			var algorithms = problem.Value;


			foreach (var algorithm in problem.Value)
			{
				// calculate the number of columns
				writer.Write("l");
			}
			writer.WriteLine("}");

			writer.Write("\\hline &");


			writer.Write(algorithms.Select(a => a.Key).Aggregate((curr, next) => curr + " & " + next));

			writer.WriteLine("\\\\");

			writer.WriteLine("\\hline");

			string m,
				s;

			// write lines
			foreach (var p in problems)
			{
				var algs = p.Value;
				// find the best value and second best value
				double bestValue;
				double bestValueIQR;
				double secondBestValue;
				double secondBestValueIQR;
				int bestIndex = -1;
				int secondBestIndex = -1;
				int count = 0;
				if (experiment.IndicatorsMinimize[indicator.Key])
				{// minimize by default
					bestValue = double.MaxValue;
					bestValueIQR = double.MaxValue;
					secondBestValue = double.MaxValue;
					secondBestValueIQR = double.MaxValue;

					foreach (var a in algs)
					{
						var mean = a.Value.Mean;
						var stdDev = a.Value.StdDeviation;
						if ((mean < bestValue) || ((mean == bestValue) && (stdDev < bestValueIQR)))
						{
							secondBestIndex = bestIndex;
							secondBestValue = bestValue;
							secondBestValueIQR = bestValueIQR;
							bestValue = mean;
							bestValueIQR = stdDev;
							bestIndex = count;
						}
						else if ((mean < secondBestValue) || ((mean == secondBestValue) && (stdDev < secondBestValueIQR)))
						{
							secondBestIndex = count;
							secondBestValue = mean;
							secondBestValueIQR = stdDev;
						}

						count++;
					}
				}
				else
				{ // indicator to maximize e.g., the HV
					bestValue = double.MinValue;
					bestValueIQR = double.MinValue;
					secondBestValue = double.MinValue;
					secondBestValueIQR = double.MinValue;
					foreach (var a in algs)
					{
						var mean = a.Value.Mean;
						var stdDev = a.Value.StdDeviation;

						if ((mean > bestValue) || ((mean == bestValue) && (stdDev < bestValueIQR)))
						{
							secondBestIndex = bestIndex;
							secondBestValue = bestValue;
							secondBestValueIQR = bestValueIQR;
							bestValue = mean;
							bestValueIQR = stdDev;
							bestIndex = count;
						}
						else if ((mean > secondBestValue) || ((mean == secondBestValue) && (stdDev < secondBestValueIQR)))
						{
							secondBestIndex = count;
							secondBestValue = mean;
							secondBestValueIQR = stdDev;
						}
					}
				}

				var stringLine = p.Key.Replace("_", "\\_") + " & ";

				count = 0;
				foreach (var a in algs)
				{
					if (count == bestIndex)
					{
						stringLine += "\\cellcolor{gray95}";
						a.Value.Color = CellColorEnum.Grey95.ToString();
					}
					if (count == secondBestIndex)
					{
						stringLine += "\\cellcolor{gray25}";
						a.Value.Color = CellColorEnum.Grey25.ToString(); ;
					}

					m = string.Format(new CultureInfo("en-us"), "{0:0.00e00}", a.Value.Mean);
					s = string.Format(new CultureInfo("en-us"), "{0:0.00e00}", a.Value.StdDeviation);

					stringLine += "$" + m + "_{" + s + "}$ & ";

					count++;
				}

				stringLine = stringLine.Substring(0, stringLine.LastIndexOf("&"));
				writer.WriteLine(stringLine + "\\\\");
			}

			writer.WriteLine("\\hline");
			writer.WriteLine("\\end{tabular}");
			writer.WriteLine("\\end{scriptsize}");
			writer.WriteLine("\\end{table}");
		}

		private void PrintMedianIQR(StreamWriter writer, KeyValuePair<string, IDictionary<string, IDictionary<string, LatexTableData>>> indicator)
		{
			writer.WriteLine("\\" + "\n");
			writer.WriteLine("\\begin{table}" + "\n");
			writer.WriteLine("\\caption{" + indicator.Key + ". Median and IQR}" + "\n");
			writer.WriteLine("\\label{table:median." + indicator.Key + "}");
			writer.WriteLine("\\begin{scriptsize}");
			writer.WriteLine("\\centering");
			writer.Write("\\begin{tabular}{l");

			var problems = indicator.Value;
			var problem = problems.First();
			var algorithms = problem.Value;


			foreach (var algorithm in problem.Value)
			{
				// calculate the number of columns
				writer.Write("l");
			}

			writer.WriteLine("}");

			writer.Write("\\hline &");


			writer.Write(algorithms.Select(a => a.Key).Aggregate((curr, next) => curr + " & " + next));

			writer.WriteLine("\\\\");

			writer.WriteLine("\\hline");

			string m,
				s;
			// write lines

			foreach (var p in problems)
			{
				var algs = p.Value;
				// find the best value and second best value
				double bestValue;
				double bestValueIQR;
				double secondBestValue;
				double secondBestValueIQR;
				int bestIndex = -1;
				int secondBestIndex = -1;
				int count = 0;
				if (experiment.IndicatorsMinimize[indicator.Key])
				{// minimize by default
					bestValue = double.MaxValue;
					bestValueIQR = double.MaxValue;
					secondBestValue = double.MaxValue;
					secondBestValueIQR = double.MaxValue;
					foreach (var a in algs)
					{
						var median = a.Value.Median;
						var iqr = a.Value.IQR;

						if ((median < bestValue) || ((median == bestValue) && (iqr < bestValueIQR)))
						{
							secondBestIndex = bestIndex;
							secondBestValue = bestValue;
							secondBestValueIQR = bestValueIQR;
							bestValue = median;
							bestValueIQR = iqr;
							bestIndex = count;
						}
						else if ((median < secondBestValue) || ((median == secondBestValue) && (iqr < secondBestValueIQR)))
						{
							secondBestIndex = count;
							secondBestValue = median;
							secondBestValueIQR = iqr;
						}
					}
				}
				else
				{ // indicator to maximize e.g., the HV
					bestValue = double.MinValue;
					bestValueIQR = double.MinValue;
					secondBestValue = double.MinValue;
					secondBestValueIQR = double.MinValue;

					foreach (var a in algs)
					{
						var median = a.Value.Median;
						var iqr = a.Value.IQR;
						if ((median > bestValue) || ((median == bestValue) && (iqr < bestValueIQR)))
						{
							secondBestIndex = bestIndex;
							secondBestValue = bestValue;
							secondBestValueIQR = bestValueIQR;
							bestValue = median;
							bestValueIQR = iqr;
							bestIndex = count;
						}
						else if ((median > secondBestValue) || ((median == secondBestValue) && (iqr < secondBestValueIQR)))
						{
							secondBestIndex = count;
							secondBestValue = median;
							secondBestValueIQR = iqr;
						}
					}
				}

				var stringLine = p.Key.Replace("_", "\\_") + " & ";
				count = 0;
				foreach (var a in algs)
				{
					if (count == bestIndex)
					{
						stringLine += "\\cellcolor{gray95}";
					}
					if (count == secondBestIndex)
					{
						stringLine += "\\cellcolor{gray25}";
					}

					m = string.Format(new CultureInfo("en-us"), "{0:0.00e00}", a.Value.Median);
					s = string.Format(new CultureInfo("en-us"), "{0:0.00e00}", a.Value.IQR);

					stringLine += "$" + m + "_{" + s + "}$ & ";
					count++;
				}

				stringLine = stringLine.Substring(0, stringLine.LastIndexOf("&"));
				writer.WriteLine(stringLine + "\\\\");
			}

			writer.WriteLine("\\hline");
			writer.WriteLine("\\end{tabular}");
			writer.WriteLine("\\end{scriptsize}");
			writer.WriteLine("\\end{table}");
		}

		private void PrintHeaderLatexCommands(StreamWriter writer)
		{
			writer.WriteLine("\\documentclass{article}");
			writer.WriteLine("\\title{" + experiment.Name + "}");
			writer.WriteLine("\\usepackage{colortbl}");
			writer.WriteLine("\\usepackage[table*]{xcolor}");
			writer.WriteLine("\\usepackage[margin=0.6in]{geometry}");
			writer.WriteLine("\\xdefinecolor{gray95}{gray}{0.65}");
			writer.WriteLine("\\xdefinecolor{gray25}{gray}{0.8}");
			writer.WriteLine("\\author{}");
			writer.WriteLine("\\begin{document}");
			writer.WriteLine("\\maketitle");
			writer.WriteLine("\\section{Tables}");
		}

		private void PrintEndLatexCommands(StreamWriter writer)
		{
			writer.WriteLine("\\end{document}" + "\n");
		}
	}
}

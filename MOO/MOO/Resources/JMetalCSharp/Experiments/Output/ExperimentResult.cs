using System.Collections.Generic;

namespace JMetalCSharp.Experiments.Output
{
	public class ExperimentResult
	{
		public IDictionary<string, IDictionary<string, IDictionary<string, List<double>>>> Indicators { get; set; }
		public IDictionary<string, IDictionary<string, IDictionary<string, LatexTableData>>> QualityTables { get; set; }
		public IDictionary<string, IDictionary<string, IDictionary<string, BoxPlotData>>> BoxPlots { get; set; }
		public string LatexPath { get; set; }
		public long ElapsedTime { get; set; }
	}
}

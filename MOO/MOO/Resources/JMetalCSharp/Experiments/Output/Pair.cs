using System;

namespace JMetalCSharp.Experiments.Output
{
	public class Pair : IComparable<Pair>
	{

		public double Index { get; set; }
		public double Value { get; set; }

		public Pair()
		{

		}

		public Pair(double i, double v)
		{
			Index = i;
			Value = v;
		}


		public int CompareTo(Pair o1)
		{ //ordena por valor absoluto

			if (Math.Abs(this.Value) > Math.Abs(o1.Value))
			{
				return 1;
			}
			else if (Math.Abs(this.Value) < Math.Abs(o1.Value))
			{
				return -1;
			}
			else return 0;
		}
	}
}

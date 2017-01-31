using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Encoding.Variable
{
	/// <summary>
	/// This class implements an integer decision encodings.variable
	/// </summary>
	public class Int : Core.Variable
	{
		/// <summary>
		/// Stores the value of the encodings.variable
		/// </summary>
		public new int Value { get; set; }

		/// <summary>
		/// Stores the lower limit of the encodings.variable
		/// </summary>
		public new int LowerBound;

		/// <summary>
		/// Stores the upper limit of the encodings.variable
		/// </summary>
		public new int UpperBound;

		/// <summary>
		/// Constructor
		/// </summary>
		public Int()
		{
			this.LowerBound = int.MinValue;
			this.UpperBound = int.MaxValue;
			Value = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="lowerBound">Variable lower bound</param>
		/// <param name="upperBound">Variable upper bound</param>
		public Int(int lowerBound, int upperBound)
		{
			this.LowerBound = lowerBound;
			this.UpperBound = upperBound;
			this.Value = JMetalRandom.Next(lowerBound, upperBound);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value">Value of the encodings.variable</param>
		/// <param name="lowerBound">Variable lower bound</param>
		/// <param name="upperBound">Variable upper bound</param>
		public Int(int value, int lowerBound, int upperBound)
		{
			this.Value = value;
			this.LowerBound = lowerBound;
			this.UpperBound = upperBound;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="variable">Variable to be copied</param>
		public Int(Core.Variable variable)
		{
			this.LowerBound = (int)variable.LowerBound;
			this.UpperBound = (int)variable.UpperBound;
			this.Value = (int)variable.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public override Core.Variable DeepCopy()
		{
			try
			{
				return new Int(this);
			}
			catch (Exception ex)
			{
				Logger.Log.Error(this.GetType().FullName + ".DeepCopy()", ex);
				Console.WriteLine("Error in " + this.GetType().FullName + ".DeepCopy()");
				return null;
			}
		}
	}
}

using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace JMetalCSharp.Encoding.SolutionType
{
	/// <summary>
	/// Class representing the solution type of solutions composed of array of reals 
	/// and a binary string.
	/// ASSUMPTIONs:
	/// - The numberOfVariables_ field in class Problem must contain the number
	///   of real variables. This field is used to apply real operators (e.g., 
	///   mutation probability)
	/// - The upperLimit_ and lowerLimit_ arrays must have the length indicated
	///   by numberOfVariables_.
	/// 
	/// </summary>
	public class ArrayRealAndBinarySolutionType : Core.SolutionType
	{
		private int _binaryStringLength;
		private int _numberOfRealVariables;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem being solved</param>
		/// <param name="realVariables">Number of real variables</param>
		/// <param name="binaryStringLength">Length of the binary string</param>
		public ArrayRealAndBinarySolutionType(Problem problem, int realVariables, int binaryStringLength)
			: base(problem)
		{
			this._binaryStringLength = binaryStringLength;
			this._numberOfRealVariables = realVariables;
		}

		/// <summary>
		/// Creates the variables of the solution
		/// </summary>
		/// <returns>the variables of the solution</returns>
		public override Core.Variable[] CreateVariables()
		{
			Core.Variable[] variables = new Core.Variable[2];

			variables[0] = new ArrayReal(_numberOfRealVariables, Problem);
			variables[1] = new Binary(_binaryStringLength);
			return variables;
		}
	}
}

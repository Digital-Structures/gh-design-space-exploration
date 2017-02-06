namespace JMetalCSharp.Utils
{
	/// <summary>
	/// This is an auxiliar class for calculating the SPEA2 environmental selection.
	/// Each instance of DistanceNode contains two parameter called
	/// <code>reference_</code> and <code>distance_</code>.
	/// <code>reference_</code> indicates one <code>Solution</code> in a
	/// <code>SolutionSet</code> and <code>distance_</code> represents the distance_
	/// to this solution.
	/// </summary>
	public class DistanceNode
	{
		#region Public Properties
		/// <summary>
		/// Indicates the position of a <code>Solution</code> in a 
		/// <code>SolutionSet</code>.
		/// </summary>
		public int Reference { get; set; }

		/// <summary>
		/// Indicates the distance to the <code>Solution</code> represented by 
		/// <code>reference_</code>.
		/// </summary>
		public double Distance { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="distance">The distance to a <code>Solution</code>.</param>
		/// <param name="reference">The position of the <code>Solution</code>.</param>
		public DistanceNode(double distance, int reference)
		{
			this.Distance = distance;
			this.Reference = reference;
		}

		#endregion
	}
}

using JMetalCSharp.Utils;
using System.Text;

namespace JMetalCSharp.Encoding.Variable
{
	/// <summary>
	/// Class implementing a permutation of integer decision encodings.variable
	/// </summary>
	public class Permutation : Core.Variable
	{
		/// <summary>
		/// Stores a permutation of <code>int</code> values
		/// </summary>
		public int[] Vector { get; set; }

		/// <summary>
		/// Stores the length of the permutation
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public Permutation()
		{
			Size = 0;
			Vector = null;
		}

		/// <summary>
		/// Constructor
		/// This constructor has been contributed by Madan Sathe
		/// </summary>
		/// <param name="Size">Length of the permutation</param>
		public Permutation(int size)
		{
			this.Size = size;
			this.Vector = new int[size];

			int[] randomSequence = new int[size];

			for (int i = 0; i < randomSequence.Length; i++)
			{
				randomSequence[i] = i;
			}

			Shuffle.ShuffleArray<int>(randomSequence);

			Vector = randomSequence;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="permutation">The permutation to copy</param>
		public Permutation(Permutation permutation)
		{
			this.Size = permutation.Size;
			this.Vector = (int[])permutation.Vector.Clone();
		}

		/// <summary>
		/// Create an exact copy of the <code>Permutation</code> object.
		/// </summary>
		/// <returns>An exact copy of the object.</returns>
		public override Core.Variable DeepCopy()
		{
			return new Permutation(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < Size; i++)
			{
				result.Append(Vector[i] + " ");
			}

			return result.ToString();
		}
	}
}

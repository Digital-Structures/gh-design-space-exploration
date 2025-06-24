using JMetalCSharp.Utils;
using System;
using System.Text;

namespace JMetalCSharp.Encoding.Variable
{
	public class Binary : Core.Variable
	{
		/// <summary>
		/// Stores the bits constituting the binary string. It is
		/// implemented using a BitArray object
		/// </summary>
		public BitSet Bits { get; set; }

		/// <summary>
		/// Store the length of the binary string
		/// </summary>
		public int NumberOfBits { get; protected set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public Binary()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="numberOfBits">Length of the bit string</param>
		public Binary(int numberOfBits)
		{
			this.NumberOfBits = numberOfBits;

			this.Bits = new BitSet(numberOfBits);

			for (int i = 0; i < numberOfBits; i++)
			{
				if (JMetalRandom.NextDouble() < 0.5)
				{
					this.Bits[i] = true;
				}
				else
				{
					this.Bits[i] = false;
				}
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="variable">The Binary encoding.variable to copy</param>
		public Binary(Binary variable)
		{
			this.NumberOfBits = variable.NumberOfBits;
			this.Bits = new BitSet(NumberOfBits);
			for (int i = 0; i < NumberOfBits; i++)
			{
				Bits[i] = variable.Bits[i];
			}
		}

		/// <summary>
		/// This method is intended to be used in subclass of <code>Binary</code>, 
		/// for examples the classes, <code>BinaryReal</code> and <code>BinaryInt<codes>. 
		/// In this classes, the method allows to decode the 
		/// value enconded in the binary string. As generic variables do not encode any
		/// value, this method do noting 
		/// </summary>
		public void Decode()
		{

		}

		/// <summary>
		/// Creates an exact copy of a Binary object
		/// </summary>
		/// <returns>An exact copy of the object</returns>
		public override Core.Variable DeepCopy()
		{
			try
			{
				return new Binary(this);
			}
			catch (Exception ex)
			{
				Logger.Log.Error(this.GetType().FullName + ".DeepCopy()", ex);
				Console.WriteLine("Error in " + this.GetType().FullName + ".DeepCopy()");
				return null;
			}
		}

		/// <summary>
		/// Obtain the hamming distance between two binary strings
		/// </summary>
		/// <param name="other">The binary string to compare</param>
		/// <returns>The hamming distance</returns>
		public int HammingDistance(Binary other)
		{
			int distance = 0;
			int i = 0;

			while (i < this.Bits.Length)
			{
				if (this.Bits[i] != other.Bits[i])
				{
					distance++;
				}
				i++;
			}

			return distance;
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < this.NumberOfBits; i++)
			{
				if (this.Bits[i])
				{
					result.Append("1");
				}
				else
				{
					result.Append("0");
				}
			}

			return result.ToString();
		}
	}
}

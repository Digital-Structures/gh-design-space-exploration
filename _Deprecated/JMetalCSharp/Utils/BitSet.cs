using System.Collections;

namespace JMetalCSharp.Utils
{
	public class BitSet
	{
		#region Properties

		private BitArray bitArray { get; set; }

		public bool this[int index]
		{
			get
			{
				return bitArray[index];
			}
			set
			{
				bitArray[index] = value;
			}
		}

		public int Length
		{
			get
			{
				return bitArray.Length;
			}
		}

		#endregion

		#region Constructors

		public BitSet(int length)
		{
			bitArray = new BitArray(length);
		}

		#endregion

		#region Public Methods

		public void Clear()
		{
			for (int i = 0; i < bitArray.Length; i++)
			{
				bitArray[i] = false;
			}
		}

		public void Clear(int bitIndex)
		{
			bitArray[bitIndex] = false;
		}


		public void Flip(int bitIndex)
		{
			bitArray[bitIndex] = !bitArray[bitIndex];
		}

		public void Set(int bitIndex)
		{
			bitArray[bitIndex] = true;
		}

		public void Set(int startIndex, int endIndex)
		{
			for (int i = 0; i < bitArray.Length; i++)
			{
				bitArray[i] = true;
			}
		}

		public int Cardinality()
		{
			int res = 0;
			for (int i = 0; i < bitArray.Length; i++)
			{
				if (bitArray[i])
				{
					res += 1;
				}
			}

			return res;
		}

		#endregion
	}
}

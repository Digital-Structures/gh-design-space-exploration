using System.Collections.Generic;

namespace JMetalCSharp.Utils
{
	public static class Shuffle
	{
		public static void ShuffleArray<T>(T[] array)
		{
			for (int i = array.Length; i > 1; i--)
			{
				int j = JMetalRandom.Next(i);

				//Swap values
				T tmp = array[j];
				array[j] = array[i - 1];
				array[i - 1] = tmp;
			}
		}

		public static void ShuffleArray<T>(List<T> array)
		{
			for (int i = array.Count; i > 1; i--)
			{
				int j = JMetalRandom.Next(i);

				//Swap values
				T tmp = array[j];
				array[j] = array[i - 1];
				array[i - 1] = tmp;
			}
		}
	}
}

using System;

namespace JMetalCSharp.Utils
{
	public static class JMetalRandom
	{
		private static Random random = new Random((int)DateTime.Now.Ticks);
		private static object syncObj = new object();

		public static int Next()
		{
			lock (syncObj)
			{
				return random.Next();
			}
		}

		public static int Next(int minValue, int maxValue)
		{
			lock (syncObj)
			{
				return random.Next(minValue, maxValue + 1);
			}
		}

		public static int Next(int maxValue)
		{
			lock (syncObj)
			{
				return random.Next(maxValue + 1);
			}
		}

		public static double NextDouble()
		{
			lock (syncObj)
			{
				return random.NextDouble();
			}
		}

		public static void SetRandom(Random r)
		{
			lock (syncObj)
			{
				random = r;
			}
		}

		public static double NextDouble(double minValue, double maxValue)
		{
			lock (syncObj)
			{
				return minValue + random.NextDouble() * (maxValue - minValue);
			}
		}
	}
}

using log4net;

namespace JMetalCSharp.Utils
{
	public static class Logger
	{
		public static ILog Log { get; set; }

		static Logger()
		{
			Log = LogManager.GetLogger(typeof(Logger));
		}
	}
}

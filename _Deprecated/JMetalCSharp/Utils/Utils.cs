using JMetalCSharp.Core;
using JMetalCSharp.Operators.Mutation;
using System.Collections.Generic;
using System.Globalization;

namespace JMetalCSharp.Utils
{
	public static class Utils
	{
		public static void GetDoubleValueFromParameter(Dictionary<string, object> parameters, string parameterText, ref double parameter)
		{
			object value;
			if (parameters.TryGetValue(parameterText, out value))
			{
				if (value.GetType() == typeof(string))
				{
					parameter = ParseDoubleInvariant((string)value);
				}
				else
				{
					parameter = (double)value;
				}
			}
		}

		public static void GetDoubleValueFromParameter(Dictionary<string, object> parameters, string parameterText, ref double? parameter)
		{
			object value;
			if (parameters.TryGetValue(parameterText, out value))
			{
				if (value.GetType() == typeof(string))
				{
					parameter = ParseDoubleInvariant((string)value);
				}
				else
				{
					parameter = (double)value;
				}
			}
		}

		public static void GetIntValueFromParameter(Dictionary<string, object> parameters, string parameterText, ref int parameter)
		{
			object value;
			if (parameters.TryGetValue(parameterText, out value))
			{
				if (value.GetType() == typeof(string))
				{
					parameter = ParseIntInvariant((string)value);
				}
				else
				{
					parameter = (int)value;
				}
			}
		}

		public static void GetStringValueFromParameter(Dictionary<string, object> parameters, string parameterText, ref string parameter)
		{
			object value;
			if (parameters.TryGetValue(parameterText, out value))
			{
				parameter = (string)value;
			}
		}

		public static void GetProblemFromParameters(Dictionary<string, object> parameters, string parameterText, ref Problem problem)
		{
			object value;
			if (parameters.TryGetValue(parameterText, out value))
			{
				problem = (Problem)value;
			}
		}

		public static void GetMutationFromParameters(Dictionary<string, object> parameters, string parameterText, ref Operator mutation)
		{
			object value;
			if (parameters.TryGetValue(parameterText, out value))
			{
				mutation = (Mutation)value;
			}
		}

		public static void GetIndicatorsFromParameters(Dictionary<string, object> parameters, string parameterText, ref QualityIndicator.QualityIndicator indicators)
		{
			object value;
			if (parameters.TryGetValue(parameterText, out value))
			{
				indicators = (QualityIndicator.QualityIndicator)value;
			}
		}

		public static double ParseDoubleInvariant(string value)
		{
			value = value.Replace(",", ".");
			return double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
		}

		public static int ParseIntInvariant(string value)
		{
			value = value.Replace(",", ".");
			return int.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
		}
	}
}

using JMetalCSharp.Core;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Utils.Comparators
{
	public interface IConstraintViolationComparator : IComparer<Solution>
	{
		bool NeedToCompare(Solution s1, Solution s2);
	}
}

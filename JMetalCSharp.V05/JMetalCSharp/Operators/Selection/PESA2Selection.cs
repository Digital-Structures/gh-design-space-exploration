using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Archive;
using System;
using System.Collections.Generic;

namespace JMetalCSharp.Operators.Selection
{
	/// <summary>
	/// This class implements a selection operator as the used in PESA-II algorithm
	/// </summary>
	public class PESA2Selection : Selection
	{
		public PESA2Selection(Dictionary<string, object> parameters)
			: base(parameters)
		{

		}

		/// <summary>
		/// Performs the operation
		/// </summary>
		/// <param name="obj">Object representing a SolutionSet. This solution set must be an instancen <code>AdaptiveGridArchive</code></param>
		/// <returns>the selected solution</returns>
		public override object Execute(object obj)
		{
			try
			{
				AdaptiveGridArchive archive = (AdaptiveGridArchive)obj;
				int selected;
				int hypercube1 = archive.Grid.RandomOccupiedHypercube();
				int hypercube2 = archive.Grid.RandomOccupiedHypercube();

				if (hypercube1 != hypercube2)
				{
					if (archive.Grid.GetLocationDensity(hypercube1) < archive.Grid.GetLocationDensity(hypercube2))
					{
						selected = hypercube1;
					}
					else if (archive.Grid.GetLocationDensity(hypercube2) <
							 archive.Grid.GetLocationDensity(hypercube1))
					{
						selected = hypercube2;
					}
					else
					{
						if (JMetalRandom.NextDouble() < 0.5)
						{
							selected = hypercube2;
						}
						else
						{
							selected = hypercube1;
						}
					}
				}
				else
				{
					selected = hypercube1;
				}
				int bas = JMetalRandom.Next(0, archive.Size() - 1);
				int cnt = 0;
				while (cnt < archive.Size())
				{
					Solution individual = archive.Get((bas + cnt) % archive.Size());
					if (archive.Grid.Location(individual) != selected)
					{
						cnt++;
					}
					else
					{
						return individual;
					}
				}
				return archive.Get((bas + cnt) % archive.Size());
			}
			catch (InvalidCastException ex)
			{
				Logger.Log.Error("Exception in " + this.GetType().FullName + ".Execute()", ex);
				throw new Exception("Exception in " + this.GetType().FullName + ".Execute()");
			}
		}
	}
}

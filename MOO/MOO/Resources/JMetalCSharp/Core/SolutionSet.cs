using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JMetalCSharp.Core
{
	public class SolutionSet
	{
		public List<Solution> SolutionsList
		{
			get;
			protected set;
		}

		public int Capacity
		{
			get;
			set;
		}

		#region Constructors

		/// <summary>
		/// Creates an unbounded solution set.
		/// </summary>
		public SolutionSet()
		{
			SolutionsList = new List<Solution>();
			Capacity = 0;
		}

		/// <summary>
		/// Creates a empty solutionSet with a maximum capacity.
		/// </summary>
		/// <param name="maximumSize">Maximum size.</param>
		public SolutionSet(int maximumSize)
		{
			SolutionsList = new List<Solution>();
			Capacity = maximumSize;
		}

		#endregion

		/// <summary>
		/// Inserts a new solution into the SolutionSet.
		/// </summary>
		/// <param name="solution">The <code>Solution</code> to store</param>
		/// <returns>True If the <code>Solution</code> has been inserted, false otherwise.</returns>
		public virtual bool Add(Solution solution)
		{
			if (SolutionsList.Count == Capacity)
			{
				Logger.Log.Error("The population is full");
				Logger.Log.Error("Capacity is : " + Capacity);
				Logger.Log.Error("\t Size is: " + this.Size());
				return false;
			}

			SolutionsList.Add(solution);
			return true;
		}

		public virtual bool Add(int index, Solution solution)
		{
			SolutionsList.Insert(index, solution);
			return true;
		}

		public Solution Get(int i)
		{
			if (i >= this.SolutionsList.Count)
			{
				throw new IndexOutOfRangeException("Index out of Bound " + i);
			}
			return this.SolutionsList[i];
		}

		/// <summary>
		/// Sorts a SolutionSet using a <code>Comparator</code>.
		/// </summary>
		/// <param name="comparator"><code>Comparator</code> used to sort</param>
		public void Sort(IComparer<Solution> comparator)
		{
			if (comparator == null)
			{
				Logger.Log.Error("SolutionSet.Sort: Comparator is null");
				return;
			}
			SolutionsList.Sort(comparator);
		}

		/// <summary>
		/// Returns the index of the best Solution using a <code>Comparator</code>.
		/// If there are more than one occurrences, only the index of the first one is returned
		/// </summary>
		/// <param name="comparator"><code>Comparator</code> used to compare solutions.</param>
		/// <returns>The index of the best Solution attending to the comparator or <code>-1<code> if the SolutionSet is empty</returns>
		public int IndexBest(IComparer<Solution> comparator)
		{
			int result = 0;

			if ((SolutionsList == null) || (SolutionsList.Count == 0))
			{
				result = -1;
			}
			else
			{
				Solution bestKnown = SolutionsList.ElementAt(0);
				Solution candidateSolution;
				int flag;

				for (int i = 1; i < SolutionsList.Count; i++)
				{
					candidateSolution = SolutionsList.ElementAt(i);
					flag = comparator.Compare(bestKnown, candidateSolution);
					if (flag == 1)
					{
						result = i;
						bestKnown = candidateSolution;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the best Solution using a <code>Comparator</code>.
		/// If there are more than one occurrences, only the first one is returned.
		/// </summary>
		/// <param name="comparator"><code>Comparator</code> used to compare solutions.<code>Comparator</code> used to compare solutions.</param>
		/// <returns>The best Solution attending to the comparator or <code>null<code> if the SolutionSet is empty</returns>
		public Solution Best(IComparer<Solution> comparator)
		{
			Solution result;
			int indexBest = IndexBest(comparator);
			if (indexBest < 0)
			{
				result = null;
			}
			else
			{
				return SolutionsList.ElementAt(indexBest);
			}

			return result;
		}

		/// <summary>
		/// Returns the index of the worst Solution using a <code>Comparator</code>.
		/// If there are more than one occurrences, only the index of the first one is returned
		/// </summary>
		/// <param name="comparator"><code>Comparator</code> used to compare solutions.</param>
		/// <returns>The index of the worst Solution attending to the comparator or <code>-1<code> if the SolutionSet is empty</returns>
		public int IndexWorst(IComparer<Solution> comparator)
		{
			int result = 0;

			if ((SolutionsList == null) || SolutionsList.Count == 0)
			{
				result = 0;
			}
			else
			{
				Solution worstKnown = SolutionsList.ElementAt(0),
					candidateSolution;
				int flag;
				for (int i = 1; i < SolutionsList.Count; i++)
				{
					candidateSolution = SolutionsList.ElementAt(i);
					flag = comparator.Compare(worstKnown, candidateSolution);
					if (flag == -1)
					{
						result = i;
						worstKnown = candidateSolution;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the worst Solution using a <code>Comparator</code>.Returns the worst Solution using a <code>Comparator</code>.
		/// If there are more than one occurrences, only the first one is returned
		/// </summary>
		/// <param name="comparator"><code>Comparator</code> used to compare solutions.</param>
		/// <returns>The worst Solution attending to the comparator or <code>null<code>The worst Solution attending to the comparator or <code>null<code> if the SolutionSet is empty</returns>
		public Solution Worst(IComparer<Solution> comparator)
		{
			Solution result;
			int index = IndexWorst(comparator);
			if (index < 0)
			{
				result = null;
			}
			else
			{
				result = SolutionsList.ElementAt(index);
			}

			return result;
		}

		/// <summary>
		/// Returns the number of solutions in the SolutionSet.
		/// </summary>
		/// <returns>The size of the SolutionSet.</returns>
		public int Size()
		{
			return this.SolutionsList.Count;
		}

		/// <summary>
		/// Writes the objective function values of the <code>Solution</code> objects into the set in a file.
		/// </summary>
		/// <param name="path">The output file name</param>
		public void PrintObjectivesToFile(string path)
		{
			try
			{
				using (StreamWriter outFile = new StreamWriter(path))
				{
					foreach (Solution s in this.SolutionsList)
					{
						outFile.WriteLine(s.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("SolutionSet.PrintObjectivesToFile", ex);
				Console.WriteLine(ex.StackTrace);
			}
		}

		public double[][] GetObjectives()
		{
			double[][] result = new double[this.SolutionsList.Count][];

			for (int i = 0, li = this.SolutionsList.Count; i < li; i++)
			{
				result[i] = this.SolutionsList[i].Objective;
			}
			return result;
		}

		public string ObjectivesToString()
		{
			StringBuilder result = new StringBuilder();
			foreach (Solution s in this.SolutionsList)
			{
				result.AppendLine(s.ToString());
			}

			return result.ToString();
		}

		public string VariablesToString()
		{
			StringBuilder result = new StringBuilder();

			if (Size() > 0)
			{
				int numberOfVariables = SolutionsList.ElementAt(0).Variable.Count();

				foreach (Solution s in this.SolutionsList)
				{
					for (int i = 0; i < numberOfVariables; i++)
					{
						result.Append(s.Variable[i].ToString() + " ");
					}
					result.Append("\r\n");
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// Writes the decision encodings.variable values of the <code>Solution</code> solutions objects into the set in a file.
		/// </summary>
		/// <param name="path">The output file name</param>
		public void PrintVariablesToFile(string path)
		{
			try
			{
				if (Size() > 0)
				{
					int numberOfVariables = SolutionsList.ElementAt(0).Variable.Count();
					using (StreamWriter outFile = new StreamWriter(path))
					{
						foreach (Solution s in this.SolutionsList)
						{
							for (int i = 0; i < numberOfVariables; i++)
							{
								outFile.Write(s.Variable[i].ToString() + " ");
							}
							outFile.Write("\r\n");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("SolutionSet.PrintVariablesToFile", ex);
				Console.WriteLine(ex.StackTrace);
			}
		}

		/// <summary>
		/// Write the function values of feasible solutions into a file
		/// </summary>
		/// <param name="path">File name</param>
		public void PrintFeasibleFUN(string path)
		{
			try
			{
				using (StreamWriter outFile = new StreamWriter(path))
				{
					foreach (Solution s in this.SolutionsList)
					{
						if (s.OverallConstraintViolation == 0.0)
						{
							outFile.WriteLine(s.ToString());
						}

					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("SolutionSet.PrintFeasibleFun", ex);
				Console.WriteLine(ex.StackTrace);
			}
		}

		/// <summary>
		/// Write the encodings.variable values of feasible solutions into a file
		/// </summary>
		/// <param name="path">File name</param>
		public void PrintFeasibleVAR(string path)
		{
			try
			{
				if (Size() > 0)
				{
					int numberOfVariables = SolutionsList.ElementAt(0).Variable.Count();
					using (StreamWriter outFile = new StreamWriter(path))
					{
						foreach (Solution s in this.SolutionsList)
						{
							if (s.OverallConstraintViolation == 0.0)
							{
								for (int i = 0; i < numberOfVariables; i++)
								{
									outFile.WriteLine(s.Variable[i].ToString());
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("SolutionSet.PrintFeasibleVAR", ex);
				Console.Write(ex.StackTrace);
			}
		}

		/// <summary>
		/// Empties the SolutionSet
		/// </summary>
		public void Clear()
		{
			this.SolutionsList.Clear();
		}

		/// <summary>
		/// Deletes the <code>Solution</code> at position i in the set.
		/// </summary>
		/// <param name="i">The position of the solution to remove.</param>
		public void Remove(int i)
		{
			if (i > SolutionsList.Count - 1)
			{
				Logger.Log.Warn("SolutionSet.Remove: Index out of bound.");
			}
			else
			{
				SolutionsList.RemoveAt(i);
			}
		}

		/// <summary>
		/// Returns a new <code>SolutionSet</code> which is the result of the union between the current solution set and the one passed as a parameter.
		/// </summary>
		/// <param name="solutionSet">SolutionSet to join with the current solutionSet.</param>
		/// <returns>The result of the union operation.</returns>
		public SolutionSet Union(SolutionSet solutionSet)
		{
			int newSize = this.Size() + solutionSet.Size();
			if (newSize < Capacity)
			{
				newSize = Capacity;
			}

			SolutionSet union = new SolutionSet(newSize);

			for (int i = 0; i < this.Size(); i++)
			{
				union.Add(this.SolutionsList[i]);
			}

			for (int i = 0; i < solutionSet.Size(); i++)
			{
				union.Add(solutionSet.SolutionsList[i]);
			}

			return union;
		}

		/// <summary>
		/// Replaces a solution by a new one
		/// </summary>
		/// <param name="position">The position of the solution to replace</param>
		/// <param name="solution">The new solution</param>
		public void Replace(int position, Solution solution)
		{
			if (position > this.SolutionsList.Count)
			{
				this.SolutionsList.Add(solution);
			}
			else
			{
				this.SolutionsList.RemoveAt(position);
				this.SolutionsList.Insert(position, solution);
			}
		}

		/// <summary>
		/// Copies the objectives of the solution set to a matrix
		/// </summary>
		/// <returns>A matrix containing the objectives</returns>
		public double[][] WriteObjectivesToMatrix()
		{
			if (this.Size() == 0)
			{
				return null;
			}

			double[][] objectives;
			objectives = new double[this.Size()][];

			for (int i = 0; i < this.Size(); i++)
			{
				objectives[i] = new double[this.SolutionsList[0].NumberOfObjectives];
			}

			for (int i = 0; i < this.Size(); i++)
			{
				for (int j = 0; j < this.SolutionsList[0].NumberOfObjectives; j++)
				{
					objectives[i][j] = this.SolutionsList[i].Objective[j];
				}
			}

			return objectives;
		}

		public void PrintObjectives()
		{
			for (int i = 0; i < this.SolutionsList.Count; i++)
			{
				Console.WriteLine("" + this.SolutionsList[i]);
			}
		}
	}
}

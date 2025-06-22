using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace JMetalCSharp.QualityIndicator.Utils
{
	/// <summary>
	/// This class provides some utilities to compute quality indicators. 
	/// </summary>
	public class MetricsUtil
	{
		#region Public Methods
		/// <summary>
		/// This method reads a Pareto Front for a file.
		/// </summary>
		/// <param name="path">The path to the file that contains the pareto front</param>
		/// <returns>double [][] whit the pareto front</returns>
		public double[][] ReadFront(string name)
		{
			string path;
			if (Path.IsPathRooted(name))
			{
				path = name;
			}
			else
			{
				path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), @"Data\ParetoFronts\" + name);
			}
			try
			{
				double[][] front;

				using (StreamReader reader = new StreamReader(path))
				{
					List<double[]> list = new List<double[]>();
					int numberOfObjectives = 0;
					string aux = reader.ReadLine();
					while (aux != null)
					{
						string[] st = aux.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
						int i = 0;
						numberOfObjectives = st.Length;
						double[] vector = new double[st.Length];

						foreach (string s in st)
						{
							double value = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(s);
							vector[i] = value;
							i++;
						}
						list.Add(vector);
						aux = reader.ReadLine();
					}

					front = new double[list.Count][];
					for (int i = 0; i < list.Count; i++)
					{
						front[i] = new double[numberOfObjectives];
					}

					for (int i = 0; i < list.Count; i++)
					{
						front[i] = list[i];
					}

				}
				return front;
			}
			catch (Exception ex)
			{
				Logger.Log.Error("InputFacilities crashed reading for file: " + path, ex);
				Console.WriteLine("InputFacilities crashed reading for file: " + path);
			}
			return null;
		}

		/// <summary>
		/// Gets the maximum values for each objectives in a given pareto front
		/// </summary>
		/// <param name="front">The pareto front</param>
		/// <param name="noObjectives">Number of objectives in the pareto front</param>
		/// <returns>An array of noOjectives values whit the maximun values for each objective</returns>
		public double[] GetMaximumValues(double[][] front, int noObjectives)
		{
			double[] maximumValue = new double[noObjectives];
			for (int i = 0; i < noObjectives; i++)
			{
				maximumValue[i] = double.NegativeInfinity;
			}

			foreach (double[] aFront in front)
			{
				for (int j = 0; j < aFront.Length; j++)
				{
					if (aFront[j] > maximumValue[j])
					{
						maximumValue[j] = aFront[j];
					}
				}
			}

			return maximumValue;
		}

		/// <summary>
		/// Gets the minimum values for each objectives in a given pareto front
		/// </summary>
		/// <param name="front">The pareto front</param>
		/// <param name="noObjectives">Number of objectives in the pareto front</param>
		/// <returns>An array of noOjectives values whit the minimum values for each objective</returns>
		public double[] GetMinimumValues(double[][] front, int noObjectives)
		{
			double[] minimumValue = new double[noObjectives];
			for (int i = 0; i < noObjectives; i++)
			{
				minimumValue[i] = double.MaxValue;
			}

			foreach (double[] aFront in front)
			{
				for (int j = 0; j < aFront.Length; j++)
				{
					if (aFront[j] < minimumValue[j])
					{
						minimumValue[j] = aFront[j];
					}
				}
			}
			return minimumValue;
		}

		/// <summary>
		/// This method returns the distance (taken the euclidean distance) between
		/// two points given as <code>double []</code>
		/// </summary>
		/// <param name="a">A point</param>
		/// <param name="b">B point</param>
		/// <returns>The euclidean distance between the points</returns>
		public double Distance(double[] a, double[] b)
		{
			double distance = 0.0;

			for (int i = 0; i < a.Length; i++)
			{
				distance += Math.Pow(a[i] - b[i], 2.0);
			}
			return Math.Sqrt(distance);
		}

		/// <summary>
		/// Gets the distance between a point and the nearest one in a given front
		/// (the front is given as <code>double [][]</code>)
		/// </summary>
		/// <param name="point">The point</param>
		/// <param name="front">The front that contains the other points to calculate the distances</param>
		/// <returns>The minimun distance between the point and the front</returns>
		public double DistanceToClosedPoint(double[] point, double[][] front)
		{
			double minDistance = Distance(point, front[0]);

			for (int i = 1; i < front.Length; i++)
			{
				double aux = Distance(point, front[i]);
				if (aux < minDistance)
				{
					minDistance = aux;
				}
			}

			return minDistance;
		}

		/// <summary>
		/// Gets the distance between a point and the nearest one in a given front,
		/// and this distance is greater than 0.0
		/// </summary>
		/// <param name="point">The point</param>
		/// <param name="front">The front that contains the other points to calculate the distances</param>
		/// <returns>The minimun distances greater than zero between the point and the front</returns>
		public double DistanceToNearestPoint(double[] point, double[][] front)
		{
			double minDistance = double.MaxValue;

			foreach (double[] aFront in front)
			{
				double aux = Distance(point, aFront);
				if ((aux < minDistance) && (aux > 0.0))
				{
					minDistance = aux;
				}
			}

			return minDistance;
		}

		/// <summary>
		/// This method receives a pareto front and two points, one whit maximum
		/// values and the other with minimum values allowed, and returns a the
		/// normalized Pareto front.
		/// </summary>
		/// <param name="front">A pareto front.</param>
		/// <param name="maximumValue">The maximum values allowed</param>
		/// <param name="minimumValue">The minimum values allowed</param>
		/// <returns>The normalized pareto front</returns>
		public double[][] GetNormalizedFront(double[][] front,
				double[] maximumValue,
				double[] minimumValue)
		{

			double[][] normalizedFront = new double[front.Length][];

			for (int i = 0; i < front.Length; i++)
			{
				normalizedFront[i] = new double[front[i].Length];
				for (int j = 0; j < front[i].Length; j++)
				{
					normalizedFront[i][j] = (front[i][j] - minimumValue[j]) / (maximumValue[j] - minimumValue[j]);
				}
			}
			return normalizedFront;
		}

		/// <summary>
		/// This method receives a normalized pareto front and return the inverted
		/// one. This operation needed for minimization problems
		/// </summary>
		/// <param name="front">The pareto front to inverse</param>
		/// <returns>The inverted pareto front</returns>
		public double[][] InvertedFront(double[][] front)
		{
			double[][] invertedFront = new double[front.Length][];

			for (int i = 0; i < front.Length; i++)
			{
				invertedFront[i] = new double[front[i].Length];
				for (int j = 0; j < front[i].Length; j++)
				{
					if (front[i][j] <= 1.0 && front[i][j] >= 0.0)
					{
						invertedFront[i][j] = 1.0 - front[i][j];
					}
					else if (front[i][j] > 1.0)
					{
						invertedFront[i][j] = 0.0;
					}
					else if (front[i][j] < 0.0)
					{
						invertedFront[i][j] = 1.0;
					}
				}
			}
			return invertedFront;
		}

		/// <summary>
		/// Reads a set of non dominated solutions from a file
		/// </summary>
		/// <param name="path">The path of the file containing the data</param>
		/// <returns>A solution set</returns>
		public SolutionSet ReadSolutionSet(string path)
		{
			try
			{
				SolutionSet solutionSet = new SolutionSet();
				/* Open the file */
				using (StreamReader reader = new StreamReader(path))
				{
					string aux = reader.ReadLine();
					while (aux != null)
					{
						string[] st = aux.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
						int i = 0;
						Solution solution = new Solution(st.Length);

						foreach (string s in st)
						{
							double value = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(s);
							solution.Objective[i] = value;
							i++;
						}
						solutionSet.Capacity = solutionSet.Capacity + 1;
						solutionSet.Add(solution);
						aux = reader.ReadLine();
					}
				}
				return solutionSet;
			}
			catch (Exception e)
			{
				Console.WriteLine("ReadNonDominatedSolutionSet: " + path);
				Console.WriteLine(e.StackTrace);
			}
			return null;
		}

		/// <summary>
		/// Reads a set of non dominated solutions from a file
		/// </summary>
		/// <param name="path">The path of the file containing the data</param>
		/// <returns>A solution set</returns>
		public SolutionSet ReadNonDominatedSolutionSet(string name)
		{
			var path = Path.GetFullPath("./Data/ParetoFronts/" + name);
			try
			{
				SolutionSet solutionSet = new NonDominatedSolutionList();

				using (StreamReader reader = new StreamReader(path))
				{
					string aux = reader.ReadLine();
					while (aux != null)
					{
						string[] st = aux.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
						int i = 0;
						Solution solution = new Solution(st.Length);
						foreach (string s in st)
						{
							double value = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(s);
							solution.Objective[i] = value;
							i++;
						}
						solutionSet.Add(solution);
						aux = reader.ReadLine();
					}
				}
				return solutionSet;
			}
			catch (Exception e)
			{
				Console.WriteLine("ReadNonDominatedSolutionSet: " + path);
				Console.WriteLine(e.StackTrace);
			}
			return null;
		}

		/// <summary>
		/// Reads a set of non dominated solutions from a file and store it in a
		/// existing non dominated solution set
		/// </summary>
		/// <param name="path">The path of the file containing the data</param>
		/// <param name="solutionSet">A solution set</param>
		public void ReadNonDominatedSolutionSet(string path, NonDominatedSolutionList solutionSet)
		{
			try
			{
				/* Open the file */
				using (StreamReader reader = new StreamReader(path))
				{
					string aux = reader.ReadLine();
					while (aux != null)
					{
						string[] st = aux.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
						int i = 0;
						Solution solution = new Solution(st.Length);

						foreach (string s in st)
						{
							double value = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(s);
							solution.Objective[i] = value;
							i++;
						}
						solutionSet.Add(solution);
						aux = reader.ReadLine();
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("ReadNonDominatedSolutionSet: " + path);
				Console.WriteLine(e.StackTrace);
			}
		}

		public void ReadNonDominatedSolutionSet(double[][] objectives, NonDominatedSolutionList solutionSet)
		{
			double[] solutionObjectives;
			for (int i = 0, li = objectives.Length; i < li; i++)
			{
				solutionObjectives = objectives[i];
				Solution solution = new Solution(solutionObjectives.Length);
				solution.Objective = solutionObjectives;

				solutionSet.Add(solution);
			}
		}

		/// <summary>
		/// Calculates how much hypervolume each point dominates exclusively. The
		/// points have to be transformed beforehand, to accommodate the assumptions
		/// of Zitzler's hypervolume code.
		/// </summary>
		/// <param name="numberOfobjectives"></param>
		/// <param name="front">transformed objective values</param>
		/// <returns>HV contributions</returns>
		public double[] HVContributions(int numberOfobjectives, double[][] front)
		{
			HyperVolume hyperVolume = new HyperVolume();
			int numberOfObjectives = numberOfobjectives;
			double[] contributions = new double[front.Length];
			double[][] frontSubset = new double[front.Length - 1][];//[front[0].Length];
			for (int i = 0; i < front.Length - 1; i++)
			{
				frontSubset[i] = new double[front[0].Length];
			}

			List<double[]> frontCopy = new List<double[]>();
			frontCopy.AddRange(front);

			double[][] totalFront = frontCopy.ToArray();

			double totalVolume = hyperVolume.CalculateHypervolume(totalFront, totalFront.Length, numberOfObjectives);
			for (int i = 0; i < front.Length; i++)
			{
				double[] evaluatedPoint = frontCopy[i];
				frontCopy.RemoveAt(i);
				frontSubset = frontCopy.ToArray();
				// STEP4. The hypervolume (control is passed to java version of Zitzler code)
				double hv = hyperVolume.CalculateHypervolume(frontSubset, frontSubset.Length, numberOfObjectives);
				double contribution = totalVolume - hv;
				contributions[i] = contribution;
				// put point back
				frontCopy.Insert(i, evaluatedPoint);
			}
			return contributions;
		}


		/// <summary>
		/// Calculates the hv contribution of different populations. Receives an
		/// array of populations and computes the contribution to HV of the
		/// population consisting in the union of all of them
		/// </summary>
		/// <param name="populations">Consisting in all the populatoins</param>
		/// <returns>HV contributions of each population</returns>
		public double[] HVContributions(SolutionSet[] populations)
		{
			bool empty = true;
			foreach (SolutionSet population2 in populations)
			{
				if (population2.Size() > 0)
				{
					empty = false;
				}
			}

			if (empty)
			{
				double[] contributions = new double[populations.Length];
				for (int i = 0; i < populations.Length; i++)
				{
					contributions[i] = 0;
				}
				for (int i = 0; i < populations.Length; i++)
				{
					Logger.Log.Info("Contributions: " + contributions[i]);
					Console.WriteLine(contributions[i]);
				}
				return contributions;
			}

			SolutionSet union;
			int size = 0;
			double offset_ = 0.0;

			//determining the global size of the population
			foreach (SolutionSet population1 in populations)
			{
				size += population1.Size();
			}

			//allocating space for the union 
			union = new SolutionSet(size);

			// filling union
			foreach (SolutionSet population in populations)
			{
				for (int j = 0; j < population.Size(); j++)
				{
					union.Add(population.Get(j));
				}
			}

			//determining the number of objectives		  
			int numberOfObjectives = union.Get(0).NumberOfObjectives;

			//writing everything in matrices
			double[][][] frontValues = new double[populations.Length + 1][][];

			frontValues[0] = union.WriteObjectivesToMatrix();
			for (int i = 0; i < populations.Length; i++)
			{
				if (populations[i].Size() > 0)
				{
					frontValues[i + 1] = populations[i].WriteObjectivesToMatrix();
				}
				else
				{
					frontValues[i + 1] = new double[0][];
				}
			}

			// obtain the maximum and minimum values of the Pareto front
			double[] maximumValues = GetMaximumValues(union.WriteObjectivesToMatrix(), numberOfObjectives);
			double[] minimumValues = GetMinimumValues(union.WriteObjectivesToMatrix(), numberOfObjectives);

			// normalized all the fronts
			double[][][] normalizedFront = new double[populations.Length + 1][][];
			for (int i = 0; i < normalizedFront.Length; i++)
			{
				if (frontValues[i].Length > 0)
				{
					normalizedFront[i] = GetNormalizedFront(frontValues[i], maximumValues, minimumValues);
				}
				else
				{
					normalizedFront[i] = new double[0][];
				}
			}

			// compute offsets for reference point in normalized space
			double[] offsets = new double[maximumValues.Length];
			for (int i = 0; i < maximumValues.Length; i++)
			{
				offsets[i] = offset_ / (maximumValues[i] - minimumValues[i]);
			}

			//Inverse all the fronts front. This is needed because the original
			//metric by Zitzler is for maximization problems
			double[][][] invertedFront = new double[populations.Length + 1][][];
			for (int i = 0; i < invertedFront.Length; i++)
			{
				if (normalizedFront[i].Length > 0)
				{
					invertedFront[i] = InvertedFront(normalizedFront[i]);
				}
				else
				{
					invertedFront[i] = new double[0][];
				}
			}

			// shift away from origin, so that boundary points also get a contribution > 0
			foreach (double[][] anInvertedFront in invertedFront)
			{
				foreach (double[] point in anInvertedFront)
				{
					for (int i = 0; i < point.Length; i++)
					{
						point[i] += offsets[i];
					}
				}
			}

			// calculate contributions 
			double[] contribution = new double[populations.Length];
			HyperVolume hyperVolume = new HyperVolume();

			for (int i = 0; i < populations.Length; i++)
			{
				if (invertedFront[i + 1].Length == 0)
				{
					contribution[i] = 0;
				}
				else
				{
					if (invertedFront[i + 1].Length != invertedFront[0].Length)
					{
						double[][] aux = new double[invertedFront[0].Length - invertedFront[i + 1].Length][];
						int startPoint = 0, endPoint;
						for (int j = 0; j < i; j++)
						{
							startPoint += invertedFront[j + 1].Length;
						}
						endPoint = startPoint + invertedFront[i + 1].Length;
						int index = 0;
						for (int j = 0; j < invertedFront[0].Length; j++)
						{
							if (j < startPoint || j >= (endPoint))
							{
								aux[index++] = invertedFront[0][j];
							}
						}

						contribution[i] = hyperVolume.CalculateHypervolume(invertedFront[0], invertedFront[0].Length, numberOfObjectives)
								- hyperVolume.CalculateHypervolume(aux, aux.Length, numberOfObjectives);
					}
					else
					{
						contribution[i] = hyperVolume.CalculateHypervolume(invertedFront[0], invertedFront[0].Length, numberOfObjectives);
					}
				}
			}

			return contribution;
		}

		/// <summary>
		/// Calculates the hv contribution of different populations. Receives an
		/// array of populations and computes the contribution to HV of the
		/// population consisting in the union of all of them
		/// </summary>
		/// <param name="archive"></param>
		/// <param name="populations">Consisting in all the populatoins</param>
		/// <returns>HV contributions of each population</returns>
		public double[] HVContributions(SolutionSet archive, SolutionSet[] populations)
		{

			SolutionSet union;
			int size = 0;
			double offset_ = 0.0;

			//determining the global size of the population
			foreach (SolutionSet population in populations)
			{
				size += population.Size();
			}

			//allocating space for the union 
			union = archive;

			//determining the number of objectives		  
			int numberOfObjectives = union.Get(0).NumberOfObjectives;

			//writing everything in matrices
			double[][][] frontValues = new double[populations.Length + 1][][];

			frontValues[0] = union.WriteObjectivesToMatrix();
			for (int i = 0; i < populations.Length; i++)
			{
				if (populations[i].Size() > 0)
				{
					frontValues[i + 1] = populations[i].WriteObjectivesToMatrix();
				}
				else
				{
					frontValues[i + 1] = new double[0][];
				}
			}

			// obtain the maximum and minimum values of the Pareto front
			double[] maximumValues = GetMaximumValues(union.WriteObjectivesToMatrix(), numberOfObjectives);
			double[] minimumValues = GetMinimumValues(union.WriteObjectivesToMatrix(), numberOfObjectives);

			// normalized all the fronts
			double[][][] normalizedFront = new double[populations.Length + 1][][];
			for (int i = 0; i < normalizedFront.Length; i++)
			{
				if (frontValues[i].Length > 0)
				{
					normalizedFront[i] = GetNormalizedFront(frontValues[i], maximumValues, minimumValues);
				}
				else
				{
					normalizedFront[i] = new double[0][];
				}
			}

			// compute offsets for reference point in normalized space
			double[] offsets = new double[maximumValues.Length];
			for (int i = 0; i < maximumValues.Length; i++)
			{
				offsets[i] = offset_ / (maximumValues[i] - minimumValues[i]);
			}

			//Inverse all the fronts front. This is needed because the original
			//metric by Zitzler is for maximization problems
			double[][][] invertedFront = new double[populations.Length + 1][][];
			for (int i = 0; i < invertedFront.Length; i++)
			{
				if (normalizedFront[i].Length > 0)
				{
					invertedFront[i] = InvertedFront(normalizedFront[i]);
				}
				else
				{
					invertedFront[i] = new double[0][];
				}
			}

			// shift away from origin, so that boundary points also get a contribution > 0
			foreach (double[][] anInvertedFront in invertedFront)
			{
				foreach (double[] point in anInvertedFront)
				{
					for (int i = 0; i < point.Length; i++)
					{
						point[i] += offsets[i];
					}
				}
			}

			// calculate contributions 
			double[] contribution = new double[populations.Length];
			HyperVolume hyperVolume = new HyperVolume();

			for (int i = 0; i < populations.Length; i++)
			{
				if (invertedFront[i + 1].Length == 0)
				{
					contribution[i] = 0;
				}
				else
				{
					int auxSize = 0;
					for (int j = 0; j < populations.Length; j++)
					{
						if (j != i)
						{
							auxSize += invertedFront[j + 1].Length;
						}
					}

					if (size == archive.Size())
					{ // the contribution is the maximum hv
						contribution[i] = hyperVolume.CalculateHypervolume(invertedFront[0], invertedFront[0].Length, numberOfObjectives);
					}
					else
					{
						//make a front with all the populations but the target one
						int index = 0;
						double[][] aux = new double[auxSize][];
						for (int j = 0; j < populations.Length; j++)
						{
							if (j != i)
							{
								for (int k = 0; k < populations[j].Size(); k++)
								{
									aux[index++] = invertedFront[j + 1][k];
								}
							}
						}
						contribution[i] = hyperVolume.CalculateHypervolume(invertedFront[0], invertedFront[0].Length, numberOfObjectives)
								- hyperVolume.CalculateHypervolume(aux, aux.Length, numberOfObjectives);
					}
				}
			}

			return contribution;
		}

		#endregion
	}
}

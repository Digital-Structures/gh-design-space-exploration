using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using JMetalCSharp.Utils;
using System;
using System.IO;

namespace JMetalCSharp.Problems.MTSP
{
	/// <summary>
	/// Class representing a multi-objective TSP (Traveling Salesman Problem) problem.
	/// This class is tested with two objectives and the KROA150 and KROB150 
	/// instances of TSPLIB
	/// </summary>
	public class MTSP : Problem
	{
		public int numberOfCities;
		public double[][] distanceMatrix;
		public double[][] costMatrix;

		/// <summary>
		/// Creates a new mTSP problem instance. It accepts data files from TSPLIB
		/// </summary>
		/// <param name="solutionType"></param>
		/// <param name="file_distances"></param>
		/// <param name="file_cost"></param>
		public MTSP(string solutionType, string file_distances, string file_cost)
		{
			NumberOfVariables = 1;
			NumberOfObjectives = 2;
			NumberOfConstraints = 0;
			ProblemName = "mTSP";

			Length = new int[NumberOfVariables];

			distanceMatrix = ReadProblem(file_distances);
			costMatrix = ReadProblem(file_cost);
			Console.WriteLine(numberOfCities);
			Length[0] = numberOfCities;
			if (solutionType == "Permutation")
			{
				SolutionType = new PermutationSolutionType(this);
			}
			else
			{
				Console.WriteLine("Error: solution type " + solutionType + " is invalid");
				Logger.Log.Error("Error: solution type " + solutionType + " is invalid");
				Environment.Exit(-1);
			}
		}

		/// <summary>
		/// Evaluates a solution
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			double fitness1;
			double fitness2;

			fitness1 = 0.0;
			fitness2 = 0.0;

			for (int i = 0; i < (numberOfCities - 1); i++)
			{
				int x;
				int y;

				x = ((Permutation)solution.Variable[0]).Vector[i];
				y = ((Permutation)solution.Variable[0]).Vector[i + 1];

				fitness1 += distanceMatrix[x][y];
				fitness2 += costMatrix[x][y];
			}
			int firstCity;
			int lastCity;

			firstCity = ((Permutation)solution.Variable[0]).Vector[0];
			lastCity = ((Permutation)solution.Variable[0]).Vector[numberOfCities - 1];
			fitness1 += distanceMatrix[firstCity][lastCity];
			fitness2 += costMatrix[firstCity][lastCity];

			solution.Objective[0] = fitness1;
			solution.Objective[1] = fitness2;
		}

		public double[][] ReadProblem(string file)
		{
			double[][] matrix = null;

			using (StreamReader reader = new StreamReader(file))
			{
				string[] tokens = reader.ReadToEnd().Split(' ');
				try
				{
					int index;

					index = Array.IndexOf<string>(tokens, "DIMENSION");

					index += 2;

					numberOfCities = int.Parse(tokens[index]);

					matrix = new double[numberOfCities][];

					for (int i = 0; i < numberOfCities; i++)
					{
						matrix[i] = new double[numberOfCities];
					}

					index = Array.IndexOf<string>(tokens, "SECTION");

					// Read the data
					double[] c = new double[2 * numberOfCities];

					for (int i = 0; i < numberOfCities; i++)
					{
						index++;
						int j = int.Parse(tokens[index]);

						index++;
						c[2 * (j - 1)] = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(tokens[index]);

						index++;
						c[2 * (j - 1) + 1] = JMetalCSharp.Utils.Utils.ParseDoubleInvariant(tokens[index]);
					}

					double dist;
					for (int k = 0; k < numberOfCities; k++)
					{
						matrix[k][k] = 0;
						for (int j = k + 1; j < numberOfCities; j++)
						{
							dist = Math.Sqrt(Math.Pow((c[k * 2] - c[j * 2]), 2.0)
									+ Math.Pow((c[k * 2 + 1] - c[j * 2 + 1]), 2));
							dist = (int)(dist + .5);
							matrix[k][j] = dist;
							matrix[j][k] = dist;
						}
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("TSP.ReadProblem(): error when reading data file " + e);
					Environment.Exit(-1);
				}
			}
			return matrix;
		}
	}
}

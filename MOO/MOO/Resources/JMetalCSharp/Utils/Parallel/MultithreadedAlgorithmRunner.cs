using JMetalCSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMetalCSharp.Utils.Parallel
{
	public class MultithreadedAlgorithmRunner : ISynchronousParallelRunner
	{
		private List<Task<Algorithm>> taskList;
		public void StartParallelRunner(object configuration)
		{
			taskList = new List<Task<Algorithm>>();
		}

		public void AddTaskForExecution(object[] taskParameters)
		{
			IEnumerable<Algorithm> algorithms = taskParameters.OfType<Algorithm>();

			foreach (var algorithm in algorithms)
			{
				taskList.Add(new Task<Algorithm>(() =>
					{
						algorithm.Execute();
						return algorithm;
					}));
			}
		}

		public object ParallelExecution()
		{
			try
			{
				foreach (var task in taskList)
				{
					task.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Error in MultithreadedAlgorithmRunner.ParallelExecution", ex);
				Console.Error.WriteLine(ex.StackTrace);
			}

			Task.WaitAll(taskList.ToArray());

			List<Algorithm> algorithmsList = new List<Algorithm>();

			foreach (Task<Algorithm> task in taskList)
			{
				Algorithm algorithm = null;
				try
				{
					algorithm = task.Result;
					algorithmsList.Add(algorithm);
				}
				catch (Exception ex)
				{
					Logger.Log.Error("Error in MultithreadedAlgorithmRunner.ParallelExecution", ex);
					Console.Error.WriteLine(ex.StackTrace);
				}
			}

			taskList = null;
			return algorithmsList;
		}
	}
}

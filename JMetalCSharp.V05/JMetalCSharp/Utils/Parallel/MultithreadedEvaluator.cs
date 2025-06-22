using JMetalCSharp.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JMetalCSharp.Utils.Parallel
{
	/// <summary>
	/// @author Antonio J. Nebro Class for evaluating solutions in parallel using
	/// threads
	/// </summary>
	public class MultithreadedEvaluator : ISynchronousParallelRunner
	{
		private Problem problem;
		//private List<EvaluationTask> taskList;
		private List<Task<Solution>> taskList;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="threads"></param>
		public MultithreadedEvaluator()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to Solve</param>
		public void StartParallelRunner(object problem)
		{
			this.problem = (Problem)problem;
			taskList = null;
		}

		/// <summary>
		/// Adds a solution to be evaluated to a list of tasks
		/// </summary>
		/// <param name="taskParameters"></param>
		public void AddTaskForExecution(object[] taskParameters)
		{
			Solution solution = (Solution)taskParameters[0];
			if (taskList == null)
			{
				taskList = new List<Task<Solution>>();
			}

			taskList.Add(new Task<Solution>(() =>
			{
				problem.Evaluate(solution);
				problem.EvaluateConstraints(solution);

				return solution;
			}));
		}

		/// <summary>
		/// Evaluates a list of solutions
		/// </summary>
		/// <returns>A list with the evaluated solutions</returns>
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
				Logger.Log.Error("Error in MultithreadedEvaluator.ParallelExecution", ex);
				Console.Error.WriteLine(ex.StackTrace);
			}

			Task.WaitAll(taskList.ToArray());

			List<Solution> solutionList = new List<Solution>();

			foreach (Task<Solution> task in taskList)
			{
				Solution solution = null;
				try
				{
					solution = task.Result;
					solutionList.Add(solution);
				}
				catch (Exception ex)
				{
					Logger.Log.Error("Error in MultithreadedEvaluator.ParallelExecution", ex);
					Console.Error.WriteLine(ex.StackTrace);
				}
			}
			taskList = null;
			return solutionList;
		}
	}
}

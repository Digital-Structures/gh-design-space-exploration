namespace JMetalCSharp.Utils.Parallel
{
	public interface ISynchronousParallelRunner
	{
		void StartParallelRunner(object configuration);

		void AddTaskForExecution(object[] taskParameters);

		object ParallelExecution();
	}
}

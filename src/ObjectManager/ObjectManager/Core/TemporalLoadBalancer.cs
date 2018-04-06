using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OA.Core
{
    /// <summary>
    /// Distributes work (the execution of coroutines) over several frames to avoid freezes by soft-limiting execution time.
    /// </summary>
    public class TemporalLoadBalancer
    {
        List<IEnumerator> tasks = new List<IEnumerator>();
        Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Adds a task coroutine and returns it.
        /// </summary>
        public IEnumerator AddTask(IEnumerator taskCoroutine)
        {
            tasks.Add(taskCoroutine);
            return taskCoroutine;
        }

        public void CancelTask(IEnumerator taskCoroutine)
        {
            tasks.Remove(taskCoroutine);
        }

        public void RunTasks(float desiredWorkTime)
        {
            Debug.Assert(desiredWorkTime >= 0);
            if (tasks.Count == 0)
                return;
            stopwatch.Reset();
            stopwatch.Start();
            // Run the tasks.
            do
            {
                if (!tasks[0].MoveNext()) // Try to execute an iteration of a task. Remove the task if it's execution has completed.
                    tasks.RemoveAt(0);
            } while ((tasks.Count > 0) && (stopwatch.Elapsed.TotalSeconds < desiredWorkTime));
            stopwatch.Stop();
        }

        public void WaitForTask(IEnumerator taskCoroutine)
        {
            Debug.Assert(tasks.Contains(taskCoroutine));
            while (taskCoroutine.MoveNext()) { }
            tasks.Remove(taskCoroutine);
        }

        public void WaitForAllTasks()
        {
            foreach (var task in tasks)
                while (task.MoveNext()) { }
            tasks.Clear();
        }
    }
}
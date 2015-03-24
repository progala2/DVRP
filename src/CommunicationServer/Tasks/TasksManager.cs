using System;
using _15pl04.Ucc.Commons.Problem;

namespace _15pl04.Ucc.CommunicationServer.Tasks
{
    internal sealed class TasksManager
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static TasksManager Instance
        {
            get { return _lazy.Value; }
        }

        private static readonly Lazy<TasksManager> _lazy = new Lazy<TasksManager>(() => new TasksManager());

        
        //fullproblems to divide
        //fullproblems being divided
        //fullproblems to merge
        //fullproblems being merged
        
        //final solutions

        //partialproblems to compute
        //partialproblems being computed
        //partialproblems to merge
        //partialproblems being merged

        private TasksManager()
        {
            
        }


    }
}

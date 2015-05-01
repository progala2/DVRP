using System;

namespace _15pl04.Ucc.TaskSolver
{
    public class DvrpTaskSolver : UCCTaskSolver.TaskSolver
    {
        public DvrpTaskSolver(byte[] problemData)
            : base(problemData)
        {
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            throw new NotImplementedException();
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            throw new NotImplementedException();
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}
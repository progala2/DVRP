namespace _15pl04.Ucc.TaskSolver
{
    public class DvrpTaskSolver : UCCTaskSolver.TaskSolver
    {
        // TODO

        public DvrpTaskSolver(byte[] problemData)
            : base(problemData)
        { }


        public override byte[][] DivideProblem(int threadCount)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            throw new System.NotImplementedException();
        }

        public override string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public override byte[] Solve(byte[] partialData, System.TimeSpan timeout)
        {
            throw new System.NotImplementedException();
        }
    }
}

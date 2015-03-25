
namespace _15pl04.Ucc.CommunicationServer.Tasks
{
    public enum ProblemInstanceState
    {
        AwaitingDivision = 1,
        BeingDivided,
        AwaitingSolution
    }

    public enum PartialProblemState
    {
        AwaitingComputation = 1,
        BeingComputed
    }

    public enum PartialSolutionState
    {
        BeingGathered = 1,
        AwaitingMerge,
        BeingMerged
    }
}

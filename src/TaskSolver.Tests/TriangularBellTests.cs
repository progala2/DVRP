using System.Diagnostics;
using Dvrp.Ucc.TaskSolver.DvrpAlgorithm;
using Xunit.Abstractions;

namespace Dvrp.Ucc.TaskSolver.Tests;

public class TriangularBellTests
{
	private readonly ITestOutputHelper _testOutputHelper;

	public TriangularBellTests(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	[Theory]
	[InlineData(1, 1)]
	[InlineData(2, 2)]
	[InlineData(3, 5)]
	[InlineData(4, 15)]
	[InlineData(13, 27644437)]
	public void CalculatingNumberOfCombination(int x, ulong expected)
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		_testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem created ");
		Assert.Equal(expected, DvrpDivider.TriangularMethodBellNumber(x));
		stopwatch.Stop();
		_testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem finished ");
	}
}
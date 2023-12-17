namespace AdventOfCode.Tests;

public class TestDay17
{
    [Theory]
    [InlineData(typeof(Day17), RunMode.Test, "102", "94")]
    [InlineData(typeof(Day17), RunMode.Real, "963", "1178")]
    public async Task Day17_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
    {
        if (Activator.CreateInstance(type, runMode) is BaseTestableDay instance)
        {
            (await instance.Solve_1()).Should().Be(expectedPart1);
            (await instance.Solve_2()).Should().Be(expectedPart2);
        }
        else
        {
            Assert.Fail($"{type} is not a BaseProblem");
        }
    }
}

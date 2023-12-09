namespace AdventOfCode.Tests;

public class TestDay09
{
    [Theory]
    [InlineData(typeof(Day09), RunMode.Test, "114", "2")]
    [InlineData(typeof(Day09), RunMode.Real, "1819125966", "1140")]
    public async Task Day9_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

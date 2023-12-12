namespace AdventOfCode.Tests;

public class TestDay12
{
    [Theory]
    [InlineData(typeof(Day12), RunMode.Test, "21", "525152")]
    [InlineData(typeof(Day12), RunMode.Real, "6871", "2043098029844")]
    public async Task Day11_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

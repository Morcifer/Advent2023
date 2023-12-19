namespace AdventOfCode.Tests;

public class TestDay19
{
    [Theory]
    [InlineData(typeof(Day19), RunMode.Test, "19114", "167409079868000")]
    [InlineData(typeof(Day19), RunMode.Real, "406849", "138625360533574")]
    public async Task Day19_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

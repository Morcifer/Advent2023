namespace AdventOfCode.Tests;

public class TestDay23
{
    [Theory]
    [InlineData(typeof(Day23), RunMode.Test, "94", "154")]
    [InlineData(typeof(Day23), RunMode.Real, "2394", "-1")]
    public async Task Day23_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

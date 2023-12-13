namespace AdventOfCode.Tests;

public class TestDay13
{
    [Theory]
    [InlineData(typeof(Day13), RunMode.Test, "405", "400")]
    [InlineData(typeof(Day13), RunMode.Real, "31956", "37617")]
    public async Task Day13_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

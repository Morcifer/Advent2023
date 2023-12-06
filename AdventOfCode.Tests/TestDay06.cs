namespace AdventOfCode.Tests;

public class TestDay06
{
    [Theory]
    [InlineData(typeof(Day06), RunMode.Test, "288", "71503")]
    [InlineData(typeof(Day06), RunMode.Real, "861300", "28101347")]
    public async Task Day6_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

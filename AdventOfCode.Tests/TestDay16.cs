namespace AdventOfCode.Tests;

public class TestDay16
{
    [Theory]
    [InlineData(typeof(Day16), RunMode.Test, "46", "51")]
    [InlineData(typeof(Day16), RunMode.Real, "8551", "8754")]
    public async Task Day16_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

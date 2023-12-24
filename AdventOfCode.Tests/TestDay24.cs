namespace AdventOfCode.Tests;

public class TestDay24
{
    [Theory]
    [InlineData(typeof(Day24), RunMode.Test, "2", "47")]
    [InlineData(typeof(Day24), RunMode.Real, "17776", "948978092202212")]
    public async Task Day24_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

namespace AdventOfCode.Tests;

public class TestDay20
{
    [Theory]
    [InlineData(typeof(Day20), RunMode.Test, "11687500", "-1")]
    [InlineData(typeof(Day20), RunMode.Real, "737679780", "227411378431763")]
    public async Task Day20_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

namespace AdventOfCode.Tests;

public class TestDay22
{
    [Theory]
    [InlineData(typeof(Day22), RunMode.Test, "5", "-1")]
    [InlineData(typeof(Day22), RunMode.Real, "459", "-1")]
    public async Task Day22_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

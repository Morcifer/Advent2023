namespace AdventOfCode.Tests;

public class TestDay18
{
    [Theory]
    [InlineData(typeof(Day18), RunMode.Test, "62", "952408144115")]
    [InlineData(typeof(Day18), RunMode.Real, "48503", "148442153147147")]
    public async Task Day18_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

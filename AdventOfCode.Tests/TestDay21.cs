namespace AdventOfCode.Tests;

public class TestDay21
{
    [Theory]
    [InlineData(typeof(Day21), RunMode.Test, 6, 16)]
    [InlineData(typeof(Day21), RunMode.Test, 10, 50)]
    [InlineData(typeof(Day21), RunMode.Test, 50, 1594)]
    [InlineData(typeof(Day21), RunMode.Test, 100, 6536)]
    [InlineData(typeof(Day21), RunMode.Test, 500, 167004)]
    [InlineData(typeof(Day21), RunMode.Test, 1000, 668697)]
    [InlineData(typeof(Day21), RunMode.Test, 5000, 16733044)]
    public void Day21_Calculation(Type type, RunMode runMode, int steps, int expectedResult)
    {
        if (Activator.CreateInstance(type, runMode) is Day21 instance)
        {
            instance.RandomWalk(steps).Value.Should().Be(expectedResult.ToString());
        }
        else
        {
            Assert.Fail($"{type} is not a BaseProblem");
        }
    }

    [Theory]
    [InlineData(typeof(Day21), RunMode.Test, "16", "16733044")]
    [InlineData(typeof(Day21), RunMode.Real, "3660", "605492675373144")]
    public async Task Day21_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

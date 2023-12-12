namespace AdventOfCode.Tests;

public class TestDay12
{
    //[Theory]
    //[InlineData(typeof(Day11), RunMode.Test, 2, 374)]
    //[InlineData(typeof(Day11), RunMode.Real, 2, 9312968)]
    //[InlineData(typeof(Day11), RunMode.Test, 10, 1030)]
    //[InlineData(typeof(Day11), RunMode.Test, 100, 8410)]
    //public void Day11_Calculation(Type type, RunMode runMode, int age, int expectedResult)
    //{
    //    if (Activator.CreateInstance(type, runMode) is Day11 instance)
    //    {
    //        instance.Calculate(age).Value.Should().Be(expectedResult.ToString());
    //    }
    //    else
    //    {
    //        Assert.Fail($"{type} is not a BaseProblem");
    //    }
    //}

    [Theory]
    [InlineData(typeof(Day12), RunMode.Test, "21", "525152")]
    [InlineData(typeof(Day12), RunMode.Real, "6871", "-1")]
    public async Task Day11_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

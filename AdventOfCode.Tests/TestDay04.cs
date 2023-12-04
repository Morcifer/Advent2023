namespace AdventOfCode.Tests
{
    public class TestDay04
    {
        [Theory]
        [InlineData(typeof(Day04), RunMode.Test, "13", "30")]
        [InlineData(typeof(Day04), RunMode.Real, "24175", "18846301")]
        public async Task Day4_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
}

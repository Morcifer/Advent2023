namespace AdventOfCode.Tests
{
    public class TestDay03
    {
        [Theory]
        [InlineData(typeof(Day03), RunMode.Test, "4361", "467835")]
        [InlineData(typeof(Day03), RunMode.Real, "527364", "79026871")]
        public async Task Day3_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

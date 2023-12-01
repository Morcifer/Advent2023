namespace AdventOfCode.Tests
{
    public class TestDay01
    {
        [Theory]
        [InlineData("1abc2", 12)]
        [InlineData("pqr3stu8vwx", 38)]
        [InlineData("a1b2c3d4e5f", 15)]
        [InlineData("treb7uchet", 77)]
        public void Day1_Part1_SingleRowCalculation(string text, int expected)
        {
            Day01.CalculateSingleDigitCalibrationValue(text).Should().Be(expected);
        }

        [Theory]
        [InlineData("two1nine", 29)]
        [InlineData("eightwothree", 83)]
        [InlineData("abcone2threexyz", 13)]
        [InlineData("xtwone3four", 24)]
        [InlineData("4nineeightseven2", 42)]
        [InlineData("zoneight234", 14)]
        [InlineData("7pqrstsixteen", 76)]
        public void Day1_Part2_SingleRowCalculation(string text, int expected)
        {
            Day01.CalculateSingleLiteralCalibrationValue(text).Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(Day01), RunMode.Test, "142", "281")]
        [InlineData(typeof(Day01), RunMode.Real, "54968", "54094")]
        public async Task Day1_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

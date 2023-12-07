using static AdventOfCode.Day07;

namespace AdventOfCode.Tests;

public class TestDay07
{
    [Theory]
    [InlineData(HandType.FullHouse, HandType.ThreeOfAKind, -1)]
    [InlineData(HandType.FullHouse, HandType.FullHouse, 0)]
    public void Day7_CompareDeckEnum(HandType handType1, HandType handType2, int expected)
    {
        handType1.CompareTo(handType2).Should().Be(expected);
        handType2.CompareTo(handType1).Should().Be(-expected);
    }

    [Theory]
    [InlineData("32T3K 1", "KK677 1", -1)]
    public void Day7_Part2_SingleRowCalculation(string hand1, string hand2, int expected)
    {
        ConvertTextToHand(hand1).CompareTo(ConvertTextToHand(hand2)).Should().Be(expected);
        ConvertTextToHand(hand2).CompareTo(ConvertTextToHand(hand1)).Should().Be(-expected);
    }

    [Theory]
    [InlineData(typeof(Day07), RunMode.Test, "6440", "5905")]
    [InlineData(typeof(Day07), RunMode.Real, "248559379", "249631254")]
    public async Task Day7_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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

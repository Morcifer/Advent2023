using System.Runtime.InteropServices;
using MoreLinq;

namespace AdventOfCode;

public sealed class Day07 : BaseTestableDay
{
    private readonly List<(Hand OriginalHand, Hand BestJokerHand, int Bid)> _input;

    public Day07() : this(RunMode.Real)
    {
    }

    public Day07(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(ConvertTextToHandsAndBid)
            .ToList();
    }

    public enum HandType
    {
        FiveOfAKind = 1,
        FourOfAKind = 2,
        FullHouse = 3,
        ThreeOfAKind = 4,
        TwoPairs = 5,
        OnePair = 6,
        HighCard = 7,
    }

    public class Hand : IComparable
    {
        private readonly List<int> _cards;
        private readonly List<int> _handTypeCards;

        public Hand(string cardString, string jokerCardString, bool jokerMode)
        {
            _cards = TranslateCardString(cardString, jokerMode ? "1" : "11");
            _handTypeCards = TranslateCardString(jokerCardString, "11"); // Since these aren't used for tie-breaker, the value can be consistent with part 1.
        }

        private static List<int> TranslateCardString(string cardString, string jValue)
        {
            return cardString
                .ToCharArray()
                .Select(c => c.ToString())
                .Select(s => s == "T" ? "10" : s)
                .Select(s => s == "J" ? jValue : s)
                .Select(s => s == "Q" ? "12" : s)
                .Select(s => s == "K" ? "13" : s)
                .Select(s => s == "A" ? "14" : s)
                .Select(int.Parse)
                .ToList();
        }

        private HandType GetHandType()
        {
            var counts = _handTypeCards.GroupBy(c => c).Select(g => g.Count()).ToList();

            return counts.Count switch
            {
                1 => HandType.FiveOfAKind,
                2 => counts.Contains(4) ? HandType.FourOfAKind : HandType.FullHouse,
                3 => counts.Contains(3) ? HandType.ThreeOfAKind : HandType.TwoPairs,
                4 => HandType.OnePair,
                _ => HandType.HighCard,
            };
        }

        public int CompareTo(object? other)
        {
            if (other is not Hand otherHand)
            {
                throw new ArgumentNullException();
            }

            var thisHandType = GetHandType();
            var otherHandType = otherHand.GetHandType();

            if (thisHandType != otherHandType)
            {
                return otherHandType.CompareTo(thisHandType); // The enum numbers are reversed
            }

            foreach (var (thisCard, otherCard) in _cards.Zip(otherHand._cards))
            {
                if (thisCard == otherCard)
                {
                    continue;
                }

                return thisCard.CompareTo(otherCard);
            }

            return 0; // Shouldn't happen!
        }
    }

    internal static (Hand, Hand, int) ConvertTextToHandsAndBid(string text)
    {
        // text = "32T3K 765" => ["32T3K", "765"]
        var split = text.Split(' ').Select(s => s.Trim()).ToList();

        var handString = split[0];
        var bidString = split[1];

        var originalHand = new Hand(handString, handString, jokerMode: false);

        // Find all possible other hands, and choose the best one.
        var replacements = handString
            .ToCharArray()
            .GroupBy(c => c)
            .Select(c => c.Key)
            .Concat(new List<char> { 'A', 'J' })
            .Distinct()
            .ToList();

        var alternativeHands = (handString[0] == 'J' ? replacements : new List<char> { handString[0] })
            .Cartesian(handString[1] == 'J' ? replacements : new List<char> { handString[1] }, (c0, c1) => (c0, c1))
            .Cartesian(handString[2] == 'J' ? replacements : new List<char> { handString[2] }, (x, c2) => (x.c0, x.c1, c2))
            .Cartesian(handString[3] == 'J' ? replacements : new List<char> { handString[3] }, (x, c3) => (x.c0, x.c1, x.c2, c3))
            .Cartesian(handString[4] == 'J' ? replacements : new List<char> { handString[4] }, (x, c4) => (x.c0, x.c1, x.c2, x.c3, c4))
            .Select(x => $"{x.c0}{x.c1}{x.c2}{x.c3}{x.c4}")
            .ToList();

        var bestJokerHand = alternativeHands
            .Select(jokerString => new Hand(handString, jokerString, jokerMode: true))
            .Aggregate((bestThusFar, next) => bestThusFar.CompareTo(next) == 1 ? bestThusFar : next); // Yuck.

        return (originalHand, bestJokerHand, int.Parse(bidString));
    }

    private Answer CalculateWinnings()
    {
        var cards = _input
            .Select(x => (x.OriginalHand, x.Bid))
            .ToList();

        cards.Sort();

        return cards
            .Enumerate()
            .Select(x => (x.Index + 1) * x.Value.Bid)
            .Sum();
    }

    private Answer CalculateJackWinnings()
    {
        var cards = _input
            .Select(x => (x.BestJokerHand, x.Bid))
            .ToList();

        cards.Sort(); // Smart C# sorting knows how to handle tuples, well done C#, you're almost as sugary as python!

        return cards
            .Enumerate()
            .Select(x => (x.Index + 1) * x.Value.Bid)
            .Sum();
    }

    public override ValueTask<string> Solve_1() => CalculateWinnings();

    public override ValueTask<string> Solve_2() => CalculateJackWinnings();
}

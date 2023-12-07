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
        private readonly bool _jokerMode;
        private readonly List<int> _cards;

        public Hand(string cardString, bool jokerMode)
        {
            _jokerMode = jokerMode;
            _cards = TranslateCardString(cardString, jokerMode ? "1" : "11");
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
            if (_jokerMode)
            {
                var counts = _cards.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
                var jokers = counts.GetValueOrDefault(1, 0);

                var betterCounts = counts.Where(kvp => kvp.Key != 1).Select(kvp => kvp.Value).ToList();

                if (betterCounts.Count == 0)
                {
                    return HandType.FiveOfAKind;
                }

                var maxCountIndex = betterCounts.Enumerate().MaxBy(x => x.Value).Index;
                betterCounts[maxCountIndex] += jokers;

                return HandTypeFromCounts(betterCounts);
            }
            else
            {
                var counts = _cards.GroupBy(c => c).Select(g => g.Count()).ToList();
                return HandTypeFromCounts(counts);
            }

        }

        private HandType HandTypeFromCounts(List<int> counts)
        {
            return counts.Max() switch
            {
                5 => HandType.FiveOfAKind,
                4 => HandType.FourOfAKind,
                3 => counts.Contains(2) ? HandType.FullHouse : HandType.ThreeOfAKind,
                2 => counts.Count == 3 ? HandType.TwoPairs : HandType.OnePair,
                1 => HandType.HighCard,
                _ => throw new ArgumentException("What did you do?!"),
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

        var originalHand = new Hand(handString, jokerMode: false);
        var jokerHand = new Hand(handString, jokerMode: true);

        return (originalHand, jokerHand, int.Parse(bidString));
    }

    private Answer CalculateWinnings()
    {
        return _input
            .Select(x => (x.OriginalHand, x.Bid))
            .OrderBy(hands => hands)
            .Enumerate()
            .Select(x => (x.Index + 1) * x.Value.Bid)
            .Sum();
    }

    private Answer CalculateJackWinnings()
    {
        return _input
            .Select(x => (x.BestJokerHand, x.Bid))
            .OrderBy(hands => hands)
            .Enumerate()
            .Select(x => (x.Index + 1) * x.Value.Bid)
            .Sum();
    }

    public override ValueTask<string> Solve_1() => CalculateWinnings();

    public override ValueTask<string> Solve_2() => CalculateJackWinnings();
}

namespace AdventOfCode;

public sealed class Day07 : BaseTestableDay
{
    private readonly List<(Hand, Hand)> _input;

    public Day07() : this(RunMode.Real)
    {
    }

    public Day07(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(ConvertTextToHands)
            .ToList();
    }

    public enum HandType
    {
        FiveOfAKind = 1,
        FourOfAKind = 2,
        FullHouse = 3,
        ThreeOfAKind = 4,
        TwoPairs  = 5,
        OnePair = 6,
        HighCard = 7,
    }

    public class Hand : IComparable
    {
        public required int Bid;
        public required List<int> Cards;
        public required List<int> OriginalCards;

        public void HackTheJacks(List<int> originalCards)
        {
            OriginalCards = originalCards.Select(n => n == 11 ? 1 : n).ToList();
            Cards = Cards.Select(n => n == 11 ? 1 : n).ToList();
        }

        public HandType GetHandType()
        {
            var counter = Cards.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            var counterCounter = counter.Values.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());

            if (counterCounter.ContainsKey(5))
            {
                return HandType.FiveOfAKind;
            }

            if (counterCounter.ContainsKey(4))
            {
                return HandType.FourOfAKind;
            }

            if (counterCounter.ContainsKey(3))
            {
                if (counterCounter.ContainsKey(2))
                {
                    return HandType.FullHouse;
                }

                return HandType.ThreeOfAKind;
            }

            if (counterCounter.Count == 1)
            {
                return HandType.HighCard;
            }

            return counterCounter[2] == 1 ? HandType.OnePair : HandType.TwoPairs;
        }

        public int CompareTo(object other)
        {
            Hand otherHand = other as Hand;

            var thisHandType = GetHandType();
            var otherHandType = otherHand.GetHandType();

            if (thisHandType == otherHandType)
            {
                foreach (var (thisCard, otherCard) in OriginalCards.Zip(otherHand.OriginalCards))
                {
                    if (thisCard == otherCard)
                    {
                        continue;
                    }

                    return thisCard.CompareTo(otherCard);
                }
            }

            return otherHandType.CompareTo(thisHandType); // The enum numbers are reversed
        }
    }

    internal static Hand ConvertTextToHand(string text)
    {
        // text = "32T3K 765"
        var split = text.Split(' ').Select(s => s.Trim()).ToList();

        var cards = split[0]
            .ToCharArray()
            .Select(c => c.ToString())
            .Select(s => s == "T" ? "10" : s)
            .Select(s => s == "J" ? "11" : s)
            .Select(s => s == "Q" ? "12" : s)
            .Select(s => s == "K" ? "13" : s)
            .Select(s => s == "A" ? "14" : s)
            .Select(int.Parse)
            .ToList();
            
        return new Hand
        {
            Bid = int.Parse(split[1]),
            Cards = cards,
            OriginalCards = cards,
        };
    }

    internal static (Hand, Hand) ConvertTextToHands(string text)
    {
        var strings = new List<string>() { text };

        var cards = text[..5].ToCharArray().GroupBy(c => c).Select(c => c.Key).ToList();
        cards.Add('A');
        if (!cards.Contains('J'))
        {
            cards.Add('J');
        }

        //var cards = new List<char>() { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

        foreach (var newCard0 in (text[0] == 'J' ? cards : new List<char>() { text[0] } ))
        {
            foreach (var newCard1 in (text[1] == 'J' ? cards : new List<char>() { text[1] }))
            {
                foreach (var newCard2 in (text[2] == 'J' ? cards : new List<char>() { text[2] }))
                {
                    foreach (var newCard3 in (text[3] == 'J' ? cards : new List<char>() { text[3] }))
                    {
                        foreach (var newCard4 in (text[4] == 'J' ? cards : new List<char>() { text[4] }))
                        {
                            var c0 = text[0] == 'J' ? newCard0 : text[0];
                            var c1 = text[1] == 'J' ? newCard1 : text[1];
                            var c2 = text[2] == 'J' ? newCard2 : text[2];
                            var c3 = text[3] == 'J' ? newCard3 : text[3];
                            var c4 = text[4] == 'J' ? newCard4 : text[4];

                            strings.Add($"{c0}{c1}{c2}{c3}{c4}{text[5..]}");
                        }
                    }
                }
            }
            //strings.Add(text.Replace("J", newCard0));
        }

        var originalHand = ConvertTextToHand(strings[0]);

        var hands = strings.Select(ConvertTextToHand).ToList();
        hands.ForEach(h => h.HackTheJacks(originalHand.Cards));
        var bestHand = hands.Aggregate((agg, next) => agg.CompareTo(next) == 1 ? agg : next);

        return (originalHand, bestHand);
    }

    private Answer CalculateWinnings()
    {
        var cards = _input
            .Select(x => x.Item1)
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
            .Select(x => x.Item2)
            .ToList();

        cards.Sort();

        return cards
            .Enumerate()
            .Select(x => (x.Index + 1) * x.Value.Bid)
            .Sum();
    }

    public override ValueTask<string> Solve_1() => CalculateWinnings();

    public override ValueTask<string> Solve_2() => CalculateJackWinnings();
}

namespace AdventOfCode;

public sealed class Day04 : BaseTestableDay
{
    private readonly List<Card> _input;

    public class Card
    {
        public required int Id;
        public required HashSet<int> Winning;
        public required HashSet<int> Have;

        public int NumberMatches()
        {
            return Winning.Intersect(Have).ToList().Count;
        }
    }

    public Day04() : this(RunMode.Test)
    {
    }

    public Day04(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(ConvertTextToCard)
            .ToList();
    }

    private Card ConvertTextToCard(string text)
    {
        // text = "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53"
        var splitByColon = text.Split(':').Select(s => s.Trim()).ToList();
        var cardId = int.Parse(splitByColon[0][5..]);

        // numbers = "41 48 83 86 17 | 83 86  6 31 17  9 48 53"
        var numbers = splitByColon[1]
            .Split('|')
            .Select(s => s.Trim())
            .ToList();

        return new Card
        {
            Id = cardId,
            Winning = numbers[0] // "41 48 83 86 17"
                .Split(' ')
                .Select(s => s.Trim())
                .Where(s => s != "")
                .Select(int.Parse)
                .ToHashSet(),
            Have = numbers[1] // "83 86  6 31 17  9 48 53"
                .Split(' ')
                .Select(s => s.Trim())
                .Where(s => s != "")
                .Select(int.Parse)
                .ToHashSet(),
        };
    }

    private Answer CalculateCardPoints()
    {
        return _input
            .Select(card => card.NumberMatches())
            .Select(matches => matches > 0 ? (int)Math.Pow(2, matches - 1) : 0)
            .Sum();
    }

    private Answer CalculateCardNumber()
    {
        var cardIds = _input.Select(card => card.Id).ToHashSet();
        var cardScores = _input.ToDictionary(card => card.Id, card => card.NumberMatches());
        var cardAmounts = _input.ToDictionary(card => card.Id, _ => 1);

        foreach (var card in _input)
        {
            var score = cardScores[card.Id];
            var amount = cardAmounts[card.Id];

            foreach (var cardsAhead in Enumerable.Range(1, score))
            {
                if (!cardIds.Contains(card.Id + cardsAhead))
                {
                    break;
                }

                cardAmounts[card.Id + cardsAhead] += amount;
            }
        }

        return cardAmounts.Values.Sum();
    }

    public override ValueTask<string> Solve_1() => CalculateCardPoints();

    public override ValueTask<string> Solve_2() => CalculateCardNumber();
}
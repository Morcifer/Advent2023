using System.Globalization;
using MoreLinq;

namespace AdventOfCode;

public sealed class Day18 : BaseTestableDay
{
    private readonly List<(char Direction, int Steps, string Hex)> _input;

    public Day18() : this(RunMode.Real)
    {
    }

    public Day18(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(ConvertText)
            .ToList();
    }

    private (char, int, string) ConvertText(string text)
    {
        var split = text.Split(' ');
        return (split[0][0], int.Parse(split[1]), split[2][1..^1]);
    }

    private Answer DigBoysDig(List<(char Direction, int Steps)> moves)
    {
        var current = (Row: 0, Column: 0);
        var points = new List<(long Row, long Column)>() { current };

        foreach (var move in moves)
        {
            current = move.Direction switch
            {
                'R' => (current.Row, current.Column + move.Steps),
                'D' => (current.Row + move.Steps, current.Column),
                'L' => (current.Row, current.Column - move.Steps),
                'U' => (current.Row - move.Steps, current.Column),
                _ => throw new ArgumentException("Are you sure you're in the right place?"),
            };

            points.Add(current);
        }

        // Now we shoe-lace, like I should have on day 10 but was too lazy to understand the formula.
        var plus = points.Pairwise((p1, p2) => p1.Column * p2.Row).Sum();
        var minus = points.Pairwise((p1, p2) => p1.Row * p2.Column).Sum();
        var perimeter = points.Pairwise((p1, p2) => Math.Abs(p1.Row - p2.Row) + Math.Abs(p1.Column - p2.Column)).Sum();

        return (plus - minus + perimeter) / 2 + 1;
    }

    private Answer CalculatePart1Answer()
    {
        return DigBoysDig(_input.Select(x => (x.Direction, x.Steps)).ToList());
    }

    private (char Direction, int Steps) ConvertHex(string text)
    {
        var direction = text[^1] switch
        {
            '0' => 'R',
            '1' => 'D',
            '2' => 'L',
            '3' => 'U',
            _ => throw new ArgumentException("Hello fellow developer"),
        };

        return (
            direction,
            int.Parse(text[1..^1], NumberStyles.HexNumber)
        );
    }

    private Answer CalculatePart2Answer()
    {
        return DigBoysDig(_input.Select(x => ConvertHex(x.Hex)).ToList());
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

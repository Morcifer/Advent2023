using System.Globalization;

namespace AdventOfCode;

public sealed class Day18 : BaseTestableDay
{
    private readonly List<(char Direction, int Steps, string Hex)> _input;

    public Day18() : this(RunMode.Test)
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

    private Answer CalculatePart1Answer()
    {
        var colors = new Dictionary<(int Row, int Column), string>();

        var current = (Row: 0, Column: 0);
        colors[current] = "#FFFFFF";

        foreach (var move in _input)
        {
            foreach (var _ in Enumerable.Range(1, move.Steps))
            {
                current = move.Direction switch
                {
                    'R' => (current.Row, current.Column + 1),
                    'D' => (current.Row + 1, current.Column),
                    'L' => (current.Row, current.Column - 1),
                    'U' => (current.Row - 1, current.Column),
                    _ => throw new ArgumentException("Are you sure you're in the right place?"),
                };

                colors[current] = move.Hex;
            }
        }

        // Now you flood-fill again, but this time on the inside.
        var queue = new Queue<(int Row, int Column)>();
        queue.Enqueue((1, 1));

        while (queue.Count > 0)
        {
            var (cRow, cColumn) = queue.Dequeue();

            if (colors.ContainsKey((cRow, cColumn)))
            {
                continue;
            }

            colors[(cRow, cColumn)] = "#FFFFFF";

            queue.Enqueue((Row: cRow, cColumn + 1));
            queue.Enqueue((Row: cRow, cColumn - 1));
            queue.Enqueue((cRow + 1, Column: cColumn));
            queue.Enqueue((cRow - 1, Column: cColumn));
        }

        return colors.Count;
    }

    private (char, int) ConvertHex(string text)
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
        var inputs = _input.Select(x => ConvertHex(x.Hex)).ToList();
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

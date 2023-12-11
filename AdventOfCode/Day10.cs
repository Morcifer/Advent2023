namespace AdventOfCode;

public sealed class Day10 : BaseTestableDay
{
    private readonly List<string> _input;

    public Day10() : this(RunMode.Real)
    {
    }

    public Day10(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .ToList();
    }

    private List<(int, int)> CalculateLoop()
    {
        var startingRowIndex = _input
            .Enumerate()
            .First(x => x.Value.Contains("S"))
            .Index;

        var startingColumnIndex = _input[startingRowIndex]
            .Enumerate()
            .First(x => x.Value == 'S')
            .Index;

        var startingPoint = (startingRowIndex, startingColumnIndex);
        var path = new List<(int, int)>() { startingPoint };

        var previousPoint = startingPoint;
        (int, int) currentPoint;

        // Specifically next point from starting is annoying.
        if ("|F7".Contains(_input[startingRowIndex + 1][startingColumnIndex])) // Up
        {
            currentPoint = (startingRowIndex + 1, startingColumnIndex);
        }
        else if ("-7J".Contains(_input[startingRowIndex][startingColumnIndex + 1])) // Right
        {
            currentPoint = (startingRowIndex, startingColumnIndex + 1);
        }
        else // Might as well go down, then.
        {
            currentPoint = (startingRowIndex - 1, startingColumnIndex);
        }

        while (currentPoint != startingPoint)
        {
            path.Add(currentPoint);
            var (cRow, cColumn) = currentPoint;

            var newCurrentPoint = _input[cRow][cColumn] switch
            {
                '|' => previousPoint != (cRow - 1, cColumn) ? (cRow - 1, cColumn) : (cRow + 1, cColumn), // Up or down
                '-' => previousPoint != (cRow, cColumn - 1) ? (cRow, cColumn - 1) : (cRow, cColumn + 1), // Left or right
                'L' => previousPoint != (cRow - 1, cColumn) ? (cRow - 1, cColumn) : (cRow, cColumn + 1), // Up or right
                'J' => previousPoint != (cRow - 1, cColumn) ? (cRow - 1, cColumn) : (cRow, cColumn - 1), // Up or left
                '7' => previousPoint != (cRow + 1, cColumn) ? (cRow + 1, cColumn) : (cRow, cColumn - 1), // Down or left
                'F' => previousPoint != (cRow + 1, cColumn) ? (cRow + 1, cColumn) : (cRow, cColumn + 1), // Down or right
                _ => throw new ArgumentOutOfRangeException($"You shouldn't be here!"),
            };

            previousPoint = currentPoint;
            currentPoint = newCurrentPoint;
        }

        return path;
    }

    private Answer CalculatePart1Answer()
    {
        var path = CalculateLoop();
        return (int)Math.Ceiling(path.Count / 2.0);
    }

    private Answer CalculatePart2Answer()
    {
        var path = CalculateLoop();

        // Clean up the input to only contain the path.
        var pathSet = path.ToHashSet();
        var cleanInput = new List<string>();

        foreach (var (rowIndex, row) in _input.Enumerate())
        {
            var toPrint = row.Enumerate().Select(x => pathSet.Contains((rowIndex, x.Index)) ? x.Value : '.').ToList();
            cleanInput.Add(string.Join("", toPrint));
        }

        // Now expand the map, so that you can squeeze between pipes.
        var oneToThreeConversions = new Dictionary<char, string>()
        {
            // ReSharper disable once StringLiteralTypo
            { 'S', "SSSSSSSSS" },
            { '.', "........." },
            { '|', ".|..|..|." },
            { '-', "...---..." },
            { 'L', ".|..L-..." },
            { 'J', ".|.-J...." },
            { '7', "...-7..|." },
            { 'F', "....F-.|." },
        };

        var expandedInput = new List<string>();

        foreach (var (rowIndex, row) in cleanInput.Enumerate())
        {
            expandedInput.AddRange(new List<string>() { "", "", "" });

            foreach (var c in row)
            {
                var conversion = oneToThreeConversions[c];

                expandedInput[3 * rowIndex] += conversion[..3];
                expandedInput[(3 * rowIndex) + 1] += conversion[3..6];
                expandedInput[(3 * rowIndex) + 2] += conversion[6..];
            }
        }

        // Now that you can squeeze between pipes, flood-fill expandedInput.
        var queue = new Queue<(int Row, int Column)>();
        queue.Enqueue((0, 0));

        while (queue.Count > 0)
        {
            var (cRow, cColumn) = queue.Dequeue();

            if (cRow < 0 || cRow >= expandedInput.Count || cColumn < 0 || cColumn >= expandedInput[0].Length)
            {
                continue;
            }

            if (expandedInput[cRow][cColumn] != '.')
            {
                continue;
            }

            expandedInput[cRow] = expandedInput[cRow][..cColumn] + 'O' + expandedInput[cRow][(cColumn + 1)..];

            queue.Enqueue((Row: cRow, cColumn + 1));
            queue.Enqueue((Row: cRow, cColumn - 1));
            queue.Enqueue((cRow + 1, Column: cColumn));
            queue.Enqueue((cRow - 1, Column: cColumn));
        }

        // And now figure out which of the original points are still "inside" (the rest were marked as 'O' in the expanded map).
        return cleanInput
            .SelectMany(
                (row, rowIndex) => row
                    .Enumerate()
                    .Where(column => column.Value == '.')
                    .Select(column => (Row: rowIndex, Column: column.Index)))
            .Count(point => expandedInput[(3 * point.Row) + 1][(3 * point.Column) + 1] == '.');
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer(); // 278 is too low, 1750 is too high.
}

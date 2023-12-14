namespace AdventOfCode;

public sealed class Day14 : BaseTestableDay
{
    private readonly int _mapRows;
    private readonly int _mapColumns;

    private readonly HashSet<(int Row, int Column)> _roundRocks = new();
    private readonly HashSet<(int Row, int Column)> _squareRocks = new();

    public Day14() : this(RunMode.Real)
    {
    }

    public Day14(RunMode runMode)
    {
        RunMode = runMode;

        var input = File
            .ReadAllLines(InputFilePath)
            .ToList();

        _mapRows = input.Count;
        _mapColumns = input[0].Length;

        foreach (var (rowIndex, row) in input.Enumerate())
        {
            foreach (var (columnIndex, c) in row.Enumerate())
            {
                switch (c)
                {
                    case 'O':
                        _roundRocks.Add((rowIndex, columnIndex));
                        break;
                    case '#':
                        _squareRocks.Add((rowIndex, columnIndex));
                        break;
                }
            }
        }
    }

    private HashSet<(int Row, int Column)> Tilt(HashSet<(int Row, int Column)> roundRocks, int deltaRow, int deltaColumn)
    {
        var newRocks = roundRocks.ToHashSet();

        // We need to go from the "lowest" spot in the tilt, to the "highest".
        foreach (var (row, column) in roundRocks.OrderBy(x => deltaRow == 0 ? -x.Column * deltaColumn : -x.Row * deltaRow))
        {
            var newRow = row;
            var newColumn = column;

            while (
                0 <= (newRow + deltaRow) && (newRow + deltaRow) < _mapRows
                && 0 <= (newColumn + deltaColumn) && (newColumn + deltaColumn) < _mapColumns
                && !_squareRocks.Contains((newRow + deltaRow, newColumn + deltaColumn))
                && !newRocks.Contains((newRow + deltaRow, newColumn + deltaColumn))
            )
            {
                newRow += deltaRow;
                newColumn += deltaColumn;
            }

            newRocks.Remove((row, column));
            newRocks.Add((newRow, newColumn));
        }

        return newRocks;
    }


    private Answer CalculatePart1Answer()
    {
        var movedRocks = Tilt(_roundRocks, deltaRow: -1, deltaColumn: 0);
        return movedRocks.Select(rock => _mapRows - rock.Row).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        var movedRocks = _roundRocks.ToHashSet();

        var rockMemory = new List<HashSet<(int Row, int Column)>>();
        var memory = new Dictionary<string, (int Cycle, HashSet<(int Row, int Column)> Rocks)>();

        foreach (var cycle in Enumerable.Range(0, 1000000000))
        {
            movedRocks = Tilt(movedRocks, deltaRow: -1, deltaColumn: 0);
            movedRocks = Tilt(movedRocks, deltaRow: 0, deltaColumn: -1);
            movedRocks = Tilt(movedRocks, deltaRow: 1, deltaColumn: 0);
            movedRocks = Tilt(movedRocks, deltaRow: 0, deltaColumn: 1);

            var representation = string.Join(",", movedRocks.OrderBy(t => t.Row).ThenBy(t => t.Column).Select(t => $"({t.Row},{t.Column})"));
            //Console.WriteLine($"Cycle {cycle}: {representation}");

            if (memory.TryGetValue(representation, out var previous))
            {
                // We found a loop!
                var previousOccurrence = previous.Cycle;
                var periodicity = cycle - previousOccurrence;

                var relevantCycle = previousOccurrence + ((1000000000 - cycle - 1) % periodicity);
                var relevantRocks = rockMemory[relevantCycle];

                return relevantRocks.Select(rock => _mapRows - rock.Row).Sum();
            }

            rockMemory.Add(movedRocks.ToHashSet());
            memory[representation] = (cycle, rockMemory[^1]);
        }

        return movedRocks.Select(rock => _mapRows - rock.Row).Sum();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

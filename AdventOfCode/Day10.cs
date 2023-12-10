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
                _ => throw new ArgumentOutOfRangeException("Why."),
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
        path.Add(path[0]); // Close the loop, because you'll need it later...

        var pathSet = path.ToHashSet();

        var cleanInput = new List<string>();

        foreach (var (rowIndex, row) in _input.Enumerate())
        {
            var toPrint = row.Enumerate().Select(x => pathSet.Contains((rowIndex, x.Index)) ? x.Value : '.').ToList();
            cleanInput.Add(string.Join("", toPrint));
            //Console.WriteLine(_cleanInput[^1]);
        }

        var points = cleanInput
            .SelectMany((row, rowIndex) => row.Enumerate().Where(column => column.Value == '.').Select(column => (rowIndex, column.Index)))
            .ToList();

        var inLoop = points.Where(p => IsInPolygon(p, path)).ToHashSet();

        //var _testInput = new List<string>();

        //foreach (var (rowIndex, row) in _cleanInput.Enumerate())
        //{
        //    var toPrint = row
        //        .Enumerate()
        //        .Select(x => pathSet.Contains((rowIndex, x.Index)) ? x.Value : (inLoop.Contains((rowIndex, x.Index)) ? 'I' : 'O'))
        //        .ToList();

        //    _testInput.Add(string.Join("", toPrint));
        //    Console.WriteLine(_testInput[^1]);
        //}
        //
        //Console.WriteLine();

        return inLoop.Count;
    }

    private bool IsInPolygon((int, int) point, List<(int, int)> polygon)
    {
        bool result = false;
        var a = polygon.Last();

        foreach (var b in polygon)
        {
            if ((b.Item2 == point.Item2) && (b.Item1 == point.Item1))
                return true;

            if ((b.Item1 == a.Item1) && (point.Item1 == a.Item1))
            {
                if ((a.Item2 <= point.Item2) && (point.Item2 <= b.Item2))
                    return true;

                if ((b.Item2 <= point.Item2) && (point.Item2 <= a.Item2))
                    return true;
            }

            if ((b.Item1 < point.Item1) && (a.Item1 >= point.Item1) || (a.Item1 < point.Item1) && (b.Item1 >= point.Item1))
            {
                if (b.Item2 + (point.Item1 - b.Item1) / (a.Item1 - b.Item1) * (a.Item2 - b.Item2) <= point.Item2)
                    result = !result;
            }

            a = b;
        }

        return result;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer(); // 278 is too low, 1750 is too high.
}

namespace AdventOfCode;

public sealed class Day11 : BaseTestableDay
{
    private readonly int _size;
    private readonly List<(int Row, int Column)> _galaxies;

    public Day11() : this(RunMode.Real)
    {
    }

    public Day11(RunMode runMode)
    {
        RunMode = runMode;

        var input = File
            .ReadAllLines(InputFilePath)
            .ToList();

        _size = input.Count;

        _galaxies = input
            .Enumerate()
            .SelectMany(r => r.Value.Enumerate().Where(c => c.Value == '#').Select(c => (r.Index, c.Index)))
            .ToList();
    }

    internal Answer Calculate(int age)
    {
        var emptyRows = Enumerable.Range(0, _size).Where(row => _galaxies.All(g => g.Row != row)).ToList();
        var emptyColumns = Enumerable.Range(0, _size).Where(row => _galaxies.All(g => g.Column != row)).ToList();

        var result = (long)0;

        foreach (var (index, galaxyFrom) in _galaxies.Enumerate())
        {
            foreach (var galaxyTo in _galaxies.Skip(index + 1))
            {
                var minRow = Math.Min(galaxyFrom.Row, galaxyTo.Row);
                var maxRow = Math.Max(galaxyFrom.Row, galaxyTo.Row);
                var minColumn = Math.Min(galaxyFrom.Column, galaxyTo.Column);
                var maxColumn = Math.Max(galaxyFrom.Column, galaxyTo.Column);

                var manhattanDistance = (maxRow - minRow) + (maxColumn - minColumn);
                var extraRows = emptyRows.Count(row => minRow < row && row < maxRow);
                var extraColumns = emptyColumns.Count(column => minColumn < column && column < maxColumn);

                result += manhattanDistance + (extraRows + extraColumns) * (age - 1);
            }
        }

        return result;
    }

    private Answer CalculatePart1Answer()
    {
        return Calculate(2);
    }

    private Answer CalculatePart2Answer()
    {
        return Calculate(1000000);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

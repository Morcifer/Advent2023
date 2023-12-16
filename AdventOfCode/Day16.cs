namespace AdventOfCode;

public enum Direction
{
    Right,
    Down,
    Left,
    Up,
}

public sealed class Day16 : BaseTestableDay
{
    private readonly List<string> _input;

    public Day16() : this(RunMode.Real)
    {
    }

    internal ((int Row, int Column), Direction Direction) MoveStraight((int Row, int Column) point, Direction direction)
    {
        return direction switch
        {
            Direction.Right => ((point.Row, point.Column + 1), direction),
            Direction.Down => ((point.Row + 1, point.Column), direction),
            Direction.Left => ((point.Row, point.Column - 1), direction),
            Direction.Up => ((point.Row - 1, point.Column), direction),
        };
    }

    internal List<((int Row, int Column), Direction Direction)> Move((int Row, int Column) point, Direction direction)
    {
        var result = (direction, _input[point.Row][point.Column]) switch
        {
            (Direction.Right, '.') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, direction) },
            (Direction.Right, '|') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Up), MoveStraight(point, Direction.Down) },
            (Direction.Right, '-') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, direction) },
            (Direction.Right, '/') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Up) },
            (Direction.Right, '\\') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Down) },
            (Direction.Down, '.') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Down) },
            (Direction.Down, '|') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Down) },
            (Direction.Down, '-') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Left), MoveStraight(point, Direction.Right) },
            (Direction.Down, '/') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Left) },
            (Direction.Down, '\\') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Right) },
            (Direction.Left, '.') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Left) },
            (Direction.Left, '|') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Up), MoveStraight(point, Direction.Down) },
            (Direction.Left, '-') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Left) },
            (Direction.Left, '/') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Down) },
            (Direction.Left, '\\') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Up) },
            (Direction.Up, '.') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Up) },
            (Direction.Up, '|') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Up) },
            (Direction.Up, '-') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Left), MoveStraight(point, Direction.Right) },
            (Direction.Up, '/') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Right) },
            (Direction.Up, '\\') => new List<((int Row, int Column), Direction Direction)> { MoveStraight(point, Direction.Left) },
            _ => throw new NotImplementedException(),
        };

        return result.Where(x => 0 <= x.Item1.Row && x.Item1.Row < _input.Count && 0 <= x.Item1.Column && x.Item1.Column < _input[0].Length).ToList();
    }

    public Day16(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .ToList();
    }

    private int CalculateEnergized((int Row, int Column) point, Direction direction)
    {
        Console.WriteLine($"{point}, {direction}");

        var beams = new List<((int Row, int Column), Direction Direction)> { (point, direction) };

        var energized = new HashSet<(int, int)>();
        energized.Add(point);

        var count = 1000; // Find better way to trim!

        while (beams.Count > 0 && count > 0)
        {
            beams = beams.SelectMany(b => Move(b.Item1, b.Direction)).Distinct().ToList();
            energized.UnionWith(beams.Select(b => b.Item1));
            count--;
        }

        return energized.Count;
    }

    private Answer CalculatePart1Answer()
    {
        return CalculateEnergized((0, 0), Direction.Right);
    }

    private Answer CalculatePart2Answer()
    {
        var directions = new List<Direction> { Direction.Right, Direction.Down, Direction.Left, Direction.Up, };

        return Enumerable.Range(0, _input.Count)
            .Select(start => directions.SelectMany(direction => new List<int>()
            {
                CalculateEnergized((start, 0), direction),
                CalculateEnergized((start, _input.Count - 1), direction),
                CalculateEnergized((0, start), direction),
                CalculateEnergized((_input.Count - 1, start), direction),
            }).Max())
            .Max();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

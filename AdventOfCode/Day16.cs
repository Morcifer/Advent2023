namespace AdventOfCode;

public enum Direction
{
    Right,
    Down,
    Left,
    Up,
}

public record DirectionalPoint(int Row, int Column, Direction Direction)
{
    public static implicit operator DirectionalPoint((int, int, Direction) x) => new(x.Item1, x.Item2, x.Item3);
};

public sealed class Day16 : BaseTestableDay
{
    private readonly List<string> _input;

    public Day16() : this(RunMode.Real)
    {
    }

    internal DirectionalPoint MoveStraight(int row, int column, Direction direction)
    {
        return direction switch
        {
            Direction.Right => (row, column + 1, direction),
            Direction.Down => (row + 1, column, direction),
            Direction.Left => (row, column - 1, direction),
            Direction.Up => (row - 1, column, direction),
            _ => throw new ArgumentException("Rust is better than C# at this"),
        };
    }

    internal List<DirectionalPoint> Move(DirectionalPoint directionalPoint)
    {
        var (row, column, direction) = directionalPoint;

        var result = (direction, _input[row][column]) switch
        {
            (Direction.Right, '.') => new List<DirectionalPoint> { MoveStraight(row, column, direction) },
            (Direction.Right, '|') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Up), MoveStraight(row, column, Direction.Down), },
            (Direction.Right, '-') => new List<DirectionalPoint> { MoveStraight(row, column, direction) },
            (Direction.Right, '/') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Up) },
            (Direction.Right, '\\') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Down) },
            (Direction.Down, '.') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Down) },
            (Direction.Down, '|') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Down) },
            (Direction.Down, '-') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Left), MoveStraight(row, column, Direction.Right), },
            (Direction.Down, '/') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Left) },
            (Direction.Down, '\\') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Right) },
            (Direction.Left, '.') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Left) },
            (Direction.Left, '|') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Up), MoveStraight(row, column, Direction.Down), },
            (Direction.Left, '-') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Left) },
            (Direction.Left, '/') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Down) },
            (Direction.Left, '\\') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Up) },
            (Direction.Up, '.') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Up) },
            (Direction.Up, '|') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Up) },
            (Direction.Up, '-') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Left), MoveStraight(row, column, Direction.Right), },
            (Direction.Up, '/') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Right) },
            (Direction.Up, '\\') => new List<DirectionalPoint> { MoveStraight(row, column, Direction.Left) },
            _ => throw new ArgumentException("You shouldn't be here, what did you do?"),
        };

        return result.Where(x => 0 <= x.Row && x.Row < _input.Count && 0 <= x.Column && x.Column < _input[0].Length).ToList();
    }

    public Day16(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .ToList();
    }

    private int CalculateEnergized(DirectionalPoint directionalPoint)
    {
        var beams = new List<DirectionalPoint> { directionalPoint };
        var explored = beams.ToHashSet();

        while (beams.Count > 0)
        {
            beams = beams
                .SelectMany(b => Move((b.Row, b.Column, b.Direction)))
                .Where(x => !explored.Contains(x))
                .Distinct()
                .ToList();

            explored.UnionWith(beams);
        }

        return explored.Select(x => (x.Row, x.Column)).Distinct().Count();
    }

    private Answer CalculatePart1Answer()
    {
        return CalculateEnergized((0, 0, Direction.Right));
    }

    private Answer CalculatePart2Answer()
    {
        var directions = new List<Direction>
        {
            Direction.Right, Direction.Down, Direction.Left, Direction.Up,
        };

        return Enumerable.Range(0, _input.Count)
            .SelectMany(
                start => directions
                    .SelectMany(
                        direction => new List<int>
                        {
                            CalculateEnergized((start, 0, direction)),
                            CalculateEnergized((start, _input.Count - 1, direction)),
                            CalculateEnergized((0, start, direction)),
                            CalculateEnergized((_input.Count - 1, start, direction)),
                        }
                    ))
            .Max();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

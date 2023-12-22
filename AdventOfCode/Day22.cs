namespace AdventOfCode;


public class Brick
{
    public readonly int Id;

    public readonly List<(int X, int Y, int Z)> Squares;

    public Brick(int id, (int X, int Y, int Z) start, (int X, int Y, int Z) end)
    {
        Id = id;

        if (start.X != end.X)
        {
            Squares = Enumerable.Range(start.X, end.X - start.X + 1).Select(x => (x, start.Y, start.Z)).ToList();
        }
        else if (start.Y != end.Y)
        {
            Squares = Enumerable.Range(start.Y, end.Y - start.Y + 1).Select(y => (start.X, y, start.Z)).ToList();
        }
        else
        {
            Squares = Enumerable.Range(start.Z, end.Z - start.Z + 1).Select(z => (start.X, start.Y, z)).ToList();
        }
    }

    public int LowestZ()
    {
        return Math.Min(Squares[0].Z, Squares[^1].Z);
    }

    public int HighestZ()
    {
        return Math.Max(Squares[0].Z, Squares[^1].Z);
    }

    public Brick Fall(List<List<int>> heightMap)
    {
        var lowestZ = LowestZ();
        var pointsAtLowestZ = Squares.Where(t => t.Z == lowestZ).ToList();

        var stoppingZ = pointsAtLowestZ
            .Select(t => heightMap[t.X][t.Y])
            .Max();
        var deltaZ = 1 + stoppingZ - lowestZ;

        //Console.WriteLine($"Brick {Id} is going to fall {deltaZ}");

        var start = Squares[0];
        var end = Squares[^1];

        var fallenBrick = new Brick(Id, (start.X, start.Y, start.Z + deltaZ), (end.X, end.Y, end.Z + deltaZ));

        var highestZ = fallenBrick.HighestZ();
        var pointsAtHighestZ = fallenBrick.Squares.Where(t => t.Z == highestZ).ToList();

        foreach (var point in pointsAtHighestZ)
        {
            heightMap[point.X][point.Y] = point.Z;
        }

        return fallenBrick;
    }

    public List<Brick> SupportedBricks(List<Brick> bricks)
    {
        var supportedBricks = Squares
            .SelectMany(
                t => bricks
                    .Where(b => b.Id != Id)
                    .Where(b => b.Squares.Any(s => s.X == t.X && s.Y == t.Y && s.Z == t.Z + 1)))
            .Distinct() // This one's important!
            .ToList();

        //Console.WriteLine($"Brick {Id} supports bricks {string.Join(',', supportedBricks.Select(b => b.Id))}");
        return supportedBricks;
    }
}

public sealed class Day22 : BaseTestableDay
{
    private readonly List<Brick> _bricks;

    private int _maxX = 0;
    private int _maxY = 0;

    public Day22() : this(RunMode.Real)
    {
    }

    public Day22(RunMode runMode)
    {
        RunMode = runMode;

        _bricks = File.ReadAllLines(InputFilePath).Select((t, i) => ConvertTextToBrick(i, t)).ToList();
    }

    private Brick ConvertTextToBrick(int id, string text)
    {
        //Console.WriteLine($"{id}: {text}");

        var split = text.Split('~');
        var start = split[0].Split(',').Select(int.Parse).ToList();
        var end = split[1].Split(',').Select(int.Parse).ToList();

        var maxX = Math.Max(start[0], end[0]);
        _maxX = Math.Max(_maxX, maxX);

        var maxY = Math.Max(start[1], end[1]);
        _maxY = Math.Max(_maxY, maxY);

        return new Brick(id, (start[0], start[1], start[2]), (end[0], end[1], end[2]));
    }

    private List<Brick> LetTheBricksFallWhereTheyMay(List<Brick> bricks)
    {
        var fallenBricks = new List<Brick>();
        var heightMap = Enumerable.Range(0, _maxX + 1).Select(_ => Enumerable.Range(0, _maxY + 1).Select(_ => 0).ToList()).ToList();

        foreach (var brick in bricks.OrderBy(b => b.LowestZ()))
        {
            var fallenBrick = brick.Fall(heightMap);
            fallenBricks.Add(fallenBrick);
        }

        return fallenBricks;
    }

    private (Dictionary<int, List<Brick>> Supporting, Dictionary<int, List<Brick>> Supported) SupportGroup(List<Brick> bricks)
    {
        var supporting = bricks.ToDictionary(b => b.Id, _ => new List<Brick>());
        var supported = bricks.ToDictionary(b => b.Id, _ => new List<Brick>());

        foreach (var fallenBrick in bricks)
        {
            var supportedBricks = fallenBrick.SupportedBricks(bricks);

            supporting[fallenBrick.Id] = supportedBricks;

            foreach (var supportedBrick in supportedBricks)
            {
                supported[supportedBrick.Id].Add(fallenBrick);
            }
        }

        return (supporting, supported);
    }

    private Answer CalculatePart1Answer()
    {
        var fallenBricks = LetTheBricksFallWhereTheyMay(_bricks);
        var (supporting, supported) = SupportGroup(fallenBricks);

        // Find bricks that join up on support
        var freeBrickCount = 0;

        foreach (var fallenBrick in fallenBricks)
        {
            var supportedBricks = supporting[fallenBrick.Id];

            if (supportedBricks.All(b => supported[b.Id].Count > 1))
            {
                freeBrickCount++;
            }
        }

        return freeBrickCount;
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

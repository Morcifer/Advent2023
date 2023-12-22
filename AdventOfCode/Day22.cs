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

    public List<int> SupportedBricks(Dictionary<int, Brick> bricks)
    {
        var supportedBricks = Squares
            .SelectMany(
                t => bricks
                    .Values
                    .Where(b => b.Id != Id)
                    .Where(b => b.Squares.Any(s => s.X == t.X && s.Y == t.Y && s.Z == t.Z + 1)))
            .Select(b => b.Id)
            .Distinct() // This one's important!
            .ToList();

        //Console.WriteLine($"Brick {Id} supports bricks {string.Join(',', supportedBricks)}");
        return supportedBricks;
    }
}

public sealed class Day22 : BaseTestableDay
{
    private readonly Dictionary<int, Brick> _bricks;
    private int _maxX = 0;
    private int _maxY = 0;

    public Day22() : this(RunMode.Real)
    {
    }

    public Day22(RunMode runMode)
    {
        RunMode = runMode;

        _bricks = File.ReadAllLines(InputFilePath).Select((t, i) => ConvertTextToBrick(i, t)).ToDictionary(b => b.Id, b => b);
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

    private Dictionary<int, Brick> LetTheBricksFallWhereTheyMay(Dictionary<int, Brick> bricks)
    {
        var fallenBricks = new Dictionary<int, Brick>();
        var heightMap = Enumerable.Range(0, _maxX + 1).Select(_ => Enumerable.Range(0, _maxY + 1).Select(_ => 0).ToList()).ToList();

        foreach (var brick in bricks.Values.OrderBy(b => b.LowestZ()))
        {
            var fallenBrick = brick.Fall(heightMap);
            fallenBricks[fallenBrick.Id] = fallenBrick;
        }

        return fallenBricks;
    }

    private (Dictionary<int, List<int>> Supporting, Dictionary<int, List<int>> Supported) SupportGroup(Dictionary<int, Brick> bricks)
    {
        var supporting = bricks.ToDictionary(kvp => kvp.Key, _ => new List<int>());
        var supported = bricks.ToDictionary(kvp => kvp.Key, _ => new List<int>());

        foreach (var (fallenBrickId, fallenBrick) in bricks)
        {
            var supportedBricks = fallenBrick.SupportedBricks(bricks);

            supporting[fallenBrickId] = supportedBricks;

            foreach (var supportedBrick in supportedBricks)
            {
                supported[supportedBrick].Add(fallenBrickId);
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

        foreach (var brickId in fallenBricks.Keys)
        {
            var supportedBricks = supporting[brickId];

            if (supportedBricks.All(b => supported[b].Count > 1))
            {
                freeBrickCount++;
            }
        }

        return freeBrickCount;
    }

    private Answer CalculatePart2Answer()
    {
        var fallenBricks = LetTheBricksFallWhereTheyMay(_bricks);
        var (supporting, supported) = SupportGroup(fallenBricks);

        supporting = supporting
            .Where(kvp => kvp.Value.Count > 0) // We don't need these for this part
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        supported = supported
            .Where(kvp => kvp.Value.Count > 0) // We don't need these for this part either
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var fallenBricksCount = 0;

        foreach (var brick in fallenBricks.Keys)
        {
            var supportingCopy = supporting.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            var supportedCopy = supported.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());

            //Console.WriteLine($"Killing brick {brick}");

            var bricksToFall = new Queue<int>();
            bricksToFall.Enqueue(brick);

            while (bricksToFall.Count > 0)
            {
                var brickToRemove = bricksToFall.Dequeue();

                supportingCopy.Remove(brickToRemove);

                foreach (var (supportedBrickId, supportingBricks) in supportedCopy.ToList())
                {
                    supportingBricks.Remove(brickToRemove);

                    if (supportingBricks.Count != 0)
                    {
                        continue;
                    }

                    //Console.WriteLine($"Brick {kvp.Key} is also going to fall");
                    supportedCopy.Remove(supportedBrickId); // Cleanup!
                    bricksToFall.Enqueue(supportedBrickId);
                    fallenBricksCount += 1;
                }
            }
        }

        return fallenBricksCount;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

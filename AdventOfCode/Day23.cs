using MathNet.Numerics.Distributions;

namespace AdventOfCode;

public sealed class Day23 : BaseTestableDay
{
    private readonly List<string> _map;

    public Day23() : this(RunMode.Real)
    {
    }

    public Day23(RunMode runMode)
    {
        RunMode = runMode;

        _map = File.ReadAllLines(InputFilePath).ToList();
    }

    private Answer FindLongestHike(List<string> map)
    {
        var start = (Row: 0, Column: 1);
        var end = (Row: map.Count - 1, Column: map[0].Length - 2);

        var deltas = new List<(int Row, int Column)>() { (0, 1), (0, -1), (1, 0), (-1, 0) };

        var queue = new PriorityQueue<
            (List<(int Row, int Column)>, HashSet<(int Row, int Column)>),
            int
        >();

        queue.Enqueue(
            (new List<(int Row, int Column)>() { start }, new HashSet<(int Row, int Column)>()),
            -1
        );

        var validPaths = new List<List<(int Row, int Column)>>();

        while (queue.Count > 0)
        {
            //Console.WriteLine($"{queue.Count} paths left to check.");
            var (path, set) = queue.Dequeue();

            if (path[^1] == end)
            {
                validPaths.Add(path);
                Console.WriteLine($"New path found, length is {path.Count} (best is {validPaths.Max(x => x.Count - 1)}), remaining queue size is {queue.Count}");
                continue;
            }

            var current = path[^1];
            var currentCharacter = map[current.Row][current.Column];

            // You've hit a tree!
            if (currentCharacter == '#')
            {
                continue;
            }

            // You've eaten your own tail!
            if (set.Contains(current))
            {
                continue;
            }

            set.Add(current);

            switch (currentCharacter)
            {
                case '>':
                    path.Add((current.Row, current.Column + 1));
                    queue.Enqueue((path, set), -path.Count);
                    break;
                case '<':
                    path.Add((current.Row, current.Column - 1));
                    queue.Enqueue((path, set), -path.Count);
                    break;
                case 'v':
                    path.Add((current.Row + 1, current.Column));
                    queue.Enqueue((path, set), -path.Count);
                    break;
                case '^':
                    path.Add((current.Row - 1, current.Column));
                    queue.Enqueue((path, set), -path.Count);
                    break;
                default:
                    if (current == start)
                    {
                        path.Add((current.Row + 1, current.Column));
                        queue.Enqueue((path, set), -path.Count);
                        break;
                    }

                    var possibleNeighbors = deltas.Select(delta => (Row: current.Row + delta.Row, Column: current.Column + delta.Column)).ToList();
                    var waysToGo = possibleNeighbors.Where(n => map[n.Row][n.Column] != '#').ToList();

                    if (waysToGo.Count == 1)
                    {
                        path.Add(waysToGo[0]);
                        queue.Enqueue((path, set), -path.Count);
                        break;
                    }

                    foreach (var wayToGo in waysToGo)
                    {
                        var newPath = path.ToList();
                        newPath.Add(wayToGo);
                        queue.Enqueue((newPath, set.ToHashSet()), -newPath.Count);
                    }

                    break;
            }
        }

        return validPaths.Max(x => x.Count - 1);
    }

    private Answer CalculatePart1Answer()
    {
        return FindLongestHike(_map);
    }

    private Answer CalculatePart2Answer()
    {
        var newMap = _map.Select(row => row.Replace('^', '.').Replace('>', '.').Replace('v', '.').Replace('<', '.')).ToList();
        return FindLongestHike(newMap);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer(); // Smaller than 7000, larger than 6000.
}

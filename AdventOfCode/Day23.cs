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

        var queue = new Queue<List<(int Row, int Column)>>();
        queue.Enqueue(new List<(int Row, int Column)>() { start });

        var validPaths = new List<List<(int Row, int Column)>>();

        while (queue.Count > 0)
        {
            //Console.WriteLine($"{queue.Count} paths left to check.");
            var path = queue.Dequeue();

            if (path[^1] == end)
            {
                validPaths.Add(path);
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
            if (path.Count(x => x == current) > 1)
            {
                continue;
            }

            switch (currentCharacter)
            {
                case '>':
                    path.Add((current.Row, current.Column + 1));
                    queue.Enqueue(path);
                    break;
                case '<':
                    path.Add((current.Row, current.Column - 1));
                    queue.Enqueue(path);
                    break;
                case 'v':
                    path.Add((current.Row + 1, current.Column));
                    queue.Enqueue(path);
                    break;
                case '^':
                    path.Add((current.Row - 1, current.Column));
                    queue.Enqueue(path);
                    break;
                default:
                    if (current == start)
                    {
                        path.Add((current.Row + 1, current.Column));
                        queue.Enqueue(path);
                    }
                    else
                    {
                        foreach (var delta in deltas)
                        {
                            var newPath = path.ToList();
                            newPath.Add((current.Row + delta.Row, current.Column + delta.Column));
                            queue.Enqueue(newPath);
                        }
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

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

using MoreLinq;

namespace AdventOfCode;

public sealed class Day23 : BaseTestableDay
{
    private readonly List<string> _map;
    private readonly (int Row, int Column) _start;
    private readonly (int Row, int Column) _end;
    private readonly Dictionary<(int Row, int Column), int> _nodes;
    private readonly List<(int Row, int Column)> _deltas;

    public Day23() : this(RunMode.Real)
    {
    }

    public Day23(RunMode runMode)
    {
        RunMode = runMode;

        _map = File.ReadAllLines(InputFilePath).ToList();

        _start = (Row: 0, Column: 1);
        _end = (Row: _map.Count - 1, Column: _map[0].Length - 2);

        _deltas = new List<(int Row, int Column)>() { (0, 1), (0, -1), (1, 0), (-1, 0) };
        _nodes = new Dictionary<(int Row, int Column), int> { [_start] = 0, [_end] = 1, };

        foreach (var (rowIndex, row) in _map.Enumerate())
        {
            foreach (var (columnIndex, column) in row.Enumerate())
            {
                if (column == '#')
                {
                    continue;
                }

                var current = (rowIndex, columnIndex);

                if (current == _start || current == _end)
                {
                    continue;
                }

                var possibleNeighbors = _deltas
                    .Select(delta => (Row: rowIndex + delta.Row, Column: columnIndex + delta.Column))
                    .Where(point => point.Row >= 0 && point.Row < _map.Count)
                    .ToList();

                var waysToGo = possibleNeighbors.Where(n => _map[n.Row][n.Column] != '#').ToList();

                if (waysToGo.Count >= 3)
                {
                    _nodes[current] = _nodes.Count + 1;
                }
            }
        }
    }

    private List<(int Source, int Target, int Length)> BuildEdges(bool ignoreSlopes)
    {
        var edges = new List<(int Source, int Target, int Length)>();

        foreach (var (nodePoint, sourceNodeId) in _nodes)
        {
            var queue = new Queue<List<(int Row, int Column)>>();
            queue.Enqueue(new List<(int Row, int Column)>() { nodePoint });

            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                var current = path[^1];

                if (current != nodePoint && _nodes.TryGetValue(current, out var targetNodeId))
                {
                    edges.Add((Source: sourceNodeId, Target: targetNodeId, Length: path.Count - 1));

                    if (ignoreSlopes || path.All(point => _map[point.Row][point.Column] == '.'))
                    {
                        edges.Add((Source: targetNodeId, Target: sourceNodeId, Length: path.Count - 1));
                    }

                    continue;
                }

                var currentCharacter = _map[current.Row][current.Column];

                // You've eaten your own tail!
                if (path.Count(point => point == current) > 1)
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
                        if (current == _start)
                        {
                            path.Add((current.Row + 1, current.Column));
                            queue.Enqueue(path);
                            break;
                        }

                        if (current == _end)
                        {
                            path.Add((current.Row - 1, current.Column));
                            queue.Enqueue(path);
                            break;
                        }

                        var neighbors = _deltas
                            .Select(delta => (Row: current.Row + delta.Row, Column: current.Column + delta.Column))
                            .ToList();

                        foreach (var neighbor in neighbors)
                        {
                            if (_map[neighbor.Row][neighbor.Column] == '#')
                            {
                                continue;
                            }

                            if (path.Count >= 2 && neighbor == path[^2])
                            {
                                continue;
                            }

                            var newPath = path.ToList();
                            newPath.Add(neighbor);
                            queue.Enqueue(newPath);
                        }

                        break;
                }
            }
        }

        return edges;
    }

    private Answer FindLongestHike(bool ignoreSlopes)
    {
        var edges = BuildEdges(ignoreSlopes);

        var graph = edges
            .GroupBy(e => e.Source)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => (e.Target, e.Length)).ToList()
            );

        var queue = new Queue<List<int>>();
        queue.Enqueue(new List<int>() { 0 }); // Starting node

        var maxLength = 0;

        while (queue.Count > 0)
        {
            //Console.WriteLine($"{queue.Count} paths left to check.");
            var path = queue.Dequeue();

            if (path[^1] == 1) // End node
            {
                var length = path.Pairwise((source, target) => (source, target))
                    .Select(edge => graph[edge.source].First(x => x.Target == edge.target).Length)
                    .Sum();

                if (length > maxLength)
                {
                    maxLength = length;
                    //Console.WriteLine($"New path found, length is {length} (best is {maxLength}), remaining queue size is {queue.Count}");
                }

                continue;
            }

            var current = path[^1];

            foreach (var (target, _) in graph[current])
            {
                // You've eaten your own tail!
                if (path.Contains(target))
                {
                    continue;
                }

                var newPath = path.ToList();
                newPath.Add(target);
                queue.Enqueue(newPath);
            }
        }

        return maxLength;
    }

    private Answer CalculatePart1Answer()
    {
        return FindLongestHike(ignoreSlopes: false);
    }

    private Answer CalculatePart2Answer()
    {
        return FindLongestHike(ignoreSlopes: true);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

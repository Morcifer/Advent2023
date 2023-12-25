using MoreLinq;

namespace AdventOfCode;

public sealed class Day25 : BaseTestableDay
{
    private readonly List<(string, string)> _input;

    public Day25() : this(RunMode.Real)
    {
    }

    public Day25(RunMode runMode)
    {
        RunMode = runMode;

        _input = File.ReadAllLines(InputFilePath).SelectMany(ConvertTextToEdges).ToList();
    }

    private List<(string, string)> ConvertTextToEdges(string text)
    {
        var split = text.Split(':', StringSplitOptions.TrimEntries).ToList();
        return split[1].Split(' ', StringSplitOptions.TrimEntries).Select(x => (split[0], x)).ToList();
    }

    private int ConnectedGraph(List<(string, string)> edges, string start)
    {
        var graph = edges.GroupBy(x => x.Item1).ToDictionary(g => g.Key, g => g.Select(x => x.Item2).ToList());

        var queue = new Queue<string>();
        queue.Enqueue(start);

        var explored = new HashSet<string>();

        while (queue.Count > 0)
        {
            var toExplore = queue.Dequeue();

            if (explored.Contains(toExplore))
            {
                continue;
            }

            explored.Add(toExplore);

            foreach (var neighbor in graph[toExplore])
            {
                if (explored.Contains(neighbor))
                {
                    continue;
                }

                queue.Enqueue(neighbor);
            }
        }

        return explored.Count;
    }

    private Dictionary<string, string> ShortestPathFromNode(List<(string, string)> edges, string start)
    {
        var graph = edges.GroupBy(x => x.Item1).ToDictionary(g => g.Key, g => g.Select(x => x.Item2).ToList());
        var nodes = graph.Keys;

        var distances = nodes.ToDictionary(n => n, _ => int.MaxValue);
        var previouses = nodes.ToDictionary(n => n, _ => "");

        var toExplore = nodes.ToHashSet();

        distances[start] = 0;

        while (toExplore.Count > 0)
        {
            //Console.WriteLine($"{toExplore.Count} left to explore.");
            var u = distances.Where(kvp => toExplore.Contains(kvp.Key)).MinBy(kvp => kvp.Value).Key;
            toExplore.Remove(u);

            foreach (var v in graph[u])
            {
                if (!toExplore.Contains(v))
                {
                    continue; 
                }

                var altDistance = distances[u] + 1;

                if (altDistance < distances[v])
                {
                    distances[v] = altDistance;
                    previouses[v] = u;
                }
            }
        }

        return previouses;
    }

    private List<string> GetPath(Dictionary<string, string> previouses, string start, string end)
    {
        var path = new List<string>();
        var current = end;

        while (current != start)
        {
            path.Add(current);
            current = previouses[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    private Answer CalculatePart1Answer()
    {
        var random = new Random();

        var edges = _input.Concat(_input.Select(x => (x.Item2, x.Item1))).Distinct().ToList();
        var nodes = edges.Select(x => x.Item1).Distinct().ToList();

        var start = nodes[0];
        var previouses = ShortestPathFromNode(edges, start);

        while (true)
        {
            var target = nodes[random.Next(nodes.Count)];
            //Console.WriteLine($"Exploring {start} -> {target}");

            if (target == start)
            {
                continue;
            }

            var path = GetPath(previouses, start, target);
            var pathEdges = path.Pairwise((x, y) => (x, y)).ToList();

            foreach (var edge in pathEdges)
            {
                var edges1 = edges.Where(x => x != edge && x != (edge.y, edge.x)).ToList();
                var previouses1 = ShortestPathFromNode(edges1, start);
                var path1 = GetPath(previouses1, start, target);
                var pathEdges1 = path1.Pairwise((x, y) => (x, y)).ToList();

                foreach (var edge2 in pathEdges1.Where(e => !pathEdges.Contains(e)))
                {
                    var edges2 = edges1.Where(x => x != edge2 && x != (edge2.y, edge2.x)).ToList();
                    var previouses2 = ShortestPathFromNode(edges2, start);
                    var path2 = GetPath(previouses2, start, target);
                    var pathEdges2 = path2.Pairwise((x, y) => (x, y)).ToList();

                    foreach (var edge3 in pathEdges2.Where(e => !pathEdges1.Contains(e)))
                    {
                        var edges3 = edges2.Where(x => x != edge3 && x != (edge3.y, edge3.x)).ToList();

                        var connected = ConnectedGraph(edges3, start);

                        if (connected != nodes.Count)
                        {
                            return connected * (nodes.Count - connected);
                        }
                    }
                }
            }
        }
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

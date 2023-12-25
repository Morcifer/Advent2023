namespace AdventOfCode;

public sealed class Day25 : BaseTestableDay
{
    private readonly List<(string, string)> _input;

    public Day25() : this(RunMode.Test)
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

    private int ConnectedGraph(List<(string, string)> edges)
    {
        var dictionary = edges.GroupBy(x => x.Item1).ToDictionary(g => g.Key, g => g.Select(x => x.Item2).ToList());

        var queue = new Queue<string>();
        queue.Enqueue(edges[0].Item1);

        var explored = new HashSet<string>();

        while (queue.Count > 0)
        {
            var toExplore = queue.Dequeue();

            if (explored.Contains(toExplore))
            {
                continue;
            }

            explored.Add(toExplore);

            foreach (var neighbor in dictionary[toExplore])
            {
                queue.Enqueue(neighbor);
            }
        }

        return explored.Count;
    }

    private Answer CalculatePart1Answer()
    {
        for (var i = 0; i < _input.Count; i++)
        {
            Console.WriteLine($"Investigating {i} out of {_input.Count}");

            for (var j = i + 1; j < _input.Count; j++)
            {
                for (var k = j + 1; k < _input.Count; k++)
                {
                    if (i == j || i == k || j == k)
                    {
                        continue;
                    }

                    var newInput = _input.Enumerate().Where(x => x.Index != i && x.Index != j && x.Index != k).Select(x => x.Value).ToList();

                    var edges = newInput.Concat(newInput.Select(x => (x.Item2, x.Item1))).Distinct().ToList();
                    var nodes = edges.Select(x => x.Item1).Distinct().ToList();

                    var connected = ConnectedGraph(edges);

                    if (connected != nodes.Count)
                    {
                        return connected * (nodes.Count - connected);
                    }
                }
            }
        }



        return -1;
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

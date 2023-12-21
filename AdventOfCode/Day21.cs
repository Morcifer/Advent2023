namespace AdventOfCode;


public sealed class Day21 : BaseTestableDay
{
    private readonly List<string> _input;

    public Day21() : this(RunMode.Real)
    {
    }

    public Day21(RunMode runMode)
    {
        RunMode = runMode;

        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    private int Modulo(int number, int modulo)
    {
        var result = number % modulo;

        return number >= 0
            ? result
            : (result == 0) ? 0 : modulo + result;
    }

    internal Answer RandomWalk(int steps)
    {
        var spot = _input
            .Enumerate()
            .Select(x => (x.Index, x.Value.IndexOf('S')))
            .First(x => x.Item2 != -1);

        var plots = new HashSet<(int Row, int Column)> { spot };

        for (var step = 0; step < steps; step++)
        {
            if (step % 200 == 0)
            {
                Console.WriteLine($"I am in step {step} and have {plots.Count} plots");
            }

            plots = plots
                .SelectMany(spot => new List<(int Row, int Column)>
                {
                    (spot.Row, spot.Column + 1),
                    (spot.Row, spot.Column - 1),
                    (spot.Row + 1, spot.Column),
                    (spot.Row - 1, spot.Column),
                })
                .Where(spot => _input[Modulo(spot.Row, _input.Count)][Modulo(spot.Column, _input[0].Length)] is '.' or 'S' )
                .ToHashSet();
        }

        return plots.Count;
    }

    private Answer CalculatePart1Answer()
    {
        return RandomWalk(RunMode == RunMode.Real ? 64 : 6);
    }

    private Answer CalculatePart2Answer()
    {
        return RandomWalk(RunMode == RunMode.Real ? 26501365 : 5000);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

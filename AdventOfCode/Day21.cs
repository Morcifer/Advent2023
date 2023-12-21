using MoreLinq;

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
        var period = _input.Count;

        var spot = _input
            .Enumerate()
            .Select(x => (Row: x.Index, Column: x.Value.IndexOf('S')))
            .First(x => x.Item2 != -1);

        var plots = new HashSet<(int Row, int Column)> { spot };
        var memory = new List<long>();
        var periodMemory = new List<long>();
        var triangle = new List<List<long>>();

        var step = 0;

        while (step < steps)
        {
            memory.Add(plots.Count);

            if (step % period == 0)
            {
                periodMemory.Add(plots.Count);

                if (periodMemory.Count >= 4)
                {
                    var temporaryTriangle = new List<List<long>>();
                    var triangleValues = periodMemory.ToList();

                    while (triangleValues.Any(x => x != 0))
                    {
                        temporaryTriangle.Add(triangleValues);
                        triangleValues = triangleValues.Pairwise((x, y) => y - x).ToList();
                    }

                    if (temporaryTriangle[2][^2] == temporaryTriangle[2][^1]) // Polynomial increase means this will eventually happen.
                    {
                        triangle = temporaryTriangle;
                        break;
                    }
                }
            }

            step += 1;

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

        // At a certain point things become periodic and the increase quadratic with the period.
        // For the real one it's immediate. For the test one it actually takes a few periods longer.
        if (step == steps)
        {
            return plots.Count;
        }

        // Find largest value in memory that is something +
        var modulo = steps % period;
        var extra = (step / period - 1) * period;

        var result = memory[modulo + extra];
        var diff = result - memory[modulo + extra - period];
        var secondDiff = triangle[2][^1];

        for (; step < steps; step += period)
        {
            diff += secondDiff;
            result += diff;
        }

        return result;
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

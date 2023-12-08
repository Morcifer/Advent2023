using MoreLinq.Extensions;

namespace AdventOfCode;

public sealed class Day08 : BaseTestableDay
{
    private readonly List<int> _directionIndex;
    private readonly Dictionary<string, List<string>> _forks;

    public Day08() : this(RunMode.Real)
    {
    }

    public Day08(RunMode runMode)
    {
        RunMode = runMode;

        var input = File
            .ReadAllLines(InputFilePath)
            .ToList();

        _directionIndex = input[0].ToCharArray().Select(c => c == 'L' ? 0 : 1).ToList();

        _forks = input
            .Skip(2)
            .Select(
                text => text
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(" ", "")
                    .Split("=")
                    .ToList()
            )
            .ToDictionary(
                x => x[0],
                x => x[1].Split(",").ToList()
            );
    }

    private Answer CalculateSteps()
    {
        var steps = 0;
        var current = "AAA";

        foreach (var step in _directionIndex.Repeat(int.MaxValue))
        {
            if (current == "ZZZ")
            {
                return steps;
            }

            current = _forks[current][step];
            steps++;
        }

        return -1;
    }

    private Answer CalculateGhostSteps()
    {
        var startingPoints = _forks.Keys.Where(node => node[^1] == 'A').ToList();

        // Fun assumption that is apparently correct - each starting point will only ever end up in a single Z in their entire life.
        var timeToFirstZ = new Dictionary<string, long>();
        var periodicityToZ = new Dictionary<string, long>();

        foreach (var startingPoint in startingPoints)
        {
            var steps = (long)0;
            var current = startingPoint;

            foreach (var step in _directionIndex.Repeat(int.MaxValue))
            {
                if (current[^1] == 'Z')
                {
                    if (!timeToFirstZ.ContainsKey(startingPoint))
                    {
                        timeToFirstZ[startingPoint] = steps;
                    }
                    else
                    {
                        periodicityToZ[startingPoint] = steps - timeToFirstZ[startingPoint];
                        break;
                    }
                }

                current = _forks[current][step];
                steps++;
            }
        }

        var numberOfInstructionsPeriodicity = periodicityToZ
            .Values
            .Select(v => v / _directionIndex.Count)
            .Aggregate((long)1, (x, y) => x * y); // .Product(). Should extract this at some point.

        var actualPeriodicity = numberOfInstructionsPeriodicity * _directionIndex.Count;

        for (var multiple = (long)1; multiple < long.MaxValue; multiple++)
        {
            for (var smallStartTime = 0; smallStartTime < _directionIndex.Count; smallStartTime++)
            {
                var realTime = smallStartTime + (multiple * actualPeriodicity);

                if (startingPoints.All(startingPoint => (realTime - timeToFirstZ[startingPoint]) % periodicityToZ[startingPoint] == 0))
                {
                    return realTime;
                }
            }
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculateSteps();

    public override ValueTask<string> Solve_2() => CalculateGhostSteps();
}

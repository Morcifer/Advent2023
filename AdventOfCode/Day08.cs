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

        // Fun assumptions that are apparently correct:
        // Each starting point will only ever end up in a single Z in their entire life.
        // And not only that, there's actually no offset to get there - if it gets to Z at time T, it will get to Z at every multiple of T.
        var timeToFirstZ = new Dictionary<string, long>();

        foreach (var startingPoint in startingPoints)
        {
            var steps = (long)0;
            var current = startingPoint;

            foreach (var step in _directionIndex.Repeat(int.MaxValue))
            {
                if (current[^1] == 'Z')
                {
                    timeToFirstZ[startingPoint] = steps;
                    break;
                }

                current = _forks[current][step];
                steps++;
            }
        }

        var numberOfInstructionsPeriodicity = timeToFirstZ
            .Values
            .Select(v => v / _directionIndex.Count)
            .Product();

        return numberOfInstructionsPeriodicity * _directionIndex.Count;
    }

    public override ValueTask<string> Solve_1() => CalculateSteps();

    public override ValueTask<string> Solve_2() => CalculateGhostSteps();
}

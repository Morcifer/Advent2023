namespace AdventOfCode;

public sealed class Day13 : BaseTestableDay
{
    private readonly List<List<string>> _input;

    public Day13() : this(RunMode.Real)
    {
    }

    public Day13(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Cluster()
            .ToList();
    }

    private List<string> RotatePattern(List<string> pattern)
    {
        var rotatedPattern = new List<string>();

        for (int column = 0; column < pattern[0].Length; column++)
        {
            rotatedPattern.Add(string.Join("", pattern.Select(row => row[column]).Reverse().ToList()));
        }

        return rotatedPattern;
    }

    private List<string> CleanPattern(List<string> pattern, int row, int column)
    {
        var cleanedPattern = pattern.ToList();

        var oldRow = cleanedPattern[row];
        var newRow = oldRow[..column] + (oldRow[column] == '.' ? '#' : '.') + oldRow[(column + 1)..];

        cleanedPattern[row] = newRow;

        return cleanedPattern;
    }

    private int FindCleanedVerticalReflection(List<string> pattern)
    {
        var rotatedPattern = RotatePattern(pattern);
        var smudgedReflection = FindHorizontalReflection(rotatedPattern);

        for (int row = 0; row < rotatedPattern.Count; row++)
        {
            for (int column = 0; column < rotatedPattern[0].Length; column++)
            {
                var cleanedPattern = CleanPattern(rotatedPattern, row, column);
                var results = FindHorizontalReflections(cleanedPattern);
                var result = results.Where(x => x != smudgedReflection).FirstOrDefault(-1);

                if (result != -1)
                {
                    return result;
                }
            }
        }

        return -1;
    }

    private int FindCleanedHorizontalReflection(List<string> pattern)
    {
        var smudgedReflection = FindHorizontalReflection(pattern);

        for (int row = 0; row < pattern.Count; row++)
        {
            for (int column = 0; column < pattern[0].Length; column++)
            {
                var cleanedPattern = CleanPattern(pattern, row, column);
                var results = FindHorizontalReflections(cleanedPattern);
                var result = results.Where(x => x != smudgedReflection).FirstOrDefault(-1);

                if (result != -1)
                {
                    return result;
                }
            }
        }

        return -1;
    }

    private int FindVerticalReflection(List<string> pattern)
    {
        return FindVerticalReflections(pattern).FirstOrDefault(-1);
    }

    private int FindHorizontalReflection(List<string> pattern)
    {
        return FindHorizontalReflections(pattern).FirstOrDefault(-1);
    }

    private List<int> FindVerticalReflections(List<string> pattern)
    {
        return FindHorizontalReflections(RotatePattern(pattern));
    }

    // This is the only method that actually does something interesting.
    private List<int> FindHorizontalReflections(List<string> pattern)
    {
        var result = new List<int>();

        for (int flipRow = 1; flipRow < pattern.Count; flipRow++)
        {
            if (Enumerable
                .Range(0, Math.Min(flipRow, pattern.Count - flipRow))
                .All(delta => pattern[flipRow - delta - 1] == pattern[flipRow + delta]))
            {
                result.Add(flipRow);
            }
        }

        return result;
    }

    private Answer CalculatePart1Answer()
    {
        var verticalReflections = _input.Select(FindVerticalReflection).Where(x => x != -1).ToList();
        var horizontalReflections = _input.Select(FindHorizontalReflection).Where(x => x != -1).ToList();

        return verticalReflections.Sum() + 100 * horizontalReflections.Sum();
    }

    private Answer CalculatePart2Answer()
    {
        var smudgedVerticalReflections = _input.Select(FindCleanedVerticalReflection).Where(x => x != -1).ToList();
        var smudgedHorizontalReflections = _input.Select(FindCleanedHorizontalReflection).Where(x => x != -1).ToList();

        return smudgedVerticalReflections.Sum() + 100 * smudgedHorizontalReflections.Sum();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

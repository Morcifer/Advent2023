using MoreLinq;

namespace AdventOfCode;

public sealed class Day12 : BaseTestableDay
{
    private readonly List<(string, List<int>)> _input;

    public Day12() : this(RunMode.Real)
    {
    }

    public Day12(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(ConvertText)
            .ToList();
    }

    private (string, List<int>) ConvertText(string text)
    {
        var split = text.Split(' ', ' ').ToList();
        return (split[0], split[1].Split(",").Select(int.Parse).ToList());
    }

    internal long CalculateOptionsCached(
        string springs,
        int countIndex,
        List<int> counts,
        Dictionary<(string, int), long> cache
    )
    {
        var key = (springs, countIndex);

        if (cache.TryGetValue(key, out var result))
        {
            return result;
        }

        if (springs == "")
        {
            return countIndex == counts.Count ? 1 : 0;
        }

        switch (springs[0])
        {
            case '.':
                result = CalculateOptionsCached(springs[1..], countIndex, counts, cache);
                break;
            case '?':
                result = CalculateOptionsCached('#' + springs[1..], countIndex, counts, cache) +
                         CalculateOptionsCached('.' + springs[1..], countIndex, counts, cache);

                break;
            case '#':
                if (countIndex == counts.Count) // We ran out of groups)
                {
                    return 0;
                }

                var count = counts[countIndex];

                // Either the substring is too short, or it doesn't match the group requirement
                if (springs.Length < count || springs[..count].Any(c => c == '.'))
                {
                    result = 0;
                    break;
                }

                if (springs.Length == count) // This is the end of the string, and it's consistent with the group
                {
                    result = counts.Count == countIndex + 1 ? 1 : 0;
                    break;
                }

                if (springs[count] == '#') // The value after the group is ALSO #. That's a problem.
                {
                    result = 0;
                    break;
                }

                // The value after the group is either a '.' (in which case we can skip it)
                // or '?' (in which case only the '.' value is valid and we can skip it).
                result = CalculateOptionsCached(springs[(count + 1)..], countIndex + 1, counts, cache);
                break;
        }

        cache[key] = result;
        return result;
    }

    internal long CalculateOptions(string springs, List<int> counts)
    {
        var cache = new Dictionary<(string, int), long>();

        var result = CalculateOptionsCached(springs, 0, counts, cache);

        return result;
    }

    private Answer CalculatePart1Answer()
    {
        return _input.Select(x => CalculateOptions(x.Item1, x.Item2)).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        return _input.Select(
                x => CalculateOptions(
                    string.Join("?", Enumerable.Range(0, 5).Select(_ => x.Item1)),
                    x.Item2.Repeat(5).ToList())
            )
            .Sum();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

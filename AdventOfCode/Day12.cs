using System.Linq;
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

    internal bool IsValidCount(string springs, List<int> counts)
    {
        // TODO: This is ugly, do better. (by counting, then comparing).
        var countIndex = 0;
        var currentCount = 0;

        foreach (var spring in springs)
        {
            if (spring == '#')
            {
                currentCount += 1;
                continue;
            }


            if (currentCount == 0)
            {
                continue;
            }

            if (countIndex >= counts.Count || counts[countIndex] != currentCount)
            {
                return false;
            }

            countIndex += 1;
            currentCount = 0;
        }

        if (currentCount == 0)
        {
            return countIndex == counts.Count;
        }

        return countIndex == counts.Count - 1 && counts[countIndex] == currentCount;
    }


    internal bool IsValid(List<int> damaged, List<int> counts)
    {
        var temp = damaged
            .OrderBy(x => x)
            .GroupConsecutive()
            .ToList();

        var orderedDamaged = damaged
            .OrderBy(x => x)
            .GroupConsecutive()
            .Select(g => g.Count())
            .ToList();

        var temptemp = orderedDamaged.SequenceEqual(counts);

        return orderedDamaged.SequenceEqual(counts);
    }

    internal int CalculateOptions(string springs, List<int> counts)
    {
        // TODO: This is also slow and stupid. A DFS will be faster because you can cut branches!
        var questions = springs.Enumerate().Where(x => x.Value == '?').Select(x => x.Index).ToList();
        var questionsSet = questions.ToHashSet();

        return questions
            .Subsets()
            .Select(operationals => operationals.ToHashSet())
            .Select(operationals => string.Join(
                "",
                springs
                    .Enumerate()
                    .Select(
                        x => questionsSet.Contains(x.Index)
                            ? (operationals.Contains(x.Index) ? '.' : '#')
                            : x.Value
                    )
                )
            )
            .Count(s => IsValidCount(s, counts));
    }

    internal int CalculateOptionsFaster(string springs, List<int> counts)
    {
        Console.WriteLine($"Evaluating {springs}");
        var questions = springs.Enumerate().Where(x => x.Value == '?').Select(x => x.Index).ToList();
        var damaged = springs.Enumerate().Where(x => x.Value == '#').Select(x => x.Index).ToList();

        return questions
            .Subsets()
            .Select(otherDamaged => damaged.ToList().Concat(otherDamaged).ToList())
            .Count(allDamaged => IsValid(allDamaged, counts));
    }

    internal long CalculateOptionsFastestCached(
        string springs,
        int countIndex,
        List<int> counts, // Can change these to be the counts index, I guess.
        Dictionary<(string, int), long> cache
    )
    {
        //Console.WriteLine($"Evaluating {springs} with {string.Join(",", counts.Select(n => n.ToString()))}");
        var key = (springs, countIndex);
        long result;

        if (cache.TryGetValue(key, out result))
        {
            return result;
        }

        if (springs == "")
        {
            return countIndex == counts.Count ? 1 : 0;
        }

        if (springs[0] == '#')
        {
            if (countIndex == counts.Count || springs.Length < counts[countIndex] || springs[..counts[countIndex]].Any(c => c != '#' && c != '?'))
            {
                result = 0;
            }
            else if (springs.Length == counts[countIndex])
            {
                result = counts.Count == countIndex + 1 ? 1 : 0;
            }
            else // Yuck, this one sucks, because it depends on the next one.
            {
                if (springs[counts[countIndex]] == '#')
                {
                    result = 0;
                } else // For '?' and '.', only '.' is allowed.
                {
                    result = CalculateOptionsFastestCached(springs[(counts[countIndex] + 1)..], countIndex + 1, counts, cache);
                }
            }
        }
        else if (springs[0] == '?')
        {
            result = CalculateOptionsFastestCached('#' + springs[1..], countIndex, counts, cache) + CalculateOptionsFastestCached('.' + springs[1..], countIndex, counts, cache);
        }
        else
        {
            result = CalculateOptionsFastestCached(springs[1..], countIndex, counts, cache);
        }

        cache[key] = result;
        //Console.WriteLine($"{springs} evaluated to {result}");
        return result;
    }

    internal long CalculateOptionsFastest(string springs, List<int> counts)
    {
        Console.WriteLine($"Evaluating {springs}");
        var cache = new Dictionary<(string, int), long>();

        var result = CalculateOptionsFastestCached(springs, 0, counts, cache);
        //Console.WriteLine($"{springs} evaluated to {result}");

        return result;
    }

    private Answer CalculatePart1Answer()
    {
        Console.WriteLine($"Evaluating Part 1");
        return _input.Select(x => CalculateOptionsFastest(x.Item1, x.Item2)).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        //if (RunMode == RunMode.Real)
        //{
        //    return -1;
        //}
        
        Console.WriteLine($"Evaluating Part 2");
        return _input.Select(x => CalculateOptionsFastest(
            string.Join("?", Enumerable.Range(0, 5).Select(i => x.Item1)),
            x.Item2.Repeat(5).ToList())
        ).Sum();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

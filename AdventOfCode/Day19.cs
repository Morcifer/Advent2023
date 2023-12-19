namespace AdventOfCode;

// TODO: Make XMAS tuple to get rid of the char.

public sealed class Day19 : BaseTestableDay
{
    private readonly Dictionary<string, List<(char Value, char Rule, int Limit, string Next)>> _rules;
    private readonly List<Dictionary<char, int>> _parts;

    public Day19() : this(RunMode.Real)
    {
    }

    public Day19(RunMode runMode)
    {
        RunMode = runMode;

        var input = File
            .ReadAllLines(InputFilePath)
            .Cluster()
            .ToList();

        _rules = input[0].Select(ConvertTextToRule).ToDictionary(x => x.Item1, x => x.Item2);
        _parts = input[1].Select(ConvertTextToPart).ToList();
    }

    private (string, List<(char Value, char Rule, int Limit, string Next)>) ConvertTextToRule(string text)
    {
        var split = text[..^1].Split('{');
        var name = split[0];
        var workflows = split[1].Split(',');
        var result = new List<(char Value, char Rule, int Limit, string Next)>();

        foreach (var workflow in workflows)
        {
            var semicolonLocation = workflow.IndexOf(':');
            result.Add(
                (
                    semicolonLocation == -1 ? 'x' : workflow[0],
                    semicolonLocation == -1 ? '>' : workflow[1],
                    semicolonLocation == -1 ? 0 : int.Parse(workflow[2..semicolonLocation]),
                    semicolonLocation == -1 ? workflow : workflow[(semicolonLocation + 1)..]
                )
            );
        }

        return (name, result);
    }

    private Dictionary<char, int> ConvertTextToPart(string text)
    {
        return text[1..^1].Split(',').Select(x => x.Split('=')).ToDictionary(x => x[0][0], x => int.Parse(x[1]));
    }

    private bool DoesRuleApply(Dictionary<char, int> part, (char Value, char Rule, int Limit, string Next) rule)
    {
        var relevantValue = part[rule.Value];
        return rule.Rule switch
        {
            '>' => relevantValue > rule.Limit,
            '<' => relevantValue < rule.Limit,
            _ => throw new ArgumentOutOfRangeException("Did you mess something up?"),
        };
    }

    private int CalculatePartRating(Dictionary<char, int> part)
    {
        //Console.WriteLine($"Evaluating Part");
        var workflow = "in";

        while (workflow != "R" && workflow != "A")
        {
            //Console.WriteLine($"Am in workflow {workflow}");
            var rule = _rules[workflow];
            workflow = rule.First(x => DoesRuleApply(part, x)).Next;
        }

        //Console.WriteLine($"Am in workflow {workflow}");
        return workflow switch
        {
            "A" => part.Values.Sum(),
            "R" => 0,
            _ => throw new ArgumentOutOfRangeException("Did you mess something up?"),
        };
    }

    private Answer CalculatePart1Answer()
    {
        return _parts.Select(CalculatePartRating).Sum();
    }

    private List<Dictionary<char, (int Start, int End)>> ApplyRuleOnRange(Dictionary<char, (int Start, int End)> range, string workflow)
    {
        var acceptedRanges = new List<Dictionary<char, (int Start, int End)>>();

        // Make splits based on this workflow. Then iteratively extend for each split and take out the accepted.
        var currentRange = range;
        var rule = _rules[workflow];

        foreach (var subrule in rule)
        {
            // Split the range into two, the one that stays and the one that continues.
            var relevantRange = currentRange[subrule.Value];

            // TODO: Combine these into one statement.
            var rangeThatApplies = subrule.Rule switch
            {
                '>' when relevantRange.Start > subrule.Limit => relevantRange,
                '>' when relevantRange.End <= subrule.Limit => (0, 0),
                '>' => (subrule.Limit + 1, relevantRange.End),
                '<' when relevantRange.End < subrule.Limit => relevantRange,
                '<' when relevantRange.Start >= subrule.Limit => (0, 0),
                '<' => (relevantRange.Start, subrule.Limit - 1),
                _ => throw new ArgumentOutOfRangeException("Did you mess something up?"),
            };

            var fullRangeThatApplies = currentRange.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == subrule.Value ? rangeThatApplies : kvp.Value);

            var rangeThatContinues = subrule.Rule switch
            {
                '>' when relevantRange.Start > subrule.Limit => (0, 0),
                '>' when relevantRange.End <= subrule.Limit => relevantRange,
                '>' => (relevantRange.Start, subrule.Limit),
                '<' when relevantRange.End < subrule.Limit => (0, 0),
                '<' when relevantRange.Start >= subrule.Limit => relevantRange,
                '<' => (subrule.Limit, relevantRange.End),
                _ => throw new ArgumentOutOfRangeException("Did you mess something up?"),
            };

            var fullRangeThatContinues = currentRange.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == subrule.Value ? rangeThatContinues : kvp.Value);

            // If the range still has some stuff in it!
            if (rangeThatApplies.Item2 > rangeThatApplies.Item1)
            {
                switch (subrule.Next)
                {
                    case "A":
                        acceptedRanges.Add(fullRangeThatApplies);
                        break;
                    case "R":
                        break;
                    default:
                        acceptedRanges.AddRange(ApplyRuleOnRange(fullRangeThatApplies, subrule.Next));
                        break;
                };
            }

            if (rangeThatContinues.Item2 > rangeThatContinues.Item1)
            {
                currentRange = fullRangeThatContinues;
            }
            else
            {
                break;
            }
        }

        return acceptedRanges;
    }

    private Answer CalculatePart2Answer()
    {
        var startingRange = new Dictionary<char, (int Start, int End)>()
        {
            { 'x', (1, 4000) },
            { 'm', (1, 4000) },
            { 'a', (1, 4000) },
            { 's', (1, 4000) },
        };

        var acceptedRanges = ApplyRuleOnRange(startingRange, "in");

        //foreach (var acceptedRange in acceptedRanges)
        //{
        //    Console.WriteLine($"Accepted range:");
        //    foreach (var kvp in acceptedRange)
        //    {
        //        Console.WriteLine($"{kvp.Key} from {kvp.Value.Start} to {kvp.Value.End}");
        //    }
        //}

        // Let's see if any of them overlap:
        foreach (var (index1, acceptedRange1) in acceptedRanges.Enumerate())
        {
            foreach (var acceptedRange2 in acceptedRanges.Skip(index1+1))
            {
                if (Overlaps(acceptedRange1['x'], acceptedRange2['x'])
                    && Overlaps(acceptedRange1['m'], acceptedRange2['m'])
                    && Overlaps(acceptedRange1['a'], acceptedRange2['a'])
                    && Overlaps(acceptedRange1['s'], acceptedRange2['s'])
                   )
                {
                    Console.WriteLine($"The following ranges overlap:");
                    foreach (var kvp in acceptedRange1)
                    {
                        Console.WriteLine($"{kvp.Key} from {kvp.Value.Start} to {kvp.Value.End}");
                    }
                    foreach (var kvp in acceptedRange2)
                    {
                        Console.WriteLine($"{kvp.Key} from {kvp.Value.Start} to {kvp.Value.End}");
                    }
                    throw new NotImplementedException("If you're here, you better implement something for it!");
                }
            }
        }

        // The total number of parts stuff
        return acceptedRanges.Select(d => d.Values.Select(t => (long)(t.End - t.Start + 1)).Product()).Sum();
    }

    private bool Overlaps((int Start, int End) range1, (int Start, int End) range2)
    {
        var temp = range1.Start <= range2.End && range2.Start <= range1.End;
        return range1.Start <= range2.End && range2.Start <= range1.End;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

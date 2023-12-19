namespace AdventOfCode;

public enum Xmas
{
    X,
    M,
    A,
    S,
}

public sealed class Day19 : BaseTestableDay
{
    private readonly Dictionary<string, List<(Xmas Value, char Rule, int Limit, string Next)>> _workflows;
    private readonly List<Dictionary<Xmas, int>> _parts;

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

        _workflows = input[0].Select(ConvertTextToWorkflow).ToDictionary(x => x.Item1, x => x.Item2);
        _parts = input[1].Select(ConvertTextToPart).ToList();
    }

    private Xmas ConvertCharToXmas(char value)
    {
        return value switch
        {
            'x' => Xmas.X,
            'm' => Xmas.M,
            'a' => Xmas.A,
            's' => Xmas.S,
            _ => throw new ArgumentOutOfRangeException("Xmas", "Did you mess something up?"),
        };
    }

    private (string, List<(Xmas Value, char Rule, int Limit, string Next)>) ConvertTextToWorkflow(string text)
    {
        var split = text[..^1].Split('{');
        var name = split[0];
        var workflows = split[1].Split(',');
        var result = new List<(Xmas Value, char Rule, int Limit, string Next)>();

        foreach (var workflow in workflows)
        {
            var semicolonLocation = workflow.IndexOf(':');

            result.Add(
                (
                    semicolonLocation == -1 ? Xmas.X : ConvertCharToXmas(workflow[0]),
                    semicolonLocation == -1 ? '>' : workflow[1],
                    semicolonLocation == -1 ? 0 : int.Parse(workflow[2..semicolonLocation]),
                    semicolonLocation == -1 ? workflow : workflow[(semicolonLocation + 1)..]
                )
            );
        }

        return (name, result);
    }

    private Dictionary<Xmas, int> ConvertTextToPart(string text)
    {
        return text[1..^1]
            .Split(',')
            .Select(x => x.Split('='))
            .ToDictionary(x => ConvertCharToXmas(x[0][0]), x => int.Parse(x[1]));
    }

    private bool DoesRuleApply(Dictionary<Xmas, int> part, (Xmas Value, char Rule, int Limit, string Next) rule)
    {
        var relevantValue = part[rule.Value];

        return rule.Rule switch
        {
            '>' => relevantValue > rule.Limit,
            '<' => relevantValue < rule.Limit,
            _ => throw new ArgumentOutOfRangeException("Rule", "Did you mess something up?"),
        };
    }

    private int CalculatePartRating(Dictionary<Xmas, int> part)
    {
        //Console.WriteLine($"Evaluating Part");
        var workflowName = "in";

        while (workflowName != "R" && workflowName != "A")
        {
            //Console.WriteLine($"Am in workflow {workflow}");
            var workflow = _workflows[workflowName];
            workflowName = workflow.First(x => DoesRuleApply(part, x)).Next;
        }

        //Console.WriteLine($"Am in workflow {workflow}");
        return workflowName switch
        {
            "A" => part.Values.Sum(),
            "R" => 0,
            _ => throw new ArgumentOutOfRangeException("workflow", "Did you mess something up?"),
        };
    }

    private Answer CalculatePart1Answer()
    {
        return _parts.Select(CalculatePartRating).Sum();
    }

    private List<Dictionary<Xmas, (int Start, int End)>> ApplyRuleOnRange(Dictionary<Xmas, (int Start, int End)> range, string workflowName)
    {
        var acceptedRanges = new List<Dictionary<Xmas, (int Start, int End)>>();

        // Make splits based on this workflow. Then iteratively extend for each split and take out the accepted.
        var currentRange = range;
        var workflow = _workflows[workflowName];

        foreach (var rule in workflow)
        {
            // Split the range into two, the one that stays and the one that continues.
            var relevantRange = currentRange[rule.Value];

            var (rangeThatApplies, rangeThatContinues) = rule.Rule switch
            {
                '>' when relevantRange.Start > rule.Limit => (relevantRange, (0, 0)),
                '>' when relevantRange.End <= rule.Limit => ((0, 0), relevantRange),
                '>' => ((rule.Limit + 1, relevantRange.End), (relevantRange.Start, rule.Limit)),
                '<' when relevantRange.End < rule.Limit => (relevantRange, (0, 0)),
                '<' when relevantRange.Start >= rule.Limit => ((0, 0), relevantRange),
                '<' => ((relevantRange.Start, rule.Limit - 1), (rule.Limit, relevantRange.End)),
                _ => throw new ArgumentOutOfRangeException("rule", "Did you mess something up?"),
            };

            var fullRangeThatApplies = currentRange.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == rule.Value ? rangeThatApplies : kvp.Value);
            var fullRangeThatContinues = currentRange.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == rule.Value ? rangeThatContinues : kvp.Value);

            // If the range still has some stuff in it!
            if (rangeThatApplies.Item2 > rangeThatApplies.Item1)
            {
                switch (rule.Next)
                {
                    case "A":
                        acceptedRanges.Add(fullRangeThatApplies);
                        break;
                    case "R":
                        break;
                    default:
                        acceptedRanges.AddRange(ApplyRuleOnRange(fullRangeThatApplies, rule.Next));
                        break;
                }
            }

            if (rangeThatContinues.Item2 <= rangeThatContinues.Item1)
            {
                break;
            }

            currentRange = fullRangeThatContinues;
        }

        return acceptedRanges;
    }

    private Answer CalculatePart2Answer()
    {
        var startingRange = new Dictionary<Xmas, (int Start, int End)>()
        {
            { Xmas.X, (1, 4000) }, { Xmas.M, (1, 4000) }, { Xmas.A, (1, 4000) }, { Xmas.S, (1, 4000) },
        };

        var acceptedRanges = ApplyRuleOnRange(startingRange, "in");

        // Let's see if any of them overlap:
        foreach (var (index1, acceptedRange1) in acceptedRanges.Enumerate())
        {
            foreach (var acceptedRange2 in acceptedRanges.Skip(index1 + 1))
            {
                if (acceptedRange1.All(kvp1 => Overlaps(kvp1.Value, acceptedRange2[kvp1.Key])))
                {
                    Console.WriteLine("These two ranges overlap:");

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
        return range1.Start <= range2.End && range2.Start <= range1.End;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

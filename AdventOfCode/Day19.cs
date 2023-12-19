namespace AdventOfCode;

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

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

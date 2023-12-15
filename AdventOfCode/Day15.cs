using System.Collections.Specialized;
using System.Text;

namespace AdventOfCode;

public sealed class Day15 : BaseTestableDay
{
    private readonly List<string> _input;

    public Day15() : this(RunMode.Real)
    {
    }

    public Day15(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)[0]
            .Split(",")
            .Select(x => x.Trim())
            .ToList();
    }

    internal int Hash(string input)
    {
        return Encoding.ASCII.GetBytes(input).Aggregate(0, (x, y) => (x + y) * 17 % 256);
    }

    private Answer CalculatePart1Answer()
    {
        return _input.Select(Hash).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        var boxes = Enumerable
            .Range(0, 256)
            .Select(_ => new OrderedDictionary()) // OrderedDictionary<string, int>
            .ToList();

        foreach (var input in _input)
        {
            var operationIndex = input.Enumerate().First(x => x.Value is '=' or '-').Index;
            var label = input[..operationIndex];
            var labelHash = Hash(label);

            var box = boxes[labelHash];
            int.TryParse(input[(operationIndex + 1)..], out var focus);

            switch (input[operationIndex])
            {
                case '-':
                    box.Remove(label);
                    break;
                case '=':
                    box[label] = focus;
                    break;
            }
        }

        return boxes
            .SelectMany(
                (box, boxNumber) => box
                    .Values
                    .Cast<int>() // What a weird thing OrderedDictionary is...
                    .Select((focus, lensNumber) => (boxNumber + 1) * (lensNumber + 1) * focus))
            .Sum();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

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
        var boxes = Enumerable.Range(0, 256).Select(x => new List<(string Label, int Focus)>()).ToList();

        foreach (var input in _input)
        {
            var operationIndex = input.Enumerate().First(x => x.Value is '=' or '-').Index;
            var label = input[..operationIndex];
            var labelHash = Hash(label);

            var box = boxes[labelHash];
            var labelInBoxIndex = box.FindIndex(x => x.Label == label);

            int.TryParse(input[(operationIndex + 1)..], out var focus);

            switch (input[operationIndex])
            {
                case '-' when labelInBoxIndex != -1:
                    box.RemoveAt(labelInBoxIndex);
                    break;
                case '=' when labelInBoxIndex == -1:
                    box.Add((label, focus));
                    break;
                case '=': // In this case, labelInBoxIndex is not -1
                    box[labelInBoxIndex] = (label, focus);
                    break;
            }
        }

        return boxes.Enumerate().SelectMany(x => x.Value.Enumerate().Select(y => (x.Index + 1) * (y.Index + 1) * y.Value.Focus)).Sum();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

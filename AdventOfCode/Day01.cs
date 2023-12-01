using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AdventOfCode.Tests")]
namespace AdventOfCode
{
    public sealed class Day01 : BaseTestableDay
    {
        private readonly List<string> _input;

        public Day01() : this(RunMode.Real)
        {
        }

        public Day01(RunMode runMode)
        {
            RunMode = runMode;

            _input = File
                .ReadAllLines(InputFilePath)
                .ToList();
        }

        internal static int CalculateSingleDigitCalibrationValue(string text)
        {
            var letters = text.ToCharArray();

            var firstDigit = letters.FirstOrDefault(char.IsDigit, '0') - '0';
            var lastDigit = letters.LastOrDefault(char.IsDigit, '0') - '0';

            return firstDigit * 10 + lastDigit;
        }

        private Answer CalculateDigitCalibrationValue()
        {
            return _input.Select(CalculateSingleDigitCalibrationValue).Sum();
        }

        internal static int CalculateSingleLiteralCalibrationValue(string text)
        {
            var literals = new Dictionary<string, string>()
            {
                { "one", "o1e" },
                { "two", "t2o" },
                { "three", "t3e" },
                { "four", "f4r"},
                { "five", "f5e" },
                { "six", "s6x"},
                { "seven", "s7n" },
                { "eight", "e8t" },
                { "nine", "n9e" }
            };

            foreach (var kvp in literals)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }

            return CalculateSingleDigitCalibrationValue(text);
        }

        internal static int CalculateSingleLiteralCalibrationValueExplicit(string text)
        {
            var literals = new Dictionary<string, string>()
            {
                { "one", "1" },
                { "two", "2" },
                { "three", "3" },
                { "four", "4"},
                { "five", "5" },
                { "six", "6"},
                { "seven", "7" },
                { "eight", "8" },
                { "nine", "9" }
            };

            var digitMatches = literals
                .SelectMany(kvp => text.AllIndexesOf(kvp.Value).Select(index => (index, number: kvp.Value)))
                .ToList();

            var literalMatches = literals
                .SelectMany(kvp => text.AllIndexesOf(kvp.Key).Select(index => (index, number: kvp.Key)))
                .ToList();

            var matches = digitMatches.Concat(literalMatches).Where(x => x.index != -1).ToList();

            var first = matches.MinBy(x => x.index).number;
            var last = matches.MaxBy(x => x.index).number;

            var firstDigit = (literals.TryGetValue(first, out var firstLiteral) ? firstLiteral : first)[0] - '0';
            var lastDigit = (literals.TryGetValue(last, out var lastLiteral) ? lastLiteral : last)[0] - '0';

            return firstDigit * 10 + lastDigit;
        }

        private Answer CalculateLiteralCalibrationValue()
        {
            return _input.Select(CalculateSingleLiteralCalibrationValue).Sum();
        }

        public override ValueTask<string> Solve_1() => new(CalculateDigitCalibrationValue());

        public override ValueTask<string> Solve_2() => new(CalculateLiteralCalibrationValue());
    }
}

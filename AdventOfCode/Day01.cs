using System;
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

        private int CalculateDigitCalibrationValue()
        {
            return _input.Select(CalculateSingleDigitCalibrationValue).Sum();
        }

        //internal static int CalculateSingleLiteralCalibrationValue(string text)
        //{
        //    var literals = new Dictionary<string, string> ()
        //    {
        //        { "one", "1" },
        //        { "two", "2" },
        //        { "three", "3" },
        //        { "four", "4"},
        //        { "five", "5" },
        //        { "six", "6"},
        //        { "seven", "7" },
        //        { "eight", "8" },
        //        { "nine", "9" }
        //    };

        //    var locations_of_numbers = literals
        //        .SelectMany(kvp => new[]
        //        {
        //            (text.IndexOf(kvp.Key), kvp.Key),
        //            (text.IndexOf(kvp.Value), kvp.Value)
        //        })
        //        .Where(x => x.Item1 != -1)
        //    .ToList();

        //    var first = locations_of_numbers.MinBy(x => x.Item1).Item2;
        //    var last = locations_of_numbers.MaxBy(x => x.Item1).Item2;

        //    var first_digit = (literals.ContainsKey(first) ? literals[first] : first)[0] - '0';
        //    var last_digit = (literals.ContainsKey(last) ? literals[last] : last)[0] - '0';

        //    Console.WriteLine($"{text}: {first_digit} -> {last_digit}");

        //    return first_digit * 10 + last_digit;
        //}

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

        private int CalculateLiteralCalibrationValue()
        {
            return _input.Select(CalculateSingleLiteralCalibrationValue).Sum();
        }

        public override ValueTask<string> Solve_1() => new($"{CalculateDigitCalibrationValue()}");

        public override ValueTask<string> Solve_2() => new($"{CalculateLiteralCalibrationValue()}");
    }
}

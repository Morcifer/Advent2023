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

            var first_digit = letters.First(c => char.IsDigit(c)) - '0';
            var last_digit = letters.Last(c => char.IsDigit(c)) - '0';

            return first_digit * 10 + last_digit;
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
                { "one", "one1one" },
                { "two", "two2two" },
                { "three", "three3three" },
                { "four", "four4four"},
                { "five", "five5five" },
                { "six", "six6six"},
                { "seven", "seven7seven" },
                { "eight", "eight8eight" },
                { "nine", "nine9nine" }
            };

            foreach (var kvp in literals)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }

            return CalculateSingleDigitCalibrationValue(text);
        }

        private int CalculateLiteralCalibrationValue()
        {
            var input = _input;
            if (RunMode == RunMode.Test)
            {
                input = new List<string>
                {
                    "two1nine",
                    "eightwothree",
                    "abcone2threexyz",
                    "xtwone3four",
                    "4nineeightseven2",
                    "zoneight234",
                    "7pqrstsixteen",
                };
            }

            return input.Select(CalculateSingleLiteralCalibrationValue).Sum();
        }

        public override ValueTask<string> Solve_1() => new($"{CalculateDigitCalibrationValue()}");

        public override ValueTask<string> Solve_2() => new($"{CalculateLiteralCalibrationValue()}");
    }
}

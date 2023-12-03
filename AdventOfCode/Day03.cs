using System.Runtime.CompilerServices;
using Spectre.Console;

[assembly: InternalsVisibleTo("AdventOfCode.Tests")]
namespace AdventOfCode
{
    public sealed class Day03 : BaseTestableDay
    {
        private readonly List<string> _input;

        public Day03() : this(RunMode.Real)
        {
        }

        public Day03(RunMode runMode)
        {
            RunMode = runMode;

            _input = File
                .ReadAllLines(InputFilePath)
                .ToList();
        }

        private bool IsNextToSymbol(string number, int row, List<int> columns)
        {
            var neighbours = new List<(int, int)>()
            {
                (row, columns.Min() - 1),
                (row, columns.Max() + 1),
            };

            for (int i = columns.Min() - 1; i <= columns.Max() + 1; i++)
            {
                neighbours.Add((row - 1, i));
                neighbours.Add((row + 1, i));
            }

            var temp = neighbours
                .Where(x => x.Item1 >= 0 && x.Item1 < _input[0].Length && x.Item2 >= 0 && x.Item2 < _input[0].Length)
                .Any(x => _input[x.Item1][x.Item2] != '.' && !char.IsDigit(_input[x.Item1][x.Item2]));

            if (temp)
            {
                Console.WriteLine($"Row {row}: {number}");
            }

            return temp;
        }

        private Answer CalculatePartNumberSum()
        {
            // Find all number locations
            var numberLocations = new List<(string Number, int Row, List<int> Columns)>();

            for (var rowIndex = 0; rowIndex < _input.Count; rowIndex++)
            {
                // I need an Enumerate method!
                var row = _input[rowIndex];
                var chars = row.ToCharArray();

                var foundDigits = new List<int>();

                for (var columnIndex = 0; columnIndex < chars.Length; columnIndex++)
                {
                    if (char.IsDigit(chars[columnIndex]))
                    {
                        foundDigits.Add(columnIndex);
                    }
                    else if (foundDigits.Any())
                    {
                        numberLocations.Add((_input[rowIndex][foundDigits[0]..(foundDigits[^1] + 1)], rowIndex, foundDigits));
                        foundDigits = new List<int>();
                    }
                }

                if (foundDigits.Any())
                {
                    numberLocations.Add((_input[rowIndex][foundDigits[0]..(foundDigits[^1] + 1)], rowIndex, foundDigits));
                }
            }

            return numberLocations
                .Where(x => IsNextToSymbol(x.Number, x.Row, x.Columns))
                .Select(x => int.Parse(x.Number))
                .Sum();
        }

        private bool IsNextToGear(string number, int row, List<int> columns, int gearRow, int gearColumn)
        {
            var neighbours = new List<(int, int)>()
            {
                (row, columns.Min() - 1),
                (row, columns.Max() + 1),
            };

            for (int i = columns.Min() - 1; i <= columns.Max() + 1; i++)
            {
                neighbours.Add((row - 1, i));
                neighbours.Add((row + 1, i));
            }

            var temp = neighbours
                .Where(x => x.Item1 >= 0 && x.Item1 < _input[0].Length && x.Item2 >= 0 && x.Item2 < _input[0].Length)
                .Any(x => x.Item1 == gearRow && x.Item2 == gearColumn);

            if (temp)
            {
                Console.WriteLine($"Row {row}: {number}");
            }

            return temp;
        }

        private Answer CalculateGearRatioSums()
        {
            // Find all number locations
            var numberLocations = new List<(string Number, int Row, List<int> Columns)>();

            for (var rowIndex = 0; rowIndex < _input.Count; rowIndex++)
            {
                // I need an Enumerate method!
                var row = _input[rowIndex];
                var chars = row.ToCharArray();

                var foundDigits = new List<int>();

                for (var columnIndex = 0; columnIndex < chars.Length; columnIndex++)
                {
                    if (char.IsDigit(chars[columnIndex]))
                    {
                        foundDigits.Add(columnIndex);
                    }
                    else if (foundDigits.Any())
                    {
                        numberLocations.Add((_input[rowIndex][foundDigits[0]..(foundDigits[^1] + 1)], rowIndex, foundDigits));
                        foundDigits = new List<int>();
                    }
                }

                if (foundDigits.Any())
                {
                    numberLocations.Add((_input[rowIndex][foundDigits[0]..(foundDigits[^1] + 1)], rowIndex, foundDigits));
                }
            }

            // Find all gear locations
            var gearLocations = new List<(int Row, int Column)>();

            for (var rowIndex = 0; rowIndex < _input.Count; rowIndex++)
            {
                var row = _input[rowIndex];
                for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    if (row[columnIndex] == '*')
                    {
                        gearLocations.Add((rowIndex, columnIndex));
                    }
                }
            }

            return gearLocations
                .Select(gear => (
                    gear, 
                    numberLocations
                        .Where(number => IsNextToGear(number.Number, number.Row, number.Columns, gear.Row, gear.Column))
                        .ToList())
                        )
                .Where(x => x.Item2.Count == 2)
                .Select(x => int.Parse(x.Item2[0].Number) * int.Parse(x.Item2[1].Number))
                .Sum();
        }

        public override ValueTask<string> Solve_1() => new(CalculatePartNumberSum());

        public override ValueTask<string> Solve_2() => new(CalculateGearRatioSums());
    }
}

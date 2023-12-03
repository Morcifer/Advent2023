using System.Runtime.CompilerServices;

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

        private List<(int Number, int Row, List<int> Columns)> FindNumbers()
        {
            var numberLocations = new List<(int Number, int Row, List<int> Columns)>();

            foreach (var (rowIndex, row) in _input.Enumerate())
            {
                var foundDigits = new List<int>();

                foreach (var (columnIndex, character) in row.ToCharArray().Enumerate())
                {
                    if (char.IsDigit(character))
                    {
                        foundDigits.Add(columnIndex);
                    }
                    else if (foundDigits.Any())
                    {
                        numberLocations.Add(
                            (
                                int.Parse(_input[rowIndex][foundDigits[0]..(foundDigits[^1] + 1)]), 
                                rowIndex, 
                                foundDigits
                            )
                        );
                        foundDigits = new List<int>();
                    }
                }

                if (foundDigits.Any())
                {
                    numberLocations.Add(
                        (
                            int.Parse(_input[rowIndex][foundDigits[0]..(foundDigits[^1] + 1)]), 
                            rowIndex, 
                            foundDigits
                        )
                    );
                }
            }

            return numberLocations;
        }

        private bool IsNextToSymbol(int row, List<int> columns)
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

            return neighbours
                .Where(x => x.Item1 >= 0 && x.Item1 < _input[0].Length && x.Item2 >= 0 && x.Item2 < _input[0].Length)
                .Any(x => _input[x.Item1][x.Item2] != '.' && !char.IsDigit(_input[x.Item1][x.Item2]));
        }

        private Answer CalculatePartNumberSum()
        {
            var numberLocations = FindNumbers();

            return numberLocations
                .Where(x => IsNextToSymbol(x.Row, x.Columns))
                .Select(x => x.Number)
                .Sum();
        }

        private bool IsNextToGear(int row, List<int> columns, int gearRow, int gearColumn)
        {
            return gearRow >= row - 1 
                   && gearRow <= row + 1 
                   && gearColumn >= columns.Min() - 1 
                   && gearColumn <= columns.Max() + 1;
        }

        private Answer CalculateGearRatioSums()
        {
            var numberLocations = FindNumbers();

            // Find all gear locations
            var gearLocations = new List<(int Row, int Column)>();

            foreach (var (rowIndex, row) in _input.Enumerate())
            {
                foreach (var (columnIndex, character) in row.ToCharArray().Enumerate())
                {
                    if (character == '*')
                    {
                        gearLocations.Add((rowIndex, columnIndex));
                    }
                }
            }

            return gearLocations
                .Select(gear => (
                    gear, 
                    numberLocations
                        .Where(number => IsNextToGear(number.Row, number.Columns, gear.Row, gear.Column))
                        .ToList())
                        )
                .Where(x => x.Item2.Count == 2)
                .Select(x => x.Item2[0].Number * x.Item2[1].Number)
                .Sum();
        }

        public override ValueTask<string> Solve_1() => new(CalculatePartNumberSum());

        public override ValueTask<string> Solve_2() => new(CalculateGearRatioSums());
    }
}

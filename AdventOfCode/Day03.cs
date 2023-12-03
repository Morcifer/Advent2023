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

            void MaybeUpdateNumberLocations(int rowIndex, List<int> columnIndices)
            {
                if (columnIndices.Any())
                {
                    numberLocations.Add(
                        (
                            int.Parse(_input[rowIndex][columnIndices[0]..(columnIndices[^1] + 1)]),
                            rowIndex,
                            columnIndices
                        )
                    );
                }
            }

            foreach (var (rowIndex, row) in _input.Enumerate())
            {
                var foundDigits = new List<int>();

                foreach (var (columnIndex, character) in row.ToCharArray().Enumerate())
                {
                    if (char.IsDigit(character))
                    {
                        foundDigits.Add(columnIndex);
                    }
                    else
                    {
                        MaybeUpdateNumberLocations(rowIndex, foundDigits);
                        foundDigits = new List<int>();
                    }
                }

                // Don't forget the last update!
                MaybeUpdateNumberLocations(rowIndex, foundDigits);
            }

            return numberLocations;
        }

        private bool IsNextToSymbol(int row, List<int> columns)
        {
            var neighbors = new List<(int, int)>()
            {
                (row, columns.Min() - 1),
                (row, columns.Max() + 1),
            };

            for (var i = columns.Min() - 1; i <= columns.Max() + 1; i++)
            {
                neighbors.Add((row - 1, i));
                neighbors.Add((row + 1, i));
            }

            return neighbors
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

        private List<(int Row, int Column)> FindGears()
        {
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

            return gearLocations;
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
            var gearLocations = FindGears();

            return gearLocations
                .Select(gear => 
                    numberLocations
                        .Where(number => IsNextToGear(number.Row, number.Columns, gear.Row, gear.Column))
                        .ToList()
                    )
                .Where(partNumbers => partNumbers.Count == 2)
                .Select(partNumbers => partNumbers[0].Number * partNumbers[1].Number)
                .Sum();
        }

        public override ValueTask<string> Solve_1() => new(CalculatePartNumberSum());

        public override ValueTask<string> Solve_2() => new(CalculateGearRatioSums());
    }
}

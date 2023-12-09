using MoreLinq.Extensions;

namespace AdventOfCode;

public sealed class Day09 : BaseTestableDay
{
    private readonly List<Polynomial> _polynomials;

    public Day09() : this(RunMode.Real)
    {
    }

    public Day09(RunMode runMode)
    {
        RunMode = runMode;

        _polynomials = File
            .ReadAllLines(InputFilePath)
            .Select(ConvertToPolynomial)
            .ToList();
    }

    public class Polynomial
    {
        private readonly List<List<long>> _triangle;

        public Polynomial(List<long> numbers)
        {
            _triangle = new List<List<long>>();
            var triangleValues = numbers.ToList();

            while (triangleValues.Any(x => x != 0))
            {
                _triangle.Add(triangleValues);
                triangleValues = triangleValues.Pairwise((x, y) => y - x).ToList();
            }
        }

        public long CalculateNextValue()
        {
            return _triangle
                .Select(x => x[^1])
                .Sum();
        }

        public long CalculatePreviousValue()
        {
            return _triangle
                .Enumerate()
                .Select(x => (long)Math.Pow(-1, x.Index) * x.Value[0])
                .Sum();
        }
    }

    private Polynomial ConvertToPolynomial(string input)
    {
        return new Polynomial(input.Split(' ').Select(long.Parse).ToList());
    }

    private Answer CalculatePart1Answer()
    {
        return _polynomials.Select(p => p.CalculateNextValue()).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        return _polynomials.Select(p => p.CalculatePreviousValue()).Sum();
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

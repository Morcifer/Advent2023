using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Day01 : BaseDay
    {
        private readonly List<int> _input;

        // With this, it's 2 + 2 + 654 + 33583 = 34,241 for part 1, and 2 + 2 + 966 + 50346 = 51,316
        //protected override string InputFileExtension => "test.txt"; 

        public Day01()
        {
            _input = File
                .ReadAllLines(InputFilePath)
                .Select(int.Parse)
                .ToList();
        }

        private int CalculateSingleFuel(int mass)
        {
            return (int)Math.Floor(mass / 3.0) - 2;
        }

        private int CalculateSingleCompoundFuel(int mass)
        {
            int extraFuel = Math.Max(0, CalculateSingleFuel(mass));
            int totalFuel = extraFuel;

            while (extraFuel > 0)
            {
                extraFuel = Math.Max(0, CalculateSingleFuel(extraFuel));
                totalFuel += extraFuel;
            }

            return totalFuel;
        }

        internal double CalculateFuel()
        {
            return _input.Select(mass => CalculateSingleFuel(mass)).Sum();
        }

        internal double CalculateCompoundFuel()
        {
            return _input.Select(mass => CalculateSingleCompoundFuel(mass)).Sum();
        }

        public override ValueTask<string> Solve_1() => new($"{CalculateFuel()}");

        public override ValueTask<string> Solve_2() => new($"{CalculateCompoundFuel()}");
    }
}

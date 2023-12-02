using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AdventOfCode.Tests")]
namespace AdventOfCode
{
    public sealed class Day02 : BaseTestableDay
    {
        private readonly List<(int, List<Dictionary<string, int>>)> _input;

        public Day02() : this(RunMode.Real)
        {
        }

        private (int, List<Dictionary<string, int>>) ConvertTextToGame(string text)
        {
            // text = Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            var splitByColon = text.Split(':').Select(s => s.Trim()).ToList();
            var gameId = int.Parse(splitByColon[0][5..]);

            // sessions = 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            var sessions = splitByColon[1]
                .Split(';')
                .Select(s => s.Trim())
                .ToList();

            var result = new List<Dictionary<string, int>>();

            foreach (var session in sessions)
            {
                var cubes = session // 3 blue, 4 red
                    .Split(',')
                    .Select(s => s.Trim())
                    .Select(s => s.Split(' ')) // 3 blue => [3, blue]
                    .ToDictionary(x => x[1], x => int.Parse(x[0]));

                result.Add(cubes);
            }

            return (gameId, result);
        }

        public Day02(RunMode runMode)
        {
            RunMode = runMode;

            _input = File
                .ReadAllLines(InputFilePath)
                .Select(ConvertTextToGame)
                .ToList();
        }

        internal static (int, int, int) GetMaxRequired((int, List<Dictionary<string, int>>) game)
        {
            var maxRed = game.Item2.Select(session => session.GetValueOrDefault("red", 0)).Max();
            var maxGreen = game.Item2.Select(session => session.GetValueOrDefault("green", 0)).Max();
            var maxBlue = game.Item2.Select(session => session.GetValueOrDefault("blue", 0)).Max();

            return (maxRed, maxGreen, maxBlue);
        }

        private Answer CalculateNumberOfPossibleGames()
        {
            return _input
                .Select(game => (game.Item1, GetMaxRequired(game)))
                .Where(x => x.Item2.Item1 <= 12 && x.Item2.Item2 <= 13 && x.Item2.Item3 <= 14)
                .Select(x => x.Item1)
                .Sum();
        }

        private Answer MinimalCubeCount()
        {
            return _input
                .Select(GetMaxRequired)
                .Select(x => x.Item1 * x.Item2 * x.Item3)
                .Sum();
        }

        public override ValueTask<string> Solve_1() => new(CalculateNumberOfPossibleGames());

        public override ValueTask<string> Solve_2() => new(MinimalCubeCount());
    }
}

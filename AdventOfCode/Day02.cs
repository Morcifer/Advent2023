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
                var temp = session // 3 blue, 4 red
                    .Split(',')
                    .Select(s => s.Trim())
                    .Select(s => s.Split(' ').ToList())
                    .ToList();

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

        internal static bool IsGamePossible((int, List<Dictionary<string, int>>) game)
        {
            // red, green, or blue
            var maxRed = 0;
            var maxGreen = 0;
            var maxBlue = 0;

            foreach (var session in game.Item2)
            {
                maxRed = Math.Max(maxRed, session.ContainsKey("red") ? session["red"] : 0);
                maxGreen = Math.Max(maxGreen, session.ContainsKey("green") ? session["green"] : 0);
                maxBlue = Math.Max(maxBlue, session.ContainsKey("blue") ? session["blue"] : 0);
            }

            return maxRed <= 12 && maxGreen <= 13 && maxBlue <= 14;
        }

        private Answer CalculateNumberOfPossibleGames()
        {
            return _input.Where(IsGamePossible).Sum(x => x.Item1);
        }

        internal static int MinimalCubeCountForGame((int, List<Dictionary<string, int>>) game)
        {
            // red, green, or blue
            var maxRed = 0;
            var maxGreen = 0;
            var maxBlue = 0;

            foreach (var session in game.Item2)
            {
                maxRed = Math.Max(maxRed, session.ContainsKey("red") ? session["red"] : 0);
                maxGreen = Math.Max(maxGreen, session.ContainsKey("green") ? session["green"] : 0);
                maxBlue = Math.Max(maxBlue, session.ContainsKey("blue") ? session["blue"] : 0);
            }

            return maxRed * maxGreen * maxBlue;
        }

        private Answer MinimalCubeCount()
        {
            return _input.Select(MinimalCubeCountForGame).Sum();
        }

        public override ValueTask<string> Solve_1() => new(CalculateNumberOfPossibleGames());

        public override ValueTask<string> Solve_2() => new(MinimalCubeCount());
    }
}

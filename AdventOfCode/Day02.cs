using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AdventOfCode.Tests")]

namespace AdventOfCode;

public sealed class Day02 : BaseTestableDay
{
    private readonly List<Game> _input;

    public class GameSession
    {
        public required int Red;
        public required int Green;
        public required int Blue;
    }

    public class Game
    {
        public required int Id;
        public required List<GameSession> Sessions;
    }

    public Day02() : this(RunMode.Real)
    {
    }

    private Game ConvertTextToGame(string text)
    {
        // text = "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green"
        var splitByColon = text.Split(':').Select(s => s.Trim()).ToList();
        var gameId = int.Parse(splitByColon[0][5..]);

        // sessions = "3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green"
        var sessions = splitByColon[1]
            .Split(';')
            .Select(s => s.Trim())
            .ToList();

        var result = new List<Dictionary<string, int>>();

        foreach (var session in sessions)
        {
            var cubes = session // session = "3 blue, 4 red"
                .Split(',')
                .Select(s => s.Trim())
                .Select(s => s.Split(' ')) // "3 blue" => ["3", "blue"]
                .ToDictionary(x => x[1], x => int.Parse(x[0]));

            result.Add(cubes);
        }

        return new Game
        {
            Id = gameId,
            Sessions = result
                .Select(
                    session => new GameSession()
                    {
                        Red = session.GetValueOrDefault("red", 0),
                        Blue = session.GetValueOrDefault("blue", 0),
                        Green = session.GetValueOrDefault("green", 0),
                    }
                )
                .ToList()
        };
    }

    public Day02(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(ConvertTextToGame)
            .ToList();
    }

    internal static (int Red, int Green, int Blue) GetMaxRequired(Game game)
    {
        var maxRed = game.Sessions.Select(session => session.Red).Max();
        var maxGreen = game.Sessions.Select(session => session.Green).Max();
        var maxBlue = game.Sessions.Select(session => session.Blue).Max();

        return (maxRed, maxGreen, maxBlue);
    }

    private Answer CalculateNumberOfPossibleGames()
    {
        return _input
            .Select(game => (gameId: game.Id, max: GetMaxRequired(game)))
            .Where(x => x.max is { Red: <= 12, Green: <= 13, Blue: <= 14 })
            .Select(x => x.gameId)
            .Sum();
    }

    private Answer MinimalCubeCount()
    {
        return _input
            .Select(GetMaxRequired)
            .Select(x => x.Red * x.Green * x.Blue)
            .Sum();
    }

    public override ValueTask<string> Solve_1() => new(CalculateNumberOfPossibleGames());

    public override ValueTask<string> Solve_2() => new(MinimalCubeCount());
}
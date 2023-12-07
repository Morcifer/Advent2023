namespace AdventOfCode;

public sealed class Day06 : BaseTestableDay
{
    private readonly List<(int Time, int DistanceRecord)> _races;
    private readonly (long Time, long DistanceRecord) _longRace;

    public Day06() : this(RunMode.Real)
    {
    }

    public Day06(RunMode runMode)
    {
        RunMode = runMode;

        _races = runMode == RunMode.Test
            ? new List<(int Time, int Distance)>() { (7, 9), (15, 40), (30, 200) }
            : new List<(int Time, int Distance)>() { (40, 219), (81, 1012), (77, 1365), (72, 1089) };

        _longRace = RunMode == RunMode.Test
            ? (71530, 940200)
            : (40817772, 219101213651089);
    }

    private Answer GetWinningPossibilities()
    {
        return _races
            .Select(
                race => Enumerable
                    .Range(0, race.Time + 1)
                    .Count(time => (race.Time - time) * time > race.DistanceRecord)
            )
            .Aggregate(1, (x, y) => x * y); // .Product(). Should extract this at some point.
    }

    // ReSharper disable once UnusedMember.Local
    private Answer GetStupidWinningPossibilities()
    {
        for (var startTime = (long)0; startTime <= _longRace.Time; startTime++)
        {
            var distance = (_longRace.Time - startTime) * startTime;

            if (distance > _longRace.DistanceRecord)
            {
                return _longRace.Time - (2 * startTime) + 1;
            }
        }

        return -1;
    }

    private Answer GetSmartWinningPossibilities()
    {
        var delta = Math.Pow(_longRace.Time, 2) - (4 * _longRace.DistanceRecord);

        var start = (long)Math.Ceiling((_longRace.Time - Math.Sqrt(delta)) / 2);
        var end = (long)Math.Floor((_longRace.Time + Math.Sqrt(delta)) / 2);

        return end - start + 1;
    }

    public override ValueTask<string> Solve_1() => GetWinningPossibilities();

    public override ValueTask<string> Solve_2() => GetSmartWinningPossibilities();
}

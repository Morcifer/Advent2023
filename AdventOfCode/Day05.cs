namespace AdventOfCode;

public sealed class Day05 : BaseTestableDay
{
    private readonly List<long> _seeds;
    private readonly List<Conversion> _conversions;

    public Day05() : this(RunMode.Real)
    {
    }

    public Day05(RunMode runMode)
    {
        RunMode = runMode;

        var clusters = File
            .ReadAllLines(InputFilePath)
            .Cluster()
            .ToList();

        _seeds = clusters[0][0].Split(' ').Skip(1).Select(long.Parse).ToList();
        _conversions = clusters.Skip(1).Select(ConvertTextToConversion).ToList();
    }

    public class Conversion
    {
        public required string From;
        public required string To;
        public required List<(long DestinationRangeStart, long SourceRangeStart, long RangeLength)> Conversions;
    }

    private Conversion ConvertTextToConversion(List<string> cluster)
    {
        var mappingName = cluster[0].Split(new[] { '-', ' ' }).ToList();

        return new Conversion
        {
            From = mappingName[0],
            To = mappingName[2],
            Conversions = cluster
                .Skip(1)
                .Select(s => s.Split(' ').Select(long.Parse).ToList())
                .Select(l => (l[0], l[1], l[2]))
                .ToList(),
        };
    }

    internal long GetLocation(long seed)
    {
        var value = seed;

        foreach (var conversion in _conversions)
        {
            var validConversion = conversion.Conversions.FirstOrDefault(x => x.SourceRangeStart <= value && value <= x.SourceRangeStart + x.RangeLength);
            value = validConversion == default ? value : validConversion.DestinationRangeStart + (value - validConversion.SourceRangeStart);
        }

        return value;
    }

    private Answer GetClosestLocation()
    {
        return _seeds
            .Select(GetLocation)
            .Min();
    }

    private Answer GetSomething()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => GetClosestLocation();

    public override ValueTask<string> Solve_2() => GetSomething();
}

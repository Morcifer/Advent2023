using MoreLinq;

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
        public required List<(long SourceRangeStart, long SourceRangeEnd, long SourceToDestinationOffset)> Ranges;
    }

    private Conversion ConvertTextToConversion(List<string> cluster)
    {
        var mappingName = cluster[0].Split(new[] { '-', ' ' }).ToList();

        return new Conversion
        {
            From = mappingName[0],
            To = mappingName[2],
            Ranges = cluster
                .Skip(1)
                .Select(s => s.Split(' ').Select(long.Parse).ToList())
                .Select(l => (DestinationRangeStart: l[0], SourceRangeStart: l[1], RangeLength: l[2]))
                .Select(x => (x.SourceRangeStart, x.SourceRangeStart + x.RangeLength - 1, x.DestinationRangeStart - x.SourceRangeStart))
                .ToList(),
        };
    }

    internal long GetLocation(long seed)
    {
        var value = seed;

        foreach (var conversion in _conversions)
        {
            var validConversion = conversion.Ranges
                .FirstOrDefault(x => x.SourceRangeStart <= value && value <= x.SourceRangeEnd);

            value = validConversion == default
                ? value
                : value + validConversion.SourceToDestinationOffset;
        }

        return value;
    }

    private Answer GetClosestLocation()
    {
        return _seeds
            .Select(GetLocation)
            .Min();
    }

    internal List<(long LocationRangeFrom, long LocationRangeTo)> GetLocationRanges(long seedRangeFrom, long seedRangeTo)
    {
        //Console.WriteLine($"Figuring out range of {seedRangeFrom} -> {seedRangeTo}");
        var ranges = new List<(long RangeFrom, long RangeTo)>() { (seedRangeFrom, seedRangeTo) };

        foreach (var conversion in _conversions)
        {
            //Console.WriteLine($"Conversion {conversion.From} to {conversion.To}");
            var newRanges = new List<(long RangeFrom, long RangeTo)>();

            foreach (var range in ranges)
            {
                var (rangeFrom, rangeTo) = range;

                while (rangeFrom <= rangeTo)
                {
                    // Find first containing conversion.
                    var validConversion = conversion.Ranges
                        .FirstOrDefault(x => x.SourceRangeStart <= rangeFrom && rangeFrom <= x.SourceRangeEnd);

                    if (validConversion == default)
                    {
                        // Find closest conversion (which might overlap with the range).
                        var closeConversions = conversion.Ranges
                            .Where(x => x.SourceRangeStart >= rangeFrom)
                            .ToList();

                        if (!closeConversions.Any())
                        {
                            newRanges.Add((rangeFrom, rangeTo));
                            break;
                        }

                        var closestConversion = closeConversions.MinBy(x => x.SourceRangeStart);

                        newRanges.Add((rangeFrom, Math.Min(rangeTo, closestConversion.SourceRangeStart - 1)));
                        rangeFrom = closestConversion.SourceRangeStart;
                    }
                    else
                    {
                        // Check if the range fully fits within the valid conversion
                        newRanges.Add(
                            (
                                validConversion.SourceToDestinationOffset + rangeFrom,
                                validConversion.SourceToDestinationOffset + Math.Min(rangeTo, validConversion.SourceRangeEnd)
                            )
                        );

                        rangeFrom = validConversion.SourceRangeEnd + 1;
                    }
                }
            }

            //Console.WriteLine($"Conversion {conversion.From} to {conversion.To} ends with {newRanges.Count} ranges");
            ranges = newRanges;
        }

        return ranges;
    }

    private Answer GetClosestLocationForRanges()
    {
        return _seeds.Batch(2)
            .SelectMany(x => GetLocationRanges(x[0], x[0] + x[1] - 1))
            .Select(range => range.LocationRangeFrom)
            .Min();
    }

    public override ValueTask<string> Solve_1() => GetClosestLocation();

    public override ValueTask<string> Solve_2() => GetClosestLocationForRanges();
}

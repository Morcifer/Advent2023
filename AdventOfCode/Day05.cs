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
            // TODO: Check if this is correct for edge cases, i.e. ranges of length 1...
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
                    var validConversion = conversion.Conversions
                        .FirstOrDefault(x => x.SourceRangeStart <= rangeFrom && rangeFrom <= x.SourceRangeStart + x.RangeLength - 1);

                    if (validConversion == default)
                    {
                        // Find closest conversion
                        var closeConversions = conversion.Conversions
                            .Where(x => x.SourceRangeStart >= rangeFrom)
                            .ToList();

                        if (!closeConversions.Any())
                        {
                            newRanges.Add((rangeFrom, rangeTo));
                            break;
                        }

                        var closestConversion = closeConversions.MinBy(x => x.SourceRangeStart);

                        if (rangeTo < closestConversion.SourceRangeStart)
                        {
                            newRanges.Add((rangeFrom, rangeTo));
                            break;
                        }

                        newRanges.Add((rangeFrom, closestConversion.SourceRangeStart - 1));
                        rangeFrom = closestConversion.SourceRangeStart;
                    }
                    else
                    {
                        // Check if I fit within the range of the valid conversion
                        var validConversionRangeEnd = validConversion.SourceRangeStart + validConversion.RangeLength - 1;
                        if (rangeTo <= validConversionRangeEnd)
                        {
                            newRanges.Add(
                                (
                                    validConversion.DestinationRangeStart + (rangeFrom - validConversion.SourceRangeStart),
                                    validConversion.DestinationRangeStart + (rangeTo - validConversion.SourceRangeStart)
                                ));

                            break;
                        }

                        newRanges.Add(
                            (
                                validConversion.DestinationRangeStart + (rangeFrom - validConversion.SourceRangeStart),
                                validConversion.DestinationRangeStart + (validConversionRangeEnd - validConversion.SourceRangeStart)
                            ));

                        rangeFrom = validConversionRangeEnd + 1;
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
        return Enumerable.Range(0, _seeds.Count / 2)
            .Select(i => (_seeds[i * 2], _seeds[i * 2] + _seeds[i * 2 + 1] - 1))
            .SelectMany(x => GetLocationRanges(x.Item1, x.Item2))
            .Select(range => range.LocationRangeFrom)
            .Min();
    }

    public override ValueTask<string> Solve_1() => GetClosestLocation();

    public override ValueTask<string> Solve_2() => GetClosestLocationForRanges();
}

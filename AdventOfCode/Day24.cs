using System;

namespace AdventOfCode;

public sealed class Day24 : BaseTestableDay
{
    private readonly List<((long Px, long Py, long Pz) Position, (long Vx, long Vy, long Vz) Velocity)> _input;

    public Day24() : this(RunMode.Real)
    {
    }

    public Day24(RunMode runMode)
    {
        RunMode = runMode;

        _input = File.ReadAllLines(InputFilePath).Select(ConvertTextToHailstone).ToList();
    }

    private ((long Px, long Py, long Pz) Position, (long Vx, long Vy, long Vz) Velocity) ConvertTextToHailstone(string text)
    {
        var split = text.Split('@', StringSplitOptions.TrimEntries);

        var position = split[0].Split(',', StringSplitOptions.TrimEntries).Select(long.Parse).ToList();
        var velocity = split[1].Split(',', StringSplitOptions.TrimEntries).Select(long.Parse).ToList();

        return ((position[0], position[1], position[2]), (velocity[0], velocity[1], velocity[2]));
    }

    private (double X, double Y, double t1, double t2)? Intersection(
        ((long Px, long Py, long Pz) Position, (long Vx, long Vy, long Vz) Velocity) stone1,
        ((long Px, long Py, long Pz) Position, (long Vx, long Vy, long Vz) Velocity) stone2
    )
    {
        var a1 = (double)stone1.Velocity.Vy / stone1.Velocity.Vx;
        var b1 = stone1.Position.Py - a1 * stone1.Position.Px;

        var a2 = (double)stone2.Velocity.Vy / stone2.Velocity.Vx;
        var b2 = stone2.Position.Py - a2 * stone2.Position.Px;

        if (Math.Abs(a1 - a2) < 0.000001)
        {
            //Console.WriteLine("No collision!");
            return null;
        }

        var xCollision = (b2 - b1) / (a1 - a2);
        var yCollision = a1 * xCollision + b1;

        var time1 = (xCollision - stone1.Position.Px) / stone1.Velocity.Vx;
        var time2 = (xCollision - stone2.Position.Px) / stone2.Velocity.Vx;

        return (xCollision, yCollision, time1, time2);
    }

    private bool IntersectsInRange(
    ((long Px, long Py, long Pz) Position, (long Vx, long Vy, long Vz) Velocity) stone1,
        ((long Px, long Py, long Pz) Position, (long Vx, long Vy, long Vz) Velocity) stone2,
        (long Min, long Max) range
    )
    {
        //Console.WriteLine($"Calculating {stone1} and {stone2}");
        var result = Intersection(stone1, stone2);

        if (result == null)
        {
            //Console.WriteLine("No collision!");
            return false;
        }

        var (xCollision, yCollision, time1, time2) = result.Value;

        var inside = range.Min <= xCollision && xCollision <= range.Max && range.Min <= yCollision && yCollision <= range.Max;
        //var insideText = inside ? "inside" : "outside";

        var inTheFuture = time1 >= 0 && time2 >= 0;
        //var futureText = inTheFuture ? "future" : "past";
        //Console.WriteLine($"Collision at x={xCollision}, y={yCollision} ({insideText}, in the {futureText})");

        return inside && inTheFuture;
    }

    private Answer CalculatePart1Answer()
    {
        var range = RunMode == RunMode.Test ? (7, 27) : (200000000000000, 400000000000000);

        return _input
            .SelectMany((h1, i1) => _input.Skip(i1 + 1).Select(h2 => (h1, h2)))
            .Count(hails => IntersectsInRange(hails.h1, hails.h2, range));
    }

    private Answer CalculatePart2Answer()
    {
        var delta = RunMode == RunMode.Real ? 2 : 0.01;
        for (var vx = -250; vx <= 250; vx++)
        {
            //Console.WriteLine($"Checking vx = {vx}");
            for (var vy = -250; vy <= 250; vy++)
            {
                var hails = _input.Select(h => (h.Position, Velocity: (h.Velocity.Vx - vx, h.Velocity.Vy - vy, h.Velocity.Vz))).ToList();
                var firstHail = hails[0];

                var result = hails
                    .Skip(1)
                    .Take(8) // Technically, 3 or 4 should be enough.
                    .Select(h => Intersection(firstHail, h))
                    .ToList();

                var firstNotNone = result.First(r => r.HasValue).Value;
                var sameCollisionPoint = result.All(r => r.HasValue && Math.Abs(r.Value.X - firstNotNone.X) < delta && Math.Abs(r.Value.Y - firstNotNone.Y) < delta);

                if (sameCollisionPoint)
                {
                    // The fake rock hits all other rocks at x = 370994826025810, y = 410411158485339, at time 432071486394 for rock1, and different t2s.
                    // This means that 
                    var collisionX = (long)firstNotNone.X;
                    var collisionY = (long)firstNotNone.Y;

                    for (var vz = -250; vz <= 250; vz++)
                    {
                        var hail1 = (_input[0].Position, Velocity: ((Vx: _input[0].Velocity.Vx - vx, Vy: _input[0].Velocity.Vy - vy, Vz: _input[0].Velocity.Vz - vz)));
                        var hail2 = (_input[1].Position, Velocity: ((Vx: _input[1].Velocity.Vx - vx, _input[1].Velocity.Vy - vy, Vz: _input[1].Velocity.Vz - vz)));

                        var z1 = hail1.Position.Pz + hail1.Velocity.Vz * firstNotNone.t1;
                        var z2 = hail2.Position.Pz + hail2.Velocity.Vz * firstNotNone.t2;

                        if (Math.Abs(z1 - z2) < delta)
                        {
                            var collisionZ = (long) z1;
                            return collisionX + collisionY + collisionZ;
                        }
                    }
                }
            }
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

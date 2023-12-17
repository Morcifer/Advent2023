namespace AdventOfCode;

public sealed class Day17 : BaseTestableDay
{
    private readonly List<List<int>> _input;

    public Day17() : this(RunMode.Real)
    {
    }

    public Day17(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(s => s.ToCharArray().Select(c => int.Parse(c.ToString())).ToList())
            .ToList();
    }

    private Answer CalculateAnswer(int minStraight, int maxStraight)
    {
        var startingPoint = (Row: 0, Column: 0);
        var endPoint = (Row: _input.Count - 1, Column: _input[0].Count - 1);

        var queue = new PriorityQueue<
            ((int Row, int Column) Point, (int Row, int Column) Traveled, int Loss),
            int
        >();

        var explored = new HashSet<((int, int), (int, int))>();

        queue.Enqueue(
            (startingPoint, (0, 0), 0),
            0
        );

        while (queue.Count > 0)
        {
            var (currentPoint, currentTraveled, currentLoss) = queue.Dequeue();

            if (currentPoint == endPoint)
            {
                return currentLoss;
            }

            if (explored.Contains((currentPoint, currentTraveled)))
            {
                continue;
            }

            explored.Add((currentPoint, currentTraveled));

            var deltas = new List<(int Row, int Column)> { (1, 0), (0, 1), (-1, 0), (0, -1) };

            foreach (var delta in deltas)
            {
                if (delta == currentTraveled) // You're not allowed to go in this direction anymore
                {
                    continue;
                }

                if (delta == (-currentTraveled.Row, -currentTraveled.Column)) // If I look back I am lost
                {
                    continue;
                }

                for (var steps = minStraight; steps < maxStraight; steps++)
                {
                    var newRow = currentPoint.Row + steps * delta.Row;
                    var newColumn = currentPoint.Column + steps * delta.Column;

                    if (newRow < 0 || newRow >= _input.Count)
                    {
                        continue;
                    }

                    if (newColumn < 0 || newColumn >= _input[0].Count)
                    {
                        continue;
                    }

                    var extraLoss = Enumerable
                        .Range(1, steps)
                        .Select(step => _input[currentPoint.Row + step * delta.Row][currentPoint.Column + step * delta.Column])
                        .Sum();

                    var newLoss = currentLoss + extraLoss;

                    queue.Enqueue(
                        (
                            (newRow, newColumn),
                            delta,
                            newLoss
                        ),
                        newLoss
                    );
                }
            }
        }

        return -1;
    }

    private Answer CalculatePart1Answer()
    {
        return CalculateAnswer(1, 4);
    }

    private Answer CalculatePart2Answer()
    {
        return CalculateAnswer(4, 11);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

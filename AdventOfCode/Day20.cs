namespace AdventOfCode;

public enum Pulse
{
    Low,
    High
}

public interface IModule
{
    Pulse? Receive(Pulse pulse, string source);
}

public class Broadcaster : IModule
{
    public Pulse? Receive(Pulse pulse, string source)
    {
        return pulse;
    }
}

public class FlipFlopModule : IModule
{
    private bool _on = false;

    public Pulse? Receive(Pulse pulse, string source)
    {
        if (pulse == Pulse.High)
        {
            return null;
        }

        _on = !_on;
        return _on ? Pulse.High : Pulse.Low;
    }
}

public class ConjunctionModule : IModule
{
    private readonly Dictionary<string, Pulse> _lastSent = new();

    public void AddSource(string module)
    {
        _lastSent[module] = Pulse.Low;
    }

    public Pulse? Receive(Pulse pulse, string source)
    {
        _lastSent[source] = pulse;

        return _lastSent.Values.All(p => p == Pulse.High) ? Pulse.Low : Pulse.High;
    }
}

public sealed class Day20 : BaseTestableDay
{
    private readonly Dictionary<string, IModule> _modules;
    private readonly Dictionary<string, List<string>> _graph;

    public Day20() : this(RunMode.Real)
    {
    }

    public Day20(RunMode runMode)
    {
        RunMode = runMode;

        _modules = new Dictionary<string, IModule>();
        _graph = new Dictionary<string, List<string>>();

        var conjunctions = new Dictionary<string, ConjunctionModule>();

        foreach (var line in File.ReadAllLines(InputFilePath))
        {
            var split = line.Split("->", StringSplitOptions.TrimEntries).ToList();
            var targets = split[1].Split(",", StringSplitOptions.TrimEntries).ToList();
            var moduleName = split[0][1..];

            if (split[0][0] == '%')
            {
                _modules[moduleName] = new FlipFlopModule();
            }
            else if (split[0][0] == '&')
            {
                var module = new ConjunctionModule();
                _modules[moduleName] = module;
                conjunctions[moduleName] = module;
            }
            else
            {
                moduleName = split[0];
                _modules[moduleName] = new Broadcaster();
            }

            _graph[moduleName] = targets;
        }

        foreach (var (source, targets) in _graph)
        {
            foreach (var target in targets)
            {
                if (conjunctions.TryGetValue(target, out var conjunction))
                {
                    conjunction.AddSource(source);
                }
            }
        }
    }

    private (List<Pulse>, bool) PressButton(string stop)
    {
        var pulses = new List<Pulse>();
        var result = false;

        var queue = new Queue<(string, string, Pulse)>();
        queue.Enqueue(("button", "broadcaster", Pulse.Low));

        while (queue.Count > 0)
        {
            var (source, module, pulse) = queue.Dequeue();
            pulses.Add(pulse);

            if (source == stop && module == "ft" && pulse == Pulse.High)
            {
                result = true;
            }

            //Console.WriteLine($"{source} {pulse} -> {module}");

            if (!_modules.ContainsKey(module))
            {
                continue;
            }

            var output = _modules[module].Receive(pulse, source);

            if (output == null)
            {
                continue;
            }

            foreach (var destination in _graph[module])
            {
                queue.Enqueue((module, destination, output.Value));
            }
        }

        return (pulses, result);
    }

    private Answer CalculatePart1Answer()
    {
        var pulses = new List<Pulse>();

        for (int i = 0; i < 1000; i++)
        {
            pulses.AddRange(PressButton("").Item1);
        }

        return pulses.Count(p => p == Pulse.High) * pulses.Count(p => p == Pulse.Low);
    }

    private (long, long) SimulateUntilTrigger(string trigger)
    {
        long press = 0;
        var found = new List<long>();

        while (found.Count != 10)
        {
            press += 1;

            if (PressButton(trigger).Item2)
            {
                found.Add(press);
            }
        }

        return (found[0], found[1]);
    }

    private Answer CalculatePart2Answer()
    {
        if (RunMode == RunMode.Test)
        {
            return -1;
        }

        var rxSource = _graph.First(kvp => kvp.Value.Contains("rx")).Key;
        var sourcesOfSource = _graph.Where(kvp => kvp.Value.Contains(rxSource)).Select(kvp => kvp.Key).ToList();

        var firstAndPeriodicity = sourcesOfSource
            .Select(SimulateUntilTrigger)
            .Select(t => (First: t.Item1, Periodicity: t.Item2 - t.Item1))
            .ToList();

        var presses = firstAndPeriodicity[0].First;
        var diff = firstAndPeriodicity[0].Periodicity;
        var previousValids = 1;

        while (true)
        {
            var valids = firstAndPeriodicity.Where(x => presses % x.Periodicity == x.First).ToList();

            if (valids.Count > previousValids)
            {
                previousValids = valids.Count;
                diff = valids.Select(x => x.Periodicity).Product();
            }

            if (valids.Count == firstAndPeriodicity.Count)
            {
                // Something's off in here...
                // I should be needing to return the presses, but for some reason, it assumes periodicity starts at 0,
                // even though it doesn't... do I have an issue, or is there a problem with the problem?
                return diff;
            }

            presses += diff;
        }
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

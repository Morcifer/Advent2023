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
    private readonly Dictionary<string, ConjunctionModule> _conjunctions;
    private readonly Dictionary<string, List<string>> _graph;

    private readonly List<Dictionary<Xmas, int>> _parts;

    public Day20() : this(RunMode.Real)
    {
    }

    public Day20(RunMode runMode)
    {
        RunMode = runMode;

        _modules = new Dictionary<string, IModule>();
        _conjunctions = new Dictionary<string, ConjunctionModule>();
        _graph = new Dictionary<string, List<string>>();


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
                _conjunctions[moduleName] = module;
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
                if (_conjunctions.TryGetValue(target, out var conjunction))
                {
                    conjunction.AddSource(source);
                }
            }
        }
    }

    private List<Pulse> PressButton()
    {
        var pulses = new List<Pulse>();

        var queue = new Queue<(string, string, Pulse)>();
        queue.Enqueue(("button", "broadcaster", Pulse.Low));

        while (queue.Count > 0)
        {
            var (source, module, pulse) = queue.Dequeue();
            pulses.Add(pulse);

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

        return pulses;
    }

    private Answer CalculatePart1Answer()
    {
        var pulses = new List<Pulse>();

        for (int i = 0; i < 1000; i++)
        {
            pulses.AddRange(PressButton());
        }

        return pulses.Count(p => p == Pulse.High) * pulses.Count(p => p == Pulse.Low);
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}

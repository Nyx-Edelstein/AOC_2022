using System.Collections.Concurrent;

namespace AOC_2022.Puzzles
{
    using Graph = Dictionary<int, Day16.Valve>;

    public class Day16
    {
        public static int SolutionA(string input)
        {
            var graph = Parse(input);
            var openableValves = graph.Values
                .Where(x => x.Rate > 0)
                .OrderByDescending(x => x.Rate)
                .Select(x => x.Id)
                .ToHashSet();
            var highestFlowrate = Solve(graph, openableValves, 30);
            return highestFlowrate.Flowrate;
        }

        //This is not even close to being efficient
        //Idea: condense the graph to only consider shortest transitions from openable nodes to openable nodes
        //Pros: This should be a very large speedup, as currently the bottleneck is heuristic pruning
        //Cons: Have to keep track of cost separate from path length, which complicates things a bit
        //Will probably return to this later...
        public static string SolutionB(string input)
        {
            const int minutesTotal = 26;
            var graph = Parse(input);
            var openableValves = graph.Values
                .Where(x => x.Rate > 0)
                .Select(x => x.Id)
                .ToArray();
            var partitionMasks = Enumerable.Range(1, Convert.ToInt32(Math.Pow(2.0, openableValves.Length) - 2)).ToArray();
            var flowrates = new ConcurrentBag<Tuple<int,string>>();
            Parallel.For(0, partitionMasks.Length, (i) =>
            {
                var partition = GeneratePartition(partitionMasks[i], openableValves);

                var a = Solve(graph, partition.Item1, minutesTotal);
                var b = Solve(graph, partition.Item2, minutesTotal);
                var totalFlowRate = a.Flowrate + b.Flowrate;

                flowrates.Add(Tuple.Create(totalFlowRate, $"({totalFlowRate}) + {a.PathString} || {b.PathString}"));
                if (flowrates.Count % 10 == 0) Console.WriteLine($"{((100.0 * flowrates.Count) / partitionMasks.Length):F1}%");
            });

            var highestFlowrate = flowrates.OrderByDescending(x => x.Item1).First();
            return highestFlowrate.Item2;
        }

        private static Graph Parse(string input)
        {
            var data = input
                .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .OrderBy(x => x[1])
                .ToList();

            //Use integers instead of strings so there's less memory IO
            var i = -2;
            var nameToId = data.Select(x => x[1]).Select(x => new
            {
                id = (i += 2),
                name = x
            }).ToDictionary(x => x.name, x => x.id);
                
            var graph = data.Select(x =>
            {
                var id = nameToId[x[1]];
                var rate = int.Parse(x[4][5..^1]);
                var connections = string.Join("", x[9..]).Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => nameToId[x])
                    .ToArray();

                //We can simplify the problem by baking "opening valves" into the graph so its stateless
                //Since we're using integers for ids we can store this in the lower bit
                if (rate > 0)
                {
                    var openValve = new Valve
                    {
                        Id = id + 1,
                        Rate = rate,
                        Connections = connections,
                    };

                    var closedValve = new Valve
                    {
                        Id = id,
                        Rate = 0,
                        Connections = connections.Concat(new[] { openValve.Id }).ToArray()
                    };

                    return new[] { closedValve, openValve };
                }

                var valve = new Valve
                {
                    Id = id,
                    Rate = rate,
                    Connections = connections
                };

                return new[] { valve };
            }).SelectMany(x => x).ToDictionary(x => x.Id);

            return graph;
        }

        private static Path Solve(IReadOnlyDictionary<int, Valve> graph, IReadOnlySet<int> partition, int minutesTotal)
        {
            var openableValves = graph.Values
                .Where(x => partition.Contains(x.Id))
                .OrderByDescending(x => x.Rate)
                .Select(x => x.Id)
                .ToArray();

            var toConsider = new List<Path> { new(graph[0]) };
            var highest = toConsider[0];
            do
            {
                var current = toConsider[0];
                toConsider.RemoveAt(0);

                if (current.Flowrate > highest.Flowrate) highest = current;

                //Done if path is max length
                if (current.PathTaken.Count >= minutesTotal) continue;

                //Done if all valves opened
                var openedValves = current.PathTaken.Where(x => x % 2 == 1).ToArray();
                if (openedValves.Length == openableValves.Length) continue;

                //Else, get connecting nodes
                var candidatePaths = new List<Path>();
                var connections = graph[current.PathTaken[^1]].Connections;

                foreach (var connection in connections)
                {
                    //Can't open the same node twice
                    if (openedValves.Contains(connection)) continue;

                    //For part B, only consider openable nodes on this side of the partition
                    if (connection % 2 == 1 && !partition.Contains(connection)) continue;

                    var candidate = new Path(current, graph[connection], minutesTotal);

                    //Prune cycles
                    var untilLastOpenValve = candidate.PathTaken.FindLastIndex(x => x % 2 == 1);
                    var pathFragment = candidate.PathTaken.Skip(untilLastOpenValve+1).ToArray();
                    if (pathFragment.Length != pathFragment.Distinct().Count()) continue;

                    //Heuristically prune unviable branches
                    var offset = current.PathTaken.Count;
                    if (offset == minutesTotal && candidate.Flowrate < highest.Flowrate) continue;

                    //Calculate score if we magically open all unopened valves reachable in the time remaining
                    var heuristicScore = 0;
                    var i = offset;
                    foreach (var v in openableValves)
                    {
                        if (i >= minutesTotal) break;
                        if (openedValves.Contains(v)) continue;

                        heuristicScore += graph[v].Rate * (minutesTotal - i);
                        i += 2;
                    }

                    if (candidate.Flowrate + heuristicScore < highest.Flowrate) continue;

                    candidatePaths.Add(candidate);
                }

                //Add new items to consideration, sorted
                //This is faster than adding the elements and sorting the list since we only have a few
                foreach (var candidate in candidatePaths)
                {
                    var index = toConsider.BinarySearch(candidate);
                    if (index < 0)
                        toConsider.Insert(~index, candidate);
                    else
                        toConsider.Insert(index, candidate);
                }
            } while (toConsider.Any());
            return highest;
        }

        private static Tuple<HashSet<int>,HashSet<int>> GeneratePartition(int partitionMask, IReadOnlyList<int> openableValves)
        {
            var a = new HashSet<int>();
            var b = new HashSet<int>();

            for (var i = 0; i < openableValves.Count; i++)
            {
                var bit = (partitionMask >> i) & 1;
                if (bit == 1) a.Add(openableValves[i]);
                else b.Add(openableValves[i]);
            }

            return Tuple.Create(a, b);
        }

        internal class Valve
        {
            internal int Id { get; init; }
            internal int Rate { get; init; }
            internal int[] Connections { get; init; }

            public override string ToString() => $"{Id}: {Rate} | {string.Join(",", Connections)}";
        }

        internal class Path : IComparable<Path>
        {
            internal List<int> PathTaken { get; }
            internal int Flowrate { get; }

            internal Path(Valve start)
            {
                PathTaken = new List<int> { start.Id };
                Flowrate = 0;
            }

            internal Path(Path previous, Valve current, int minutesTotal)
            {
                PathTaken = previous.PathTaken.Concat(new[] { current.Id }).ToList();

                var MinutesRemaining = minutesTotal - PathTaken.Count + 1;
                Flowrate = previous.Flowrate + current.Rate * MinutesRemaining;
            }
            
            //Sort backwards
            public int CompareTo(Path? other) => other == null? 1 : other.Flowrate.CompareTo(Flowrate);

            //For debugging
            public string PathString => string.Join(",", PathTaken);
            public override string ToString() => $"Flowrate: {Flowrate}, Path: {PathString}";
        }
    }
}


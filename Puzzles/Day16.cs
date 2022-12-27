using System.Collections.Concurrent;
using AOC_2022.Common;
using AOC_2022.Extensions;

namespace AOC_2022.Puzzles
{
    public class Day16
    {
        public static int SolutionA(string input)
        {
            const int minutesRemaining = 30;
            var nodes = Parse(input);
            var graph = new Graph(nodes);
            var simplifiedGraph = graph.Simplifiy(costLimit: minutesRemaining);
            var openableValves = simplifiedGraph.Nodes.Values
                .Where(x => x.Value > 0)
                .OrderByDescending(x => x.Value)
                .ToArray();

            int CalculateFlowRate(ValueWalk walk, Edge edge)
            {
                return walk.Value + simplifiedGraph.Nodes[edge.ConnectingNodeId].Value * (walk.CostRemaining - edge.Cost);
            }

            int HeuristicFlowRate(ValueWalk walk, Edge edge)
            {
                if (walk.CostRemaining == 0) return walk.Value;

                //Calculate score if we magically open all unopened valves reachable in the time remaining
                var openedValves = walk.Path.Select(x => x.ConnectingNodeId).ToList();
                var heuristicScore = 0;
                var offset = walk.CostLimit - walk.CostRemaining;
                var i = offset;
                foreach (var valve in openableValves)
                {
                    if (openedValves.Contains(valve.Id)) continue;
                    if (i >= walk.CostLimit) break;

                    heuristicScore += valve.Value * (walk.CostLimit - i);
                    i += 2;
                }

                return walk.Value + heuristicScore;
            }

            var bestWalk = simplifiedGraph.FindBestWalk(0, minutesRemaining, CalculateFlowRate, HeuristicFlowRate);
            return bestWalk.Value;
        }

        public static int SolutionB(string input)
        {
            const int minutesTotal = 26;

            var nodes = Parse(input);
            var graph = new Graph(nodes);
            var simplifiedGraph = graph.Simplifiy(costLimit: minutesTotal);
            var openableValves = simplifiedGraph.Nodes.Values
                .Where(x => x.Value > 0)
                .OrderByDescending(x => x.Value)
                .ToArray();

            int CalculateFlowRate(ValueWalk walk, Edge edge)
            {
                return walk.Value + simplifiedGraph.Nodes[edge.ConnectingNodeId].Value * (walk.CostRemaining - edge.Cost);
            }

            int HeuristicFlowRate(ValueWalk walk, Edge edge)
            {
                if (walk.CostRemaining == 0) return walk.Value;

                //Calculate score if we magically open all unopened valves reachable in the time remaining
                var openedValves = walk.Path.Select(x => x.ConnectingNodeId).ToList();
                var heuristicScore = 0;
                var offset = walk.CostLimit - walk.CostRemaining;
                var i = offset;
                foreach (var valve in openableValves)
                {
                    if (openedValves.Contains(valve.Id)) continue;
                    if (i >= walk.CostLimit) break;

                    heuristicScore += valve.Value * (walk.CostLimit - i);
                    i += 2;
                }

                return walk.Value + heuristicScore;
            }

            var partitionMasks = Enumerable.Range(1, Convert.ToInt32(Math.Pow(2.0, openableValves.Length) - 2)).ToArray();
            var flowrates = new ConcurrentBag<int>();
            Parallel.For(0, partitionMasks.Length, (i) =>
            {
                var partition = GeneratePartition(partitionMasks[i], openableValves.Select(x => x.Id).ToList());

                var a = simplifiedGraph.FindBestWalkPartitioned(0, partition.Item1, minutesTotal, CalculateFlowRate, HeuristicFlowRate);
                var b = simplifiedGraph.FindBestWalkPartitioned(0, partition.Item2, minutesTotal, CalculateFlowRate, HeuristicFlowRate);
                var totalFlowRate = a.Value + b.Value;

                flowrates.Add(totalFlowRate);
                //if (flowrates.Count % 1000 == 0)
                //{
                //    Console.Clear();
                //    Console.WriteLine($"{((100.0 * flowrates.Count) / partitionMasks.Length):F1}%");
                //}
            });

            var highestFlowrate = flowrates.Max();
            return highestFlowrate;
        }


        private static IDictionary<int, Node> Parse(string input)
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

            var graph = new Dictionary<int, Node>();
            foreach (var x in data)
            {
                var id = nameToId[x[1]];
                var rate = int.Parse(x[4][5..^1]);
                var connections = string.Join("", x[9..]).Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => nameToId[x])
                    .Select(x => new Edge(x))
                    .ToArray();

                //We can simplify the problem by baking "opening valves" into the graph so its stateless
                //Since we're using integers for ids we can store this in the lower bit
                if (rate > 0)
                {
                    var openValve = new Node(id + 1, rate, connections);
                    var closedValve = new Node(id, 0, connections.ConcatSingle(new Edge(openValve.Id)).ToArray());

                    graph.Add(openValve.Id, openValve);
                    graph.Add(closedValve.Id, closedValve);
                }
                else
                {
                    var valve = new Node(id, rate, connections);
                    graph.Add(valve.Id, valve);
                }
            }

            return graph;
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
    }
}


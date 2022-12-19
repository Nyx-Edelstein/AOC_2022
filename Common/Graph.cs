using AOC_2022.Extensions;

namespace AOC_2022.Common
{
    using ValueFunc = Func<ValueWalk, Edge, int>;

    public class Graph
    {
        public IDictionary<int, Node> Nodes { get;}

        public Graph(IDictionary<int, Node> nodes)
        {
            Nodes = nodes;
        }

        public Dictionary<int, CostWalk> FindShortestWalks(int start, int costLimit)
        {
            var toConsider = new List<CostWalk> { new(Nodes[start]) };
            var shortestWalks = new Dictionary<int, CostWalk>();
            do
            {
                var currentWalk = toConsider[0];
                toConsider.RemoveAt(0);

                var currentNode = currentWalk.CurrentNode;

                if (!shortestWalks.ContainsKey(currentNode))
                    shortestWalks.Add(currentNode, currentWalk);
                else if (currentWalk.Cost < shortestWalks[currentNode].Cost)
                    shortestWalks[currentNode] = currentWalk;

                var connections = Nodes[currentWalk.CurrentNode].Connections;
                foreach (var edge in connections)
                {
                    if (currentWalk.Path.Contains(edge)) continue;

                    var cost = currentWalk.Cost + edge.Cost;
                    if (cost > costLimit) continue;
                    if (shortestWalks.ContainsKey(edge.ConnectingNodeId) &&
                        cost >= shortestWalks[edge.ConnectingNodeId].Cost) continue;

                    var next = currentWalk.Step(edge);
                    toConsider.AddSorted(next);
                }
            } while (toConsider.Any());

            return shortestWalks;
        }

        public Graph Simplifiy(int costLimit)
        {
            var newNodes = new Dictionary<int, Node>();

            var valuableNodes = Nodes.Values
                .Where(x => x.Id == 0 || x.Value > 0)
                .Select(x => x.Id)
                .ToArray();

            foreach (var nodeId in valuableNodes)
            {
                var edges = FindShortestWalks(nodeId, costLimit)
                    .Where(x => valuableNodes.Contains(x.Key))
                    .Where(x => x.Key != nodeId)
                    .Select(x => new Edge(x.Key, x.Value.Cost))
                    .ToArray();

                var newNode = new Node(nodeId, Nodes[nodeId].Value, edges);
                newNodes.Add(nodeId, newNode);
            }

            return new Graph(newNodes);
        }

        public ValueWalk FindBestWalk(int start, int costLimit, ValueFunc valueFunc, ValueFunc heuristicFunc)
        {
            var partition = Nodes.Values
                .OrderByDescending(x => x.Value)
                .Select(x => x.Id)
                .ToHashSet();

            return FindBestWalkPartitioned(start, partition, costLimit, valueFunc, heuristicFunc);
        }

        public ValueWalk FindBestWalkPartitioned(int start, IReadOnlySet<int> partition, int costLimit, ValueFunc valueFunc, ValueFunc heuristicFunc)
        {
            var initial = new ValueWalk(Nodes[start], costLimit, valueFunc, heuristicFunc);
            var toConsider = new List<ValueWalk> { initial };
            var best = toConsider[0];
            do
            {
                var current = toConsider[0];
                toConsider.RemoveAt(0);

                if (current.Value > best.Value) best = current;

                //Done if path is max length
                if (current.CostRemaining == 0) continue;

                //Done if all nodes visited
                var visited = current.Path;
                if (visited.Count == partition.Count + 1) continue;

                //Else, get connecting nodes
                var connections = Nodes[current.CurrentNode].Connections;
                foreach (var connection in connections)
                {
                    //Do not exceed cost limit
                    if (current.CostRemaining - connection.Cost < 0) continue;

                    //Only consider openable nodes on this side of the partition
                    if (!partition.Contains(connection.ConnectingNodeId)) continue;

                    //No cycles
                    if (current.Path.Any(x => x.ConnectingNodeId == connection.ConnectingNodeId)) continue;

                    //Heuristically prune unviable branches
                    var candidate = current.Step(connection);
                    if (candidate.HeuristicValue < best.Value) continue;

                    toConsider.AddSorted(candidate);
                }
            } while (toConsider.Any());
            return best;
        }
    }

    public class Node
    {
        public int Id { get; }
        public int Value { get; }
        public Edge[] Connections { get; }

        public Node(int id, int value, Edge[] connections)
        {
            Id = id;
            Value = value;
            Connections = connections;
        }

        public override string ToString() => $"Id: {Id}; Value: {Value}; Connections: {string.Join(",", Connections.ToList())}";
    }

    public class Edge
    {
        public int ConnectingNodeId { get; }
        public int Cost { get; }

        public Edge(int connectingNodeId, int cost = 1)
        {
            ConnectingNodeId = connectingNodeId;
            Cost = cost;
        }

        public override string ToString() => $"{ConnectingNodeId}({Cost})";
    }

    public class CostWalk : IComparable<CostWalk>
    {
        public int CurrentNode { get; private init; }
        public List<Edge> Path { get; private init; }
        public int Cost { get; private init; }

        public CostWalk(Node start)
        {
            CurrentNode = start.Id;
            Path = new List<Edge> { new(start.Id, 0) };
            Cost = 0;
        }

        private CostWalk() {}

        public CostWalk Step(Edge next) => new()
        {
            CurrentNode = next.ConnectingNodeId,
            Path = this.Path.Concat(new[] { next }).ToList(),
            Cost = this.Cost + next.Cost,
        };

        public int CompareTo(CostWalk? other) => other == null ? 0 : this.Cost.CompareTo(other.Cost);

        public override string ToString() => $"Cost: {Cost}, Path: {string.Join(",", Path)}";
    }
    
    public class ValueWalk : IComparable<ValueWalk>
    {
        public int CurrentNode { get; private init; }
        public List<Edge> Path { get; private init; }
        public int CostLimit { get; private init; }
        public int CostRemaining { get; private init; }
        public int Value { get; private set; }
        public int HeuristicValue { get; private set; }

        private ValueFunc ValueFunc { get; init; }
        private ValueFunc HeuristicFunc { get; init; }

        

        public ValueWalk(Node start, int costLimit, ValueFunc valueFunc, ValueFunc heuristicFunc)
        {
            CurrentNode = start.Id;
            Path = new List<Edge> { new(start.Id, 0) };
            CostLimit = costLimit;
            CostRemaining = costLimit;
            Value = start.Value;
            ValueFunc = valueFunc;
            HeuristicFunc = heuristicFunc;
        }

        private ValueWalk() {}

        public ValueWalk Step(Edge next)
        {
            var walk = new ValueWalk
            {
                CurrentNode = next.ConnectingNodeId,
                Path = this.Path.Concat(new[] { next }).ToList(),
                CostLimit = this.CostLimit,
                CostRemaining = this.CostRemaining - next.Cost,
                ValueFunc = this.ValueFunc,
                HeuristicFunc = this.HeuristicFunc,
                Value = ValueFunc(this, next)
            };
            
            walk.HeuristicValue = HeuristicFunc(walk, next);
            return walk;
        }

        public int CompareTo(ValueWalk? other) => other == null ? 0 : other.Value.CompareTo(this.Value);

        public override string ToString() => $"Value: {Value}; Cost Remaining: {CostRemaining}; Path: {string.Join(",", Path)}";
    }
}

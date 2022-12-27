namespace AOC_2022.Puzzles
{
    internal class Day24
    {
        public static int SolutionA(string input)
        {
            var (graph, blizzardMap) = Parse(input);

            var start = graph.Nodes.First().Key;
            var goal = graph.Nodes.Last().Key;

            var time = graph.FindShortestPath(start, goal, blizzardMap);
            return time;
        }

        public static int SolutionB(string input)
        {
            var (graph, blizzardMap) = Parse(input);

            var start = graph.Nodes.First().Key;
            var goal = graph.Nodes.Last().Key;

            var t1 = graph.FindShortestPath(start, goal, blizzardMap);
            var t2 = graph.FindShortestPath(goal, start, blizzardMap, t1);
            var t3 = graph.FindShortestPath(start, goal, blizzardMap, t1+t2);

            return t1 + t2 + t3;
        }

        private static (Graph, BlizzardMap) Parse(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var nodes = new List<Node>();
            var blizzards = new List<Blizzard>();

            var y_min = 1;
            var y_max = data.Length - 2;
            var x_min = 1;
            var x_max = data[0].Length - 2;

            for (var y = 0; y < data.Length; y++)
            for (var x = 0; x < data[0].Length; x++)
            {
                var cell = data[y][x];
                if (cell == '#') continue;

                var loc = (y, x);
                var direction = cell switch
                {
                    '^' => (-1, 0),
                    '>' => (0, 1),
                    'v' => (1, 0),
                    '<' => (0, -1),
                    _ => (0, 0)
                };
                if (direction != (0, 0)) blizzards.Add(new Blizzard(loc, direction, y_min, y_max, x_min, x_max));

                var edges = new List<Edge>();

                void TryAddEdge(int y_offset, int x_offset)
                {
                    var connectingLocY = y + y_offset;
                    if (connectingLocY < 0) return;
                    if (connectingLocY >= data.Length) return;

                    var connectingLocX = x + x_offset;
                    if (connectingLocX < 0) return;
                    if (connectingLocX >= data[0].Length) return;

                    var connectingCell = data[connectingLocY][connectingLocX];
                    if (connectingCell == '#') return;

                    var connectingLoc = (connectingLocY, connectingLocX);
                    var edge = new Edge(connectingLoc);
                    edges.Add(edge);
                }

                TryAddEdge(-1, 0);
                TryAddEdge(0, 1);
                TryAddEdge(1, 0);
                TryAddEdge(0, -1);
                TryAddEdge(0, 0);

                var node = new Node(loc, edges.ToArray());
                nodes.Add(node);
            }

            var graph = new Graph(nodes);
            var blizzardMap = new BlizzardMap(blizzards, 1000);

            return (graph, blizzardMap);
        }

        internal class Blizzard
        {
            internal (int, int) Location { get; private set; }
            private readonly (int, int) MovementDirection;
            private readonly int _YMin;
            private readonly int _YMax;
            private readonly int _XMin;
            private readonly int _XMax;

            internal Blizzard((int, int) location, (int, int) movementDirection, int y_min, int y_max, int x_min, int x_max)
            {
                Location = location;
                MovementDirection = movementDirection;
                _YMin = y_min;
                _YMax = y_max;
                _XMin = x_min;
                _XMax = x_max;
            }

            internal void UpdateLocation()
            {
                var y = Location.Item1 + MovementDirection.Item1;
                if (y < _YMin) y = _YMax;
                if (y > _YMax) y = _YMin;

                var x = Location.Item2 + MovementDirection.Item2;
                if (x < _XMin) x = _XMax;
                if (x > _XMax) x = _XMin;

                Location = (y, x);
            }
        }

        internal class BlizzardMap
        {
            internal HashSet<(int, int, int)> Map { get; } = new();

            internal BlizzardMap(List<Blizzard> blizzards, int cycles)
            {
                foreach (var blizzard in blizzards) 
                {
                    for (var t = 0; t < cycles; t++)
                    {
                        Map.Add((blizzard.Location.Item1, blizzard.Location.Item2, t));
                        blizzard.UpdateLocation();
                    }
                }
            }
        }

        internal class Graph
        {
            internal IDictionary<(int,int), Node> Nodes { get; }

            internal Graph(IEnumerable<Node> nodes)
            {
                Nodes = nodes.ToDictionary(x => x.Id);
            }

            internal int FindShortestPath((int,int) start, (int,int) goal, BlizzardMap blizzardMap, int startingCycleNum = 0)
            {
                var initial = new Walk(start, goal);
                var toConsider = new PriorityQueueSet<(int,int,int), Walk>();
                toConsider.Add((start.Item1, start.Item2, 0), initial);

                var lowestCostAtCycleNum = new Dictionary<(int, int, int), int>();
                var lowestCostToGoal = int.MaxValue;
                do
                {
                    var current = toConsider.Pull();
                    if (ReferenceEquals(current, null)) break;

                    //Done when reaching goal
                    if (current.CurrentNode == goal)
                    {
                        if (current.Cost < lowestCostToGoal)
                        {
                            lowestCostToGoal = current.Cost;
                        }
                        continue;
                    }

                    bool AvoidsBlizzards(Edge edge, int t)
                    {
                        var pointAtTime = (edge.ConnectingNodeId.Item1, edge.ConnectingNodeId.Item2, t);
                        return !blizzardMap.Map.Contains(pointAtTime);
                    }

                    //Else, get connecting nodes
                    var cycleNum = (current.Path.Count + startingCycleNum);
                    var connections = Nodes[current.CurrentNode].Connections;
                    foreach (var connection in connections)
                    {
                        if (!AvoidsBlizzards(connection, cycleNum)) continue;

                        var candidate = current.Step(connection);

                        //Heuristically prune unviable branches
                        //We can't avoid cycles altogether, but we can avoid cycles per cycle!
                        int GetSetLowestCost((int, int, int) pointAtCycleNum, int cost)
                        {
                            if (!lowestCostAtCycleNum.ContainsKey(pointAtCycleNum))
                            {
                                lowestCostAtCycleNum.Add(pointAtCycleNum, cost);
                                return cost;
                            }

                            if (current.Cost < lowestCostAtCycleNum[pointAtCycleNum])
                            {
                                lowestCostAtCycleNum[pointAtCycleNum] = cost;
                                return cost;
                            }

                            return lowestCostAtCycleNum[pointAtCycleNum];
                        }

                        //Prune if we can get to this location earlier at the same cycle num
                        var locAtCycleNum = (candidate.CurrentNode.Item1, candidate.CurrentNode.Item2, cycleNum);
                        var lowestCost = GetSetLowestCost(locAtCycleNum, candidate.Cost);
                        if (candidate.Cost > lowestCost) continue;

                        //Prune if it's impossible to reach the goal faster than another path
                        if (candidate.HeuristicCost > lowestCostToGoal) continue;

                        toConsider.Add(locAtCycleNum, candidate);
                    }
                } while (toConsider.Any());
                
                toConsider.Dispose();

                return lowestCostToGoal;
            }
        }

        internal class Node
        {
            internal (int,int) Id { get; }
            internal Edge[] Connections { get; }

            internal Node((int,int) id, Edge[] connections)
            {
                Id = id;
                Connections = connections;
            }

            public override string ToString() => $"Id: {Id}; Connections: {string.Join(",", Connections.ToList())}";
        }

        internal class Edge
        {
            internal (int,int) ConnectingNodeId { get; }
            internal int Cost { get; }

            internal Edge((int,int) connectingNodeId, int cost = 1)
            {
                ConnectingNodeId = connectingNodeId;
                Cost = cost;
            }

            public override string ToString() => $"{ConnectingNodeId}";
        }

        internal class Walk : IComparable<Walk>
        {
            internal (int,int) CurrentNode { get; }
            internal (int,int) Goal { get; }
            internal List<Edge> Path { get; }
            internal int Cost { get; }
            internal int HeuristicCost { get; }

            internal Walk((int,int) startId, (int,int) goalId)
            {
                CurrentNode = startId;
                Goal = goalId;
                Path = new List<Edge> { new(startId, 0) };
                Cost = 0;
                HeuristicCost = HammingDistance(Goal, CurrentNode);
            }

            private Walk(Walk prev, Edge next)
            {
                CurrentNode = next.ConnectingNodeId;
                Goal = prev.Goal;
                Path = prev.Path.Concat(new[] { next }).ToList();
                Cost = prev.Cost + next.Cost;
                HeuristicCost = Cost + HammingDistance(Goal, CurrentNode);
            }

            internal Walk Step(Edge next) => new(this, next);

            private static int HammingDistance((int, int) goal, (int, int) currentNode)
            {
                var distX = Math.Abs(goal.Item2 - currentNode.Item2);
                var distY = Math.Abs(goal.Item1 - currentNode.Item1);
                return distX + distY;
            }

            public int CompareTo(Walk? other)
            {
                if (other == null) return 0;

                //Prefer lower heuristic cost
                var heuristicCompare = HeuristicCost.CompareTo(other.HeuristicCost);
                if (heuristicCompare != 0) return heuristicCompare;

                //Prefer lower actual cost
                var costCompare = Cost.CompareTo(other.Cost);
                if (costCompare != 0) return costCompare;

                return 0;
            }

            public override string ToString() => $"Cost: {Cost}; Path: {string.Join(",", Path)}";
        }

        internal class PriorityQueueSet<TKey, TItem> : IDisposable where TItem : class, IComparable<TItem>
        {
            private readonly HashSet<TKey> Filter = new();
            private readonly C5.IntervalHeap<TItem> Queue = new();

            public bool Add(TKey key, TItem item)
            {
                var seen = Filter.Contains(key);
                if (seen) return false;

                Filter.Add(key);
                Queue.Add(item);
                return true;
            }

            public TItem? Pull()
            {
                if (Queue.IsEmpty) return null;

                var item = Queue.FindMin();
                Queue.DeleteMin();
                return item;
            }

            public bool Any() => !Queue.IsEmpty;

            public void Dispose() => Filter.Clear();
        }
    }
}

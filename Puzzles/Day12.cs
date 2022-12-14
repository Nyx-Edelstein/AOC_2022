namespace AOC_2022.Puzzles
{
    public class Day12
    {
        public static int SolutionA(string input)
        {
            var grid = Parse(input);
            var toVisit = new List<CandidatePath> { new(grid.Start, grid.End) };
            return Solution(grid, toVisit);
        }


        public static int SolutionB(string input)
        {
            var grid = Parse(input);
            var toVisit = new List<CandidatePath>();

            for (var y = 0; y < grid.TerrainHeights.Length; y++)
            {
                for (var x = 0; x < grid.TerrainHeights[0].Length; x++)
                {
                    if (grid.TerrainHeights[y][x] == 0)
                    {
                        toVisit.Add(new CandidatePath(Tuple.Create(y, x), grid.End));
                    }
                }
            }

            toVisit.Sort((a, b) => (a.Length + a.DistanceToEnd).CompareTo(b.Length + b.DistanceToEnd));

            return Solution(grid, toVisit);

        }

        internal static int Solution(Grid grid, List<CandidatePath> toVisit)
        {
            var shortestPathToNode = new Dictionary<Tuple<int, int>, int>();
            do
            {
                var current = toVisit[0];
                toVisit.RemoveAt(0);

                if (!shortestPathToNode.ContainsKey(current.Location))
                {
                    shortestPathToNode.Add(current.Location, current.Length);
                }
                else if (current.Length < shortestPathToNode[current.Location])
                {
                    shortestPathToNode[current.Location] = current.Length;
                }

                var candidatePaths = grid.GetVisitable(current.Location)
                    .Where(x => !current.Path.Contains(x))
                    .Where(x => !shortestPathToNode.ContainsKey(x) || (current.Length + 1) < shortestPathToNode[x])
                    .Select(x => new CandidatePath(current, x, grid.End));

                toVisit.AddRange(candidatePaths);
                toVisit.Sort((a, b) => (a.Length + a.DistanceToEnd).CompareTo(b.Length + b.DistanceToEnd));
            } while (toVisit.Any());

            return shortestPathToNode[grid.End] - 1;
        }

        private static Grid Parse(string input)
        {
            var heightChars = input.Split("\r\n").Select(x => x.ToArray()).ToArray();

            var grid = new Grid();
            grid.TerrainHeights = heightChars.Select((r,y) => r.Select((c,x) =>
            {
                if (heightChars[y][x] == 'S') grid.Start = Tuple.Create(y, x);
                if (heightChars[y][x] == 'E') grid.End = Tuple.Create(y, x);
                return c switch
                {
                    'S' => 0,
                    'E' => 25,
                    _ => c - 'a',
                };
            }).ToArray()).ToArray();
            return grid;
        }

        internal class Grid
        {
            internal Tuple<int, int> Start { get; set; }
            internal Tuple<int, int> End { get; set; }
            internal int[][] TerrainHeights { get; set; }

            internal IEnumerable<Tuple<int, int>> GetVisitable(Tuple<int, int> loc)
            {
                var visitable = new List<Tuple<int, int>>();

                if (loc.Item1 == End.Item1 && loc.Item2 == End.Item2) return visitable;

                var y = loc.Item1;
                var y_max = TerrainHeights.Length-1;
                var x = loc.Item2;
                var x_max = TerrainHeights[0].Length-1;
                var reachable_height = TerrainHeights[y][x] + 1;

                //Left
                if (x > 0 && TerrainHeights[y][x-1] <= reachable_height) visitable.Add(Tuple.Create(y, x - 1));

                //Top
                if (y > 0 && TerrainHeights[y-1][x] <= reachable_height) visitable.Add(Tuple.Create(y - 1, x));

                //Right
                if (x < x_max && TerrainHeights[y][x+1] <= reachable_height) visitable.Add(Tuple.Create(y, x + 1));

                //Down
                if (y < y_max && TerrainHeights[y+1][x] <= reachable_height) visitable.Add(Tuple.Create(y + 1, x));

                return visitable;
            }
        }

        internal class CandidatePath
        {
            internal CandidatePath(CandidatePath priorPath, Tuple<int, int> nextNode, Tuple<int, int> endNode)
            {
                Path.AddRange(priorPath.Path);
                Path.Add(nextNode);
                Location = nextNode;
                DistanceToEnd = Math.Abs(nextNode.Item1 - endNode.Item1) + Math.Abs(nextNode.Item2 - endNode.Item2);
            }

            internal CandidatePath(Tuple<int, int> startNode, Tuple<int, int> endNode)
            {
                Path.Add(startNode);
                Location = startNode;
                DistanceToEnd = Math.Abs(startNode.Item1 - endNode.Item1) + Math.Abs(startNode.Item2 - endNode.Item2);
            }

            internal List<Tuple<int, int>> Path { get; } = new();
            internal Tuple<int, int> Location { get; }
            internal int DistanceToEnd { get; }
            internal int Length => Path.Count;
        }
    }
}


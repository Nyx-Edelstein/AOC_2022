namespace AOC_2022.Puzzles
{
    public class Day18
    {
        public static int SolutionA(string input)
        {
            var cubes = Parse(input);
            var connections = GetConnections(cubes);

            var numCubes = cubes.Count;
            var numConnections = connections.Count;
            var surfaceArea = (numCubes * 6) - (numConnections * 2);

            return surfaceArea;
        }

        public static int SolutionB(string input)
        {
            var cubes = Parse(input);
            var connections = GetConnections(cubes);

            var x_min = cubes.Min(x => x.Item1);
            var x_max = cubes.Max(x => x.Item1);
            var y_min = cubes.Min(x => x.Item2);
            var y_max = cubes.Max(x => x.Item2);
            var z_min = cubes.Min(x => x.Item3);
            var z_max = cubes.Max(x => x.Item3);

            //Get all the cubes that lie inside the boundaries
            var internalCubes = new HashSet<(int, int, int)>();
            for (var x = x_min; x < x_max; x++)
            for (var y = y_min; y < y_max; y++)
            for (var z = z_min; z < z_max; z++)
            {
                var c = (x, y, z);
                if (!cubes.Contains(c)) internalCubes.Add(c);
            }

            //Generate all cubes immediately outside the boundaries
            var externalCubes = new HashSet<(int, int, int)>();
            for (var y = y_min; y < y_max; y++)
            for (var z = z_min; z < z_max; z++)
            {
                externalCubes.Add((x_min - 1, y, z));
                externalCubes.Add((x_max + 1, y, z));
            }
            for (var x = x_min; x < x_max; x++)
            for (var z = z_min; z < z_max; z++)
            {
                externalCubes.Add((x, y_min - 1, z));
                externalCubes.Add((x, y_max + 1, z));
            }
            for (var x = x_min; x < x_max; x++)
            for (var y = y_min; y < y_max; y++)
            {
                externalCubes.Add((x, y, z_min - 1));
                externalCubes.Add((x, y, z_max + 1));
            }
            
            //For each external cube:
            // - Find connecting cubes that do not intersect the boundary
            // - Add those to "external" list
            // - Remove this cube from the internal candidates list
            // - Iterate until we exhaust the entire space
            // - What's left will be just the cubes that are actually internal
            var toConsider = externalCubes.ToList();
            void Consider((int, int, int) cube)
            {
                if (cube.Item1 < x_min || cube.Item1 > x_max) return;
                if (cube.Item2 < y_min || cube.Item2 > y_max) return;
                if (cube.Item3 < z_min || cube.Item3 > z_max) return;
                if (externalCubes.Contains(cube)) return;
                if (cubes.Contains(cube)) return;

                externalCubes.Add(cube);
                internalCubes.Remove(cube);
                toConsider.Add(cube);
            }
            do
            {
                var c = toConsider[0];
                toConsider.RemoveAt(0);

                Consider((c.Item1 - 1, c.Item2, c.Item3));
                Consider((c.Item1 + 1, c.Item2, c.Item3));
                Consider((c.Item1, c.Item2 - 1, c.Item3));
                Consider((c.Item1, c.Item2 + 1, c.Item3));
                Consider((c.Item1, c.Item2, c.Item3 - 1));
                Consider((c.Item1, c.Item2, c.Item3 + 1));
            } while (toConsider.Any());

            //Now we can get connections from these internal cubes to the boundary
            var internalConnections = GetConnectionsFromTo(internalCubes, cubes);

            var numCubes = cubes.Count;
            var numConnections = connections.Count;
            var numInternalConnections = internalConnections.Count;
            var surfaceArea = (numCubes * 6) - (numConnections * 2) - numInternalConnections;

            return surfaceArea;
        }

        private static HashSet<(int, int, int)> Parse(string input)
        {
            var cubes = input.Split("\r\n").Select(x => x.Split(",").Select(int.Parse).ToArray())
                .Select(x => (x[0], x[1], x[2]))
                .ToHashSet();

            return cubes;
        }

        private static HashSet<Connection> GetConnections(HashSet<(int, int, int)> cubes) => GetConnectionsFromTo(cubes, cubes);

        private static HashSet<Connection> GetConnectionsFromTo(HashSet<(int, int, int)> from, HashSet<(int, int, int)> to)
        {
            var connections = new HashSet<Connection>();

            void AddIfConnecting((int, int, int) cube, (int, int, int) offset)
            {
                if (to.Contains(offset)) connections.Add(new Connection(cube, offset));
            }

            foreach (var c in from)
            {
                AddIfConnecting(c, (c.Item1 - 1, c.Item2, c.Item3));
                AddIfConnecting(c, (c.Item1 + 1, c.Item2, c.Item3));
                AddIfConnecting(c, (c.Item1, c.Item2 - 1, c.Item3));
                AddIfConnecting(c, (c.Item1, c.Item2 + 1, c.Item3));
                AddIfConnecting(c, (c.Item1, c.Item2, c.Item3 - 1));
                AddIfConnecting(c, (c.Item1, c.Item2, c.Item3 + 1));
            }

            return connections;
        }

        private class Connection
        {
            private readonly (int, int, int) A;
            private readonly (int, int, int) B;

            internal Connection((int, int, int) a, (int, int, int) b)
            {
                A = a;
                B = b;
            }

            public static bool operator ==(Connection left, Connection right)
            {
                return (left.A == right.A && left.B == right.B)
                    || (left.A == right.B && left.B == right.A);
            }

            public static bool operator !=(Connection left, Connection right)
            {
                return !(left == right);
            }

            public override bool Equals(object? other)
            {
                if (ReferenceEquals(this, other)) return true;
                if (other is not Connection connection) return false;
                return this == connection;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return HashCode.Combine(A, B) + HashCode.Combine(B, A);
                }
            }
        }
    }
}


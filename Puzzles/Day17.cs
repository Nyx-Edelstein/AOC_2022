namespace AOC_2022.Puzzles
{
    public class Day17
    {
        public static int SolutionA(string input)
        {
            const int target = 2022;
            var directions = Parse(input);
            var rocks = new Rocks();
            var chamber = new Chamber(rocks, directions);

            for (var i = 1; i <= target; i++)
                chamber.SimulateOneRock();

            return chamber.Height;
        }

        public static long SolutionB(string input)
        {
            var directions = Parse(input);
            var rocks = new Rocks();
            var chamber = new Chamber(rocks, directions);

            (int, int) cycleInfo;
            bool cycleDetected;
            do
            {
                chamber.SimulateOneRock();
                cycleDetected = chamber.DetectCycle(out cycleInfo);

            } while (!cycleDetected);

            //Do some fancy math
            var init_index = cycleInfo.Item1;
            var init_height = cycleInfo.Item2;
            var curr_index = chamber.NumRocks;
            var curr_height = chamber.Height;

            var cycleLength = curr_index - init_index;
            var heightDiff = curr_height - init_height;

            var remainingCycles = (1000000000000 - curr_index) / cycleLength;
            var cycleHeight = remainingCycles * heightDiff;

            var remainingRocks = (1000000000000 - curr_index) % cycleLength;
            
            for (var j = curr_index; j < curr_index + remainingRocks; j++)
                chamber.SimulateOneRock();

            var totalHeight = chamber.Height + cycleHeight;
            return totalHeight;
        }
        

        private static Directions Parse(string input)
        {
            var dir = input.ToCharArray()
                .Select(x => x switch
                {
                    '<' => Direction.Left,
                    '>' => Direction.Right
                }).ToArray();

            return new Directions(dir);
        }
        
        private enum Direction
        {
            Left = 0,
            Right = 1
        }

        private abstract class CyclicCollection<T>
        {
            protected T[] Collection;
            private int i;
            public int Index => i % Collection.Length;
            public T GetNext() => Collection[i++ % Collection.Length];
        }

        private class Directions : CyclicCollection<Direction>
        {
            public Directions(Direction[] _dir) => Collection = _dir;
        }

        private class Rocks : CyclicCollection<(int, int)[]>
        {
            public Rocks() => Collection = new[]
            {
                new[] { (0,0), (0,1), (0,2), (0,3) },
                new[] { (0,1), (1,0), (1,1), (1,2), (2,1) },
                new[] { (0,0), (0,1), (0,2), (1,2), (2,2) },
                new[] { (0,0), (1,0), (2,0), (3,0) },
                new[] { (0,0), (0,1), (1,0), (1,1) },
            };
        }

        private class Chamber
        {
            private readonly Rocks Rocks;
            private readonly Directions Directions;
            private readonly HashSet<(int, int)> CollisionMap = new();
            private readonly Dictionary<(int, int, ulong), (int,int)> PreviousStates = new();

            public int Height { get; private set; }
            public int NumRocks { get; private set; }
            
            public Chamber(Rocks rocks, Directions directions)
            {
                Rocks = rocks;
                Directions = directions;

                //Add bottom row
                for (var i = -1; i <= 7; i++)
                {
                    CollisionMap.Add((-1, i));
                }
            }

            public void SimulateOneRock()
            {
                //Add left and right walls
                for (var i = Height; i <= Height + 7; i++)
                {
                    CollisionMap.Add((i, -1));
                    CollisionMap.Add((i, 7));
                }

                var rock = Rocks.GetNext();
                var position = (Height+3, 2);
                while(true)
                {
                    //Try move sideways
                    var testPosition = Directions.GetNext() == Direction.Left
                        ? (position.Item1, position.Item2 - 1)
                        : (position.Item1, position.Item2 + 1);
                    position = CheckCollision(rock, testPosition) ? position : testPosition;

                    //Try move down
                    testPosition = (position.Item1-1, position.Item2);
                    if (CheckCollision(rock, testPosition))
                    {
                        Settle(rock, position);
                        break;
                    }
                    position = testPosition;
                }

                NumRocks += 1;
            }

            private bool CheckCollision((int, int)[] rock, (int, int) position)
            {
                foreach (var r in rock)
                {
                    var colllisionPoint = (r.Item1 + position.Item1, r.Item2 + position.Item2);
                    if (CollisionMap.Contains(colllisionPoint)) return true;
                }

                return false;
            }

            private void Settle((int, int)[] rock, (int, int) position)
            {
                foreach (var r in rock)
                {
                    var point = (r.Item1 + position.Item1, r.Item2 + position.Item2);
                    CollisionMap.Add(point);
                    if (point.Item1 + 1 > Height) Height = point.Item1 + 1;
                }
            }

            //For debugging
            public override string ToString()
            {
                var str = new string[Height][];
                for (var r = 0; r < Height; r++)
                {
                    str[r] = new string[7];
                    for (var c = 0; c < 7; c++)
                    {
                        str[r][c] = CollisionMap.Contains((r, c)) ? "#" : ".";
                    }
                }

                return string.Join("\r\n", str.Select(x => string.Join("", x)).Reverse());
            }

            public bool DetectCycle(out (int, int) cycleInfo)
            {
                cycleInfo = (0, 0);

                //This is a completely arbitrary constant
                //I thought it would have to be higher -- but no, this works lol
                const int prev = 5;
                if (Height < prev) return false;

                var previousHash = GetPreviousNHash(prev);
                var val = (Rocks.Index, Directions.Index, previousHash);

                if (PreviousStates.ContainsKey(val))
                {
                    cycleInfo = PreviousStates[val];
                    return true;
                }

                PreviousStates[val] = (NumRocks, Height);
                return false;
            }

            private ulong GetPreviousNHash(int prev)
            {
                const ulong fnv_prime = 0x100000001b3;
                ulong hash = 0xcbf29ce484222325;
                for (var r = Height - 1; r > Height - prev; r--)
                {
                    byte mask = 0b0;
                    var i = 1;
                    for (byte c = 0; c < 7; c++)
                    {
                        var bit = CollisionMap.Contains((r, c)) ? 1 : 0;
                        mask += (byte)(i*bit);
                        i *= 2;
                    }

                    unchecked
                    {
                        hash ^= mask;
                        hash *= fnv_prime;
                    }
                }
                return hash;
            }
        }
    }
}


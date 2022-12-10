namespace AOC_2022.Puzzles
{
    public class Day9
    {
        public static int SolutionA(string input)
        {
            return Solution(input, 2);
        }

        public static int SolutionB(string input)
        {
            return Solution(input, 9);
        }

        public static int Solution(string input, int numKnots)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            var instructions = data.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .Select(y => new
                {
                    direction = y[0],
                    distance = int.Parse(y[1])
                });

            var knots = Enumerable.Repeat(Tuple.Create(0, 0), numKnots).ToArray();
            var uniquePositions = new HashSet<Tuple<int, int>>
            {
                knots[numKnots-1]
            };
            foreach (var instruction in instructions)
            {
                var direction = instruction.direction switch
                {
                    "U" => Tuple.Create(0, 1),
                    "R" => Tuple.Create(1, 0),
                    "D" => Tuple.Create(0, -1),
                    "L" => Tuple.Create(-1, 0),
                };

                for (var i = 0; i < instruction.distance; i++)
                {
                    knots[0] = Tuple.Create(knots[0].Item1 + direction.Item1, knots[0].Item2 + direction.Item2);
                    for (var j = 0; j < numKnots; j++)
                    {
                        if (Dist(knots[j], knots[j + 1]) > 1)
                            knots[j + 1] = UpdateKnot(knots[j], knots[j + 1]);
                    }
                    uniquePositions.Add(knots[numKnots-1]);
                }
            }

            return uniquePositions.Count;
        }

        private static int Dist(Tuple<int, int> a, Tuple<int, int> b)
        {
            return Math.Max(Math.Abs(a.Item1 - b.Item1), Math.Abs(a.Item2 - b.Item2));
        }

        private static Tuple<int, int> UpdateKnot(Tuple<int, int> a, Tuple<int, int> b)
        {
            var x = a.Item1 > b.Item1 ? 1
                : a.Item1 < b.Item1 ? -1
                : 0;

            var y = a.Item2 > b.Item2 ? 1
                : a.Item2 < b.Item2 ? -1
                : 0;

            return Tuple.Create(b.Item1 + x, b.Item2 + y);
        }
    }
}

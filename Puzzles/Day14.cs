namespace AOC_2022.Puzzles
{
    public class Day14
    {
        public static int SolutionA(string input)
        {
            var data = Parse(input);
            var grid = new Grid(data, resize: true);
            while (!grid.IsFull) grid.Iterate();
            return grid.Iterations - 1;
        }

        public static int SolutionB(string input)
        {
            var data = Parse(input);
            var grid = new Grid(data, false);
            while (!grid.SourceIsFull) grid.Iterate();
            return grid.Iterations;
        }

        private static Tuple<int, int>[][] Parse(string input)
        {
            return input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" -> ", StringSplitOptions.RemoveEmptyEntries).Select(y =>
                {
                    var coords = y.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    return Tuple.Create(int.Parse(coords[1]), int.Parse(coords[0]));
                }).ToArray())
                .ToArray();
        }

        internal class Grid
        {
            internal string[][] Cells { get; }
            private Tuple<int, int> Source { get; }
            
            public int Iterations { get; private set; }

            internal Grid(Tuple<int, int>[][] data, bool resize)
            {
                //Initialize
                Cells = new string[500][];
                for (var i = 0; i < 500; i++)
                {
                    var cells = new List<string>();
                    for (var j = 0; j < 1000; j++) cells.Add(".");
                    Cells[i] = cells.ToArray();
                }

                //Draw lines
                foreach (var cluster in data)
                {
                    for (var i = 0; i < cluster.Length-1; i++)
                    {
                        var startCoord = cluster[i];
                        var endCoord = cluster[i+1];

                        DrawLine(startCoord, endCoord);
                    }
                }

                //Normalize rows
                var untilLastRow = Cells.Select((x, i) => new
                {
                    i,
                    hasRocks = x.Any(c => c == "#")
                }).Where(x => x.hasRocks).Max(x => x.i) + 1;
                Cells = Cells.Take(untilLastRow + 1).ToArray();

                //Normalize columns for part A (makes it smaller)
                if (resize)
                {
                    var cols = data.SelectMany(x => x).Select(x => x.Item2).ToArray();
                    for (var i = 0; i < Cells.Length; i++)
                    {
                        Cells[i] = Cells[i].Skip(cols.Min() - 2).Take(cols.Max() - cols.Min() + 4).ToArray();
                    }

                    //Set source
                    Source = Tuple.Create(0, 500 - cols.Min() + 1);
                }
                //Add floor for part B
                else
                {
                    Cells = Cells.Concat(new[] { Cells[0].Select(s => "#").ToArray() }).ToArray();

                    //Set source
                    Source = Tuple.Create(0, 500);
                }

            }

            private void DrawLine(Tuple<int, int> startCoord, Tuple<int, int> endCoord)
            {
                if (startCoord.Item1 != endCoord.Item1) DrawVertical(startCoord, endCoord);
                else DrawHorizontal(startCoord, endCoord);
            }

            private void DrawVertical(Tuple<int, int> startCoord, Tuple<int, int> endCoord)
            {
                var col = startCoord.Item2;
                var min = Math.Min(startCoord.Item1, endCoord.Item1);
                var max = Math.Max(startCoord.Item1, endCoord.Item1);
                for (var i = min; i <= max; i++) Cells[i][col] = "#";
            }

            private void DrawHorizontal(Tuple<int, int> startCoord, Tuple<int, int> endCoord)
            {
                var row = startCoord.Item1;
                var min = Math.Min(startCoord.Item2, endCoord.Item2);
                var max = Math.Max(startCoord.Item2, endCoord.Item2);
                for (var i = min; i <= max; i++) Cells[row][i] = "#";
            }
            public override string ToString() => string.Join("\r\n", Cells.Select(row => string.Join("", row)));

            public bool IsFull => Cells.Last().Any(x => x == "O");

            public bool SourceIsFull => GetCell(Source) == "O";

            public void Iterate()
            {
                //Create sand at source
                var sandLoc = Source;

                //Make sand fall according to rules
                do
                {
                    var belowLoc = Tuple.Create(sandLoc.Item1+1, sandLoc.Item2);
                    if (GetCell(belowLoc) == ".") sandLoc = belowLoc;
                    else
                    {
                        var belowLeftLoc = Tuple.Create(sandLoc.Item1+1, sandLoc.Item2 - 1);
                        if (GetCell(belowLeftLoc) == ".") sandLoc = belowLeftLoc;
                        else
                        {
                            var belowRightLoc = Tuple.Create(sandLoc.Item1+1, sandLoc.Item2 + 1);
                            if (GetCell(belowRightLoc) == ".") sandLoc = belowRightLoc;
                            else break;
                        }
                    }
                } while (sandLoc.Item1+1 < Cells.Length);

                //Sand settles at sandLoc
                Set(sandLoc, "O");
                Iterations += 1;
            }

            private string GetCell(Tuple<int, int> loc) => Cells[loc.Item1][loc.Item2];

            private void Set(Tuple<int, int> coord, string s) => Cells[coord.Item1][coord.Item2] = s;
        }
    }
}


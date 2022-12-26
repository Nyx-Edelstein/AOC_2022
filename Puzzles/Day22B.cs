namespace AOC_2022.Puzzles
{
    //Normally I wouldn't do this, but this problem is just tedious.
    //In principle you could figure out folding rules for any given cube arrangement, however I only have to solve it for ONE.
    //(Probably what I'd do is transform an arbitrary arrangement to a known state, keeping track of necessary transformations.)
    //...But that is way more work than I want to put in right now.
    //Also, part B is much more complicated than part A, and I see no reason to backfit the solution here.
    internal class Day22B
    {
        const int RIGHT = 0;
        const int DOWN = 1;
        const int LEFT = 2;
        const int UP = 3;

        public static int Solution(string input)
        {
            var map = Parse(input);

            map.FollowInstructions();

            var row = map.Row + 1;
            var column = map.Column + 1;
            var facing = map.Facing;

            return (row * 1000) + (column * 4) + (facing);
        }

        private static Map Parse(string input)
        {
            var parts = input.Split("\r\n\r\n");

            var board = parts[0].Split("\r\n")
                .Select(x => x.ToCharArray().Select(c => "" + c).ToArray())
                .ToArray();

            var instructions = parts[1];

            var map = new Map(board, instructions);
            return map;
        }

        internal class Map
        {
            private readonly int NumRows;
            private readonly int NumColumns;
            private readonly string[][] Board;

            private string RemainingInstructions;

            internal int Row { get; private set; }
            internal int Column { get; private set; }
            internal int Facing { get; private set; }

            private Dictionary<(int, int, int), (int, int, int)> FoldingRules = new();

            public Map(string[][] board, string instructions)
            {
                NumRows = board.Length;
                NumColumns = board.Select(x => x.Length).Max();

                var rows = new List<List<string>>();
                foreach (var row in board)
                {
                    var columns = row.ToList();
                    for (var i = row.Length; i < NumColumns; i++)
                    {
                        columns.Add(" ");
                    }
                    rows.Add(columns);
                }
                
                Board = rows.Select(x => x.ToArray()).ToArray();
                RemainingInstructions = instructions;

                Row = 0;
                Column = NumColumns / 3;
                Facing = RIGHT;

                GenerateFoldingRules();
            }

            private void GenerateFoldingRules()
            {
                //Faces 1-6 for cube map are arranged like so:
                // X12
                // X3X
                // 45X
                // 6XX
                //And we have the same for height and width because square
                var edgeOffset = NumColumns / 3;
                var edgeRange = Enumerable.Range(0, edgeOffset).ToArray();
                

                //Compute points for edges
                //Face 1
                var rowOffset = 0;
                var rowEnding = edgeOffset - 1;
                var colOffset = edgeOffset;
                var colEnding = edgeOffset * 2 - 1;
                var edge_1t = edgeRange.Select(x => (row: rowOffset, col: colOffset + x)).ToArray();
                var edge_1b = edgeRange.Select(x => (row: rowEnding, col: colOffset + x)).ToArray();
                var edge_1l = edgeRange.Select(x => (row: rowOffset + x, col: colOffset)).ToArray();
                var edge_1r = edgeRange.Select(x => (row: rowOffset + x, col: colEnding)).ToArray();
                //Face 2
                rowOffset = 0;
                rowEnding = edgeOffset - 1;
                colOffset = edgeOffset * 2;
                colEnding = edgeOffset * 3 - 1;
                var edge_2t = edgeRange.Select(x => (row: rowOffset, col: colOffset + x)).ToArray();
                var edge_2b = edgeRange.Select(x => (row: rowEnding, col: colOffset + x)).ToArray();
                var edge_2l = edgeRange.Select(x => (row: rowOffset + x, col: colOffset)).ToArray();
                var edge_2r = edgeRange.Select(x => (row: rowOffset + x, col: colEnding)).ToArray();
                //Face 3
                rowOffset = edgeOffset;
                rowEnding = edgeOffset*2 - 1;
                colOffset = edgeOffset;
                colEnding = edgeOffset * 2 - 1;
                var edge_3t = edgeRange.Select(x => (row: rowOffset, col: colOffset + x)).ToArray();
                var edge_3b = edgeRange.Select(x => (row: rowEnding, col: colOffset + x)).ToArray();
                var edge_3l = edgeRange.Select(x => (row: rowOffset + x, col: colOffset)).ToArray();
                var edge_3r = edgeRange.Select(x => (row: rowOffset + x, col: colEnding)).ToArray();
                //Face 4
                rowOffset = edgeOffset * 2;
                rowEnding = edgeOffset * 3 - 1;
                colOffset = 0;
                colEnding = edgeOffset - 1;
                var edge_4t = edgeRange.Select(x => (row: rowOffset, col: colOffset + x)).ToArray();
                var edge_4b = edgeRange.Select(x => (row: rowEnding, col: colOffset + x)).ToArray();
                var edge_4l = edgeRange.Select(x => (row: rowOffset + x, col: colOffset)).ToArray();
                var edge_4r = edgeRange.Select(x => (row: rowOffset + x, col: colEnding)).ToArray();
                //Face 5
                rowOffset = edgeOffset * 2;
                rowEnding = edgeOffset * 3 - 1;
                colOffset = edgeOffset;
                colEnding = edgeOffset*2 - 1;
                var edge_5t = edgeRange.Select(x => (row: rowOffset, col: colOffset + x)).ToArray();
                var edge_5b = edgeRange.Select(x => (row: rowEnding, col: colOffset + x)).ToArray();
                var edge_5l = edgeRange.Select(x => (row: rowOffset + x, col: colOffset)).ToArray();
                var edge_5r = edgeRange.Select(x => (row: rowOffset + x, col: colEnding)).ToArray();
                //Face 6
                rowOffset = edgeOffset * 3;
                rowEnding = edgeOffset * 4 - 1;
                colOffset = 0;
                colEnding = edgeOffset - 1;
                var edge_6t = edgeRange.Select(x => (row: rowOffset, col: colOffset + x)).ToArray();
                var edge_6b = edgeRange.Select(x => (row: rowEnding, col: colOffset + x)).ToArray();
                var edge_6l = edgeRange.Select(x => (row: rowOffset + x, col: colOffset)).ToArray();
                var edge_6r = edgeRange.Select(x => (row: rowOffset + x, col: colEnding)).ToArray();


                //Generate folding rules
                void AddFoldingRule((int row, int col)[] vectorFrom, (int row, int col)[] vectorTo, int oldFacing, int newFacing)
                {
                    (int row, int col) move = oldFacing switch
                    {
                        RIGHT => (0, 1),
                        DOWN => (1, 0),
                        LEFT => (0, -1),
                        UP => (-1, 0)
                    };

                    for (var i = 0; i < edgeOffset; i++)
                    {
                        var from = vectorFrom[i];
                        var to = vectorTo[i];

                        FoldingRules.Add((from.row + move.row, from.col + move.col, oldFacing), (to.row, to.col, newFacing));
                    }
                }

                //Does not need to be coded:
                // 1R <-> 2L
                // 1B <-> 3T
                // 3B <-> 5T
                // 4R <-> 5L
                // 4B <-> 6T

                //1L -> 4L mirrored, left -> right
                var vectorFrom = edge_1l;
                var vectorTo = edge_4l.Reverse().ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: LEFT, newFacing: RIGHT);
                //4L -> 1L mirrored, left -> right
                vectorFrom = edge_4l;
                vectorTo = edge_1l.Reverse().ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: LEFT, newFacing: RIGHT);
                
                //1T -> 6L, up -> right
                vectorFrom = edge_1t;
                vectorTo = edge_6l.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: UP, newFacing: RIGHT);
                //6L -> 1T, left -> down
                vectorFrom = edge_6l;
                vectorTo = edge_1t.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: LEFT, newFacing: DOWN);

                //2T -> 6B, up -> up
                vectorFrom = edge_2t;
                vectorTo = edge_6b.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: UP, newFacing: UP);
                //6B -> 2t, down -> down
                vectorFrom = edge_6b;
                vectorTo = edge_2t.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: DOWN, newFacing: DOWN);

                //2R -> 5R mirrored, right -> left
                vectorFrom = edge_2r;
                vectorTo = edge_5r.Reverse().ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: RIGHT, newFacing: LEFT);
                //5R -> 2B mirrored, right -> left
                vectorFrom = edge_5r;
                vectorTo = edge_2r.Reverse().ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: RIGHT, newFacing: LEFT);

                //2B -> 3R, down -> left
                vectorFrom = edge_2b;
                vectorTo = edge_3r.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: DOWN, newFacing: LEFT);
                //3R -> 2B, right -> up
                vectorFrom = edge_3r;
                vectorTo = edge_2b.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: RIGHT, newFacing: UP);

                //3L -> 4T, left -> down
                vectorFrom = edge_3l;
                vectorTo = edge_4t.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: LEFT, newFacing: DOWN);
                //4T -> 3L, up -> right
                vectorFrom = edge_4t;
                vectorTo = edge_3l.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: UP, newFacing: RIGHT);

                //5B -> 6R, down -> left
                vectorFrom = edge_5b;
                vectorTo = edge_6r.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: DOWN, newFacing: LEFT);
                //6R -> 5B, right -> up
                vectorFrom = edge_6r;
                vectorTo = edge_5b.ToArray();
                AddFoldingRule(vectorFrom, vectorTo, oldFacing: RIGHT, newFacing: UP);
            }

            public void FollowInstructions()
            {
                do
                {
                    var (dist, turn) = GetNextInstruction();
                    UpdatePosition(dist, turn);

                } while (RemainingInstructions.Any());
            }

            private (int, char) GetNextInstruction()
            {
                var nextFacing = RemainingInstructions.ToList().FindIndex(char.IsLetter);

                int dist;
                char turn;
                if (nextFacing == -1)
                {
                    dist = int.Parse(RemainingInstructions);
                    turn = ' ';
                    RemainingInstructions = "";
                }
                else
                {
                    dist = int.Parse(RemainingInstructions[..nextFacing]);
                    turn = RemainingInstructions[nextFacing];
                    RemainingInstructions = RemainingInstructions[(nextFacing + 1)..];
                }

                return (dist, turn);
            }

            private void UpdatePosition(int dist, char turn)
            {
                bool stopped;
                do
                {
                    stopped = StepForward();
                    dist--;
                } while (!stopped && dist > 0);
                
                Facing = turn switch
                {
                    'L' => (Facing - 1) % 4,
                    'R' => (Facing + 1) % 4,
                    _ => Facing
                };
                if (Facing == -1) Facing = 3;
            }

            private bool StepForward() => Facing switch
            {
                0 => Move(0, 1),
                1 => Move(1, 0),
                2 => Move(0, -1),
                3 => Move(-1, 0)
            };

            private bool Move(int row, int col)
            {
                var toMove = (Row: Row + row, Column: Column + col, Facing);
                if (FoldingRules.ContainsKey(toMove))
                    toMove = FoldingRules[toMove];
                    
                var nextPosition = Board[toMove.Row][toMove.Column];
                if (nextPosition == "#") return true;

                Row = toMove.Row;
                Column = toMove.Column;
                Facing = toMove.Facing;
                return false;
            }
        }
    }
}

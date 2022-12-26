namespace AOC_2022.Puzzles
{
    internal class Day22A
    {
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
                var stopped = false;
                do
                {
                    StepForward(ref stopped);
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

            private void StepForward(ref bool stopped) => stopped = Facing switch
            {
                0 => MoveRight(),
                1 => MoveDown(),
                2 => MoveLeft(),
                3 => MoveUp()
            };

            private bool MoveRight()
            {
                var toMove = Column;
                string nextPosition;
                do
                {
                    toMove += 1;
                    if (toMove >= NumColumns) toMove = 0;
                    nextPosition = Board[Row][toMove];
                } while (nextPosition == " ");

                if (nextPosition == "#") return true;

                Column = toMove;
                return false;
            }

            private bool MoveLeft()
            {
                var toMove = Column;
                string nextPosition;
                do
                {
                    toMove -= 1;
                    if (toMove < 0) toMove = NumColumns - 1;
                    nextPosition = Board[Row][toMove];
                } while (nextPosition == " ");

                if (nextPosition == "#") return true;

                Column = toMove;
                return false;
            }

            private bool MoveDown()
            {
                var toMove = Row;
                string nextPosition;
                do
                {
                    toMove += 1;
                    if (toMove >= NumRows) toMove = 0;
                    nextPosition = Board[toMove][Column];
                } while (nextPosition == " ");

                if (nextPosition == "#") return true;

                Row = toMove;
                return false;
            }

            private bool MoveUp()
            {
                var toMove = Row;
                string nextPosition;
                do
                {
                    toMove -= 1;
                    if (toMove < 0) toMove = NumRows - 1;
                    nextPosition = Board[toMove][Column];
                } while (nextPosition == " ");

                if (nextPosition == "#") return true;

                Row = toMove;
                return false;
            }
        }
    }
}

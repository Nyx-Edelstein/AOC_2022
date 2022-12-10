namespace AOC_2022.Puzzles
{
    public class Day05
    {
        public static string SolutionA(string input)
        {
            return Solution(input, reverseStacks: true);
        }

        public static string SolutionB(string input)
        {
            return Solution(input, reverseStacks: false);
        }

        public static string Solution(string input, bool reverseStacks)
        {
            var data = input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

            var boxData = data[0].Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Contains("["));

            var boxContents = boxData.Select(x => x.ToCharArray())
                .Select(x => x.Select(y => "" + y).ToArray())
                .Select(x => x.Where(y => Array.FindIndex(x, z => ReferenceEquals(y, z)) % 4 == 1));

            //Transpose and filter out empty spaces
            var boxStacks = boxContents.SelectMany(x => x.Select((c, i) => Tuple.Create(c, i)))
                .GroupBy(x => x.Item2)
                .OrderBy(x => x.Key)
                .Select(x => x.Select(y => y.Item1).Where(y => y != " "))
                .Select(x => x.ToList())
                .ToArray();

            var instructionsData = data[1].Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var instructions = instructionsData.Select(x => x.Split())
                .Select(x => x.Where(y => y.ToCharArray().All(char.IsDigit)))
                .Select(x => x.Select(int.Parse).ToArray())
                .Select(x => new
                {
                    from = x[1] - 1,
                    to = x[2] - 1,
                    num = x[0]
                }).ToList();

            var movedBoxes = instructions.Aggregate(boxStacks, (state, instruction) =>
            {
                var toMove = reverseStacks
                    ? state[instruction.from].Take(instruction.num).Reverse().ToList()
                    : state[instruction.from].Take(instruction.num).ToList();
                state[instruction.to] = toMove.Concat(state[instruction.to]).ToList();
                state[instruction.from] = state[instruction.from].Skip(instruction.num).ToList();
                return state;
            });

            var topOfStacks = movedBoxes.Select(x => x.FirstOrDefault());
            var message = string.Join("", topOfStacks);

            return message;
        }
    }
}

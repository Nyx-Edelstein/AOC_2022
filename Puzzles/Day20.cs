namespace AOC_2022.Puzzles
{
    internal class Day20
    {
        public static long SolutionA(string input)
        {
            return Solve(input);
        }

        public static long SolutionB(string input)
        {
            return Solve(input, 811589153, 10);
        }

        private static long Solve(string input, long key = 1, int mixCount = 1)
        {
            var file = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x) * key)
                //To ensure uniqueness
                .Select((val, i) => (val, i))
                .ToArray();

            var mixed = file.ToList();
            var eof = file.Length-1;

            do
            {
                foreach (var item in file)
                {
                    var prev_i = mixed.IndexOf(item);
                    var next_i = (item.val + prev_i) % eof;
                    if (next_i < 0) next_i += eof;

                    mixed.Remove(item);
                    mixed.Insert((int)next_i, item);
                }

                mixCount--;
            } while (mixCount > 0);

            var zeroIndex = mixed.FindIndex(x => x.val == 0);
            var zi_1k = (1000 + zeroIndex) % file.Length;
            var zi_2k = (2000 + zeroIndex) % file.Length;
            var zi_3k = (3000 + zeroIndex) % file.Length;
            var coords = mixed[zi_1k].val + mixed[zi_2k].val + mixed[zi_3k].val;
            return coords;
        }
    }
}

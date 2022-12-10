namespace AOC_2022.Puzzles
{
    public class Day01
    {
        public static int SolutionA(string input)
        {
            return input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Select(int.Parse))
                .Select(x => x.Sum())
                .Max();
        }

        public static int SolutionB(string input)
        {
            return input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Select(int.Parse))
                .Select(x => x.Sum())
                .OrderByDescending(x => x)
                .Take(3)
                .Sum();
        }
    }
}

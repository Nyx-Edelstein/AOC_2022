namespace AOC_2022.Puzzles
{
    public class Day04
    {
        public static int SolutionA(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var pairData = data.Select(x => x.Split(new []{'-',','}, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Select(int.Parse).ToArray())
                .Select(x => new
                {
                    a_lower = x[0],
                    a_upper = x[1],
                    b_lower = x[2],
                    b_upper = x[3]
                });

            var pairSubsets = pairData.Where(x =>
            {
                return (x.a_lower <= x.b_lower && x.a_upper >= x.b_upper)
                       || (x.b_lower <= x.a_lower && x.b_upper >= x.a_upper);
            });

            var count = pairSubsets.Count();

            return count;
        }

        public static int SolutionB(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var pairData = data.Select(x => x.Split(new[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Select(int.Parse).ToArray())
                .Select(x => new
                {
                    a_lower = x[0],
                    a_upper = x[1],
                    b_lower = x[2],
                    b_upper = x[3]
                });

            var pairOverlaps = pairData.Where(x =>
            {
                return ((x.a_lower <= x.b_lower && x.b_lower <= x.a_upper) || (x.a_lower <= x.b_upper && x.b_upper <= x.a_upper))
                       || ((x.b_lower <= x.a_lower && x.a_lower <= x.b_upper) || (x.b_lower <= x.a_upper && x.a_upper <= x.b_upper));
            });

            var count = pairOverlaps.Count();

            return count;
        }
    }
}

namespace AOC_2022.Puzzles
{
    internal class Day25
    {
        public static string SolutionA(string input)
        {
            var sum = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(SnafuToDecimal)
                .Sum();

            var snafuSum = DecimalToSnafu(sum);
            return snafuSum;
        }

        private static long SnafuToDecimal(string str) => str.Reverse()
            .Select((c,place) => (long)Math.Pow(5, place) * c switch
            {
                '-' => -1,
                '=' => -2,
                _ => long.Parse(c + "")
            }).Sum();

        private static string DecimalToSnafu(long val)
        {
            var str = "";
            do
            {
                var digit = val % 5;
                val /= 5;
                if (digit > 2)
                {
                    val++;
                    digit -= 5;
                }

                var c = digit switch
                {
                    -2 => '=',
                    -1 => '-',
                    0 => '0',
                    1 => '1',
                    2 => '2',
                };
                str = c + str;
            } while (val > 0);
            return str;
        }
    }
}

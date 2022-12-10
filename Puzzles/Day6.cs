namespace AOC_2022.Puzzles
{
    public class Day6
    {
        public static int SolutionA(string input)
        {
            return Solution(input, 4);
        }

        public static int SolutionB(string input)
        {
            return Solution(input, 14);
        }

        public static int Solution(string input, int packetSize)
        {
            var marker = input.Select((x, i) => new
            {
                index = i,
                group = input[i..packetSize]
            }).First(x => x.group.ToHashSet().Count == packetSize);

            return marker.index + packetSize;
        }
    }
}

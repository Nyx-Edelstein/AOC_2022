namespace AOC_2022.Puzzles
{
    public class Day03
    {
        public static int SolutionA(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var rucksacks = data.Select(x => new
            {
                first = x[..(x.Length / 2)],
                second = x[(x.Length / 2)..],
            });

            var rucksackSets = rucksacks.Select(x => new
            {
                first = new HashSet<char>(x.first.ToCharArray()),
                second = new HashSet<char>(x.second.ToCharArray()),
            });

            var duplicateItems = rucksackSets.Select(x =>  x.first.Intersect(x.second))
                .Select(x => x.First());

            var itemPriorities = duplicateItems.Select(PriorityMap);

            var sum = itemPriorities.Sum();

            return sum;
        }

        public static int SolutionB(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var groups = data.GroupBy(x => Array.IndexOf(data, x) / 3)
                .Select(g => g.ToList());

            var groupSets = groups.Select(g => g.Select(x => new HashSet<char>(x.ToCharArray())).ToList());

            var duplicateItems = groupSets.Select(g => g[0].Intersect(g[1]).Intersect(g[2]))
                .Select(x => x.First());

            var itemPriorities = duplicateItems.Select(PriorityMap);

            var sum = itemPriorities.Sum();

            return sum;
        }

        private static int PriorityMap(char item)
        {
            return char.IsLower(item) ? (item - 'a') + 1 : (item - 'A') + 27;
        }
    }
}

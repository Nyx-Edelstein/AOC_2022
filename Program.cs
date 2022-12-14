using AOC_2022.Puzzles;

namespace AOC_2022
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var input = GetInput(@"11_1", test: false);
            //var solution = Day11.SolutionA(input);
            var solution = Day11.SolutionB(input);

            Console.WriteLine(solution);
            Console.WriteLine("\r\nPress any key to continue...");
            Console.ReadKey();
        }

        private static string GetInput(string puzzle, bool test)
        {
            var path = test
                ? $@"..\..\..\Input\Sample\{puzzle}.txt"
                : $@"..\..\..\Input\{puzzle}.txt";
            return File.ReadAllText(path);
        }
    }
}
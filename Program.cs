using System.Diagnostics;
using AOC_2022.Puzzles;

namespace AOC_2022
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var input = GetInput(@"19_1", test: false);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //var solution = Day19.SolutionA(input);
            var solution = Day19.SolutionB(input);

            watch.Stop();

            Console.WriteLine(solution);
            Console.WriteLine($"{watch.ElapsedMilliseconds}ms");
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
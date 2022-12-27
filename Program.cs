using System.Diagnostics;
using AOC_2022.Puzzles;

namespace AOC_2022
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();

            //var totalms = 0L;
            //for (var i = 1; i <= 25; i++)
            //{
                const int i = 25;

                var input = GetInput(@$"{i:00}_1", test: false);
                var SolutionA = Solutions[$"{i}A"];
                var SolutionB = Solutions[$"{i}B"];

                watch.Start();
                var a = SolutionA(input);
                watch.Stop();
                //totalms += watch.ElapsedMilliseconds;
                Console.WriteLine($"Day {i}A:\r\n{a}");
                Console.WriteLine($"({watch.ElapsedMilliseconds}ms)");

                watch.Reset();
                watch.Start();
                var b = SolutionB(input);
                watch.Stop();
                //totalms += watch.ElapsedMilliseconds;
                Console.WriteLine($"\r\nDay {i}B:\r\n{b}");
                Console.WriteLine($"({watch.ElapsedMilliseconds}ms)");

                watch.Reset();
                Console.WriteLine("\r\n==========\r\n");
            //}

            //Console.WriteLine($"Total time: {totalms}");
            Console.WriteLine("\r\nPress any key to continue...\r\n");
            Console.ReadKey();
        }

        private static Dictionary<string, Func<string, string>> Solutions = new()
        {
            {"1A", input => Day01.SolutionA(input).ToString()},
            {"1B", input => Day01.SolutionB(input).ToString()},
            {"2A", input => Day02.SolutionA(input).ToString()},
            {"2B", input => Day02.SolutionB(input).ToString()},
            {"3A", input => Day03.SolutionA(input).ToString()},
            {"3B", input => Day03.SolutionB(input).ToString()},
            {"4A", input => Day04.SolutionA(input).ToString()},
            {"4B", input => Day04.SolutionB(input).ToString()},
            {"5A", input => Day05.SolutionA(input).ToString()},
            {"5B", input => Day05.SolutionB(input).ToString()},
            {"6A", input => Day06.SolutionA(input).ToString()},
            {"6B", input => Day06.SolutionB(input).ToString()},
            {"7A", input => Day07.SolutionA(input).ToString()},
            {"7B", input => Day07.SolutionB(input).ToString()},
            {"8A", input => Day08.SolutionA(input).ToString()},
            {"8B", input => Day08.SolutionB(input).ToString()},
            {"9A", input => Day09.SolutionA(input).ToString()},
            {"9B", input => Day09.SolutionB(input).ToString()},
            {"10A", input => Day10.SolutionA(input).ToString()},
            {"10B", input => Day10.SolutionB(input).ToString()},
            {"11A", input => Day11.SolutionA(input).ToString()},
            {"11B", input => Day11.SolutionB(input).ToString()},
            {"12A", input => Day12.SolutionA(input).ToString()},
            {"12B", input => Day12.SolutionB(input).ToString()},
            {"13A", input => Day13.SolutionA(input).ToString()},
            {"13B", input => Day13.SolutionB(input).ToString()},
            {"14A", input => Day14.SolutionA(input).ToString()},
            {"14B", input => Day14.SolutionB(input).ToString()},
            {"15A", input => Day15.SolutionA(input).ToString()},
            {"15B", input => Day15.SolutionB(input).ToString()},
            {"16A", input => Day16.SolutionA(input).ToString()},
            {"16B", input => Day16.SolutionB(input).ToString()},
            {"17A", input => Day17.SolutionA(input).ToString()},
            {"17B", input => Day17.SolutionB(input).ToString()},
            {"18A", input => Day18.SolutionA(input).ToString()},
            {"18B", input => Day18.SolutionB(input).ToString()},
            {"19A", input => Day19.SolutionA(input).ToString()},
            {"19B", input => Day19.SolutionB(input).ToString()},
            {"20A", input => Day20.SolutionA(input).ToString()},
            {"20B", input => Day20.SolutionB(input).ToString()},
            {"21A", input => Day21.SolutionA(input).ToString()},
            {"21B", input => Day21.SolutionB(input).ToString()},
            {"22A", input => Day22A.Solution(input).ToString()},
            {"22B", input => Day22B.Solution(input).ToString()},
            {"23A", input => Day23.SolutionA(input).ToString()},
            {"23B", input => Day23.SolutionB(input).ToString()},
            {"24A", input => Day24.SolutionA(input).ToString()},
            {"24B", input => Day24.SolutionB(input).ToString()},
            {"25A", input => Day25.SolutionA(input).ToString()},
            {"25B", input => "No Part B"},
        };


        private static string GetInput(string puzzle, bool test)
        {
            var path = test
                ? $@"..\..\..\Input\Sample\{puzzle}.txt"
                : $@"..\..\..\Input\{puzzle}.txt";
            return File.ReadAllText(path);
        }
    }
}

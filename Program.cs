﻿using System.Diagnostics;
using AOC_2022.Puzzles;

namespace AOC_2022
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var input = GetInput(@"22_1", test: false);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //var solution = Day22A.Solution(input);
            var solution = Day22B.Solution(input);

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
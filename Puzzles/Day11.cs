namespace AOC_2022.Puzzles
{
    public class Day11
    {
        public static long SolutionA(string input)
        {
            return Solution(input, 20, 3);
        }

        public static long SolutionB(string input)
        {
            return Solution(input, 10000, 1);
        }

        private static long Solution(string input, int rounds, int worryCoef)
        {
            var monkeys = Parse(input);

            //All the test divisors are prime numbers and are therefore relatively prime / coprime
            //This means that modding the new value by the product of all test divisors will always preserve the test operation
            var magicDivisor = monkeys.Select(x => x.Divisor).Aggregate((long)1, (x, d) => x * d);

            for (var i = 0; i < rounds; i++)
            {
                foreach (var m in monkeys)
                {
                    foreach (var itemValue in m.Items)
                    {
                        var newValue = (m.Inspect(itemValue) / worryCoef)%(magicDivisor);
                        var testResult = m.Test(newValue);
                        if (testResult)
                            monkeys[m.TrueMonkey].Items.Add(newValue);
                        else
                            monkeys[m.FalseMonkey].Items.Add(newValue);

                        m.NumInspections += 1;
                    }
                    m.Items.Clear();
                }

                //Console.WriteLine($"Round {i+1}:");
                //foreach (var m in monkeys)
                //{
                //    Console.WriteLine($"Monkey {m.MonkeyId}: {string.Join(", ", m.Items)}");
                //}
                //Console.WriteLine("");
            }

            var monkeyBusiness = monkeys.OrderByDescending(x => x.NumInspections).Take(2).Select(x => x.NumInspections).ToArray();
            return monkeyBusiness[0] * monkeyBusiness[1];
        }

        private static MonkeyData[] Parse(string input)
        {
            var monkeyDataRaw = input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

            var monkeyData = new List<MonkeyData>();
            foreach (var mdr in monkeyDataRaw)
            {
                var s = mdr.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                
                var m = new MonkeyData
                {
                    MonkeyId = int.Parse(string.Join("", s[0].Where(char.IsDigit))),
                    Items = s[1][18..].Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList(),
                };

                var operationData = s[2][23..].Split(" ");
                Func<long,long,long> operation = operationData[0] switch
                {
                    "*" => (x,y) => x * y,
                    "+" => (x,y) => x + y
                };
                var operationValue = operationData[1];
                m.Inspect = operationValue switch
                {
                    "old" => x => operation(x, x),
                    _ => x => operation(x, int.Parse(operationValue)),
                };

                var divsor = int.Parse(s[3][21..]);
                m.Divisor = divsor;
                m.Test = x => x % divsor == 0;

                m.TrueMonkey = int.Parse(s[4][29..]);
                m.FalseMonkey = int.Parse(s[5][30..]);

                monkeyData.Add(m);
            }

            return monkeyData.ToArray();
        }

        private class MonkeyData
        {
            internal int MonkeyId { get; set; }
            internal List<long> Items { get; set; }
            internal Func<long, long> Inspect { get; set; }
            internal int Divisor { get; set; }
            internal Func<long, bool> Test { get; set; }
            internal int TrueMonkey { get; set; }
            internal int FalseMonkey { get; set; }
            internal long NumInspections { get; set; }
        }
    }

}

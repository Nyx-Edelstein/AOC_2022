namespace AOC_2022.Puzzles
{
    public class Day10
    {
        public static int SolutionA(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();

            var keyCycles = new[]{ 20, 60, 100, 140, 180, 220 };
            var cycleNum = 1;
            var x = 1;
            var blocked = false;
            var signalStrength = 0;
            while (data.Any() && cycleNum <= keyCycles.Last())
            {
                if (keyCycles.Contains(cycleNum)) signalStrength += cycleNum * x;

                var instruction = data[0];
                if (instruction.StartsWith("addx"))
                {
                    if (!blocked)
                    {
                        blocked = true;
                    }
                    else
                    {
                        x += int.Parse(instruction.Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]);
                        blocked = false;
                    }
                }

                if (!blocked) data.RemoveAt(0);

                cycleNum += 1;
            }

            return signalStrength;
        }

        public static string SolutionB(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();

            var cycleNum = 1;
            var x = 1;
            var blocked = false;
            var crt = "";
            while (data.Any())
            {
                crt += Math.Abs((cycleNum-1)%40 - x) switch
                {
                    < 2 => "#",
                    _ => "."
                };

                var instruction = data[0];
                if (instruction.StartsWith("addx"))
                {
                    if (!blocked)
                    {
                        blocked = true;
                    }
                    else
                    {
                        x += int.Parse(instruction.Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]);
                        blocked = false;
                    }
                }

                if (!blocked)
                {
                    data.RemoveAt(0);
                }

                cycleNum += 1;

                if ((cycleNum - 1) % 40 == 0) crt += "\r\n";
            }

            return crt;
        }
    }
}

namespace AOC_2022.Puzzles
{
    public class Day02
    {
        public static int SolutionA(string input)
        {
            return input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new
                {
                    responseScore = CalculateResponseScore_A(x[1]),
                    roundScore = CalculateRoundScore_A(x[0], x[1]),
                }).Select(x => x.responseScore + x.roundScore)
                .Sum();
        }


        public static int SolutionB(string input)
        {
            return input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new
                {
                    responseScore = CalculateResponseScore_B(x[0], x[1]),
                    roundScore = CalculateRoundScore_B(x[1]),
                }).Select(x => x.responseScore + x.roundScore)
                .Sum();
        }

        private static int CalculateResponseScore_A(string response)
        {
            return response switch
            {
                "X" => 1,
                "Y" => 2,
                "Z" => 3,
                _ => 0
            };
        }

        private static int CalculateRoundScore_A(string play, string response)
        {
            switch (play)
            {
                case "A" when response == "Z":
                case "B" when response == "X":
                case "C" when response == "Y":
                    return 0;
                case "A" when response == "X":
                case "B" when response == "Y":
                case "C" when response == "Z":
                    return 3;
                case "A" when response == "Y":
                case "B" when response == "Z":
                case "C" when response == "X":
                    return 6;
                default:
                    return 0;
            }
        }


        private static int CalculateResponseScore_B(string play, string strategy)
        {
            switch (play)
            {
                case "A" when strategy == "Y": //rock && draw
                case "B" when strategy == "X": //paper && lose
                case "C" when strategy == "Z": //scissors && win
                    return 1; //play rock

                case "A" when strategy == "Z": //rock && win
                case "B" when strategy == "Y": //paper && draw
                case "C" when strategy == "X": //scissors && lose
                    return 2; //play paper

                case "A" when strategy == "X": //rock && lose
                case "B" when strategy == "Z": //paper && win
                case "C" when strategy == "Y": //scissors && draw
                    return 3; //play scissors

                default:
                    return 0;
            }
        }

        private static int CalculateRoundScore_B(string strategy)
        {
            return strategy switch
            {
                "X" => 0,
                "Y" => 3,
                "Z" => 6,
                _ => 0
            };
        }
    }
}

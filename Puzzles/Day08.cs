using AOC_2022.Extensions;

namespace AOC_2022.Puzzles
{
    public class Day08
    {
        public static int SolutionA(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Select(y => int.Parse("" + y)).ToArray())
                .ToArray();

            bool[][] Visible(int[][] x) => x.Select(y => y.Select((h, i) => i == 0 || h > y.Take(i).Max()).ToArray()).ToArray();

            var leftVisible = Visible(data);
            var rightVisible = Visible(data.ReverseElements()).ReverseElements();
            var topVisible = Visible(data.Transpose()).Transpose();
            var bottomVisible = Visible(data.Transpose().ReverseElements()).ReverseElements().Transpose();

            var visibleMap = data.Select((x, i) => x.Select((y, j) =>
            {
                return leftVisible[i][j]
                || rightVisible[i][j]
                || topVisible[i][j]
                || bottomVisible[i][j];
            }));

            var visibleCount = visibleMap.Select(x => x.Count(y => y)).Sum();

            return visibleCount;
        }

        public static int SolutionB(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Select(y => int.Parse("" + y)).ToArray())
                .ToArray();

            int[][] NumVisible(int[][] x) => x.Select(y => y.Select((h, i) =>
            {
                var visible = y.Take(i).Reverse().TakeWhile(z => z < h).Count();
                
                if (i > 0 && visible < i) visible += 1;

                return visible;
            }).ToArray()).ToArray();

            var leftNumVisible = NumVisible(data);
            var rightNumVisible = NumVisible(data.ReverseElements()).ReverseElements();
            var topNumVisible = NumVisible(data.Transpose()).Transpose();
            var bottomNumVisible = NumVisible(data.Transpose().ReverseElements()).ReverseElements().Transpose();

            var scenicScore = data.Select((x, i) => x.Select((y, j) =>
            {
                return leftNumVisible[i][j]
                * rightNumVisible[i][j]
                * topNumVisible[i][j]
                * bottomNumVisible[i][j];
            }));

            var maxScenicScore = scenicScore.Max(x => x.Max());

            return maxScenicScore;
        }
    }
}

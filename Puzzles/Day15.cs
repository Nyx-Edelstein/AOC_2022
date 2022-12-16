namespace AOC_2022.Puzzles
{
    public class Day15
    {
        //This isn't optimized enough to be reused for part B but I'm leaving it as-is to show my evolving thought process
        public static int SolutionA(string input)
        {
            const int targetY = 2000000;

            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new[]
                {
                    x[2][2..^1],
                    x[3][2..^1],
                    x[8][2..^1],
                    x[9][2..]
                }.Select(int.Parse).ToArray()).ToList();

            var beaconsOnTargetRow = data.Select(x => Tuple.Create(x[2], x[3]))
                .Where(x => x.Item2 == targetY)
                .Select(x => x.Item1)
                .ToHashSet();

            var positionsWithoutBeacons = data.Select(x => new
            {
                SensorX = x[0],
                SensorY = x[1],
                ClosestBeacon = Math.Abs(x[0] - x[2]) + Math.Abs(x[1] - x[3]),
                DistanceToTargetY = Math.Abs(x[1] - targetY),
            }).Select(x => new
            {
                x.SensorX,
                x.SensorY,
                DistanceRemaining = x.ClosestBeacon - x.DistanceToTargetY
            }).Where(x => x.DistanceRemaining >= 0)
            .Select(x => Enumerable.Range(x.SensorX - x.DistanceRemaining, x.DistanceRemaining * 2 + 1).ToHashSet())
            .Aggregate(new HashSet<int>(), (set, x) =>
            {
                set.UnionWith(x);
                return set;
            }).Except(beaconsOnTargetRow);

            return positionsWithoutBeacons.Count();
        }

        public static long SolutionB(string input)
        {
            const int x_min = 0;
            const int x_max = 4000000;
            const int y_min = 0;
            const int y_max = 4000000;

            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new[]
                {
                    x[2][2..^1],
                    x[3][2..^1],
                    x[8][2..^1],
                    x[9][2..]
                }.Select(int.Parse).ToArray()).ToList();

            var sensors = data.Select(x => new
            {
                SensorX = x[0],
                SensorY = x[1],
                ClosestBeacon = Math.Abs(x[0] - x[2]) + Math.Abs(x[1] - x[3]),
            }).ToArray();

            var beaconX = 0;
            var y = y_min;
            for (; y <= y_max; y++)
            {
                var intervalsCovered = sensors.Select(x => new
                {
                    x.SensorX,
                    DistanceRemaining = x.ClosestBeacon - Math.Abs(x.SensorY - y)
                }).Where(x => x.DistanceRemaining >= 0)
                .Select(x => new
                {
                    start = Math.Max(x_min, x.SensorX - x.DistanceRemaining),
                    end = Math.Min(x_max, x.SensorX + x.DistanceRemaining)
                }).OrderBy(x => x.start).ToArray();

                var start = intervalsCovered[0].start;
                var end = intervalsCovered[0].end;
                for (var index = 1; index < intervalsCovered.Length; index++)
                {
                    var interval = intervalsCovered[index];

                    if (interval.start > end)
                    {
                        beaconX = end + 1;
                        break;
                    }

                    if (interval.end > end)
                        end = interval.end;
                }

                if (beaconX != 0) break;

                //Check that endpoints line up
                if (start != x_min)
                {
                    beaconX = x_min;
                    break;
                }

                if (end != x_max)
                {
                    beaconX = x_max;
                    break;
                }
            }

            var tuningFrequency = beaconX * (long)4000000 + y;
            return tuningFrequency;
        }
    }
}


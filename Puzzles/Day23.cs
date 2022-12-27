namespace AOC_2022.Puzzles
{
    internal class Day23
    {
        public static int SolutionA(string input)
        {
            var (elfLocations, i) = Solve(input, 10);

            var bound_north = elfLocations.Min(x => x.Item1);
            var bound_east = elfLocations.Max(x => x.Item2);
            var bound_south = elfLocations.Max(x => x.Item1);
            var bound_west = elfLocations.Min(x => x.Item2);

            var numElves = elfLocations.Count;
            var rectangleWidth = bound_east - bound_west + 1;
            var rectangleHeight = bound_south - bound_north + 1;
            var rectangleArea = rectangleWidth * rectangleHeight;
            var emptySpaces = rectangleArea - numElves;

            return emptySpaces;
        }

        public static int SolutionB(string input)
        {
            var (elfLocations, i) = Solve(input, 0);
            return i;
        }
        private static HashSet<(int, int)> Parse(string input) => input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Select((c,i) => (c,i)).Where(y => y.c == '#').Select(y => y.i))
            .Select((x,i) => x.Select(y => (i,y)))
            .SelectMany(x => x)
            .ToHashSet();

        private static (HashSet<(int, int)> elfLocations, int i) Solve(string input, int numCycles)
        {
            var elfLocations = Parse(input);
            var directionMasks = new LocationCollection();
            var proposedLocations = new Dictionary<(int, int), List<(int,int)>>();

            Func<bool> LoopCondition = numCycles == 0
                ? () => proposedLocations.Any()
                : () =>
                {
                    numCycles -= 1;
                    return numCycles > 0;
                };

            var i = 0;
            do
            {
                //Get proposed locations
                proposedLocations.Clear();
                var directionChecks = new[]
                {
                    directionMasks.GetNext(),
                    directionMasks.GetNext(),
                    directionMasks.GetNext(),
                    directionMasks.GetNext(),
                };
                foreach (var elfLocation in elfLocations)
                {
                    if (IsAlone(elfLocation, elfLocations)) continue;
                    
                    foreach (var directionCheck in directionChecks)
                    {
                        if (IsAnyOccupied(elfLocation, directionCheck, elfLocations)) continue;

                        var dest = (elfLocation.Item1 + directionCheck[0].Item1, elfLocation.Item2 + directionCheck[0].Item2);

                        if (!proposedLocations.ContainsKey(dest))
                            proposedLocations.Add(dest, new List<(int, int)>
                            {
                                elfLocation
                            });
                        else
                            proposedLocations[dest].Add(elfLocation);

                        break;
                    }
                }

                //For each unique proposed location, make the movement
                var uniqueProposedLocations = proposedLocations.Where(x => x.Value.Count == 1);
                foreach (var proposedLocation in uniqueProposedLocations)
                {
                    var elfOldLoc = proposedLocation.Value.First();
                    var elfNewLoc = proposedLocation.Key;

                    elfLocations.Remove(elfOldLoc);
                    elfLocations.Add(elfNewLoc);
                }

                //Cycle list of directions
                directionMasks.SkipNext();

                i++;
            } while (LoopCondition());
            
            return (elfLocations, i);
        }

        private static bool IsAlone((int, int) loc, HashSet<(int, int)> locations)
        {
            for (var row = -1; row <= 1; row++)
            for (var col = -1; col <= 1; col++)
            {
                if (row == 0 && col == 0) continue;

                var check = (loc.Item1 + row, loc.Item2 + col);
                if (locations.Contains(check)) return false;
            }

            return true;
        }

        private static bool IsAnyOccupied((int, int) loc, (int, int)[] masks, HashSet<(int, int)> locations)
        {
            foreach (var mask in masks)
            {
                var check = (loc.Item1 + mask.Item1, loc.Item2 + mask.Item2);
                if (locations.Contains(check)) return true;
            }

            return false;
        }

        private abstract class CyclicCollection<T>
        {
            private int i;
            protected T[] Collection;
            internal T GetNext() => Collection[i++ % Collection.Length];
            internal void SkipNext() => i++;
        }

        private class LocationCollection : CyclicCollection<(int, int)[]>
        {
            internal LocationCollection()
            {
                Collection = new[]
                {
                    new[] { (-1,  0), (-1, -1), (-1,  1) }, //North mask
                    new[] { ( 1,  0), ( 1, -1), ( 1,  1) }, //South mask
                    new[] { (0,  -1), (-1, -1), ( 1, -1) }, //West mask
                    new[] { ( 0,  1), (-1,  1), ( 1,  1) }, //East mask
                };
            }
        }
    }
}

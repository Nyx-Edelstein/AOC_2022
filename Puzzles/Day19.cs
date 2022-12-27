using System.Collections.Concurrent;

namespace AOC_2022.Puzzles
{
    public class Day19
    {
        public static int SolutionA(string input)
        {
            const int minuteLimit = 24;
            var blueprints = Parse(input);

            var scores = new ConcurrentBag<int>();
            Parallel.ForEach(blueprints, b =>
            {
                var score = GetBestScore(b, minuteLimit);
                scores.Add(b.Id * score);
            });

            return scores.Sum();
        }

        public static int SolutionB(string input)
        {
            const int minuteLimit = 32;
            var blueprints = Parse(input).Take(3).ToList();

            var scores = new ConcurrentBag<int>();
            Parallel.ForEach(blueprints, b =>
            {
                var score = GetBestScore(b, minuteLimit);
                scores.Add(score);
            });

            return scores.Aggregate(1, (total, s) => total * s);
        }

        private static int GetBestScore(Blueprint blueprint, int minuteLimit)
        {
            var init = new ResourceState(blueprint, minuteLimit);
            var toConsider = new PriorityQueueSet<ResourceState>();
            toConsider.Add(init);

            var bestAtTime = new Dictionary<int, int>();
            var bestOverall = 0;
            do
            {
                var current = toConsider.Pull();
                if (ReferenceEquals(current, null)) continue;

                if (current.Geode > bestOverall) bestOverall = current.Geode;


                int GetSetBestGeodesAtTime(int t, int geodeCount)
                {
                    if (bestAtTime.ContainsKey(t))
                    {
                        var best = bestAtTime[t];
                        if (geodeCount < best) return best;

                        bestAtTime[t] = geodeCount;
                        return geodeCount;
                    }

                    bestAtTime.Add(t, geodeCount);
                    return geodeCount;
                }
                GetSetBestGeodesAtTime(current.MinuteNumber, current.Geode);
                

                //Done if time is up
                if (current.MinuteNumber == minuteLimit) continue;

                void HeuristicAdd(Func<ResourceState?> tryNext)
                {
                    var next = tryNext();

                    if (next == null) return;
                    if (next.GeodesUpperBound < bestOverall) return;

                    var bestAtTime = GetSetBestGeodesAtTime(next.MinuteNumber, next.Geode);
                    if (next.Geode + 3 <= bestAtTime) return;

                    toConsider.Add(next);
                }

                //Branch on trying to construct each type of robot
                HeuristicAdd(current.TryOreRobot);
                HeuristicAdd(current.TryClayRobot);
                HeuristicAdd(current.TryObsidianRobot);
                HeuristicAdd(current.TryGeodeRobot);
                HeuristicAdd(current.TryWait);

            } while (toConsider.Any());
            
            Console.WriteLine(toConsider.Size());
            toConsider.Dispose();
            return bestOverall;
        }

        private static Blueprint[] Parse(string input) => input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" "))
                .Select(x => new[]
                {
                    x[1][..^1],
                    x[6],
                    x[12],
                    x[18],
                    x[21],
                    x[27],
                    x[30]
                }).Select(x => x.Select(int.Parse).ToArray())
                .Select(x => new Blueprint(x))
                .ToArray();

        internal class Blueprint
        {
            internal int Id { get; }
            internal int OreRobotCost { get; }
            internal int ClayRobotCost { get; }
            internal (int, int) ObsidianRobotCost { get; }
            internal (int, int) GeodeRobotCost { get; }

            internal int MaxRobotOreCost { get; }
            internal int ObsidianRobotClayCost => ObsidianRobotCost.Item2;
            internal int GeodeRobotObsidianCost => GeodeRobotCost.Item2;

            internal Blueprint(params int[] costs)
            {
                if (costs.Length != 7) throw new Exception("Invalid data");

                Id = costs[0];
                OreRobotCost = costs[1];
                ClayRobotCost = costs[2];
                ObsidianRobotCost = (costs[3], costs[4]);
                GeodeRobotCost = (costs[5], costs[6]);

                MaxRobotOreCost = new[]
                {
                    OreRobotCost,
                    ClayRobotCost,
                    ObsidianRobotCost.Item1,
                    ObsidianRobotCost.Item2
                }.Max();
            }
        }

        internal enum RobotType
        {
            Ore,
            Clay,
            Obsidian,
            Geode
        }

        internal class ResourceState : IComparable<ResourceState>
        {
            private readonly Blueprint Blueprint;
            private int MinuteLimit { get; }
            
            internal int MinuteNumber { get; }
            private int Ore { get; }
            private int Clay { get; }
            private int Obsidian { get; }
            public int Geode { get; }

            private int OreRobots { get; } = 1;
            private int ClayRobots { get; }
            private int ObsidianRobots { get; }
            private int GeodeRobots { get; }

            internal int GeodesUpperBound { get; }

            //Init
            internal ResourceState(Blueprint blueprint, int minuteLimit)
            {
                Blueprint = blueprint;
                MinuteLimit = minuteLimit;
            }

            //Wait
            private ResourceState(ResourceState prev, int timeToWait)
            {
                Blueprint = prev.Blueprint;
                MinuteLimit = prev.MinuteLimit;

                MinuteNumber = prev.MinuteNumber + timeToWait;
                Ore = prev.Ore + prev.OreRobots * timeToWait;
                Clay = prev.Clay + prev.ClayRobots * timeToWait;
                Obsidian = prev.Obsidian + prev.ObsidianRobots * timeToWait;
                Geode = prev.Geode + prev.GeodeRobots * timeToWait;
                OreRobots = prev.OreRobots;
                ClayRobots = prev.ClayRobots;
                ObsidianRobots = prev.ObsidianRobots;
                GeodeRobots = prev.GeodeRobots;
                GeodesUpperBound = Geode;
            }

            //Wait and build
            private ResourceState(ResourceState prev, int timeToWait, RobotType type, (int,int) cost)
            {
                Blueprint = prev.Blueprint;
                MinuteLimit = prev.MinuteLimit;

                //Simulate waiting for some number of minutes
                MinuteNumber = prev.MinuteNumber + timeToWait;
                Ore = prev.Ore + prev.OreRobots * timeToWait;
                Clay = prev.Clay + prev.ClayRobots * timeToWait;
                Obsidian = prev.Obsidian + prev.ObsidianRobots * timeToWait;
                Geode = prev.Geode + prev.GeodeRobots * timeToWait;
                OreRobots = prev.OreRobots;
                ClayRobots = prev.ClayRobots;
                ObsidianRobots = prev.ObsidianRobots;
                GeodeRobots = prev.GeodeRobots;

                //Consume resources to build a new robot
                switch (type)
                {
                    case RobotType.Ore:
                        Ore -= cost.Item1;
                        break;
                    case RobotType.Clay:
                        Ore -= cost.Item1;
                        break;
                    case RobotType.Obsidian:
                        Ore -= cost.Item1;
                        Clay -= cost.Item2;
                        break;
                    case RobotType.Geode:
                        Ore -= cost.Item1;
                        Obsidian -= cost.Item2;
                        break;
                }

                //Collect ore from one additional cycle
                Ore += OreRobots;
                Clay += ClayRobots;
                Obsidian += ObsidianRobots;
                Geode += GeodeRobots;

                //Increase number of robots
                switch (type)
                {
                    case RobotType.Ore:
                        OreRobots++;
                        break;
                    case RobotType.Clay:
                        ClayRobots++;
                        break;
                    case RobotType.Obsidian:
                        ObsidianRobots++;
                        break;
                    case RobotType.Geode:
                        GeodeRobots++;
                        break;
                }

                //Robot is now caught up to the current minute
                //Compute upper bound and return
                MinuteNumber += 1;
                GeodesUpperBound = ComputeGeodesUpperBound();
            }

            public ResourceState? TryGeodeRobot()
            {
                if (ObsidianRobots == 0) return null;

                var timeToWait = TimeToWait(Blueprint.GeodeRobotCost, (Ore, Obsidian), (OreRobots, ObsidianRobots));
                if (MinuteNumber + timeToWait >= MinuteLimit) return null;

                return new ResourceState(this, timeToWait, RobotType.Geode, Blueprint.GeodeRobotCost);
            }

            public ResourceState? TryObsidianRobot()
            {
                if (ClayRobots == 0) return null;
                if (ObsidianRobots > Blueprint.GeodeRobotObsidianCost) return null;

                var timeToWait = TimeToWait(Blueprint.ObsidianRobotCost, (Ore, Clay), (OreRobots, ClayRobots));
                if (MinuteNumber + timeToWait >= MinuteLimit) return null;

                return new ResourceState(this, timeToWait, RobotType.Obsidian, Blueprint.ObsidianRobotCost);
            }

            public ResourceState? TryClayRobot()
            {
                if (ClayRobots > Blueprint.ObsidianRobotClayCost) return null;

                var timeToWait = TimeToWait(Blueprint.ClayRobotCost, Ore, OreRobots);
                if (MinuteNumber + timeToWait >= MinuteLimit) return null;

                return new ResourceState(this, timeToWait, RobotType.Clay, (Blueprint.ClayRobotCost, 0));
                
            }

            public ResourceState? TryOreRobot()
            {
                if (OreRobots > Blueprint.MaxRobotOreCost) return null;

                var timeToWait = TimeToWait(Blueprint.OreRobotCost, Ore, OreRobots);
                if (MinuteNumber + timeToWait >= MinuteLimit) return null;

                return new ResourceState(this, timeToWait, RobotType.Ore, (Blueprint.OreRobotCost, 0));
            }

            public ResourceState? TryWait()
            {
                if (GeodeRobots < 1) return null;
                var minutesWaited = MinuteLimit - MinuteNumber;
                return new ResourceState(this, minutesWaited);
            }

            private static int TimeToWait((int, int) resourceCosts, (int,int) resourceAmounts, (int,int) robotCounts)
            {
                var resource1TimeToWait = TimeToWait(resourceCosts.Item1, resourceAmounts.Item1, robotCounts.Item1);
                var resource2TimeToWait = TimeToWait(resourceCosts.Item2, resourceAmounts.Item2, robotCounts.Item2);

                return Math.Max(resource1TimeToWait, resource2TimeToWait);
            }

            private static int TimeToWait(int resourceCost, int resourceAmount, int robotCount)
            {
                if (resourceAmount >= resourceCost) return 0;

                var resourcesNeeded = (resourceCost - resourceAmount) * 1.0;
                var timeToWait = Convert.ToInt32(Math.Ceiling(resourcesNeeded / robotCount));

                return timeToWait;
            }

            private int ComputeGeodesUpperBound()
            {
                if (MinuteNumber == MinuteLimit) return Geode;

                var projectedClay = Clay;
                var projectedObsidian = Obsidian;
                var projectedGeode = Geode;

                var projectedClayRobots = ClayRobots;
                var projectedObsidianRobots = ObsidianRobots;
                var projectedGeodeRobots = GeodeRobots;

                //The goal isn't to be accurate, but to compute an upper bound on geodes cheaply
                //To do this, we are ignoring time limits to build all relevant robots
                //We are also assuming we have unlimited ore, making ore robots irrelevant
                for (var i = MinuteNumber; i < MinuteLimit; i++)
                {
                    projectedClay += projectedClayRobots;
                    projectedObsidian += projectedObsidianRobots;
                    projectedGeode += projectedGeodeRobots;

                    projectedClayRobots += 1;

                    if (projectedClay >= Blueprint.ObsidianRobotClayCost)
                    {
                        projectedClay -= Blueprint.ObsidianRobotClayCost;
                        projectedObsidianRobots += 1;
                    }

                    if (projectedObsidian >= Blueprint.GeodeRobotObsidianCost)
                    {
                        projectedObsidian -= Blueprint.GeodeRobotObsidianCost;
                        projectedGeodeRobots += 1;
                    }
                }

                return Geode + projectedGeode;
            }

            public int CompareTo(ResourceState? other)
            {
                if (ReferenceEquals(other, null)) return 1;

                //Prefer more actual geodes
                var geodesCompare = other.Geode.CompareTo(Geode);
                if (geodesCompare != 0) return geodesCompare;

                //Prefer higher upper bound
                var upperBoundCompare = other.GeodesUpperBound.CompareTo(GeodesUpperBound);
                if (upperBoundCompare != 0) return upperBoundCompare;

                //Prefer more time remaining
                var timeCompare = MinuteNumber.CompareTo(other.MinuteNumber);
                if (timeCompare != 0) return timeCompare;

                return 0;
            }

            public static bool operator==(ResourceState? left, ResourceState? right)
            {
                if (ReferenceEquals(left, right)) return true;
                if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

                return left.MinuteNumber == right.MinuteNumber
                    && left.Ore == right.Ore
                    && left.Clay == right.Clay
                    && left.Obsidian == right.Obsidian
                    && left.Geode == right.Geode
                    && left.OreRobots == right.OreRobots
                    && left.ClayRobots == right.ClayRobots
                    && left.ObsidianRobots == right.ObsidianRobots
                    && left.GeodeRobots == right.GeodeRobots;
            }

            public static bool operator !=(ResourceState? left, ResourceState? right)
            {
                return !(left == right);
            }

            public override bool Equals(object? obj)
            {
                if (obj == null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj is not ResourceState rs) return false;
                return this == rs;
            }
            
            private int _hash;
            public override int GetHashCode()
            {
                if (_hash != 0) return _hash;

                var hash = new HashCode();
                hash.Add(MinuteNumber);
                hash.Add(Ore);
                hash.Add(Clay);
                hash.Add(Obsidian);
                hash.Add(Geode);
                hash.Add(OreRobots);
                hash.Add(ClayRobots);
                hash.Add(ObsidianRobots);
                hash.Add(GeodeRobots);

                _hash = hash.ToHashCode();
                return _hash;
            }
        }

        internal class PriorityQueueSet<T> : IDisposable where T : class, IComparable<T>
        {
            private readonly HashSet<T> Filter = new();
            private readonly C5.IntervalHeap<T> Queue = new();

            public bool Add(T item)
            {
                var seen = Filter.Contains(item);
                if (seen) return false;

                Queue.Add(item);
                Filter.Add(item);
                return true;
            }

            public T? Pull()
            {
                if (Queue.IsEmpty) return null;

                var item = Queue.FindMin();
                Queue.DeleteMin();
                return item;
            }

            public bool Any() => !Queue.IsEmpty;

            public void Dispose() => Filter.Clear();

            public int Size()
            {
                return Filter.Count;
            }
        }
    }
}


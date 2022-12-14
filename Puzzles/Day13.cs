namespace AOC_2022.Puzzles
{
    public class Day13
    {
        public static int SolutionA(string input)
        {
            var data = input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

            var indexSum = 0;
            for (var index = 0; index < data.Length; index++)
            {
                var packets = data[index].Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                var packetA = new ListItem(packets[0]);
                var packetB = new ListItem(packets[1]);

                //Console.WriteLine($"== Pair {index + 1} ==");

                var result = Item.Compare(packetA, packetB);
                if (result <= 0)
                {
                    indexSum += index + 1;
                }
                
                //Console.WriteLine("");
            }

            return indexSum;
        }
        
        public static int SolutionB(string input)
        {
            var data = input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
                .SelectMany(x => x)
                .Concat(new[] { "[[2]]", "[[6]]" })
                .Select(x => new ListItem(x))
                .ToList();

            data.Sort(Item.Compare);

            var packetStrings = data.Select(x => x.ToString()).ToList();
            var firstDivider = packetStrings.IndexOf("[[2]]") + 1;
            var secondDivider = packetStrings.IndexOf("[[6]]") + 1;

            return firstDivider * secondDivider;
        }

        private abstract class Item
        {
            public static int Compare(Item left, Item right)
            {
                //Console.WriteLine($"Comparing {left} and {right} | Result = ");

                var intLeft = left as IntegerItem;
                var listLeft = left as ListItem;
                var intRight = right as IntegerItem;
                var listRight = right as ListItem;


                if (intLeft != null && intRight != null)
                {
                    var result = intLeft.Value.CompareTo(intRight.Value);
                    //Console.Write($"{result}\r\n");
                    return result;
                }
                    

                if (listLeft != null && listRight != null)
                {
                    var leftItems = listLeft.Items;
                    var rightItems = listRight.Items;
                    var leftListCount = leftItems.Count;
                    var rightListCount = rightItems.Count;
                    var lowerCount = Math.Min(leftListCount, rightListCount);

                    for (var i = 0; i < lowerCount; i++)
                    {
                        var l = listLeft.Items[i];
                        var r = listRight.Items[i];
                        var c = Compare(l, r);
                        if (c != 0)
                        {
                            //Console.Write($"{c}\r\n");
                            return c;
                        }
                    }

                    var result = leftListCount.CompareTo(rightListCount);
                    //Console.Write($"{result}\r\n");
                    return result;
                }

                if (intLeft != null && listRight != null)
                {
                    var newLeft = new ListItem(intLeft.Value);

                    var result = Compare(newLeft, listRight);
                    //Console.Write($"{result}\r\n");
                    return result;
                }

                if (listLeft != null && intRight != null)
                {
                    var newRight = new ListItem(intRight.Value);

                    var result = Compare(listLeft, newRight);
                    //Console.Write($"{result}\r\n");
                    return result;
                }

                //Console.Write($"0\r\n");
                return 0;
            }

            public abstract override string ToString();
        }

        private class ListItem : Item
        {
            public ListItem() { }

            public ListItem(string raw)
            {
                var items = TopLevelSplit(raw).Select(x =>
                {
                    Item newItem = string.IsNullOrEmpty(x)
                        ? new ListItem()
                        : char.IsDigit(x[0])
                            ? new IntegerItem(int.Parse(x))
                            : new ListItem(x);

                    return newItem;
                });

                Items.AddRange(items);
            }

            private List<string> TopLevelSplit(string raw)
            {
                var rawItems = new List<string>();

                //Strip outside brackets
                if (raw.StartsWith('[')) raw = raw[1..];
                if (raw.EndsWith(']')) raw = raw[..^1];

                //We have five cases.
                //1: raw is an empty string | raw = []
                //2: raw is a single integer item | raw = [123]
                //3: raw is an integer item followed by more data | [123,..]
                //4: raw is a single list item | [[..]]
                //5: raw is a list item followed by more data | [[..],..]
                while (raw.Length > 0)
                {
                    //Case 1: empty string
                    if (raw == string.Empty) break;

                    //Case 2: single integer item
                    if (raw.All(char.IsDigit))
                    {
                        rawItems.Add(raw);
                        break;
                    }

                    //Case 3: integer item followed by more data
                    //Take the first item and continue parsing the rest.
                    if (char.IsDigit(raw[0]))
                    {
                        var firstComma = raw.IndexOf(',');

                        var firstItem = raw[..firstComma];
                        rawItems.Add(firstItem);

                        raw = raw[(firstComma + 1)..];
                        continue;
                    }

                    //Case 4: single list item
                    //Case 5: list item followed by more data
                    if (raw[0] == '[')
                    {
                        var openBracketCount = 1;
                        var i = 1;
                        while (openBracketCount > 0 && i < raw.Length)
                        {
                            openBracketCount += raw[i++] switch
                            {
                                '[' => 1,
                                ']' => -1,
                                _ => 0
                            };

                            if (openBracketCount == 0) break;
                        }

                        //Case 4
                        if (i == raw.Length)
                        {
                            rawItems.Add(raw);
                            break;
                        }

                        //Case 5
                        //Take the first item and continue parsing the rest.
                        var firstItem = raw[..i];
                        rawItems.Add(firstItem);
                        raw = raw[(i + 1)..];
                        continue;
                    }

                    //Should be unreachable...
                    throw new Exception("Unhandled case");
                }

                return rawItems;
            }

            public ListItem(int value)
            {
                var intItem = new IntegerItem(value);
                Items.Add(intItem);
            }

            internal List<Item> Items { get; } = new();

            public override string ToString()
            {
                var itemStrings = Items.Select(x => x.ToString());
                return $"[{string.Join(',', itemStrings)}]";
            }
        }

        private class IntegerItem : Item
        {
            public IntegerItem(int value)
            {
                Value = value;
            }

            internal int Value { get; }

            public override string ToString()
            {
                return Value.ToString();
            }
        }
    }
}


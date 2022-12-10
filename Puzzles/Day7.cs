namespace AOC_2022.Puzzles
{
    public class Day7
    {
        public static int SolutionA(string input)
        {
            var folders = Parse(input);
            var dir_sum = folders.Where(x => x.Size <= 100000).Sum(x => x.Size);
            return dir_sum;
        }

        public static int SolutionB(string input)
        {
            var folders = Parse(input);
            var spaceUsed = 70000000 - folders.First(x => x.Name == "/").Size;
            var spaceNeeded = 30000000 - spaceUsed;
            var candidates = folders.Where(x => x.Size >= spaceNeeded).OrderBy(x => x.Size);
            var smallestCandidate = candidates.First();

            return smallestCandidate.Size;
        }

        private static List<Folder> Parse(string input)
        {
            var data = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var dir = new Folder { Name = "/" };
            dir.Parent = dir;
            var currentDirectory = dir;
            var folders = new List<Folder> { dir };
            foreach (var line in data)
            {
                if (line.StartsWith("$ cd "))
                {
                    var cdArg = line[5..];
                    currentDirectory = cdArg switch
                    {
                        ".." => currentDirectory.Parent,
                        "/" => dir,
                        _ => currentDirectory.Folders.First(x => x.Name == cdArg)
                    };
                }
                else if (line.StartsWith("$ ls"))
                {
                    folders.RemoveAll(x => currentDirectory.Folders.Contains(x));
                    currentDirectory.Folders.Clear();
                    currentDirectory.Files.Clear();
                }
                else
                {
                    var fileData = line.Split().ToArray();
                    var fileDescriptor = fileData[0];
                    var fileName = fileData[1];

                    if (fileDescriptor == "dir")
                    {
                        var f = new Folder { Name = fileName, Parent = currentDirectory };
                        currentDirectory.Folders.Add(f);
                        folders.Add(f);
                    }

                    else
                        currentDirectory.Files.Add(new File
                        { Name = fileName, Parent = currentDirectory, Size = int.Parse(fileDescriptor) });
                }
            }

            return folders;
        }

        internal class Folder
        {
            internal string Name { get; init; } = string.Empty;
            internal Folder Parent { get; set; }
            internal List<Folder> Folders { get; } = new();
            internal List<File> Files { get; } = new();

            internal int Size => Files.Sum(x => x.Size) + Folders.Sum(x => x.Size);
        }

        internal class File
        {
            internal string Name { get; init; } = string.Empty;
            internal Folder Parent { get; set; }
            internal int Size { get; init; }
        }
    }
}

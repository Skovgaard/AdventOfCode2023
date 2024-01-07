using Utilities;

var map = new List<List<char>>();
var columnsWithGalaxy = new HashSet<int>();
const char emptyChar = '.';
const char galaxyChar = '#';

var emptyRows = new List<int>();
var originalGalaxySet = new HashSet<Point>();

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    var emptyRowsAdded = 0;
    while (await sr.ReadLineAsync() is { } line)
    {
        var chars = line.ToList();
        map.Add(chars);

        // Adds empty row if no galaxy is found in row
        if (!chars.Contains(galaxyChar))
        {
            var newLine = new char[chars.Count];
            Array.Fill(newLine, emptyChar);
            map.Add(newLine.ToList());
            emptyRowsAdded++;

            // Part 2
            emptyRows.Add(map.Count - 1 - emptyRowsAdded);
            continue;
        }

        // Adds all columns with a galaxy to a set
        for (var i = 0; i < chars.Count; i++)
        {
            if (chars[i] == galaxyChar)
            {
                columnsWithGalaxy.Add(i);

                // Part 2
                var y = map.Count - 1 - emptyRowsAdded;
                originalGalaxySet.Add(new Point(i, y));
            }
        }
    }


    // Adds empty line if no galaxy is found in column
    var emptyColumns = new List<int>();
    for (var x = 0; x < map[0].Count; x++)
    {
        if (!columnsWithGalaxy.Contains(x))
            emptyColumns.Add(x);
    }

    emptyColumns.Reverse();

    foreach (var index in emptyColumns)
    {
        foreach (var row in map)
        {
            row.Insert(index, emptyChar);
        }
    }

    // Adds all galaxys to a list
    var galaxyList = new List<Point>();
    for (var y = 0; y < map.Count; y++)
    {
        for (var x = 0; x < map[y].Count; x++)
        {
            if (map[y][x] == galaxyChar)
                galaxyList.Add(new Point(x, y));
        }
    }

    // Find all distances between galaxy's
    var distances = new List<long>();
    for (var i = 0; i < galaxyList.Count; i++)
    {
        var galaxy = galaxyList[i];
        for (var j = i + 1; j < galaxyList.Count; j++)
        {
            var otherGalaxy = galaxyList[j];
            var distance = Math.Abs(galaxy.X - otherGalaxy.X) + Math.Abs(galaxy.Y - otherGalaxy.Y);
            distances.Add(distance);
        }
    }

    Console.WriteLine($"Part 1: Sum of all distances: {distances.Sum()}");

    // Part 2
    var newGalaxySet = new List<Point>();
    const int multiplier = 1000000 - 1; // - 1 because its replaced by 1 mil and not added 1 mil more
    foreach (var (x, y) in originalGalaxySet)
    {
        var xToAdd = emptyColumns.Count(e => e < x);
        var yToAdd = emptyRows.Count(e => e < y);
        var newGalaxy = new Point(x + xToAdd * multiplier, y + yToAdd * multiplier);
        newGalaxySet.Add(newGalaxy);
    }

    var distancesPart2 = new List<long>();
    for (var i = 0; i < newGalaxySet.Count; i++)
    {
        var galaxy = newGalaxySet[i];
        for (var j = i + 1; j < newGalaxySet.Count; j++)
        {
            var otherGalaxy = newGalaxySet[j];
            var distance = Math.Abs(galaxy.X - otherGalaxy.X) + Math.Abs(galaxy.Y - otherGalaxy.Y);
            distancesPart2.Add(distance);
        }
    }

    Console.WriteLine($"Part 2: Sum of all distances: {distancesPart2.Sum()}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

internal record Point(long X, long Y);
using Utilities;

var map = new List<List<char>>();
var columnsWithGalaxy = new HashSet<int>();
const char emptyChar = '.';
const char galaxyChar = '#';

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

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
            continue;
        }

        // Adds all columns with a galaxy to a set
        for (var i = 0; i < chars.Count; i++)
        {
            if (chars[i] == galaxyChar)
                columnsWithGalaxy.Add(i);
        }
    }


    // Adds empty line if no galaxy is found in column
    var indexesToAdd = new List<int>();
    for (var x = 0; x < map[0].Count; x++)
    {
        if (!columnsWithGalaxy.Contains(x))
            indexesToAdd.Add(x);
    }

    indexesToAdd.Reverse();

    foreach (var index in indexesToAdd)
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
    var distances = new List<int>();
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
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

internal record Point(int X, int Y);
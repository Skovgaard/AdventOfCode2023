using System.Text;
using Utilities;

var map = new char[140, 140];

var startX = 0;
var startY = 0;

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    var column = 0;
    while (await sr.ReadLineAsync() is { } line)
    {
        for (var row = 0; row < line.Length; row++)
        {
            var character = line[row];
            if (character == (char)PointType.Start)
            {
                startX = row;
                startY = column;
            }

            map[row, column] = character;
        }

        column++;
    }

    // Replace S
    map[startX, startY] = (char)PointType.NorthWest;

    var start = new Point(startX, startY);
    var path = new LinkedList<Point>();
    var nextPoint = start;
    Point? lastPoint = null;
    while (nextPoint != start || path.Count == 0)
    {
        var connectedPoints = nextPoint.FindConnectedPoints(map);
        var next = connectedPoints.First(x => x != lastPoint);
        lastPoint = nextPoint;
        nextPoint = next;
        path.AddLast(nextPoint);
    }

    Console.WriteLine("Part1: Steps to point furthest away: " + path.Count / 2);

    // Part 2
    var border = new HashSet<Point>();
    foreach (var p in path.Select(point => new Point(point.X * 2, point.Y * 2)))
    {
        border.Add(p);
    }

    // Inserts extra tiles between all points in path
    var part2Map = new char[map.GetLength(0) * 2 - 1, map.GetLength(1) * 2 - 1];
    for (var y = 0; y < part2Map.GetLength(0); y++)
    {
        var newRow = y % 2 != 0;
        for (var x = 0; x < part2Map.GetLength(1); x++)
        {
            var point = new Point(x, y);
            var newColumn = x % 2 != 0;
            if (newRow || newColumn)
            {
                part2Map[x, y] = '.'; // New tiles
            }
            else if (border.Contains(point))
            {
                part2Map[x, y] = map[x / 2, y / 2]; // Path
            }
            else part2Map[x, y] = '.'; // Empty
        }
    }

    // Find all border points that should be connected
    for (var y = 0; y < part2Map.GetLength(0); y++)
    {
        for (var x = 0; x < part2Map.GetLength(1); x++)
        {
            if (y > 0 && y < part2Map.GetLength(0) - 1)
            {
                char[] allowedSouths =
                [
                    (char)PointType.SouthEast, (char)PointType.SouthWest,
                    (char)PointType.Vertical
                ];
                char[] allowedNorths =
                [
                    (char)PointType.NorthEast, (char)PointType.NorthWest,
                    (char)PointType.Vertical
                ];
                if (allowedSouths.Contains(part2Map[x, y - 1]) && allowedNorths.Contains(part2Map[x, y + 1]))
                {
                    part2Map[x, y] = (char)PointType.Vertical;
                    border.Add(new Point(x, y));
                    continue;
                }
            }

            if (x > 0 && x < part2Map.GetLength(1) - 1)
            {
                char[] allowedEasts =
                [
                    (char)PointType.NorthEast, (char)PointType.SouthEast,
                    (char)PointType.Horizontal
                ];
                char[] allowedWests =
                [
                    (char)PointType.NorthWest, (char)PointType.SouthWest,
                    (char)PointType.Horizontal
                ];
                if (allowedEasts.Contains(part2Map[x - 1, y]) && allowedWests.Contains(part2Map[x + 1, y]))
                {
                    part2Map[x, y] = (char)PointType.Horizontal;
                    border.Add(new Point(x, y));
                }
            }
        }
    }

    var outsideGroup = new Point(0, 0).FindAllPointsInGroup(part2Map, border).ToHashSet();
    var pointsEnclosed = 0;

    // Prints the map
    for (var y = 0; y < part2Map.GetLength(0); y++)
    {
        var sb = new StringBuilder();
        for (var x = 0; x < part2Map.GetLength(1); x++)
        {
            sb.Append(part2Map[x, y]);

            // Removes all the extra tiles and count remaining that are not in border or outside group
            var point = new Point(x, y);
            if (!border.Contains(point) && !outsideGroup.Contains(point))
            {
                if (y % 2 != 0) continue;
                if (x % 2 != 0) continue;
                pointsEnclosed++;
            }
        }

        // Comment in to print map
        // Console.WriteLine(sb.ToString());
    }

    Console.WriteLine("Part2: Points enclosed: " + pointsEnclosed);
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

internal record Point(int X, int Y)
{
    public IEnumerable<Point> FindAllPointsInGroup(char[,] map, IEnumerable<Point> borderPoints)
    {
        var points = new HashSet<Point> { this };
        var visitedPoints = new HashSet<Point> { this };
        visitedPoints.UnionWith(borderPoints);
        var surroundingPoints = FindSurroundingPoints(map);

        var pointsToVisit = new Queue<Point>(surroundingPoints.Where(x => !visitedPoints.Contains(x)));
        while (pointsToVisit.Count > 0)
        {
            var point = pointsToVisit.Dequeue();
            if (visitedPoints.Contains(point)) continue;

            points.Add(point);
            visitedPoints.Add(point);

            var nextSurroundingPointsNotVisited =
                point.FindSurroundingPoints(map).Where(nextPoint => !visitedPoints.Contains(nextPoint)).ToArray();

            foreach (var nextPoint in nextSurroundingPointsNotVisited)
            {
                pointsToVisit.Enqueue(nextPoint);
            }
        }

        return points;
    }

    public IEnumerable<Point> FindConnectedPoints(char[,] map)
    {
        var character = map[X, Y];
        return character switch
        {
            (char)PointType.Vertical => [this with { Y = Y - 1 }, this with { Y = Y + 1 }],
            (char)PointType.Horizontal => [this with { X = X - 1 }, this with { X = X + 1 }],
            (char)PointType.NorthEast => [this with { Y = Y - 1 }, this with { X = X + 1 }],
            (char)PointType.NorthWest => [this with { Y = Y - 1 }, this with { X = X - 1 }],
            (char)PointType.SouthWest => [this with { Y = Y + 1 }, this with { X = X - 1 }],
            (char)PointType.SouthEast => [this with { Y = Y + 1 }, this with { X = X + 1 }],
            (char)PointType.Start => FindConnectedPointsBasedOnSurroundingPoints(map),
            _ => []
        };
    }

    private List<Point> FindConnectedPointsBasedOnSurroundingPoints(char[,] map)
    {
        IEnumerable<Point> surroundingPoints =
        [
            new Point(X: X, Y: Y - 1),
            new Point(X: X + 1, Y: Y),
            new Point(X: X, Y: Y + 1),
            new Point(X: X - 1, Y: Y),
        ];

        List<Point> list = [];
        foreach (var point in surroundingPoints)
        {
            var connectedPoints = point.FindConnectedPoints(map);
            if (connectedPoints.Any(x => x == this)) list.Add(point);
        }

        return list;
    }

    private List<Point> FindSurroundingPoints(char[,] map)
    {
        IEnumerable<Point> surroundingPoints =
        [
            new Point(X: X, Y: Y - 1),
            new Point(X: X + 1, Y: Y - 1),
            new Point(X: X + 1, Y: Y),
            new Point(X: X + 1, Y: Y + 1),
            new Point(X: X, Y: Y + 1),
            new Point(X: X - 1, Y: Y + 1),
            new Point(X: X - 1, Y: Y),
            new Point(X: X - 1, Y: Y - 1),
        ];

        var mapXLength = map.GetLength(0);
        var mapYLength = map.GetLength(1);

        return surroundingPoints.Where(p => p is { X: >= 0, Y: >= 0 } && p.X < mapXLength && p.Y < mapYLength)
            .ToList();
    }
};

internal enum PointType
{
    Vertical = '|',
    Horizontal = '-',
    NorthEast = 'L',
    NorthWest = 'J',
    SouthWest = '7',
    SouthEast = 'F',
    Start = 'S'
}
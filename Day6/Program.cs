using Utilities;

var times = new List<int>();
var distances = new List<int>();

var data = new List<List<int>>();

var numberOfWaysToBeat = new List<int>();

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    while (await sr.ReadLineAsync() is { } line)
    {
        var split = line.Split(":");
        var type = split[0];
        var values = split[1].Split(" ").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse)
            .ToList();

        if (type == "Time")
            times.AddRange(values);
        else if (type == "Distance")
            distances.AddRange(values);
    }

    for (var i = 0; i < times.Count; i++)
    {
        var time = times[i];
        var distance = distances[i];

        for (var j = 0; j < time; j++)
        {
            var calculatedDistance = (int)CalculateDistance(j, time);

            if (i > data.Count - 1)
                data.Add(new List<int>(calculatedDistance));
            else
                data[i].Add(calculatedDistance);
        }

        numberOfWaysToBeat.Add(data[i].Count(x => x > distance));
    }

    var numberOfWaysToBeatMultiplied = numberOfWaysToBeat.Aggregate((acc, next) => acc * next);

    Console.WriteLine($"PART 1: Number of ways to beat: {numberOfWaysToBeatMultiplied}");

    // Part 2
    var time2 = long.Parse(times.Aggregate("", (acc, next) => acc + next));
    var distance2 = long.Parse(distances.Aggregate("", (acc, next) => acc + next));

    var count = FindNumberOfWaysToBeat(time2, distance2);

    Console.WriteLine($"PART 2: Number of ways to beat: {count}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

return;

long CalculateDistance(long timeCharging, long totalTime) => (totalTime - timeCharging) * timeCharging;

long FindNumberOfWaysToBeat(long time, long distance)
{
    var chargingTime = time / 2;
    var lowestBeatTime = FindBorderValueRecursive(0, time, distance, chargingTime / 2, true);
    var highestBeatTime = FindBorderValueRecursive(chargingTime, time, distance, chargingTime / 2, false);
    return highestBeatTime - lowestBeatTime + 1; // For inclusive
}

// Some kind of binary search - this is not pretty, but it works
long FindBorderValueRecursive(long lowerBorder, long time, long limit, long dif, bool findLowest)
{
    var chargingTime = lowerBorder + dif;

    var distanceForTime = CalculateDistance(chargingTime, time);
    var isBeaten = distanceForTime > limit;

    var distanceForTimeBefore = CalculateDistance(chargingTime - 1, time);
    var isBeatenBefore = distanceForTimeBefore > limit;
    if (isBeatenBefore && !isBeaten) return chargingTime - 1;
    if (!isBeatenBefore && isBeaten) return chargingTime;

    var distanceForTimeAfter = CalculateDistance(chargingTime + 1, time);
    var isBeatenAfter = distanceForTimeAfter > limit;
    if (isBeatenAfter && !isBeaten) return chargingTime + 1;
    if (!isBeatenAfter && isBeaten) return chargingTime;

    if (findLowest && distanceForTime > limit || !findLowest && distanceForTime < limit)
        return FindBorderValueRecursive(lowerBorder, time, limit, dif / 2, findLowest);

    if (findLowest && distanceForTime < limit || !findLowest && distanceForTime > limit)
        return FindBorderValueRecursive(chargingTime, time, limit, dif / 2, findLowest);

    return 0;
}
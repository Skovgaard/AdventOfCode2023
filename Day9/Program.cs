using Utilities;

var sumOfExtrapolatedValues = 0L;
var sumOfExtrapolatedValuesPart2 = 0L;

var values = new List<List<List<long>>>();

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    while (await sr.ReadLineAsync() is { } line)
    {
        var data = line.Split(" ");
        var entry = data.Select(long.Parse).ToList();
        var history = new List<List<long>> { entry };

        while (history.Last().Any(x => x != 0))
        {
            var lastHistoryEntry = history.Last();
            var newHistoryEntry = new List<long>();
            for (var i = 0; i < lastHistoryEntry.Count - 1; i++)
            {
                var v1 = lastHistoryEntry[i];
                var v2 = lastHistoryEntry[i + 1];
                var dif = v2 - v1;
                newHistoryEntry.Add(dif);
            }

            history.Add(newHistoryEntry);
        }

        for (var i = history.Count - 1; i > 0; i--)
        {
            var currentHistoryEntry = history[i];
            var beforeHistoryEntry = history[i - 1];
            var newValue = beforeHistoryEntry.Last() + currentHistoryEntry.Last();
            beforeHistoryEntry.Add(newValue);
        }

        foreach (var longs in history)
        {
            longs.Insert(0, 0);
        }

        for (var i = history.Count - 1; i > 0; i--)
        {
            var currentHistoryEntry = history[i];
            var beforeHistoryEntry = history[i - 1];
            var newValue = beforeHistoryEntry[1] - currentHistoryEntry.First();
            beforeHistoryEntry[0] = newValue;
        }

        values.Add(history);
        sumOfExtrapolatedValues += history[0].Last();
        sumOfExtrapolatedValuesPart2 += history[0].First();
    }

    Console.WriteLine("Part 1: Sum of extrapolated values: " + sumOfExtrapolatedValues);
    Console.WriteLine("Part 2: Sum of extrapolated values: " + sumOfExtrapolatedValuesPart2);
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}
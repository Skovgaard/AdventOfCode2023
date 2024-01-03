using Utilities;

var sumOfExtrapolatedValues = 0L;

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

        values.Add(history);
        sumOfExtrapolatedValues += history[0].Last();
    }

    Console.WriteLine("Part 1: Sum of extrapolated values: " + sumOfExtrapolatedValues);
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}